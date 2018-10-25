using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mltd;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenMLTD.MillionDance.Utilities;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull, ItemNotNull]
        private static IReadOnlyList<VmdFacialFrame> CreateFacialFrames([NotNull] ScenarioObject scenarioObject, int songPosition) {
            VmdFacialFrame CreateFacialFrame(float time, string mltdTruncMorphName, float value) {
                var n = (int)(time * 60.0f);
                int frameIndex;

                if (ConversionConfig.Current.Transform60FpsTo30Fps) {
                    frameIndex = n / 2;
                } else {
                    frameIndex = n;
                }

                string expressionName;

                if (ConversionConfig.Current.TranslateFacialExpressionNamesToMmd) {
                    expressionName = MorphUtils.LookupMorphName(mltdTruncMorphName);
                } else {
                    expressionName = mltdTruncMorphName;
                }

                var frame = new VmdFacialFrame(frameIndex, expressionName);
                frame.Weight = value;

                return frame;
            }

            var facialFrameList = new List<VmdFacialFrame>();

            // Lip motion
            {
                var lipSyncControls = scenarioObject.Scenario.Where(s => s.Type == ScenarioDataType.LipSync).ToArray();

                Debug.Assert(lipSyncControls.Length > 0, "Lip-sync controls should exist.");
                Debug.Assert(lipSyncControls[0].Param == 54, "The first control op should be 54.");
                Debug.Assert(lipSyncControls[lipSyncControls.Length - 1].Param == 54, "The last control op should be 54.");

                const float lipTransitionTime = 0.2f;
                float lastLipSyncTime = 0;

                for (var i = 0; i < lipSyncControls.Length; i++) {
                    var sync = lipSyncControls[i];
                    var currentTime = (float)sync.AbsoluteTime;
                    var hasNext = i < lipSyncControls.Length - 1;
                    var hasPrev = i > 0;

                    switch (sync.Param) {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 50:
                            // The whole song ends with a "mouse-closed" (54) op.
                            Debug.Assert(hasNext, "The song should end with control op 54 (mouse closed).");
                            // The whole song starts with a "mouse-closed" (54) op.
                            Debug.Assert(hasPrev, "The song should start with control op 54 (mouse closed).");

                            string morphName;

                            switch (sync.Param) {
                                case 0:
                                    morphName = "M_a";
                                    break;
                                case 1:
                                    morphName = "M_i";
                                    break;
                                case 2:
                                    morphName = "M_u";
                                    break;
                                case 3:
                                    morphName = "M_e";
                                    break;
                                case 4:
                                    morphName = "M_o";
                                    break;
                                case 50:
                                    morphName = "M_n";
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("Not possible.");
                            }

                            var prevTime = (float)lipSyncControls[i - 1].AbsoluteTime;

                            if (currentTime - prevTime > lipTransitionTime) {
                                facialFrameList.Add(CreateFacialFrame(currentTime - lipTransitionTime, morphName, 0));
                            } else {
                                facialFrameList.Add(CreateFacialFrame(prevTime, morphName, 0));
                            }

                            facialFrameList.Add(CreateFacialFrame(currentTime, morphName, 1));

                            var nextTime = (float)lipSyncControls[i + 1].AbsoluteTime;

                            if (nextTime - currentTime > lipTransitionTime) {
                                facialFrameList.Add(CreateFacialFrame(nextTime - lipTransitionTime, morphName, 1));
                                facialFrameList.Add(CreateFacialFrame(nextTime, morphName, 0));
                            } else {
                                facialFrameList.Add(CreateFacialFrame(nextTime, morphName, 0));
                            }

                            break;
                        case 54:
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_a", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_i", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_u", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_e", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_o", 0));
                            facialFrameList.Add(CreateFacialFrame(currentTime, "M_n", 0));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(sync.Param), sync.Param, null);
                    }
                }
            }

            // Facial expression
            {
                var expControls = scenarioObject.Scenario.Where(s => s.Type == ScenarioDataType.FacialExpression && s.Idol == songPosition - 1).ToArray();

                Debug.Assert(expControls.Length > 0, "Expression controls should exist.");

                // Note that here we don't process blinkings (which happens in MLTD)
                for (var i = 0; i < expControls.Length; i++) {
                    var exp = expControls[i];
                    var currentTime = (float)exp.AbsoluteTime;

                    const float eyeBlinkTime = 0.1f;

                    var eyesClosedRatio = exp.EyeClosed ? 1.0f : 0.0f;

                    facialFrameList.Add(CreateFacialFrame(currentTime, "E_metoji_r", eyesClosedRatio));
                    facialFrameList.Add(CreateFacialFrame(currentTime, "E_metoji_l", eyesClosedRatio));

                    if (i > 0) {
                        if (expControls[i - 1].EyeClosed != exp.EyeClosed) {
                            facialFrameList.Add(CreateFacialFrame(currentTime - eyeBlinkTime, "E_metoji_r", 1 - eyesClosedRatio));
                            facialFrameList.Add(CreateFacialFrame(currentTime - eyeBlinkTime, "E_metoji_l", 1 - eyesClosedRatio));
                        }
                    }

                    do {
                        var expressionKey = (FacialExpression)exp.Param;

                        if (!FacialExpressionTable.ContainsKey(expressionKey)) {
                            Trace.TraceWarning("Facial expression key {0} is not found, using default emotion instead.", exp.Param);

                            expressionKey = FacialExpression.Default;
                        }

                        const float faceTransitionTime = 0.1333333f;

                        foreach (var kv in FacialExpressionTable[expressionKey]) {
                            if ((kv.Key != "E_metoji_r" && kv.Key != "E_metoji_l")) {
                                facialFrameList.Add(CreateFacialFrame(currentTime, kv.Key, kv.Value));
                            }
                        }

                        if (i > 0) {
                            if (expControls[i - 1].Param != exp.Param) {
                                var lastExpressionKey = (FacialExpression)expControls[i - 1].Param;

                                if (!FacialExpressionTable.ContainsKey(lastExpressionKey)) {
                                    Trace.TraceWarning("Facial expression key {0} is not found, using default emotion instead.", expControls[i - 1].Param);

                                    lastExpressionKey = FacialExpression.Default;
                                }

                                var expectedTransitionStartTime = currentTime - faceTransitionTime;

                                foreach (var kv in FacialExpressionTable[lastExpressionKey]) {
                                    // TODO: Actually we should do a more thorough analysis, because in this time window the eye CAN be opened again so we actually need these values.
                                    // But whatever. This case is rare. Fix it in the future.
                                    if ((kv.Key != "E_metoji_r" && kv.Key != "E_metoji_l")) {
                                        facialFrameList.Add(CreateFacialFrame(expectedTransitionStartTime, kv.Key, kv.Value));
                                    }
                                }

                            }
                        }
                    } while (false);
                }
            }

            return facialFrameList;
        }

    }
}
