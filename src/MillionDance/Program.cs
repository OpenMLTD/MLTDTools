using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MillionDance.Core;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Mltd.Sway;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;
using UnityStudio.UnityEngine;
using UnityStudio.UnityEngine.Animation;
using UnityStudio.Utilities;

namespace MillionDance {
    internal static class Program {

        private const bool WritePmx = true;
        private const bool CreateDanceVmd = false;
        private const bool WriteDanceVmd = false;
        private const bool CreateCameraVmd = false;
        private const bool WriteCameraVmd = false;

        private static void Main() {
            var bodyAvatar = LoadBodyAvatar();
            var bodyMesh = LoadBodyMesh();
            var headAvatar = LoadHeadAvatar();
            var headMesh = LoadHeadMesh();
            var (bodySway, headSway) = LoadSwayControllers();

            var combinedAvatar = CompositeAvatar.FromAvatars(bodyAvatar, headAvatar);
            var combinedMesh = CompositeMesh.FromMeshes(bodyMesh, headMesh);

            // ss001_015siz -> 015ss001
            // Note: PMD allows max 19 characters in texture file names.
            // In the format below, textures will be named like:
            // tex\015ss001_01.png
            // which is at the limit.
            var texPrefix = AvatarName.Substring(6, 3) + AvatarName.Substring(0, 5);
            texPrefix = @"tex\" + texPrefix + "_";

            var pmxCreator = new PmxCreator();

            var newPmx = pmxCreator.CreateFrom(combinedAvatar, combinedMesh, bodyMesh.VertexCount, texPrefix, bodySway, headSway);

            if (WritePmx) {
                var pmxFileName = $@"C:\Users\MIC\Desktop\MikuMikuMoving64_v1275\te\mayu\{AvatarName}_gen.pmx";
                using (var w = new PmxWriter(File.Open(pmxFileName, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                    w.Write(newPmx);
                }
            }

            if (CreateDanceVmd) {
                var (dan, _, _) = LoadDance();
                var scenario = LoadScenario();

                var vmdCreator = new VmdCreator {
                    ProcessBoneFrames = true,
                    ProcessCameraFrames = false,
                    ProcessFacialFrames = true,
                    ProcessLightFrames = false
                };

                var danceVmd = vmdCreator.CreateFrom(dan, combinedAvatar, newPmx, null, scenario, SongPosition);

                if (WriteDanceVmd) {
                    var danceVmdFileName = $@"C:\Users\MIC\Desktop\MikuMikuMoving64_v1275\te\{SongName}_{SongPosition:00}-{AvatarName}.vmd";
                    using (var w = new VmdWriter(File.Open(danceVmdFileName, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                        w.Write(danceVmd);
                    }
                }
            }

            if (CreateCameraVmd) {
                var cam = LoadCamera();

                var creator = new MvdCreator {
                    ProcessBoneFrames = false,
                    ProcessCameraFrames = true,
                    ProcessFacialFrames = false,
                    ProcessLightFrames = false
                };

                creator.ProcessBoneFrames = false;
                creator.ProcessCameraFrames = true;

                var cameraVmd = creator.CreateFrom(null, null, null, cam, null, SongPosition);

                if (WriteCameraVmd) {
                    var cameraVmdFileName = $@"C:\Users\MIC\Desktop\MikuMikuMoving64_v1275\te\{SongName}_cam.mvd";
                    using (var w = new MvdWriter(File.Open(cameraVmdFileName, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                        w.Write(cameraVmd);
                    }
                }
            }
        }

        private static Mesh LoadBodyMesh() {
            Mesh mesh = null;

            using (var fileStream = File.Open("Resources/cb_" + AvatarName + ".unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.Mesh) {
                                continue;
                            }

                            mesh = preloadData.LoadAsMesh();
                            break;
                        }
                    }
                }
            }

            return mesh;
        }

        private static Avatar LoadBodyAvatar() {
            Avatar avatar = null;

            using (var fileStream = File.Open("Resources/cb_" + AvatarName + ".unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.Avatar) {
                                continue;
                            }

                            avatar = preloadData.LoadAsAvatar();
                            break;
                        }
                    }
                }
            }

            return avatar;
        }

