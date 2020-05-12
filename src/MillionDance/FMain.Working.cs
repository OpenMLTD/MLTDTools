using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using AssetStudio.Extended.CompositeModels;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Entities.Extensions;
using OpenMLTD.MLTDTools.Applications.TDFacial.Entities;

namespace OpenMLTD.MillionDance {
    partial class FMain {

        private void DoWork(object state) {
            ConversionConfig PrepareConversionConfig(InputParams ip) {
                var cc = new ConversionConfig();

                cc.MotionFormat = ip.MotionSource;
                cc.ScaleToPmxSize = ip.ScalePmx;
                cc.ApplyPmxCharacterHeight = ip.ConsiderIdolHeight;
                cc.TranslateBoneNamesToMmd = ip.TranslateBoneNames;
                cc.AppendIKBones = ip.AppendLegIkBones;
                cc.FixMmdCenterBones = ip.FixCenterBones;
                cc.FixTdaBindingPose = ip.ConvertBindingPose;
                cc.AppendEyeBones = ip.AppendEyeBones;
                cc.HideUnityGeneratedBones = ip.HideUnityGeneratedBones;
                cc.SkeletonFormat = ip.MotionSource == MotionFormat.Mltd ? SkeletonFormat.Mltd : SkeletonFormat.Mmd;
                cc.TranslateFacialExpressionNamesToMmd = ip.TranslateFacialExpressionNames;
                cc.ImportPhysics = ip.ImportPhysics;

                cc.Transform60FpsTo30Fps = ip.TransformTo30Fps;
                cc.ScaleToVmdSize = ip.ScaleVmd;

                {
                    var mappingsJson = File.ReadAllText(ip.FacialExpressionMappingFilePath, Encoding.UTF8);
                    var mappingsObj = JsonConvert.DeserializeObject<FacialConfig>(mappingsJson);

                    var dict = new Dictionary<int, IReadOnlyDictionary<string, float>>();

                    foreach (var expr in mappingsObj.Expressions) {
                        var d2 = new Dictionary<string, float>();

                        foreach (var kv in expr.Data) {
                            d2[kv.Key] = kv.Value;
                        }

                        dict[expr.Key] = d2;
                    }

                    cc.FacialExpressionMappings = dict;
                }

                return cc;
            }

            Debug.Assert(InvokeRequired, "The worker procedure should be running on the worker thread.");

            Log("Worker started.");

            try {
                var p = (InputParams)state;

                ConversionConfig.Current = PrepareConversionConfig(p);

                if (p.IdolHeight <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(p.IdolHeight), "Invalid idol height.");
                }

                ScalingConfig.CharacterHeight = p.IdolHeight;

                do {
                    Log("Loading body avatar...");
                    var bodyAvatar = ResourceLoader.LoadBodyAvatar(p.InputBody);
                    if (bodyAvatar == null) {
                        Log("Failed to load body avatar.");
                        break;
                    }

                    Log("Loading body mesh...");
                    var bodyMesh = ResourceLoader.LoadBodyMesh(p.InputBody);
                    if (bodyMesh == null) {
                        Log("Failed to load body mesh.");
                        break;
                    }

                    Log("Loading head avatar...");
                    var headAvatar = ResourceLoader.LoadHeadAvatar(p.InputHead);
                    if (headAvatar == null) {
                        Log("Failed to load head avatar.");
                        break;
                    }

                    Log("Loading head avatar...");
                    var headMesh = ResourceLoader.LoadHeadMesh(p.InputHead);
                    if (headMesh == null) {
                        Log("Failed to load head mesh.");
                        break;
                    }

                    Log("Loading head/body sway controllers...");
                    var (bodySway, headSway) = ResourceLoader.LoadSwayControllers(p.InputBody, p.InputHead);
                    if (bodySway == null || headSway == null) {
                        Log("Failed to load sway controllers.");
                        break;
                    }

                    Log("Combining avatars and meshes...");
                    var combinedAvatar = CompositeAvatar.FromAvatars(bodyAvatar, headAvatar);
                    var combinedMesh = CompositeMesh.FromMeshes(bodyMesh, headMesh);

                    IBodyAnimationSource dance;
                    ScenarioObject baseScenario, facialExpr;

                    if (p.GenerateCharacterMotion) {
                        Log("Loading dance motion...");
                        (dance, _, _) = ResourceLoader.LoadDance(p.InputDance, p.SongPosition);
                        if (dance == null) {
                            Log("Failed to load dance data.");
                            break;
                        }

                        Log("Loading lip sync and facial expression...");
                        var (main, yoko, tate) = ResourceLoader.LoadScenario(p.InputFacialExpression);
                        if (main == null) {
                            Log("Failed to load base scenario object.");
                            break;
                        }

                        baseScenario = main;

                        if (main.HasFacialExpressionFrames()) {
                            facialExpr = main;
                        } else {
                            Log("Main scenario object does not contain facial expressions. Trying with landscape and portrait.");

                            if (yoko == null || tate == null) {
                                Log("Failed to load either landscape or portrait.");
                                break;
                            }

                            var foundFacialExpr = true;

                            switch (p.PreferredFacialExpressionSource) {
                                case InputParams.FallbackFacialExpressionSource.Landscape: {
                                    if (yoko.HasFacialExpressionFrames()) {
                                        facialExpr = yoko;
                                    } else if (tate.HasFacialExpressionFrames()) {
                                        Log("Facial expressions are not found in landscape, but found in portrait. Use portrait instead.");
                                        facialExpr = tate;
                                    } else {
                                        Log("No scenario object contains facial expressions.");
                                        facialExpr = null;
                                        foundFacialExpr = false;
                                    }

                                    break;
                                }
                                case InputParams.FallbackFacialExpressionSource.Portrait:
                                    if (yoko.HasFacialExpressionFrames()) {
                                        facialExpr = yoko;
                                    } else if (tate.HasFacialExpressionFrames()) {
                                        Log("Facial expressions are not found in portrait, but found in landscape. Use landscape instead.");
                                        facialExpr = tate;
                                    } else {
                                        Log("No scenario object contains facial expressions.");
                                        facialExpr = null;
                                        foundFacialExpr = false;
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(p.PreferredFacialExpressionSource), p.PreferredFacialExpressionSource, "Invalid facial expression source.");
                            }

                            if (!foundFacialExpr) {
                                break;
                            }
                        }
                    } else {
                        dance = null;
                        baseScenario = null;
                        facialExpr = null;
                    }

                    CharacterImasMotionAsset camera;

                    if (p.GenerateCameraMotion) {
                        Log("Loading camera motion...");
                        camera = ResourceLoader.LoadCamera(p.InputCamera);
                        if (camera == null) {
                            Log("Failed to load camera data.");
                            break;
                        }
                    } else {
                        camera = null;
                    }

                    do {
                        // Now file names are like "ch_pr001_201xxx.unity3d".
                        var avatarName = (new FileInfo(p.InputHead).Name).Substring(3, 12);

                        // ss001_015siz -> 015ss001
                        // Note: PMD allows max 19 characters in texture file names.
                        // In the format below, textures will be named like:
                        // tex\015ss001_01.png
                        // which is at the limit.
                        var texPrefix = avatarName.Substring(6, 3) + avatarName.Substring(0, 5);
                        texPrefix = @"tex\" + texPrefix + "_";

                        Log("Generating model...");

                        var pmxCreator = new PmxCreator();
                        var pmx = pmxCreator.CreateFrom(combinedAvatar, combinedMesh, bodyMesh.VertexCount, texPrefix, bodySway, headSway);

                        if (p.GenerateModel) {
                            Log("Saving model...");

                            using (var w = new PmxWriter(File.Open(p.OutputModel, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                                w.Write(pmx);
                            }
                        }

                        if (p.GenerateCharacterMotion) {
                            Log("Generating character motion...");

                            var creator = new VmdCreator {
                                ProcessBoneFrames = true,
                                ProcessCameraFrames = false,
                                ProcessFacialFrames = true,
                                ProcessLightFrames = false
                            };

                            var danceVmd = creator.CreateFrom(dance, combinedAvatar, pmx, null, baseScenario, facialExpr, p.SongPosition);

                            Log("Saving character motion...");

                            using (var w = new VmdWriter(File.Open(p.OutputCharacterAnimation, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                                w.Write(danceVmd);
                            }
                        }

                        if (p.GenerateCameraMotion) {
                            Log("Generating camera motion...");

                            if (p.UseMvd) {
                                var creator = new MvdCreator {
                                    ProcessBoneFrames = false,
                                    ProcessCameraFrames = true,
                                    ProcessFacialFrames = false,
                                    ProcessLightFrames = false
                                };

                                var motion = creator.CreateFrom(null, null, null, camera, null, null, p.SongPosition);

                                Log("Writing camera motion...");

                                using (var w = new MvdWriter(File.Open(p.OutputCamera, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                                    w.Write(motion);
                                }
                            } else {
                                var creator = new VmdCreator {
                                    ProcessBoneFrames = false,
                                    ProcessCameraFrames = true,
                                    ProcessFacialFrames = false,
                                    ProcessLightFrames = false
                                };

                                var motion = creator.CreateFrom(null, null, null, camera, null, null, p.SongPosition);

                                Log("Writing camera motion...");

                                using (var w = new VmdWriter(File.Open(p.OutputCamera, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                                    w.Write(motion);
                                }
                            }
                        }
                    } while (false);
                } while (false);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Log("Done.");

            Invoke(() => EnableMainControls(true));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Invoke([NotNull] Action action) {
            Invoke((Delegate)action);
        }

        private sealed class InputParams {

            public bool GenerateModel { get; set; }

            public bool GenerateCharacterMotion { get; set; }

            public bool GenerateCameraMotion { get; set; }

            public string InputHead { get; set; }

            public string InputBody { get; set; }

            public string InputDance { get; set; }

            public string InputFacialExpression { get; set; }

            public string InputCamera { get; set; }

            public string OutputModel { get; set; }

            public string OutputCharacterAnimation { get; set; }

            public string OutputCamera { get; set; }

            public MotionFormat MotionSource { get; set; }

            public bool ScalePmx { get; set; }

            public bool ConsiderIdolHeight { get; set; }

            public float IdolHeight { get; set; }

            public bool TranslateBoneNames { get; set; }

            public bool AppendLegIkBones { get; set; }

            public bool FixCenterBones { get; set; }

            public bool ConvertBindingPose { get; set; }

            public bool AppendEyeBones { get; set; }

            public bool HideUnityGeneratedBones { get; set; }

            public bool TranslateFacialExpressionNames { get; set; }

            public bool ImportPhysics { get; set; }

            public bool TransformTo30Fps { get; set; }

            public bool ScaleVmd { get; set; }

            public bool UseMvd { get; set; }

            public uint FixedFov { get; set; }

            public int SongPosition { get; set; }

            public string FacialExpressionMappingFilePath { get; set; }

            public FallbackFacialExpressionSource PreferredFacialExpressionSource { get; set; }

            public enum FallbackFacialExpressionSource {

                Landscape = 0,

                Portrait = 1,

            }

        }

    }
}
