using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AssetStudio.Extended.CompositeModels;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Extensions;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenMLTD.MillionDance.Extensions;
using OpenMLTD.MillionDance.Utilities;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull, ItemNotNull]
        private VmdBoneFrame[] CreateBoneFrames([NotNull] IBodyAnimationSource bodyMotionSource, [CanBeNull] ScenarioObject scenario, [NotNull] PrettyAvatar avatar, [NotNull] PmxModel pmx, int idolPosition) {
            var boneLookup = new BoneLookup(_conversionConfig);

            var mltdHierarchy = boneLookup.BuildBoneHierarchy(avatar);

            mltdHierarchy.AssertAllUnique();

            var pmxHierarchy = boneLookup.BuildBoneHierarchy(pmx);

            pmxHierarchy.AssertAllUnique();

            if (_conversionConfig.AppendIKBones || _conversionConfig.AppendEyeBones) {
                throw new NotSupportedException("Character motion frames generation (from MLTD) is not supported when appending bones (eyes and/or IK) is enabled.");
            } else {
                Debug.Assert(mltdHierarchy.Length == pmxHierarchy.Length, "Hierarchy number should be equal between MLTD and MMD.");
            }

            foreach (var mltdBone in mltdHierarchy) {
                mltdBone.Initialize();
            }

            foreach (var pmxBone in pmxHierarchy) {
                pmxBone.Initialize();
            }

            var animation = bodyMotionSource.Convert();
            var mltdBoneCount = mltdHierarchy.Length;
            var animatedBoneCount = animation.BoneCount;
            var keyFrameCount = animation.KeyFrames.Length;

            {
                var names1 = animation.KeyFrames.Take(animatedBoneCount)
                    .Select(kf => kf.Path).ToArray();
                var names = names1.Select(boneLookup.GetVmdBoneNameFromBonePath).ToArray();

                // Mark MLTD key bones.
                foreach (var name in names) {
                    MarkNamedBoneAsKeyBone(pmx, name);
                }

                // Special cases
                MarkNamedBoneAsKeyBone(pmx, "KUBI");
                MarkNamedBoneAsKeyBone(pmx, "頭");
            }

            Debug.Assert(keyFrameCount % animatedBoneCount == 0, "keyFrameCount % animatedBoneCount == 0");

            var resultFrameCount = keyFrameCount / animatedBoneCount;
            var boneFrameList = new List<VmdBoneFrame>();

            // Reduce memory pressure of allocating new delegates (see mltdHierarchy.FirstOrDefault(...))
            var boneMatchPredicateCache = new Predicate<PmxBone>[mltdBoneCount];

            for (var j = 0; j < mltdBoneCount; j += 1) {
                var refBone = pmx.Bones[j];
                boneMatchPredicateCache[j] = bone => bone.Name == refBone.Name;
            }

            // Cache `mltdBoneName`s so we don't have to compute them all in every iteration
            var boneNameCache = new Dictionary<string, string>();

            var transform60FpsTo30Fps = _conversionConfig.Transform60FpsTo30Fps;
            var scaleToVmdSize = _conversionConfig.ScaleToVmdSize;
            var unityToVmdScale = _scalingConfig.ScaleUnityToVmd;

            var formationList = CollectFormationChanges(scenario);

            // OK, now perform iterations
            for (var i = 0; i < resultFrameCount; ++i) {
                if (transform60FpsTo30Fps) {
                    if (i % 2 == 1) {
                        continue;
                    }
                }

                var keyFrameIndexStart = i * animatedBoneCount;

                var bb = formationList.TryGetCurrentValue(i, out var formations);
                Vector4 idolPosOffset;

                if (formations == null || formations.Length < idolPosition) {
                    idolPosOffset = Vector4.Zero;
                } else {
                    idolPosOffset = formations[idolPosition - 1];
                }

                for (var j = 0; j < animatedBoneCount; ++j) {
                    var keyFrame = animation.KeyFrames[keyFrameIndexStart + j];
                    string mltdBoneName;

                    if (boneNameCache.ContainsKey(keyFrame.Path)) {
                        mltdBoneName = boneNameCache[keyFrame.Path];
                    } else {
                        if (keyFrame.Path.Contains(BoneLookup.BoneNamePart_BodyScale)) {
                            mltdBoneName = keyFrame.Path.Replace(BoneLookup.BoneNamePart_BodyScale, string.Empty);
                        } else {
                            mltdBoneName = keyFrame.Path;
                        }

                        boneNameCache.Add(keyFrame.Path, mltdBoneName);
                    }

                    // Uniqueness is asserted above
                    var targetBone = mltdHierarchy.Find(bone => bone.Name == mltdBoneName);

                    if (targetBone == null) {
                        //throw new ArgumentException("Bone not found.");
                        continue; // Shika doesn't have the "POSITION" bone.
                    }

                    BoneNode transferredBone = null;

                    foreach (var kv in BoneAttachmentMap) {
                        if (kv.Key != mltdBoneName) {
                            continue;
                        }

                        var attachmentTarget = kv.Value;

                        // Uniqueness is asserted above
                        transferredBone = mltdHierarchy.Find(bone => bone.Name == attachmentTarget);

                        if (transferredBone == null) {
                            throw new ArgumentException("Cannot find transferred bone.");
                        }

                        break;
                    }

                    if (keyFrame.HasPositions) {
                        // ReSharper disable once PossibleInvalidOperationException
                        var x = keyFrame.PositionX.Value;
                        // ReSharper disable once PossibleInvalidOperationException
                        var y = keyFrame.PositionY.Value;
                        // ReSharper disable once PossibleInvalidOperationException
                        var z = keyFrame.PositionZ.Value;

                        if (string.Equals(keyFrame.Path, "MODEL_00", StringComparison.Ordinal)) {
                            x += idolPosOffset.X;
                            y += idolPosOffset.Y;
                            z += idolPosOffset.Z;
                        }

                        var t = new Vector3(x, y, z);

                        t = t.FixUnityToOpenTK();

                        if (scaleToVmdSize) {
                            t = t * unityToVmdScale;
                        }

                        targetBone.LocalPosition = t;

                        //if (transferredBone != null) {
                        //    transferredBone.LocalPosition = t;
                        //}
                    }

                    if (keyFrame.HasRotations) {
                        // ReSharper disable once PossibleInvalidOperationException
                        var x = keyFrame.AngleX.Value;
                        // ReSharper disable once PossibleInvalidOperationException
                        var y = keyFrame.AngleY.Value;
                        // ReSharper disable once PossibleInvalidOperationException
                        var z = keyFrame.AngleZ.Value;

                        var q = UnityRotation.EulerDeg(x, y, z);

                        q = q.FixUnityToOpenTK();

                        targetBone.LocalRotation = q;

                        if (transferredBone != null) {
                            transferredBone.LocalRotation = q;
                        }
                    }
                }

                foreach (var mltdBone in mltdHierarchy) {
                    mltdBone.UpdateTransform();
                }

                for (var j = 0; j < mltdBoneCount; ++j) {
                    var pmxBone = pmxHierarchy[j];
                    var mltdBone = mltdHierarchy[j];

                    {
                        var predicate = boneMatchPredicateCache[j];
                        var pb = pmx.Bones.Find(predicate);

#if DEBUG
                        if (pb == null) {
                            // Lazy evaluation of the assertion message
                            Debug.Assert(pb != null, $"PMX bone with the name \"{pmxBone.Name}\" should exist.");
                        }
#endif

                        if (!pb.IsMltdKeyBone) {
                            continue;
                        }
                    }

                    var skinMatrix = mltdBone.SkinMatrix;
                    var mPmxBindingPose = pmxBone.BindingPose;
                    var mWorld = pmxBone.Parent?.WorldMatrix ?? Matrix4.Identity;

                    // skinMatrix == inv(mPmxBindingPose) x mLocal x mWorld
                    var mLocal = mPmxBindingPose * skinMatrix * mWorld.Inverted();

                    // Here, translation is in... world coords? WTF?
                    var t = mLocal.ExtractTranslation();
                    var q = mLocal.ExtractRotation();

                    if (pmxBone.Parent != null) {
                        t = t - (pmxBone.InitialPosition - pmxBone.Parent.InitialPosition);
                    }

                    int frameIndex;

                    if (_conversionConfig.Transform60FpsTo30Fps) {
                        frameIndex = i / 2;
                    } else {
                        frameIndex = i;
                    }

                    var vmdBoneName = boneLookup.GetVmdBoneNameFromBoneName(mltdBone.Path);
                    var boneFrame = new VmdBoneFrame(frameIndex, vmdBoneName);

                    boneFrame.Position = t;
                    boneFrame.Rotation = q;

                    boneFrameList.Add(boneFrame);

                    pmxBone.LocalPosition = t;
                    pmxBone.LocalRotation = q;
                    pmxBone.UpdateTransform();
                }
            }

            return boneFrameList.ToArray();
        }

        [NotNull]
        private TimedList<int, Vector4[]> CollectFormationChanges([CanBeNull] ScenarioObject scenario) {
            var result = new TimedList<int, Vector4[]>();

            if (scenario == null) {
                return result;
            }

            if (scenario.HasFormationChangeFrames()) {
                var events = scenario.Scenario.WhereToArray(s => s.Type == ScenarioDataType.FormationChange);

                foreach (var ev in events) {
                    Debug.Assert(ev != null, nameof(ev) + " != null");

                    // TODO: What does 'Param' field mean here? And 'Layer'?
                    // e.g. Glow Map (glowmp) has param=279, layer=0,1,2
                    // Currently we only handle non-appeal formations
                    if (ev.Layer != 0) {
                        continue;
                    }

                    var frameIndex = (int)Math.Round(ev.AbsoluteTime * FrameRate.Mltd);

                    var formations = ev.Formation;
                    Debug.Assert(formations != null && formations.Length > 0);

                    var f = new Vector4[formations.Length];

                    for (var i = 0; i < formations.Length; i += 1) {
                        var v = formations[i];
                        f[i] = new Vector4(v.X, v.Y, v.Z, v.W);
                    }

                    result.AddOrUpdate(frameIndex, f);
                }
            }

            return result;
        }

        // Set the bones that appear in the animation file as "key bone"
        private static void MarkNamedBoneAsKeyBone([NotNull] PmxModel pmx, [NotNull] string name) {
            var bone = pmx.Bones.Find(b => b.Name == name);

            if (bone != null) {
                bone.IsMltdKeyBone = true;
            } else {
                Debug.Print("Warning: trying to mark bone {0} as MLTD key bone but the bone is missing from the model.", name);
            }
        }

        [NotNull]
        private static readonly Dictionary<string, string> BoneAttachmentMap = new Dictionary<string, string> {
            //["MODEL_00/BASE/MUNE1/MUNE2/KUBI"] = "KUBI",
            ["MODEL_00/BASE/MUNE1/MUNE2/KUBI/ATAMA"] = "KUBI/ATAMA"
        };

    }
}
