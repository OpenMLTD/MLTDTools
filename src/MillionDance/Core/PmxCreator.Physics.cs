using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mltd.Sway;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Entities.Pmx.Extensions;
using OpenMLTD.MillionDance.Utilities;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class PmxCreator {

        private static class Physics {

            internal static (IReadOnlyList<PmxRigidBody> Bodies, IReadOnlyList<PmxJoint> Joints) ImportPhysics([NotNull] IReadOnlyList<PmxBone> bones, [NotNull] SwayController bodySway, [NotNull] SwayController headSway) {
                var bodies = new List<PmxRigidBody>();

                var swayColliders = new List<SwayCollider>();

                swayColliders.AddRange(bodySway.Colliders);
                swayColliders.AddRange(headSway.Colliders);

                AppendStaticBodies(bones, swayColliders, bodies);

                var swayBones = new List<SwayBone>();

                swayBones.AddRange(bodySway.SwayBones);
                swayBones.AddRange(headSway.SwayBones);

                AppendSwayBones(bones, swayBones, bodies);

                // TODO: fix rigid body rotation (with bone, e.g. the bones of arms)
                if (ConversionConfig.Current.FixTdaBindingPose) {
                    Trace.TraceWarning("Rigid body rotation correction for TDA pose has not been implemented yet!");
                }

                var joints = new List<PmxJoint>();

                AppendJoints(bones, bodies, joints);

                return (bodies, joints);
            }

            private static void AppendStaticBodies([NotNull, ItemNotNull] IReadOnlyList<PmxBone> bones, [NotNull, ItemNotNull] IReadOnlyList<SwayCollider> swayColliders, [NotNull, ItemNotNull] List<PmxRigidBody> bodies) {
                foreach (var collider in swayColliders) {
                    var mltdBoneName = collider.Path;

                    if (mltdBoneName.Contains("BODY_SCALE/")) {
                        mltdBoneName = mltdBoneName.Replace("BODY_SCALE/", string.Empty);
                    }

                    var pmxBoneName = BoneUtils.GetPmxBoneName(mltdBoneName);

                    var body = new PmxRigidBody();
                    var correspondingBone = bones.FirstOrDefault(o => o.Name == pmxBoneName);

                    Debug.Assert(correspondingBone != null, nameof(correspondingBone) + " != null");

                    body.Name = correspondingBone.Name;
                    body.NameEnglish = correspondingBone.NameEnglish;

                    body.BoneIndex = correspondingBone.BoneIndex;

                    body.KineticMode = KineticMode.Static;

                    body.Mass = 1;
                    body.PositionDamping = 0.9f;
                    body.RotationDamping = 1.0f;
                    body.Friction = 0.0f;
                    body.Restitution = 0.0f;

                    var part = GetBodyCoordSystemPart(collider);

                    body.Part = part;

                    switch (part) {
                        case CoordSystemPart.Torso:
                            body.GroupIndex = PassGroupIndex.Torso;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            body.PassGroup.Pass(PassGroupIndex.Arms);
                            body.PassGroup.Pass(PassGroupIndex.Legs);
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            break;
                        case CoordSystemPart.LeftArm:
                        case CoordSystemPart.RightArm:
                            body.GroupIndex = PassGroupIndex.Arms;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            body.PassGroup.Pass(PassGroupIndex.Arms);
                            body.PassGroup.Pass(PassGroupIndex.Legs);
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            break;
                        case CoordSystemPart.Legs:
                            body.GroupIndex = PassGroupIndex.Legs;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            body.PassGroup.Pass(PassGroupIndex.Arms);
                            body.PassGroup.Pass(PassGroupIndex.Legs);
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            break;
                        case CoordSystemPart.Head:
                            body.GroupIndex = PassGroupIndex.Head;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            body.PassGroup.Pass(PassGroupIndex.Arms);
                            body.PassGroup.Pass(PassGroupIndex.Legs);
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (collider.Type) {
                        case ColliderType.Sphere: {
                                body.BoundingBoxKind = BoundingBoxKind.Sphere;
                                body.BoundingBoxSize = new Vector3(collider.Radius, 0, 0);
                                body.Position = correspondingBone.InitialPosition;
                            }
                            break;
                        case ColliderType.Capsule: {
                                body.BoundingBoxKind = BoundingBoxKind.Capsule;
                                body.BoundingBoxSize = new Vector3(collider.Radius, collider.Distance, 0);
                                body.RotationAngles = MapRotation(part, collider.Axis);

                                var offset = MapTranslation(collider.Offset, part, collider.Axis);

                                if (ConversionConfig.Current.ScaleToPmxSize) {
                                    offset = offset * ScalingConfig.ScaleUnityToPmx;
                                }

                                body.Position = correspondingBone.InitialPosition + offset;
                            }
                            break;
                        case ColliderType.Plane:
                        case ColliderType.Bridge:
                            // TODO: How to handle these?
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (ConversionConfig.Current.ScaleToPmxSize) {
                        body.BoundingBoxSize = body.BoundingBoxSize * ScalingConfig.ScaleUnityToPmx;
                    }

                    bodies.Add(body);
                }
            }

            private static void AppendSwayBones([NotNull, ItemNotNull] IReadOnlyList<PmxBone> bones, [NotNull, ItemNotNull] List<SwayBone> swayBones, [NotNull, ItemNotNull] List<PmxRigidBody> bodies) {
                // "Semi-root" sway bones
                foreach (var swayBone in swayBones) {
                    var part = GetBodyCoordSystemPart(swayBone);

                    switch (part) {
                        case CoordSystemPart.Torso:
                        case CoordSystemPart.LeftArm:
                        case CoordSystemPart.RightArm:
                        case CoordSystemPart.Legs:
                        case CoordSystemPart.Head:
                            continue;
                        case CoordSystemPart.Breasts:
                        case CoordSystemPart.Accessories:
                            continue;
                        case CoordSystemPart.Skirt:
                        case CoordSystemPart.Hair:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var guessedParentPath = swayBone.Path.BreakFirst('/');
                    var parentBody = swayBones.Find(sb => sb.Path == guessedParentPath);

                    if (parentBody != null) {
                        // This is a child rigid body.
                        continue;
                    }

                    var mltdBoneName = swayBone.Path;

                    if (mltdBoneName.Contains("BODY_SCALE/")) {
                        mltdBoneName = mltdBoneName.Replace("BODY_SCALE/", string.Empty);
                    }

                    var pmxBoneName = BoneUtils.GetPmxBoneName(mltdBoneName);

                    var body = new PmxRigidBody();

                    body.Part = part;

                    var correspondingBone = bones.FirstOrDefault(o => o.Name == pmxBoneName);

                    Debug.Assert(correspondingBone != null, "Semi-root sway: " + nameof(correspondingBone) + " != null");

                    body.BoneIndex = correspondingBone.BoneIndex;

                    body.Name = correspondingBone.Name;
                    body.NameEnglish = correspondingBone.NameEnglish;

                    body.KineticMode = KineticMode.Dynamic;

                    body.Mass = ComputeMass(swayBone.Radius);

                    body.PositionDamping = 0.9f;
                    body.RotationDamping = 1.0f;
                    body.Friction = 0.0f;
                    body.Restitution = 0.0f;

                    switch (part) {
                        case CoordSystemPart.Head:
                            body.GroupIndex = PassGroupIndex.Head;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            body.PassGroup.Pass(PassGroupIndex.Arms);
                            body.PassGroup.Pass(PassGroupIndex.Legs);
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            break;
                        case CoordSystemPart.Hair:
                            body.GroupIndex = PassGroupIndex.Hair;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            body.PassGroup.Pass(PassGroupIndex.Hair);
                            break;
                        case CoordSystemPart.Skirt:
                            body.GroupIndex = PassGroupIndex.Skirt;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Skirt);
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            break;
                        case CoordSystemPart.Breasts:
                            body.GroupIndex = PassGroupIndex.Breasts;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Breasts);
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            break;
                        case CoordSystemPart.Accessories:
                            body.GroupIndex = PassGroupIndex.Accessories;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Accessories);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (swayBone.Type) {
                        case ColliderType.Sphere: {
                                body.BoundingBoxKind = BoundingBoxKind.Sphere;
                                body.BoundingBoxSize = new Vector3(swayBone.Radius, 0, 0);

                                var childBone = bones.FirstOrDefault(bo => bo.ParentIndex == correspondingBone.BoneIndex);

                                Debug.Assert(childBone != null, nameof(childBone) + " != null");

                                body.Position = (correspondingBone.InitialPosition + childBone.InitialPosition) / 2;
                            }
                            break;
                        case ColliderType.Capsule:
                        case ColliderType.Plane:
                        case ColliderType.Bridge:
                            throw new ArgumentOutOfRangeException(nameof(swayBone.Type), swayBone.Type, "Only sphere colliders are supported in SwayBone.");
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (ConversionConfig.Current.ScaleToPmxSize) {
                        body.BoundingBoxSize = body.BoundingBoxSize * ScalingConfig.ScaleUnityToPmx;
                    }

                    bodies.Add(body);
                }

                // Normal sway bones
                foreach (var swayBone in swayBones) {
                    var mltdBoneName = swayBone.Path;

                    if (mltdBoneName.Contains("BODY_SCALE/")) {
                        mltdBoneName = mltdBoneName.Replace("BODY_SCALE/", string.Empty);
                    }

                    var pmxBoneName = BoneUtils.GetPmxBoneName(mltdBoneName);

                    var body = new PmxRigidBody();
                    var part = GetBodyCoordSystemPart(swayBone);

                    body.Part = part;

                    var correspondingBone = bones.FirstOrDefault(o => o.Name == pmxBoneName);

                    Debug.Assert(correspondingBone != null, "Normal sway: " + nameof(correspondingBone) + " != null");

                    switch (part) {
                        case CoordSystemPart.Skirt:
                        case CoordSystemPart.Hair:
                            correspondingBone = bones.FirstOrDefault(b => b.ParentIndex == correspondingBone.BoneIndex);
                            Debug.Assert(correspondingBone != null, "Normal sway: " + nameof(correspondingBone) + " != null");
                            break;
                        case CoordSystemPart.Accessories:
                        case CoordSystemPart.Breasts:
                            break;
                    }

                    body.BoneIndex = correspondingBone.BoneIndex;

                    body.Name = correspondingBone.Name;
                    body.NameEnglish = correspondingBone.NameEnglish;

                    switch (part) {
                        case CoordSystemPart.Skirt:
                        case CoordSystemPart.Hair:
                            body.KineticMode = KineticMode.Dynamic;
                            break;
                        case CoordSystemPart.Accessories:
                        case CoordSystemPart.Breasts:
                            body.KineticMode = KineticMode.DynamicWithBone;
                            break;
                    }

                    body.Mass = ComputeMass(swayBone.Radius);

                    body.PositionDamping = 0.9f;
                    body.RotationDamping = 1.0f;
                    body.Friction = 0.0f;
                    body.Restitution = 0.0f;

                    switch (part) {
                        case CoordSystemPart.Head:
                            body.GroupIndex = PassGroupIndex.Head;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            body.PassGroup.Pass(PassGroupIndex.Arms);
                            body.PassGroup.Pass(PassGroupIndex.Legs);
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            break;
                        case CoordSystemPart.Hair:
                            body.GroupIndex = PassGroupIndex.Hair;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Hair);
                            body.PassGroup.Pass(PassGroupIndex.Head);
                            break;
                        case CoordSystemPart.Skirt:
                            body.GroupIndex = PassGroupIndex.Skirt;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Skirt);
                            break;
                        case CoordSystemPart.Breasts:
                            body.GroupIndex = PassGroupIndex.Breasts;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Breasts);
                            body.PassGroup.Pass(PassGroupIndex.Torso);
                            break;
                        case CoordSystemPart.Accessories:
                            body.GroupIndex = PassGroupIndex.Accessories;
                            body.PassGroup.BlockAll();
                            body.PassGroup.Pass(PassGroupIndex.Accessories);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (swayBone.Type) {
                        case ColliderType.Sphere: {
                                body.BoundingBoxKind = BoundingBoxKind.Sphere;
                                body.BoundingBoxSize = new Vector3(swayBone.Radius, 0, 0);

                                var childBone = bones.FirstOrDefault(bo => bo.ParentIndex == correspondingBone.BoneIndex);

                                if (childBone != null) {
                                    body.Position = (correspondingBone.InitialPosition + childBone.InitialPosition) / 2;
                                } else {
                                    var parentBone = bones.FirstOrDefault(bo => bo.BoneIndex == correspondingBone.ParentIndex);

                                    Debug.Assert(parentBone != null, nameof(parentBone) + " != null");

                                    var delta = correspondingBone.InitialPosition - parentBone.InitialPosition;

                                    body.Position = correspondingBone.InitialPosition + delta / 2;
                                }
                            }
                            break;
                        case ColliderType.Capsule:
                        case ColliderType.Plane:
                        case ColliderType.Bridge:
                            throw new ArgumentOutOfRangeException(nameof(swayBone.Type), swayBone.Type, "Only sphere colliders are supported in SwayBone.");
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (ConversionConfig.Current.ScaleToPmxSize) {
                        body.BoundingBoxSize = body.BoundingBoxSize * ScalingConfig.ScaleUnityToPmx;
                    }

                    bodies.Add(body);
                }
            }

            private static void AppendJoints([NotNull, ItemNotNull] IReadOnlyList<PmxBone> bones, [NotNull, ItemNotNull] List<PmxRigidBody> bodies, [NotNull, ItemNotNull] List<PmxJoint> joints) {
                foreach (var body in bodies) {
                    var addJoint = false;

                    switch (body.Part) {
                        case CoordSystemPart.Torso:
                        case CoordSystemPart.LeftArm:
                        case CoordSystemPart.RightArm:
                        case CoordSystemPart.Legs:
                        case CoordSystemPart.Head:
                            break;
                        case CoordSystemPart.Skirt:
                        case CoordSystemPart.Hair:
                        case CoordSystemPart.Breasts:
                            addJoint = true;
                            break;
                        case CoordSystemPart.Accessories:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!addJoint) {
                        continue;
                    }

                    var pmxBone = bones[body.BoneIndex];
                    var parentBone = bones[pmxBone.ParentIndex];

                    var parentBody = bodies.Find(bo => bo.BoneIndex == parentBone.BoneIndex);

                    // This rigid body is attached to a semi-root bone. Create it as a new "root" for its sub rigid bodies.
                    if (parentBody == null) {
                        switch (body.Part) {
                            case CoordSystemPart.Skirt:
                                var koshi = BoneUtils.GetPmxBoneName("MODEL_00/BASE/KOSHI");
                                parentBody = bodies.FirstOrDefault(bo => bones[bo.BoneIndex].Name == koshi);
                                break;
                            case CoordSystemPart.Hair:
                                // The first one is the one on body (which is larger).
                                // The second one is what should be applied to the hair.
                                parentBody = bodies.Where(bo => bones[bo.BoneIndex].Name == "ATAMA").Skip(1).FirstOrDefault();
                                break;
                            case CoordSystemPart.Breasts:
                                var mune2 = BoneUtils.GetPmxBoneName("MODEL_00/BASE/MUNE1/MUNE2");
                                parentBody = bodies.FirstOrDefault(bo => bones[bo.BoneIndex].Name == mune2);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    var joint = new PmxJoint();

                    joint.BodyIndex1 = bodies.IndexOf(parentBody);
                    joint.BodyIndex2 = bodies.IndexOf(body);

                    joint.Name = body.Name;
                    joint.NameEnglish = body.NameEnglish;

                    switch (body.Part) {
                        case CoordSystemPart.Hair:
                        case CoordSystemPart.Skirt:
                            joint.Kind = JointKind.Spring6Dof;
                            break;
                        default:
                            joint.Kind = JointKind.Spring6Dof;
                            break;
                    }

                    joint.Position = pmxBone.InitialPosition;

                    // Now calculate rotation for this joint.
                    var childBones = bones.Where(b => b.ParentIndex == pmxBone.BoneIndex).ToArray();

                    if (childBones.Length > 1) {
                        Debug.Print("Warning: more than one child bone for joint rotation calculation. Aborting.");
                        continue;
                    }

                    PmxBone rotBone1, rotBone2;

                    if (childBones.Length == 0) {
                        rotBone1 = parentBone;
                        rotBone2 = pmxBone;
                    } else {
                        rotBone1 = pmxBone;
                        rotBone2 = childBones[0];
                    }

                    var delta = rotBone2.InitialPosition - rotBone1.InitialPosition;

                    var qy = (float)Math.Atan2(delta.X, delta.Z);
                    var qx = (float)Math.Atan2(delta.Y, delta.Z);
                    //const float qx = 0;
                    //var qz = (float)Math.Atan2(delta.X, delta.Y);
                    const float qz = 0;

                    joint.Rotation = new Vector3(qx, qy, qz);

                    if (joint != null) {
                        joints.Add(joint);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static CoordSystemPart GetBodyCoordSystemPart([NotNull] SwayCollider collider) {
                var n = collider.Path.LastIndexOf('/');
                var seg = collider.Path.Substring(n + 1).ToLowerInvariant();

                switch (seg) {
                    case "kubi":
                    case "atama":
                        return CoordSystemPart.Head;
                    case "mune1":
                    case "mune2":
                    case "koshi":
                        return CoordSystemPart.Torso;
                    case "kata_l":
                    case "ude_l":
                    case "te_l":
                        return CoordSystemPart.LeftArm;
                    case "kata_r":
                    case "ude_r":
                    case "te_r":
                        return CoordSystemPart.RightArm;
                    case "momo_l":
                    case "hiza_l":
                    case "ashi_l":
                        return CoordSystemPart.Legs;
                    case "momo_r":
                    case "hiza_r":
                    case "ashi_r":
                        return CoordSystemPart.Legs;
                }

                return CoordSystemPart.Invalid;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static CoordSystemPart GetBodyCoordSystemPart([NotNull] SwayBone swayBone) {
                if (swayBone.IsSkirt) {
                    return CoordSystemPart.Skirt;
                }

                var n = swayBone.Path.LastIndexOf('/');
                var seg = swayBone.Path.Substring(n + 1).ToLowerInvariant();

                switch (seg) {
                    case "kubi":
                    case "atama":
                        return CoordSystemPart.Head;
                }

                if (swayBone.Path.Contains("hair_")) {
                    return CoordSystemPart.Hair;
                }

                if (swayBone.Path.Contains("skirt_") || swayBone.Path.Contains("skrt_")) {
                    return CoordSystemPart.Skirt;
                }

                if (swayBone.Path.Contains("OPAI_")) {
                    return CoordSystemPart.Breasts;
                }

                return CoordSystemPart.Accessories;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static Vector3 MapRotation(CoordSystemPart part, CollidingAxis axis) {
                Vector3 rotDeg;

                // MMD default capsule: along Y-axis, Y-up
                // MLTD default: "X" (changes with body coord system)
                // ref: http://nw.tsuda.ac.jp/lec/unity5/Humanoid/index-en.html
                switch (part) {
                    case CoordSystemPart.Torso:
                    case CoordSystemPart.Head:
                        rotDeg = new Vector3(0, 0, 0);
                        break;
                    case CoordSystemPart.LeftArm:
                        rotDeg = new Vector3(0, 0, -90);
                        break;
                    case CoordSystemPart.RightArm:
                        rotDeg = new Vector3(0, 0, 90);
                        break;
                    case CoordSystemPart.Legs:
                        rotDeg = new Vector3(180, 0, 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(part), part, null);
                }

                var rotRad = rotDeg * Deg2Rad;

                return rotRad;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static Vector3 MapTranslation(Vector3 v, CoordSystemPart part, CollidingAxis axis) {
                Vector3 r;

                switch (part) {
                    case CoordSystemPart.Torso:
                    case CoordSystemPart.Head:
                        r = new Vector3(v.Y, -v.X, -v.Z);
                        break;
                    case CoordSystemPart.LeftArm:
                        r = new Vector3(-v.X, v.Y, v.Z);
                        break;
                    case CoordSystemPart.RightArm:
                        r = new Vector3(v.X, v.Y, v.Z);
                        break;
                    case CoordSystemPart.Legs:
                        r = new Vector3(-v.Z, v.X, v.Y);
                        break;
                    case CoordSystemPart.Hair:
                    case CoordSystemPart.Skirt:
                    case CoordSystemPart.Breasts:
                    case CoordSystemPart.Accessories:
                        r = new Vector3(v.Y, -v.X, -v.Z);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(part), part, null);
                }

                return r;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static float Triple(float v) {
                return v * v * v;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static float ComputeMass(float radius) {
                //return MassCoeff * Triple(radius / StandardRadius);
                return 1;
            }

            private static class PassGroupIndex {

                public const int Torso = 1;
                public const int Arms = 2;
                public const int Legs = 3;
                public const int Head = 4;
                public const int Hair = 5;
                public const int Skirt = 6;
                public const int Breasts = 7;
                public const int Accessories = 8;

            }

            private const float Deg2Rad = MathHelper.Pi / 180;
            private const float MassCoeff = 10;
            private const float StandardRadius = 0.1f;

        }

    }
}

/*
    1、短距离前向附着刚体：
      - 裙子最后一个关节
      （头发不用！）
      
      附加规则：
      r = r0/2
      骨骼延伸
      与关节位置相切
      
    2、与下一级之间加刚体：
      - 欧派
      - 头发的中间部分
      - 裙子的中间部分

    3、补充铰链：
      - 头发的同一层（仅需要检测 hair_ushiro[A][#]）
      - 裙子的同一层（skirt_/skrt_[L/R][A][#]）
 */
