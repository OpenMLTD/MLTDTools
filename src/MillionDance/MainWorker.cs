using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AssetStudio.Extended.CompositeModels;
using Imas.Data.Serialized;
using Imas.Data.Serialized.Sway;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Core.IO;
using OpenMLTD.MillionDance.Entities.Extensions;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Utilities;
using OpenMLTD.MLTDTools.Applications.TDFacial.Entities;

namespace OpenMLTD.MillionDance {
    internal sealed class MainWorker {

        public MainWorker([NotNull] FMain form, [NotNull] MainWorkerInputParams inputParams) {
            _form = form;
            _inputParams = inputParams;
        }

        public void Start() {
            var thread = new Thread(DoWork);

            thread.Name = "Conversion thread";
            thread.IsBackground = true;

            thread.Start(_inputParams);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Log([NotNull] string message) {
            _form.Log(message);
        }

        private void DoWork([NotNull] object state) {
            try {
                DoWorkInternal(state);

                Log("Done.");
            } catch (Exception ex) {
                var exDesc = ex.ToString();

                Log("Error occurred.");
                Log(exDesc);

                InvokeInForm(() => {
                    MessageBox.Show(exDesc, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            } finally {
                InvokeInForm(() => _form.EnableMainControls(true));
            }
        }

        private void DoWorkInternal([NotNull] object state) {
            Debug.Assert(InvokeRequired, "The worker procedure should be running on the worker thread.");

            Log("Worker started.");

            var p = (MainWorkerInputParams)state;

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

                IBodyAnimationSource mainDance;
                IBodyAnimationSource danceAppeal = null;

                if (p.GenerateCharacterMotion) {
                    Log("Loading dance motion...");
                    var loadedDance = ResourceLoader.LoadDance(p.InputDance, p.MotionNumber, p.FormationNumber);
                    IBodyAnimationSource apSpecial, apAnother, apGorgeous;
                    (mainDance, apSpecial, apAnother, apGorgeous) = (loadedDance.AnimationSet.Default, loadedDance.AnimationSet.Special, loadedDance.AnimationSet.Another, loadedDance.AnimationSet.Gorgeous);
                    if (mainDance == null) {
                        if (MltdAnimation.MinMotion <= loadedDance.SuggestedPosition && loadedDance.SuggestedPosition <= MltdAnimation.MaxMotion) {
                            Log($"Cannot load dance data. However, this file may contain animation for idol using motion {loadedDance.SuggestedPosition.ToString()}. Please check whether you selected the correct motion number (in 'Motions' tab).");
                        } else {
                            Log("Cannot load dance data. Please check whether you selected a dance data file.");
                        }

                        break;
                    }

                    if (p.AppealType != AppealType.None) {
                        Log($"Trying to load dance appeal: {p.AppealType}");

                        switch (p.AppealType) {
                            case AppealType.Special: {
                                if (apSpecial != null) {
                                    danceAppeal = apSpecial;
                                }

                                break;
                            }
                            case AppealType.Another: {
                                if (apAnother != null) {
                                    danceAppeal = apAnother;
                                }

                                break;
                            }
                            case AppealType.Gorgeous: {
                                if (apGorgeous != null) {
                                    danceAppeal = apGorgeous;
                                }

                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        if (danceAppeal == null) {
                            Log($"Selected dance appeal for idol {p.FormationNumber.ToString()} is not found in the main dance data file. Trying with external file.");

                            if (string.IsNullOrWhiteSpace(p.ExternalDanceAppealFile)) {
                                Log("External dance appeal file is empty. Please set the path to the file.");
                                break;
                            }

                            var externalAppealData = ResourceLoader.LoadDance(p.ExternalDanceAppealFile, p.MotionNumber, p.FormationNumber);
                            (apSpecial, apAnother, apGorgeous) = (externalAppealData.AnimationSet.Special, externalAppealData.AnimationSet.Another, externalAppealData.AnimationSet.Gorgeous);

                            switch (p.AppealType) {
                                case AppealType.Special: {
                                    if (apSpecial != null) {
                                        danceAppeal = apSpecial;
                                    }

                                    break;
                                }
                                case AppealType.Another: {
                                    if (apAnother != null) {
                                        danceAppeal = apAnother;
                                    }

                                    break;
                                }
                                case AppealType.Gorgeous: {
                                    if (apGorgeous != null) {
                                        danceAppeal = apGorgeous;
                                    }

                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            if (danceAppeal == null) {
                                Log("Cannot load dance appeal data. Possible causes: 1) the file is not an appeal data file; 2) this song does not have the appeal you selected; 3) there is no corresponding appeal for selected position (e.g. each appeal of クルリウタ [Kururi Uta] only matches 3 out of 5 characters). Using default option (no appeal).");
                            }
                        }
                    }
                } else {
                    mainDance = null;
                    danceAppeal = null;
                }

                ScenarioObject baseScenario, landscapeScenario, portraitScenario;
                ScenarioObject formationInfo;

                if (p.GenerateCharacterMotion || p.GenerateLipSync || p.GenerateFacialExpressions || p.GenerateCameraMotion) {
                    (baseScenario, landscapeScenario, portraitScenario) = ResourceLoader.LoadScenario(p.InputScenario);
                    if (baseScenario == null) {
                        Log("Cannot load base scenario object.");
                        break;
                    }

                    if (landscapeScenario == null) {
                        Log("Cannot load landscape scenario.");
                        break;
                    }

                    if (portraitScenario == null) {
                        Log("Cannot load portrait scenario.");
                        break;
                    }
                } else {
                    baseScenario = null;
                    landscapeScenario = null;
                    portraitScenario = null;
                }

                if (p.GenerateCharacterMotion) {
                    Debug.Assert(baseScenario != null, nameof(baseScenario) + " != null");
                    Debug.Assert(landscapeScenario != null, nameof(landscapeScenario) + " != null");
                    Debug.Assert(portraitScenario != null, nameof(portraitScenario) + " != null");

                    if (baseScenario.HasFormationChangeEvents()) {
                        formationInfo = baseScenario;
                    } else {
                        Log("Main scenario object does not contain facial expressions. Trying with landscape and portrait.");

                        if (landscapeScenario.HasFormationChangeEvents()) {
                            Log("Using formation info from landscape.");
                            formationInfo = landscapeScenario;
                        } else if (portraitScenario.HasFormationChangeEvents()) {
                            Log("Using formation info from portrait.");
                            formationInfo = portraitScenario;
                        } else {
                            Log("No scenario object contains formation info.");
                            break;
                        }
                    }
                } else {
                    formationInfo = null;
                }

                ScenarioObject lipSyncInfo, facialExprInfo;

                if (p.GenerateLipSync || p.GenerateFacialExpressions) {
                    Debug.Assert(baseScenario != null, nameof(baseScenario) + " != null");
                    Debug.Assert(landscapeScenario != null, nameof(landscapeScenario) + " != null");
                    Debug.Assert(portraitScenario != null, nameof(portraitScenario) + " != null");

                    if (p.GenerateLipSync) {
                        lipSyncInfo = baseScenario;
                    } else {
                        lipSyncInfo = null;
                    }

                    if (p.GenerateFacialExpressions) {
                        if (baseScenario.HasFacialExpressionEvents()) {
                            facialExprInfo = baseScenario;
                        } else {
                            Log("Main scenario object does not contain facial expressions. Trying with landscape and portrait.");

                            var foundFacialExpr = true;

                            switch (p.PreferredFacialExpressionSource) {
                                case MainWorkerInputParams.FallbackFacialExpressionSource.Landscape: {
                                    if (landscapeScenario.HasFacialExpressionEvents()) {
                                        facialExprInfo = landscapeScenario;
                                        Log("Using facial expressions: landscape.");
                                    } else if (portraitScenario.HasFacialExpressionEvents()) {
                                        Log("Facial expressions are not found in landscape, but found in portrait. Use portrait instead.");
                                        facialExprInfo = portraitScenario;
                                    } else {
                                        Log("No scenario object contains facial expressions.");
                                        facialExprInfo = null;
                                        foundFacialExpr = false;
                                    }

                                    break;
                                }
                                case MainWorkerInputParams.FallbackFacialExpressionSource.Portrait:
                                    if (portraitScenario.HasFacialExpressionEvents()) {
                                        facialExprInfo = portraitScenario;
                                        Log("Using facial expressions: portrait.");
                                    } else if (landscapeScenario.HasFacialExpressionEvents()) {
                                        Log("Facial expressions are not found in portrait, but found in landscape. Use landscape instead.");
                                        facialExprInfo = landscapeScenario;
                                    } else {
                                        Log("No scenario object contains facial expressions.");
                                        facialExprInfo = null;
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
                        facialExprInfo = null;
                    }
                } else {
                    lipSyncInfo = null;
                    facialExprInfo = null;
                }

                CharacterImasMotionAsset mainCamera;
                CharacterImasMotionAsset cameraAppeal = null;

                if (p.GenerateCameraMotion) {
                    Log("Loading camera motion...");
                    CharacterImasMotionAsset apSpecial, apAnother, apGorgeous;
                    var loadedCamera = ResourceLoader.LoadCamera(p.InputCamera);
                    (mainCamera, apSpecial, apAnother, apGorgeous) = (loadedCamera.Default, loadedCamera.Special, loadedCamera.Another, loadedCamera.Gorgeous);
                    if (mainCamera == null) {
                        Log("Cannot load camera data.");
                        break;
                    }

                    if (p.AppealType != AppealType.None) {
                        Log($"Trying to load appeal for camera: {p.AppealType}");

                        switch (p.AppealType) {
                            case AppealType.Special: {
                                if (apSpecial != null) {
                                    cameraAppeal = apSpecial;
                                }

                                break;
                            }
                            case AppealType.Another: {
                                if (apAnother != null) {
                                    cameraAppeal = apAnother;
                                }

                                break;
                            }
                            case AppealType.Gorgeous: {
                                if (apGorgeous != null) {
                                    cameraAppeal = apGorgeous;
                                }

                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // Otherwise not possible; camera appeals are always stored in the main camera file
                        if (cameraAppeal == null) {
                            Log("Cannot load camera appeal data. Please check whether this song actually has the appeal you selected.");
                            break;
                        }
                    }
                } else {
                    mainCamera = null;
                }

                // And now the job starts!
                {
                    PmxModel pmx;
                    string texPrefix;
                    BakedMaterial[] materialList;

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
                        var pmxConversionDetails = new PmxCreator.ConversionDetails(texPrefix, p.ApplyGameStyledToon, p.SkinToonNumber, p.ClothesToonNumber);

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

                        foreach (var material in materialList) {
                            var textureFilePath = Path.Combine(modelDir, material.TextureName);

                            using (var memoryStream = new MemoryStream()) {
                                material.BakedTexture.Save(memoryStream, ImageFormat.Png);

                                using (var fileStream = File.Open(textureFilePath, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                                    memoryStream.Position = 0;
                                    memoryStream.CopyTo(fileStream);
                                }
                            }
                        }

                        foreach (var material in materialList) {
                            material.Dispose();
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

                        var danceVmd = creator.CreateCharacterAnimation(mainDance, baseScenario, formationInfo, combinedAvatar, pmx, danceAppeal, p.FormationNumber, p.AppealType);

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

                        var lipVmd = creator.CreateLipSync(lipSyncInfo, p.FormationNumber);

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

                        var facialVmd = creator.CreateFacialExpressions(facialExprInfo, p.FormationNumber);

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

                            var motion = creator.CreateCameraMotion(mainCamera, baseScenario, cameraAppeal, p.AppealType);

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

                            var motion = creator.CreateCameraMotion(mainCamera, baseScenario, cameraAppeal, p.AppealType);

                            Log("Writing camera motion...");

                            using (var w = new VmdWriter(File.Open(p.OutputCamera, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                                w.Write(motion);
                            }
                        }
                    }
                }
            } while (false);
        }

        private static ConversionConfig PrepareConversionConfig([NotNull] MainWorkerInputParams ip) {
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
            cc.AddHairHighlights = ip.AddHairHighlights;
            cc.AddEyesHighlights = ip.AddEyesHighlights;

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
        private void InvokeInForm([NotNull] Action action) {
            _form.Invoke(action);
        }

        private bool InvokeRequired {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _form.InvokeRequired;
        }

        [NotNull]
        private readonly FMain _form;

        [NotNull]
        private readonly MainWorkerInputParams _inputParams;

    }
}
