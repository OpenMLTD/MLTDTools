using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

        [NotNull]
        public VmdMotion CreateDanceMotion([CanBeNull] IBodyAnimationSource mainDance, [NotNull] ScenarioObject baseScenario, [CanBeNull] ScenarioObject formationInfo, [CanBeNull] PrettyAvatar avatar, [CanBeNull] PmxModel mltdPmxModel, [CanBeNull] IBodyAnimationSource danceAppeal, int formationNumber, AppealType appealType) {
            VmdBoneFrame[] frames;

            if (ProcessBoneFrames && (mainDance != null && avatar != null && mltdPmxModel != null)) {
                frames = CreateBoneFrames(mainDance, avatar, mltdPmxModel, baseScenario, formationInfo, danceAppeal, formationNumber, appealType);
            } else {
                frames = Array.Empty<VmdBoneFrame>();
            }

            return new VmdMotion(ModelName, frames, null, null, null, null);
        }

        [NotNull, ItemNotNull]
        private VmdBoneFrame[] CreateBoneFrames([NotNull] IBodyAnimationSource mainDance, [NotNull] PrettyAvatar avatar, [NotNull] PmxModel pmx, [NotNull] ScenarioObject baseScenario, [CanBeNull] ScenarioObject formationInfo, [CanBeNull] IBodyAnimationSource danceAppeal, int formationNumber, AppealType appealType) {
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

            var mainAnimation = mainDance.Convert();
            var appealAnimation = danceAppeal?.Convert();
            var mltdBoneCount = mltdHierarchy.Length;
            var animatedBoneCount = mainAnimation.BoneCount;
            var keyFrameCount = mainAnimation.KeyFrames.Length;

            {
                var names1 = mainAnimation.KeyFrames.Take(animatedBoneCount)
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

            // Use this value to export visible frames only
            var resultFrameCount = (int)(mainAnimation.Duration * FrameRate.Mltd);
            // Use this value to export all frames, including invisible frames in normal MVs (e.g. seek targets)
            // var resultFrameCount = keyFrameCount / animatedBoneCount;

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

            var baseFormationList = CollectFormationChanges(formationInfo, AppealType.None);
            var appealFormationList = CollectFormationChanges(formationInfo, appealType);
            var appealTimes = AppealHelper.CollectAppealTimeInfo(baseScenario);
            var seekFrameControls = CollectSeekFrames(baseScenario, formationNumber);

            var seekFrameCounter = 0;
            var lastSoughtFrame = -1;

            // OK, now perform iterations
            for (var mltdFrameIndex = 0; mltdFrameIndex < resultFrameCount; ++mltdFrameIndex) {
                if (transform60FpsTo30Fps) {
                    if (mltdFrameIndex % 2 == 1) {
                        continue;
                    }
                }

                var shouldUseAppeal = appealType != AppealType.None && (appealTimes.StartFrame <= mltdFrameIndex && mltdFrameIndex < appealTimes.EndFrame) && appealAnimation != null;

                var animation = shouldUseAppeal ? appealAnimation : mainAnimation;

                int projectedFrameIndex;

                if (shouldUseAppeal) {
                    var indexInAppeal = mltdFrameIndex - appealTimes.StartFrame;

                    if (indexInAppeal >= appealAnimation.FrameCount) {
                        indexInAppeal = appealAnimation.FrameCount - 1;
                    }

                    // `indexInAppeal`, unlike `mltdFrameIndex`, has not been scaled yet
                    if (transform60FpsTo30Fps) {
                        projectedFrameIndex = indexInAppeal / 2;
                    } else {
                        projectedFrameIndex = indexInAppeal;
                    }
                } else {
                    projectedFrameIndex = CalculateSeekFrameTarget(mltdFrameIndex, seekFrameControls, ref lastSoughtFrame, ref seekFrameCounter);
                }

                var formationList = shouldUseAppeal ? appealFormationList : baseFormationList;

                formationList.TryGetCurrentValue(mltdFrameIndex, out var formations);

                Vector4 idolOffset;

                if (formations == null || formations.Length < formationNumber) {
                    idolOffset = Vector4.Zero;
                } else {
                    idolOffset = formations[formationNumber - 1];
                }

                var keyFrameIndexStart = projectedFrameIndex * animatedBoneCount;

                for (var j = 0; j < animatedBoneCount; ++j) {
                    var keyFrame = animation.KeyFrames[keyFrameIndexStart + j];
                    var mltdBoneName = GetMltdBoneNameWithoutBodyScale(boneNameCache, keyFrame);

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
                            var worldRotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(idolOffset.W));
                            var newOrigin = worldRotation * new Vector3(x, y, z);
                            var newPosition = newOrigin + idolOffset.Xyz;

                            (x, y, z) = (newPosition.X, newPosition.Y, newPosition.Z);
                        }

                        var t = new Vector3(x, y, z);

                        t = t.FixUnityToMmd();

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

                        if (string.Equals(keyFrame.Path, "MODEL_00", StringComparison.Ordinal)) {
                            // The W component stores rotation
                            y += idolOffset.W;
                        }

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

                    int vmdFrameIndex;

                    if (_conversionConfig.Transform60FpsTo30Fps) {
                        vmdFrameIndex = mltdFrameIndex / 2;
                    } else {
                        vmdFrameIndex = mltdFrameIndex;
                    }

                    var mltdBoneName = GetMltdBoneNameWithoutBodyScale(boneNameCache, mltdBone.Path);
                    var vmdBoneName = boneLookup.GetVmdBoneNameFromBoneName(mltdBone.Path);
                    var boneFrame = new VmdBoneFrame(vmdFrameIndex, vmdBoneName);

                    var isMovable = BoneLookup.IsBoneMovable(mltdBoneName);

                    boneFrame.Position = isMovable ? t : Vector3.Zero;
                    boneFrame.Rotation = q;

                    boneFrameList.Add(boneFrame);

                    pmxBone.LocalPosition = t;
                    pmxBone.LocalRotation = q;
                    pmxBone.UpdateTransform();
                }
            }

            return boneFrameList.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        private static string GetMltdBoneNameWithoutBodyScale([NotNull] Dictionary<string, string> boneNameCache, [NotNull] KeyFrame keyFrame) {
            return GetMltdBoneNameWithoutBodyScale(boneNameCache, keyFrame.Path);
        }

        [NotNull]
        private static string GetMltdBoneNameWithoutBodyScale([NotNull] Dictionary<string, string> boneNameCache, [NotNull] string fullBonePath) {
            string mltdBoneName;

            if (boneNameCache.ContainsKey(fullBonePath)) {
                mltdBoneName = boneNameCache[fullBonePath];
            } else {
                if (fullBonePath.Contains(BoneLookup.BoneNamePart_BodyScale)) {
                    mltdBoneName = fullBonePath.Replace(BoneLookup.BoneNamePart_BodyScale, string.Empty);
                } else {
                    mltdBoneName = fullBonePath;
                }

                boneNameCache.Add(fullBonePath, mltdBoneName);
            }

            return mltdBoneName;
        }

        private static int CalculateSeekFrameTarget(int naturalFrameIndex, [NotNull] TimedList<int, int> seekFrameControls, ref int lastSoughtFrame, ref int seekFrameCounter) {
            if (seekFrameControls.TryGetCurrentValue(naturalFrameIndex, out var targetFrame)) {
                int result;

                if (targetFrame != lastSoughtFrame) {
                    seekFrameCounter = 0;
                    lastSoughtFrame = targetFrame;
                    result = targetFrame;
                } else {
                    result = lastSoughtFrame + seekFrameCounter;
                }

                seekFrameCounter += 1;

                return result;
            } else {
                // Use natural frame index
                return naturalFrameIndex;
            }
        }

        [NotNull]
        private static TimedList<int, int> CollectSeekFrames([CanBeNull] ScenarioObject scenario, int formationNumber) {
            var result = new TimedList<int, int>();

            if (scenario == null) {
                return result;
            }

            if (scenario.HasSeekFrameEvents()) {
                var events = scenario.Scenario.WhereToArray(s => s.Type == ScenarioDataType.DanceAnimationSeekFrame);

                foreach (var ev in events) {
                    Debug.Assert(ev != null, nameof(ev) + " != null");

                    if (ev.Idol != formationNumber + 10) {
                        continue;
                    }

                    var frameIndex = (int)Math.Round(ev.AbsoluteTime * FrameRate.Mltd);
                    var targetFrame = ev.SeekFrame;

                    result.AddOrUpdate(frameIndex, targetFrame);
                }
            }

            return result;
        }

        [NotNull]
        private static TimedList<int, Vector4[]> CollectFormationChanges([CanBeNull] ScenarioObject scenario, AppealType appealType) {
            var result = new TimedList<int, Vector4[]>();

            if (scenario == null) {
                return result;
            }

            if (scenario.HasFormationChangeEvents()) {
                var events = scenario.Scenario.WhereToArray(s => s.Type == ScenarioDataType.FormationChange);

                foreach (var ev in events) {
                    Debug.Assert(ev != null, nameof(ev) + " != null");

                    // Layer=0: applied to all appeal variants (including no appeal)
                    // Layer>0: applied to that appeal only
                    if (ev.Layer == 0 || ev.Layer == (int)appealType) {
                        var formations = ev.Formation;
                        Debug.Assert(formations != null && formations.Length > 0);

                        var frameIndex = (int)Math.Round(ev.AbsoluteTime * FrameRate.Mltd);

                        var f = new Vector4[formations.Length];

                        for (var i = 0; i < formations.Length; i += 1) {
                            var v = formations[i];
                            f[i] = new Vector4(v.X, v.Y, v.Z, v.W);
                        }

                        result.AddOrUpdate(frameIndex, f);
                    }
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
                Debug.WriteLine($"Warning: trying to mark bone {name} as MLTD key bone but the bone is missing from the model.");
            }
        }

        [NotNull]
        private static readonly Dictionary<string, string> BoneAttachmentMap = new Dictionary<string, string> {
            //["MODEL_00/BASE/MUNE1/MUNE2/KUBI"] = "KUBI",
            ["MODEL_00/BASE/MUNE1/MUNE2/KUBI/ATAMA"] = "KUBI/ATAMA"
        };

    }
}
