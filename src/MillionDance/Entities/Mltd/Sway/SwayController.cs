using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Mltd.Sway {
    public sealed class SwayController {

        private SwayController() {
        }

        [NotNull]
        public string Top { get; internal set; } = string.Empty;

        [NotNull, ItemNotNull]
        public IReadOnlyList<SwayManager> Managers { get; private set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<SwayCollider> Colliders { get; private set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<SwayBone> SwayBones { get; private set; }

        [NotNull]
        public static SwayController Parse([NotNull] string text) {
            var lines = text.Split(LineSeps);

            for (var i = 0; i < lines.Length; ++i) {
                var l = lines[i];

                if (l.Length == 0) {
                    continue;
                }

                if (l[l.Length - 1] == '\r') {
                    lines[i] = l.TrimEnd(TrimSeps);
                }
            }

            var controller = new SwayController();

            var managers = new List<SwayManager>();
            var colliders = new List<SwayCollider>();
            var swayBones = new List<SwayBone>();

            for (var i = 0; i < lines.Length; ++i) {
                var line = lines[i];

                if (line.Length == 0) {
                    continue;
                }

                var topKeyValue = ParseKeyValue(line);

                switch (topKeyValue.Key) {
                    case "Top":
                        controller.Top = topKeyValue.Value;
                        break;
                    case "SwayManager":
                        var sm = ReadSwayManger();
                        managers.Add(sm);
                        break;
                    case "SwayCollider":
                        var sc = ReadSwayCollider();
                        colliders.Add(sc);
                        break;
                    case "SwayBone":
                        var sb = ReadSwayBone();
                        swayBones.Add(sb);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(topKeyValue.Key), topKeyValue.Key, null);
                }

                SwayManager ReadSwayManger() {
                    var kv = ParseKeyValue(line);
                    var manager = new SwayManager();
                    var anyPropSet = false;

                    do {
                        switch (kv.Key) {
                            case "SwayManager":
                                manager.Path = kv.Value;
                                break;
                            case "gravity":
                                manager.Gravity = ParseVector3(kv.Value);
                                anyPropSet = true;
                                break;
                            case "stiffnessForce":
                                manager.StiffnessForce = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "dragForce":
                                manager.DragForce = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "followForce":
                                manager.FollowForce = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "lineMoveLimit":
                                manager.LineMoveLimit = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "sidelineMoveLimit":
                                manager.SideLineMoveLimit = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            default:
                                Debug.Print("Unknow property key/value: {0} / {1}", kv.Key, kv.Value);
                                break;
                        }

                        {
                            ++i;
                            line = lines[i];

                            if (line.Length == 0 || line[0] != ' ') {
                                // A new entry (manager/collider/bone) starts.
                                // Decrease counter so the next reading attempt will work with auto counter increment.
                                --i;
                                break;
                            }

                            kv = ParseKeyValue(line);
                        }
                    } while (true);

                    if (!anyPropSet) {
                        throw new ArgumentException("Empty SwayManager body");
                    }

                    return manager;
                }

                SwayCollider ReadSwayCollider() {
                    var kv = ParseKeyValue(line);
                    var collider = new SwayCollider();
                    var anyPropSet = false;

                    do {
                        switch (kv.Key) {
                            case "SwayCollider":
                                collider.Path = kv.Value;
                                break;
                            case "type":
                                collider.Type = ParseColliderType(kv.Value);
                                anyPropSet = true;
                                break;
                            case "axis":
                                collider.Axis = ParseCollidingAxis(kv.Value);
                                anyPropSet = true;
                                break;
                            case "radius":
                                collider.Radius = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "distance":
                                collider.Distance = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "offset":
                                collider.Offset = ParseVector3(kv.Value);
                                anyPropSet = true;
                                break;
                            case "planeAxis":
                                collider.PlaneAxis = ParsePlaneAxis(kv.Value);
                                anyPropSet = true;
                                break;
                            case "CapsuleSphere1":
                                collider.CapsuleSphere1 = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "CapsuleSphere2":
                                collider.CapsuleSphere2 = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "planeRotOn":
                                collider.PlaneRotationEnabled = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "planeRotEuler":
                                collider.PlaneRotationEuler = ParseVector3(kv.Value);
                                anyPropSet = true;
                                break;
                            case "bridgetgt":
                                collider.BridgeTargetPath = kv.Value;
                                anyPropSet = true;
                                break;
                            default:
                                Debug.Print("Unknow property key/value: {0} / {1}", kv.Key, kv.Value);
                                break;
                        }

                        {
                            ++i;
                            line = lines[i];

                            if (line.Length == 0 || line[0] != ' ') {
                                // A new entry (manager/collider/bone) starts.
                                // Decrease counter so the next reading attempt will work with auto counter increment.
                                --i;
                                break;
                            }

                            kv = ParseKeyValue(line);
                        }
                    } while (true);

                    if (!anyPropSet) {
                        throw new ArgumentException("Empty SwayCollider body");
                    }

                    return collider;
                }

                SwayBone ReadSwayBone() {
                    var kv = ParseKeyValue(line);
                    var bone = new SwayBone();
                    var anyPropSet = false;

                    do {
                        switch (kv.Key) {
                            case "SwayBone":
                                bone.Path = kv.Value;
                                break;
                            case "radius":
                                bone.Radius = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "type":
                                bone.Type = ParseColliderType(kv.Value);
                                anyPropSet = true;
                                break;
                            case "isSkirt":
                                bone.IsSkirt = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "HitMuteForce":
                                bone.HitMuteForce = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "HitMuteRate":
                                bone.HitMuteRate = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "limitAngle":
                                bone.HasAngleLimit = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "useBindDir":
                                bone.UseBindingDirection = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "minYAngle":
                                bone.MinYAngle = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "maxYAngle":
                                bone.MaxYAngle = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "minZAngle":
                                bone.MinZAngle = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "maxZAngle":
                                bone.MaxZAngle = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "followForce":
                                bone.FollowForce = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "refParam":
                                bone.RefParam = ParseRefParam(kv.Value);
                                anyPropSet = true;
                                break;
                            case "stiffnessForce":
                                bone.StiffnessForce = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "dragForce":
                                bone.DragForce = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "hitTwice":
                                bone.HitTwice = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "useLinkHit":
                                bone.UseLinkHit = Convert.ToBoolean(kv.Value);
                                anyPropSet = true;
                                break;
                            case "gravity":
                                bone.Gravity = ParseVector3(kv.Value);
                                anyPropSet = true;
                                break;
                            case "colliderOffset":
                                bone.ColliderOffset = ParseVector3(kv.Value);
                                anyPropSet = true;
                                break;
                            case "sideSpringTorelance":
                                bone.SideSpringTolerance = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "sideSpringForce":
                                bone.SideSpringForce = Convert.ToSingle(kv.Value);
                                anyPropSet = true;
                                break;
                            case "colliders": {
                                    var colliderCount = Convert.ToInt32(kv.Value);
                                    var colliderPathList = new List<string>();

                                    for (var j = 0; j < colliderCount; ++j) {
                                        ++i;
                                        line = lines[i];

                                        kv = ParseKeyValue(line);

                                        colliderPathList.Add(kv.Value);
                                    }

                                    anyPropSet = true;

                                    bone.ColliderPaths = colliderPathList.ToArray();
                                }
                                break;
                            case "sideLink":
                                bone.SideLinkPath = kv.Value;
                                anyPropSet = true;
                                break;
                            default:
                                Debug.Print("Unknow property key/value: {0} / {1}", kv.Key, kv.Value);
                                break;
                        }

                        {
                            ++i;
                            line = lines[i];

                            if (line.Length == 0 || line[0] != ' ') {
                                // A new entry (manager/collider/bone) starts.
                                // Decrease counter so the next reading attempt will work with auto counter increment.
                                --i;
                                break;
                            }

                            kv = ParseKeyValue(line);
                        }
                    } while (true);

                    if (!anyPropSet) {
                        throw new ArgumentException("Empty SwayBone body");
                    }

                    return bone;
                }
            }

            controller.Managers = managers.ToArray();
            controller.Colliders = colliders.ToArray();
            controller.SwayBones = swayBones.ToArray();

            return controller;
        }

        public static void FixSwayReferences([NotNull, ItemNotNull] params SwayController[] controllers) {
            var colliders = new List<SwayCollider>();
            var joints = new List<SwayBone>();

            foreach (var controller in controllers) {
                colliders.AddRange(controller.Colliders);
                joints.AddRange(controller.SwayBones);
            }

            foreach (var collider in colliders) {
                if (collider.BridgeTargetPath == null) {
                    continue;
                }

                var target = colliders.Find(col => col.Path == collider.BridgeTargetPath);

                Debug.Assert(target != null);

                collider.BridgeTarget = target;
            }

            foreach (var joint in joints) {
                if (joint.SideLinkPath != null) {
                    var sideLink = joints.Find(link => link.Path == joint.SideLinkPath);

                    Debug.Assert(sideLink != null);

                    joint.SideLink = sideLink;
                }

                if (joint.ColliderPaths != null) {
                    var swayBoneColliders = new SwayCollider[joint.ColliderPaths.Length];

                    for (var i = 0; i < joint.ColliderPaths.Length; ++i) {
                        // e.g.: MODEL_00/BODY_SCALE/BASE/KOSHI/MOMO_L#0
                        var targetPath = joint.ColliderPaths[i];

                        var indexOfHash = targetPath.IndexOf('#');

                        if (indexOfHash >= 0) {
                            targetPath = targetPath.Substring(0, indexOfHash);
                        }

                        var collider = colliders.Find(col => col.Path == targetPath);

                        Debug.Assert(collider != null);

                        swayBoneColliders[i] = collider;
                    }

                    joint.Colliders = swayBoneColliders;
                }
            }
        }

        private static KeyValuePair<string, string> ParseKeyValue(string str) {
            var ss = str.Split(KeyValueSeps, StringSplitOptions.RemoveEmptyEntries);

            Debug.Assert(ss.Length == 2);

            var key = ss[0];
            var value = ss[1];

            if (key[0] == ' ') {
                key = key.TrimStart(OptKeyStart);
            }

            return new KeyValuePair<string, string>(key, value);
        }

        private static Vector3 ParseVector3([NotNull] string str) {
            var ss = str.Split(ParamSeps);

            Debug.Assert(ss.Length == 3);

            var x = Convert.ToSingle(ss[0]);
            var y = Convert.ToSingle(ss[1]);
            var z = Convert.ToSingle(ss[2]);

            return new Vector3(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ColliderType ParseColliderType([NotNull] string str) {
            switch (str) {
                case "Sphere":
                    return ColliderType.Sphere;
                case "Capsule":
                    return ColliderType.Capsule;
                case "Plane":
                    return ColliderType.Plane;
                case "Bridge":
                    return ColliderType.Bridge;
                default:
                    throw new ArgumentOutOfRangeException(nameof(str), str, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static CollidingAxis ParseCollidingAxis([NotNull] string str) {
            switch (str) {
                case "X":
                    return CollidingAxis.X;
                case "Y":
                    return CollidingAxis.Y;
                case "Z":
                    return CollidingAxis.Z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(str), str, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PlaneAxis ParsePlaneAxis([NotNull] string str) {
            switch (str) {
                case "plusX":
                    return PlaneAxis.PlusX;
                case "minusX":
                    return PlaneAxis.MinusX;
                case "plusY":
                    return PlaneAxis.PlusY;
                case "minusY":
                    return PlaneAxis.MinusY;
                case "plusZ":
                    return PlaneAxis.PlusZ;
                case "minusZ":
                    return PlaneAxis.MinusZ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(str), str, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static RefParam ParseRefParam([NotNull] string str) {
            switch (str) {
                case "Manager":
                    return RefParam.Manager;
                case "Self":
                    return RefParam.Self;
                default:
                    throw new ArgumentOutOfRangeException(nameof(str), str, null);
            }
        }

        private static readonly char[] LineSeps = { '\n' };

        private static readonly char[] TrimSeps = { '\r' };

        private static readonly string[] KeyValueSeps = { ": " };

        private static readonly char[] OptKeyStart = { ' ' };

        private static readonly char[] ParamSeps = { ',' };

    }
}
