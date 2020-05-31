using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AssetStudio.Extended.CompositeModels;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenMLTD.MillionDance.Extensions;
using OpenMLTD.MillionDance.Utilities;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull, ItemNotNull]
        private IReadOnlyList<VmdBoneFrame> CreateBoneFrames([NotNull] IBodyAnimationSource bodyMotionSource, [NotNull] PrettyAvatar avatar, [NotNull] PmxModel pmx) {
            var boneLookup = new BoneLookup(_conversionConfig);

            var mltdHierarchy = boneLookup.BuildBoneHierarchy(avatar);
            var pmxHierarchy = boneLookup.BuildBoneHierarchy(pmx);

            if (_conversionConfig.AppendIKBones || _conversionConfig.AppendEyeBones) {
                throw new NotSupportedException("Character motion frames generation (from MLTD) is not supported when appending bones (eyes and/or IK) is enabled.");
            } else {
                Debug.Assert(mltdHierarchy.Count == pmxHierarchy.Count, "Hierarchy number should be equal between MLTD and MMD.");
            }

            foreach (var mltdBone in mltdHierarchy) {
                mltdBone.Initialize();
            }

            foreach (var pmxBone in pmxHierarchy) {
                pmxBone.Initialize();
            }

            var animation = bodyMotionSource.Convert();
            var boneCount = mltdHierarchy.Count;
            var animatedBoneCount = animation.BoneCount;
            var keyFrameCount = animation.KeyFrames.Count;

            {
                void MarkNamedBone(string name) {
                    var bone = pmx.Bones.FirstOrDefault(b => b.Name == name);

                    if (bone != null) {
                        bone.IsMltdKeyBone = true;
                    } else {
                        Debug.Print("Warning: trying to mark bone {0} as MLTD key bone but the bone is missing from the model.", name);
                    }
                }

                var names1 = animation.KeyFrames.Take(animatedBoneCount)
                    .Select(kf => kf.Path).ToArray();
                var names = names1.Select(boneLookup.GetVmdBoneNameFromBonePath).ToArray();
                // Mark MLTD key bones.
                foreach (var name in names) {
                    MarkNamedBone(name);
                }

                // Special cases
                MarkNamedBone("KUBI");
                MarkNamedBone("頭");
            }

            Debug.Assert(keyFrameCount % animatedBoneCount == 0, "keyFrameCount % animatedBoneCount == 0");

            var iterationTimes = keyFrameCount / animatedBoneCount;
            var boneFrameList = new List<VmdBoneFrame>();

            // Reduce memory pressure of allocating new delegates (see mltdHierarchy.FirstOrDefault(...))
            var boneMatchPredicateCache = new Func<PmxBone, bool>[boneCount];

            for (var j = 0; j < boneCount; j += 1) {
                var refBone = pmx.Bones[j];
                boneMatchPredicateCache[j] = bone => bone.Name == refBone.Name;
            }

            // Cache `mltdBoneName`s so we don't have to compute them all in every iteration
            var boneNameCache = new Dictionary<string, string>();

            // OK, now perform iterations
            for (var i = 0; i < iterationTimes; ++i) {
                if (_conversionConfig.Transform60FpsTo30Fps) {
                    if (i % 2 == 1) {
                        continue;
                    }
                }

                var keyFrameIndexStart = i * animatedBoneCount;

                for (var j = 0; j < animatedBoneCount; ++j) {
                    var keyFrame = animation.KeyFrames[keyFrameIndexStart + j];
                    string mltdBoneName;

                    if (boneNameCache.ContainsKey(keyFrame.Path)) {
                        mltdBoneName = boneNameCache[keyFrame.Path];
                    } else {
                        if (keyFrame.Path.Contains("BODY_SCALE/")) {
                            mltdBoneName = keyFrame.Path.Replace("BODY_SCALE/", string.Empty);
                        } else {
                            mltdBoneName = keyFrame.Path;
                        }

                        boneNameCache.Add(keyFrame.Path, mltdBoneName);
                    }

                    var targetBone = mltdHierarchy.SingleOrDefault(bone => bone.Name == mltdBoneName);

                    if (targetBone == null) {
                        //throw new ArgumentException("Bone not found.");
                        continue; // Shika doesn't have the "POSITION" bone.
                    }

                    BoneNode transferredBone = null;

                    foreach (var kv in BoneAttachmentMap) {
                        if (kv.Key == mltdBoneName) {
                            transferredBone = mltdHierarchy.SingleOrDefault(bone => bone.Name == kv.Value);

                            if (transferredBone == null) {
                                throw new ArgumentException();
                            }

                            break;
                        }
                    }

                    if (keyFrame.HasPositions) {
                        // ReSharper disable once PossibleInvalidOperationException
                        var x = keyFrame.PositionX.Value;
                        // ReSharper disable once PossibleInvalidOperationException
                        var y = keyFrame.PositionY.Value;
                        // ReSharper disable once PossibleInvalidOperationException
                        var z = keyFrame.PositionZ.Value;

                        var t = new Vector3(x, y, z);

                        t = t.FixUnityToOpenTK();

                        if (_conversionConfig.ScaleToVmdSize) {
                            t = t * _scalingConfig.ScaleUnityToPmx;
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

                for (var j = 0; j < boneCount; ++j) {
                    var pmxBone = pmxHierarchy[j];
                    var mltdBone = mltdHierarchy[j];

                    {
                        var predicate = boneMatchPredicateCache[j];
                        var pb = pmx.Bones.FirstOrDefault(predicate);

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

            return boneFrameList;
        }

        private static readonly IReadOnlyDictionary<string, string> BoneAttachmentMap = new Dictionary<string, string> {
            //["MODEL_00/BASE/MUNE1/MUNE2/KUBI"] = "KUBI",
            ["MODEL_00/BASE/MUNE1/MUNE2/KUBI/ATAMA"] = "KUBI/ATAMA"
        };

    }
}
