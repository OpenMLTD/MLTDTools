using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Vmd;
using MillionDance.Utilities;

namespace MillionDance.Core {
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

                Debug.Assert(lipSyncControls.Length > 0);
                Debug.Assert(lipSyncControls[0].Param == 54);
                Debug.Assert(lipSyncControls[lipSyncControls.Length - 1].Param == 54);

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
                            Debug.Assert(hasNext);
                            // The whole song starts with a "mouse-closed" (54) op.
                            Debug.Assert(hasPrev);

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

                Debug.Assert(expControls.Length > 0);

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
                            Debug.Print("Warning: facial expression key {0} not found, using default emotion instead.", exp.Param);

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
                                    Debug.Print("Warning: facial expression key {0} not found, using default emotion instead.", expControls[i - 1].Param);

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

        private enum FacialExpression {

            Default = 0,
            VeryMildSmile = 1,
            StaringFarAway = 3,
            Happy = 5,
            RightEyeWink = 26 // Used in 自分REST@RT, the famous wink

        }

        // You can pose and test in PMXE
        private static readonly IReadOnlyDictionary<FacialExpression, IReadOnlyDictionary<string, float>> FacialExpressionTable = new Dictionary<FacialExpression, IReadOnlyDictionary<string, float>> {
            [FacialExpression.Default] = new Dictionary<string, float> {
                ["M_egao"] = 0,
                ["M_shinken"] = 0,
                ["M_wide"] = 0,
                ["M_up"] = 0,
                ["M_n2"] = 0,
                ["M_down"] = 0,
                ["M_odoroki"] = 0,
                ["M_narrow"] = 0,
                ["B_v_r"] = 0,
                ["B_v_l"] = 0,
                ["B_hati_r"] = 0,
                ["B_hati_l"] = 0,
                ["B_agari_r"] = 0,
                ["B_agari_l"] = 0,
                ["B_odoroki_r"] = 0,
                ["B_odoroki_l"] = 0,
                ["B_down"] = 0,
                ["B_yori"] = 0,
                ["E_metoji_r"] = 0,
                ["E_metoji_l"] = 0,
                ["E_wink_r"] = 0,
                ["E_wink_l"] = 0,
                ["E_open_r"] = 0,
                ["E_open_l"] = 0,
                ["EL_wide"] = 0,
                ["EL_up"] = 0,
            },
            // I can't differentiate this with the default expression...
            [FacialExpression.VeryMildSmile] = new Dictionary<string, float> {
                ["M_egao"] = 0,
                ["M_shinken"] = 0,
                ["M_wide"] = 0,
                ["M_up"] = 0,
                ["M_n2"] = 0,
                ["M_down"] = 0,
                ["M_odoroki"] = 0,
                ["M_narrow"] = 0,
                ["B_v_r"] = 0,
                ["B_v_l"] = 0,
                ["B_hati_r"] = 0,
                ["B_hati_l"] = 0,
                ["B_agari_r"] = 0,
                ["B_agari_l"] = 0,
                ["B_odoroki_r"] = 0,
                ["B_odoroki_l"] = 0,
                ["B_down"] = 0,
                ["B_yori"] = 0,
                ["E_metoji_r"] = 0,
                ["E_metoji_l"] = 0,
                ["E_wink_r"] = 0,
                ["E_wink_l"] = 0,
                ["E_open_r"] = 0,
                ["E_open_l"] = 0,
                ["EL_wide"] = 0,
                ["EL_up"] = 0,
            },
            [FacialExpression.StaringFarAway] = new Dictionary<string, float> {
                ["M_egao"] = 0,
                ["M_shinken"] = 0.32f,
                ["M_wide"] = 0,
                ["M_up"] = 0,
                ["M_n2"] = 0,
                ["M_down"] = 0,
                ["M_odoroki"] = 0,
                ["M_narrow"] = 0,
                ["B_v_r"] = 0.12f,
                ["B_v_l"] = 0.12f,
                ["B_hati_r"] = 0.40f,
                ["B_hati_l"] = 0.40f,
                ["B_agari_r"] = 0,
                ["B_agari_l"] = 0,
                ["B_odoroki_r"] = 0,
                ["B_odoroki_l"] = 0,
                ["B_down"] = 0,
                ["B_yori"] = 0,
                ["E_metoji_r"] = 0,
                ["E_metoji_l"] = 0,
                ["E_wink_r"] = 0,
                ["E_wink_l"] = 0,
                ["E_open_r"] = 0,
                ["E_open_l"] = 0,
                ["EL_wide"] = 0,
                ["EL_up"] = 0,
            },
            [FacialExpression.Happy] = new Dictionary<string, float> {
                ["M_egao"] = 0,
                ["M_shinken"] = 0,
                ["M_wide"] = 0,
                ["M_up"] = 0,
                ["M_n2"] = 0,
                ["M_down"] = 0,
                ["M_odoroki"] = 0,
                ["M_narrow"] = 0,
                ["B_v_r"] = 0,
                ["B_v_l"] = 0,
                ["B_hati_r"] = 0,
                ["B_hati_l"] = 0,
                ["B_agari_r"] = 0,
                ["B_agari_l"] = 0,
                ["B_odoroki_r"] = 0,
                ["B_odoroki_l"] = 0,
                ["B_down"] = 0,
                ["B_yori"] = 0,
                ["E_metoji_r"] = 0,
                ["E_metoji_l"] = 0,
                ["E_wink_r"] = 1.0f,
                ["E_wink_l"] = 1.0f,
                ["E_open_r"] = 0,
                ["E_open_l"] = 0,
                ["EL_wide"] = 0,
                ["EL_up"] = 0,
            },
            [FacialExpression.RightEyeWink] = new Dictionary<string, float> {
                ["M_egao"] = 0,
                ["M_shinken"] = 0,
                ["M_wide"] = 0,
                ["M_up"] = 0,
                ["M_n2"] = 0,
                ["M_down"] = 0,
                ["M_odoroki"] = 0,
                ["M_narrow"] = 0,
                ["B_v_r"] = 0,
                ["B_v_l"] = 0,
                ["B_hati_r"] = 0,
                ["B_hati_l"] = 0,
                ["B_agari_r"] = 0,
                ["B_agari_l"] = 0,
                ["B_odoroki_r"] = 0,
                ["B_odoroki_l"] = 0,
                ["B_down"] = 0,
                ["B_yori"] = 0,
                ["E_metoji_r"] = 0,
                ["E_metoji_l"] = 0,
                ["E_wink_r"] = 1,
                ["E_wink_l"] = 0,
                ["E_open_r"] = 0,
                ["E_open_l"] = 0,
                ["EL_wide"] = 0,
                ["EL_up"] = 0,
            },
        };

    }
}