        private static Mesh LoadHeadMesh() {
            var meshList = new List<Mesh>();

            using (var fileStream = File.Open("Resources/ch_" + AvatarName + ".unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.Mesh) {
                                continue;
                            }

                            var mesh = preloadData.LoadAsMesh();

                            meshList.Add(mesh);
                        }
                    }
                }
            }

            var compositeMesh = CompositeMesh.FromMeshes(meshList);

            return compositeMesh;
        }

        private static Avatar LoadHeadAvatar() {
            Avatar avatar = null;

            using (var fileStream = File.Open("Resources/ch_" + AvatarName + ".unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.Avatar) {
                                continue;
                            }

                            avatar = preloadData.LoadAsAvatar();
                            break;
                        }
                    }
                }
            }

            return avatar;
        }

        private static CharacterImasMotionAsset LoadCamera() {
            CharacterImasMotionAsset cam = null;

            using (var fileStream = File.Open("Resources/cam_" + SongName + ".imo.unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            if (behaviour.Name != "cam_" + SongName + "_cam.imo") {
                                continue;
                            }

                            behaviour = preloadData.LoadAsMonoBehaviour(false);

                            var ser = new MonoBehaviourSerializer();
                            cam = ser.Deserialize<CharacterImasMotionAsset>(behaviour);

                            break;
                        }
                    }
                }
            }

            return cam;
        }

        private static (CharacterImasMotionAsset, CharacterImasMotionAsset, CharacterImasMotionAsset) LoadDance() {
            CharacterImasMotionAsset dan = null, apa = null, apg = null;

            var fileName = $"Resources/dan_{SongName}_{SongPosition:00}.imo.unity3d";
            var danName = $"dan_{SongName}_{SongPosition:00}_dan.imo";
            var apaName = $"dan_{SongName}_{SongPosition:00}_apa.imo";
            var apgName = $"dan_{SongName}_{SongPosition:00}_apg.imo";

            var ser = new MonoBehaviourSerializer();

            using (var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            if (behaviour.Name == danName) {
                                behaviour = preloadData.LoadAsMonoBehaviour(false);
                                dan = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                            } else if (behaviour.Name == apaName) {
                                behaviour = preloadData.LoadAsMonoBehaviour(false);
                                apa = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                            } else if (behaviour.Name == apgName) {
                                behaviour = preloadData.LoadAsMonoBehaviour(false);
                                apg = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                            }

                            if (dan != null && apa != null && apg != null) {
                                break;
                            }
                        }
                    }
                }
            }

            return (dan, apa, apg);
        }

        private static ScenarioObject LoadScenario() {
            ScenarioObject result = null;

            using (var fileStream = File.Open("Resources/scrobj_" + SongName + ".unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var asset in bundle.AssetFiles) {
                        foreach (var preloadData in asset.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            if (!behaviour.Name.EndsWith("scenario_sobj")) {
                                continue;
                            }

                            behaviour = preloadData.LoadAsMonoBehaviour(false);

                            var serializer = new MonoBehaviourSerializer();
                            result = serializer.Deserialize<ScenarioObject>(behaviour);
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private static (SwayController Body, SwayController Head) LoadSwayControllers() {
            SwayController LoadSwayController(string filePath) {
                SwayController result = null;

                using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (var bundle = new BundleFile(fileStream, false)) {
                        foreach (var assetFile in bundle.AssetFiles) {
                            foreach (var preloadData in assetFile.PreloadDataList) {
                                if (preloadData.KnownType != KnownClassID.TextAsset) {
                                    continue;
                                }

                                var textAsset = preloadData.LoadAsTextAsset(true);

                                if (!textAsset.Name.EndsWith("_sway")) {
                                    continue;
                                }

                                textAsset = preloadData.LoadAsTextAsset(false);

                                var text = textAsset.GetString();

                                Debug.Assert(!string.IsNullOrWhiteSpace(text));

                                result = SwayController.Parse(text);

                                break;
                            }
                        }
                    }
                }

                return result;
            }

            var body = LoadSwayController("Resources/cb_" + AvatarName + ".unity3d");
            var head = LoadSwayController("Resources/ch_" + AvatarName + ".unity3d");

            SwayController.FixSwayReferences(body, head);

            return (Body: body, Head: head);
        }

        private const string AvatarName = "gs001_201xxx";
        private const string SongName = "jiburi";
        private const int SongPosition = 1; // 1..5

    }
}
