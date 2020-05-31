using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenMLTD.MillionDance.Utilities;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull]
        public VmdMotion CreateLipSync([CanBeNull] ScenarioObject baseScenario, int songPosition) {
            IReadOnlyList<VmdFacialFrame> frames;

            if (ProcessFacialFrames && baseScenario != null) {
                frames = CreateLipSyncFrames(baseScenario, songPosition);
            } else {
                frames = null;
            }

            return new VmdMotion(ModelName, null, frames, null, null, null);
        }

        [NotNull]
        public VmdMotion CreateFacialExpressions([CanBeNull] ScenarioObject facialExpr, int songPosition) {
            IReadOnlyList<VmdFacialFrame> frames;

            if (ProcessFacialFrames && facialExpr != null) {
                frames = CreateFacialExpressionFrames(facialExpr, songPosition);
            } else {
                frames = null;
            }

            return new VmdMotion(ModelName, null, frames, null, null, null);
        }

        [NotNull, ItemNotNull]
        private List<VmdFacialFrame> CreateLipSyncFrames([NotNull] ScenarioObject lipSync, int idolPosition) {
            var frameList = new List<VmdFacialFrame>();

            var lipSyncControls = lipSync.Scenario.Where(s => s.Type == ScenarioDataType.LipSync).ToArray();
            var singControlList = lipSync.Scenario.Where(s => s.Type == ScenarioDataType.SingControl).ToList();

            // Make sure the events are sorted.
            singControlList.Sort((s1, s2) => s1.AbsoluteTime.CompareTo(s2.AbsoluteTime));

            var singControls = singControlList.ToArray();
            var singControlTimes = singControls.Select(s => s.AbsoluteTime).ToArray();

            Debug.Assert(lipSyncControls.Length > 0, "Lip-sync controls should exist.");
            Debug.Assert(lipSyncControls[0].Param == 54, "The first control op should be 54.");
            Debug.Assert(lipSyncControls[lipSyncControls.Length - 1].Param == 54, "The last control op should be 54.");

            var lastFrameTime = float.NaN;

            for (var i = 0; i < lipSyncControls.Length; i++) {
                var sync = lipSyncControls[i];
                var currentTime = (float)sync.AbsoluteTime;

                if (currentTime < 0) {
                    // Some facial control frames have negative time (e.g. scrobj_s02ann), which eventually leads to negative frame indices when writing VMD file.
                    // MMM interprets this as uint64 (read int32 -> convert to int64 -> unchecked convert to uint64), MMD interprets this as int32.
                    // Both of them crash on negative frame indices. We have to avoid that.
                    continue;
                }

                if (float.IsNaN(lastFrameTime) && !currentTime.Equals(0)) {
                    // Manually insert a silence frame at the beginning.
                    AddSilenceFrame(frameList, 0);
                }

                var isSinging = IsSingingAt(singControls, singControlTimes, sync.AbsoluteTime, idolPosition);

                if (isSinging) {
                    var lipCode = (LipCode)sync.Param;

                    switch (lipCode) {
                        case LipCode.A:
                        case LipCode.I:
                        case LipCode.U:
                        case LipCode.E:
                        case LipCode.O:
                        case LipCode.N: {
                            // The whole song ends with a "mouse-closed" (54) op.
                            Debug.Assert(i < lipSyncControls.Length - 1, "The song should end with control op 54 (mouse closed).");
                            // The whole song starts with a "mouse-closed" (54) op.
                            Debug.Assert(i > 0, "The song should start with control op 54 (mouse closed).");

                            string morphName;

                            switch (lipCode) {
                                case LipCode.A:
                                    morphName = "M_a";
                                    break;
                                case LipCode.I:
                                    morphName = "M_i";
                                    break;
                                case LipCode.U:
                                    morphName = "M_u";
                                    break;
                                case LipCode.E:
                                    morphName = "M_e";
                                    break;
                                case LipCode.O:
                                    morphName = "M_o";
                                    break;
                                case LipCode.N:
                                    morphName = "M_n";
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(lipCode), lipCode, "Not possible.");
                            }

                            var prevTime = (float)lipSyncControls[i - 1].AbsoluteTime;

                            if (currentTime - prevTime > LipTransitionTime) {
                                frameList.Add(CreateFacialFrame(currentTime - LipTransitionTime, morphName, 0));
                            } else {
                                frameList.Add(CreateFacialFrame(prevTime, morphName, 0));
                            }

                            frameList.Add(CreateFacialFrame(currentTime, morphName, 1));

                            var nextTime = (float)lipSyncControls[i + 1].AbsoluteTime;

                            if (nextTime - currentTime > LipTransitionTime) {
                                frameList.Add(CreateFacialFrame(nextTime - LipTransitionTime, morphName, 1));
                                frameList.Add(CreateFacialFrame(nextTime, morphName, 0));
                            } else {
                                frameList.Add(CreateFacialFrame(nextTime, morphName, 0));
                            }

                            break;
                        }
                        case LipCode.Closed: {
                            AddSilenceFrame(frameList, currentTime);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lipCode), lipCode, "Not possible");
                    }
                } else {
                    // Muted
                    AddSilenceFrame(frameList, currentTime);
                }

                lastFrameTime = currentTime;
            }

            return frameList;
        }

        private void AddSilenceFrame([NotNull, ItemNotNull] List<VmdFacialFrame> frameList, float currentTime) {
            frameList.Add(CreateFacialFrame(currentTime, "M_a", 0));
            frameList.Add(CreateFacialFrame(currentTime, "M_i", 0));
            frameList.Add(CreateFacialFrame(currentTime, "M_u", 0));
            frameList.Add(CreateFacialFrame(currentTime, "M_e", 0));
            frameList.Add(CreateFacialFrame(currentTime, "M_o", 0));
            frameList.Add(CreateFacialFrame(currentTime, "M_n", 0));
        }

        [NotNull, ItemNotNull]
        private List<VmdFacialFrame> CreateFacialExpressionFrames([NotNull] ScenarioObject facialExpr, int idolPosition) {
            var frameList = new List<VmdFacialFrame>();

            var expControls = facialExpr.Scenario.Where(s => s.Type == ScenarioDataType.FacialExpression && s.Idol == idolPosition - 1).ToArray();

            Debug.Assert(expControls.Length > 0, "Expression controls should exist.");

            var mappings = _conversionConfig.FacialExpressionMappings;

            // Note that here we don't process blinks (which happens in MLTD)
            for (var i = 0; i < expControls.Length; i++) {
                var exp = expControls[i];
                var currentTime = (float)exp.AbsoluteTime;

                bool areEyesOpen;

                var eyesClosedRatio = exp.EyeClosed ? 1.0f : 0.0f;

                frameList.Add(CreateFacialFrame(currentTime, "E_metoji_r", eyesClosedRatio));
                frameList.Add(CreateFacialFrame(currentTime, "E_metoji_l", eyesClosedRatio));

                if (i > 0) {
                    if (expControls[i - 1].EyeClosed != exp.EyeClosed) {
                        frameList.Add(CreateFacialFrame(currentTime - HalfEyeBlinkTime, "E_metoji_r", 1 - eyesClosedRatio));
                        frameList.Add(CreateFacialFrame(currentTime - HalfEyeBlinkTime, "E_metoji_l", 1 - eyesClosedRatio));
                    }
                }

                if (exp.EyeClosed) {
                    areEyesOpen = false;
                } else {
                    do {
                        if (i > 0) {
                            if (expControls[i - 1].EyeClosed) {
                                areEyesOpen = false;
                                break;
                            }
                        }

                        if (i < expControls.Length - 1) {
                            if (expControls[i + 1].EyeClosed) {
                                if (currentTime >= expControls[i + 1].AbsoluteTime - HalfEyeBlinkTime) {
                                    areEyesOpen = false;
                                    break;
                                }
                            }
                        }

                        areEyesOpen = true;
                    } while (false);
                }

                {
                    // The key associated with a group of morph values, representing a whole facial expression
                    var expressionKey = exp.Param;

                    if (!mappings.ContainsKey(expressionKey)) {
                        Trace.TraceWarning("Facial expression key {0} is not found (at time {1}), using default emotion instead.", exp.Param, currentTime);

                        expressionKey = 0;
                    }

                    foreach (var kv in mappings[expressionKey]) {
                        var morphName = kv.Key;

                        if (EyesFilteredMorphs.Contains(morphName)) {
                            continue;
                        }

                        if (EyesAffectedMorphs.Contains(morphName) && !areEyesOpen) {
                            continue;
                        }

                        frameList.Add(CreateFacialFrame(currentTime, morphName, kv.Value));
                    }

                    // TODO: There is still one problem though, if an eye morph (say, E_wink_l) is activated BEFORE blinking, there is still a disaster.
                    //       But I am not sure how to handle that case. If that happens, it probably means something went wrong in animation directing.
                    //       Logical animations won't let you blink while you are winking.

                    if (i > 0) {
                        if (expControls[i - 1].Param != exp.Param) {
                            var lastExpressionKey = expControls[i - 1].Param;

                            if (!mappings.ContainsKey(lastExpressionKey)) {
                                Trace.TraceWarning("Facial expression key {0} is not found (at time {1}), using default emotion instead.", expControls[i - 1].Param, (float)expControls[i - 1].AbsoluteTime);

                                lastExpressionKey = 0;
                            }

                            var expectedTransitionStartTime = currentTime - FacialExpressionTransitionTime;

                            foreach (var kv in mappings[lastExpressionKey]) {
                                var morphName = kv.Key;

                                if (EyesFilteredMorphs.Contains(morphName)) {
                                    continue;
                                }

                                if (EyesAffectedMorphs.Contains(morphName) && !areEyesOpen) {
                                    continue;
                                }

                                // So... do we have to "restore" some morphs after blinking? I think not. Otherwise it will become very strange.
                                frameList.Add(CreateFacialFrame(expectedTransitionStartTime, morphName, kv.Value));
                            }
                        }
                    }
                }
            }

            return frameList;
        }

        [NotNull]
        private VmdFacialFrame CreateFacialFrame(float time, [NotNull] string mltdTruncMorphName, float value) {
            var n = (int)(time * FrameRate.Mltd);
            int frameIndex;

            if (_conversionConfig.Transform60FpsTo30Fps) {
                frameIndex = n / 2;
            } else {
                frameIndex = n;
            }

            string expressionName;

            if (_conversionConfig.TranslateFacialExpressionNamesToMmd) {
                expressionName = MorphUtils.LookupMorphName(mltdTruncMorphName);
            } else {
                expressionName = mltdTruncMorphName;
            }

            var frame = new VmdFacialFrame(frameIndex, expressionName);
            frame.Weight = value;

            return frame;
        }

        private static bool IsSingingAt([NotNull, ItemNotNull] EventScenarioData[] singControls, [NotNull] double[] singControlTimes, double lipSyncTime, int position) {
            const bool isSingingByDefault = true;

            Debug.Assert(singControls.Length == singControlTimes.Length);

            var muteControlPointCount = singControls.Length;

            if (muteControlPointCount == 0) {
                // In general, this case shouldn't happen.
                return isSingingByDefault;
            }

            var index = Array.BinarySearch(singControlTimes, lipSyncTime);
            EventScenarioData control;

            if (index >= 0) {
                control = singControls[index];
            } else {
                index = ~index;

                // MSDN:
                // If value is not found and value is less than one or more elements in array,
                // the negative number returned is the bitwise complement of the index of the
                // first element that is larger than value. If value is not found and value is
                // greater than all elements in array, the negative number returned is the
                // bitwise complement of (the index of the last element plus 1).
                if (index == 0) {
                    // lipSyncTime is earlier than the first control point
                    return isSingingByDefault;
                } else {
                    // index points to the next control point (maybe it does not exist, i.e.
                    // index equals the array length), and we should take the one before that.
                    control = singControls[index - 1];
                }
            }

            var isSinging = control.IsSinging;
            Debug.Assert(isSinging.Length >= position);

            return isSinging[position - 1];
        }

        [NotNull, ItemNotNull]
        private static readonly HashSet<string> EyesFilteredMorphs = new HashSet<string> {
            "E_metoji_l",
            "E_metoji_r",
        };

        // Used to skip generating animation frame while eyes are closed.
        [NotNull, ItemNotNull]
        private static readonly HashSet<string> EyesAffectedMorphs = new HashSet<string> {
            "E_wink_l",
            "E_wink_r",
            "E_open_l",
            "E_open_r",
        };

        private const float LipTransitionTime = 0.1f;

        // Wikipedia:
        // The duration of a blink is on average 100–150 milliseconds according to UCL researcher and
        // between 100–400 ms according to the Harvard Database of Useful Biological Numbers.
        //
        // Since we should consider both 30fps and 60fps, the minimum resolution for halfEyeBlinkTime is
        // 1/30s (0.0333s).
        private const float HalfEyeBlinkTime = 0.120f / 2;

        private const float FacialExpressionTransitionTime = 0.1f;

        private enum LipCode {

            A = 0,

            I = 1,

            U = 2,

            E = 3,

            O = 4,

            N = 50,

            Closed = 54,

        }

    }
}
