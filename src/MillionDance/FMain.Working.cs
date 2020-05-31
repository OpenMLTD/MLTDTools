using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using AssetStudio;
using AssetStudio.Extended.CompositeModels;
using Imas.Data.Serialized;
using Imas.Data.Serialized.Sway;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Core.IO;
using OpenMLTD.MillionDance.Entities.Extensions;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Utilities;
using OpenMLTD.MLTDTools.Applications.TDFacial.Entities;

namespace OpenMLTD.MillionDance {
    partial class FMain {

        private void DoWork([NotNull] object state) {
            try {
                DoWorkInternal(state);

                Log("Done.");
            } catch (Exception ex) {
                var exDesc = ex.ToString();

                Log("Error occurred.");
                Log(exDesc);

                Invoke(() => {
                    MessageBox.Show(exDesc, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            } finally {
                Invoke(() => EnableMainControls(true));
            }
        }

        private void DoWorkInternal([NotNull] object state) {
            Debug.Assert(InvokeRequired, "The worker procedure should be running on the worker thread.");

            Log("Worker started.");

            var p = (InputParams)state;

            var conversionConfig = PrepareConversionConfig(p);
            var scalingConfig = new ScalingConfig(conversionConfig);

            if (p.IdolHeight <= 0) {
                throw new ArgumentOutOfRangeException(nameof(p.IdolHeight), "Invalid idol height.");
            }

            scalingConfig.CharacterHeight = p.IdolHeight;

            do {
                CompositeAvatar combinedAvatar;
                CompositeMesh combinedMesh;
                int bodyMeshVertexCount;
                SwayController headSway;
                SwayController bodySway;

                if (p.GenerateModel || p.GenerateCharacterMotion) {
                    Log("Loading body avatar...");
                    var bodyAvatar = ResourceLoader.LoadBodyAvatar(p.InputBody);
                    if (bodyAvatar == null) {
                        Log("Cannot load body avatar.");
                        break;
                    }

                    Log("Loading body mesh...");
                    var bodyMesh = ResourceLoader.LoadBodyMesh(p.InputBody);
                    if (bodyMesh == null) {
                        Log("Cannot load body mesh.");
                        break;
                    }

                    Log("Loading head avatar...");
                    var headAvatar = ResourceLoader.LoadHeadAvatar(p.InputHead);
                    if (headAvatar == null) {
                        Log("Cannot load head avatar.");
                        break;
                    }

                    Log("Loading head avatar...");
                    var headMesh = ResourceLoader.LoadHeadMesh(p.InputHead);
                    if (headMesh == null) {
                        Log("Cannot load head mesh.");
                        break;
                    }

                    Log("Loading head/body sway controllers...");
                    (bodySway, headSway) = ResourceLoader.LoadSwayControllers(p.InputBody, p.InputHead);
                    if (bodySway == null || headSway == null) {
                        Log("Cannot load sway controllers.");
                        break;
                    }

                    Log("Combining avatars and meshes...");
                    combinedAvatar = CompositeAvatar.FromAvatars(bodyAvatar, headAvatar);
                    combinedMesh = CompositeMesh.FromMeshes(bodyMesh, headMesh);
                    bodyMeshVertexCount = bodyMesh.VertexCount;
                } else {
                    combinedAvatar = null;
                    combinedMesh = null;
                    bodyMeshVertexCount = 0;
                    bodySway = null;
                    headSway = null;
                }

                IBodyAnimationSource dance;

                if (p.GenerateCharacterMotion) {
                    Log("Loading dance motion...");
                    var loadedDance = ResourceLoader.LoadDance(p.InputDance, p.SongPosition);
                    (dance, _, _) = loadedDance.AnimationSet;
                    if (dance == null) {
                        if (1 <= loadedDance.SuggestedPosition && loadedDance.SuggestedPosition <= 5) {
                            Log($"Cannot load dance data. However, this file may contain animation for idol at position {loadedDance.SuggestedPosition.ToString()}. Please check whether you selected the correct idol position (in 'Motions' tab).");
                        } else {
                            Log("Cannot load dance data. Please check whether you selected a dance data file.");
                        }

                        break;
                    }
                } else {
                    dance = null;
                }

                ScenarioObject lipSync, facialExpr;

                if (p.GenerateLipSync || p.GenerateFacialExpressions) {
                    Log("Loading lip sync and facial expression...");
                    var (main, yoko, tate) = ResourceLoader.LoadScenario(p.InputFacialExpression);
                    if (main == null) {
                        Log("Cannot load base scenario object.");
                        break;
                    }

                    if (p.GenerateLipSync) {
                        lipSync = main;
                    } else {
                        lipSync = null;
                    }

                    if (p.GenerateFacialExpressions) {
                        if (main.HasFacialExpressionFrames()) {
                            facialExpr = main;
                        } else {
                            Log("Main scenario object does not contain facial expressions. Trying with landscape and portrait.");

                            if (yoko == null || tate == null) {
                                Log("Cannot load either landscape or portrait.");
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
                        facialExpr = null;
                    }
                } else {
                    lipSync = null;
                    facialExpr = null;
                }

                CharacterImasMotionAsset camera;

                if (p.GenerateCameraMotion) {
                    Log("Loading camera motion...");
                    (camera, _, _) = ResourceLoader.LoadCamera(p.InputCamera);
                    if (camera == null) {
                        Log("Cannot load camera data.");
                        break;
                    }
                } else {
                    camera = null;
                }

                {
                    PmxModel pmx;
                    string texPrefix;
                    (string FileName, TexturedMaterial Material)[] materialList;

                    if (p.GenerateModel || p.GenerateCharacterMotion) {
                        // Now file names are like "ch_pr001_201xxx.unity3d".
                        var avatarName = (new FileInfo(p.InputHead).Name).Substring(3, 12);

                        // ss001_015siz -> 015ss001
                        // Note: PMD allows max 19 characters in texture file names.
                        // In the format below, textures will be named like:
                        // tex\015ss001_01.png
                        // which is at the limit.
                        texPrefix = avatarName.Substring(6, 3) + avatarName.Substring(0, 5);
                        // TODO: PMX seems to store path in this way. If so, MillionDance only works on Windows.
                        texPrefix = $@"tex\{texPrefix}_";

                        Log("Generating model...");

                        Debug.Assert(combinedAvatar != null);
                        Debug.Assert(combinedMesh != null);

                        var boneLookup = new BoneLookup(conversionConfig);

                        var pmxCreator = new PmxCreator(conversionConfig, scalingConfig, boneLookup);
                        var pmxConversionDetails = new PmxCreator.ConversionDetails(texPrefix, p.GameStyledToon, p.ToonNumber);

                        pmx = pmxCreator.CreateModel(combinedAvatar, combinedMesh, bodyMeshVertexCount, bodySway, headSway, pmxConversionDetails, out materialList);
                    } else {
                        pmx = null;
                        texPrefix = null;
                        materialList = null;
                    }

                    if (p.GenerateModel) {
                        Log("Saving model...");

                        Debug.Assert(pmx != null);

                        using (var w = new PmxWriter(File.Open(p.OutputModel, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                            w.Write(pmx);
                        }

                        Log("Saving textures...");

                        var modelDir = (new FileInfo(p.OutputModel)).DirectoryName;
                        Debug.Assert(modelDir != null);

                        {
                            var texDir = (new FileInfo(Path.Combine(modelDir, texPrefix))).DirectoryName;
                            Debug.Assert(texDir != null);

                            if (!Directory.Exists(texDir)) {
                                Directory.CreateDirectory(texDir);
                            }
                        }

                        foreach (var (subFileName, mat) in materialList) {
                            var textureFilePath = Path.Combine(modelDir, subFileName);

                            using (var image = mat.MainTexture.ConvertToBitmap(mat.ExtraProperties.ShouldFlip)) {
                                // Only auto composite textures for eyes (with highlights)
                                if (mat.SubTexture != null && mat.MaterialName.Contains("eye")) {
                                    using (var subTex = mat.SubTexture.ConvertToBitmap(mat.ExtraProperties.ShouldFlip)) {
                                        using (var g = Graphics.FromImage(image)) {
                                            g.DrawImageUnscaled(subTex, 0, 0);
                                        }
                                    }
                                }

                                // Sometimes it throws AccessViolationException and the thread immediately halts (on ss001_016tsu,
                                // with hair/hairh composited; no exception when only eyes are composited)
                                image.Save(textureFilePath, ImageFormat.Png);
                            }
                        }
                    }

                    if (p.GenerateCharacterMotion) {
                        Log("Generating character motion...");

                        var creator = new VmdCreator(conversionConfig, scalingConfig) {
                            ProcessBoneFrames = true,
                            ProcessCameraFrames = false,
                            ProcessFacialFrames = false,
                            ProcessLightFrames = false
                        };

                        var danceVmd = creator.CreateCharacterAnimation(dance, combinedAvatar, pmx);

                        Log("Saving character motion...");

                        using (var w = new VmdWriter(File.Open(p.OutputCharacterAnimation, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                            w.Write(danceVmd);
                        }
                    }

                    if (p.GenerateLipSync) {
                        Log("Generating lip sync...");

                        var creator = new VmdCreator(conversionConfig, scalingConfig) {
                            ProcessBoneFrames = false,
                            ProcessCameraFrames = false,
                            ProcessFacialFrames = true,
                            ProcessLightFrames = false
                        };

                        var lipVmd = creator.CreateLipSync(lipSync, p.SongPosition);

                        Log("Saving lip sync...");

                        using (var w = new VmdWriter(File.Open(p.OutputLipSync, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                            w.Write(lipVmd);
                        }
                    }

                    if (p.GenerateFacialExpressions) {
                        Log("Generating facial expressions...");

                        var creator = new VmdCreator(conversionConfig, scalingConfig) {
                            ProcessBoneFrames = false,
                            ProcessCameraFrames = false,
                            ProcessFacialFrames = true,
                            ProcessLightFrames = false
                        };

                        var facialVmd = creator.CreateFacialExpressions(facialExpr, p.SongPosition);

                        Log("Saving facial expressions...");

                        using (var w = new VmdWriter(File.Open(p.OutputFacialExpressions, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                            w.Write(facialVmd);
                        }
                    }

                    if (p.GenerateCameraMotion) {
                        Log("Generating camera motion...");

                        if (p.UseMvdForCamera) {
                            var creator = new MvdCreator(conversionConfig, scalingConfig) {
                                ProcessBoneFrames = false,
                                ProcessCameraFrames = true,
                                ProcessFacialFrames = false,
                                ProcessLightFrames = false,
                            };

                            var motion = creator.CreateCameraMotion(camera);

                            Log("Writing camera motion...");

                            using (var w = new MvdWriter(File.Open(p.OutputCamera, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                                w.Write(motion);
                            }
                        } else {
                            var creator = new VmdCreator(conversionConfig, scalingConfig) {
                                ProcessBoneFrames = false,
                                ProcessCameraFrames = true,
                                ProcessFacialFrames = false,
                                ProcessLightFrames = false,
                            };

                            creator.FixedFov = p.FixedFov;

                            var motion = creator.CreateCameraMotion(camera);

                            Log("Writing camera motion...");

                            using (var w = new VmdWriter(File.Open(p.OutputCamera, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                                w.Write(motion);
                            }
                        }
                    }
                }
            } while (false);
        }

        private static ConversionConfig PrepareConversionConfig([NotNull] InputParams ip) {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Invoke([NotNull] Action action) {
            Invoke((Delegate)action);
        }

    }
}
