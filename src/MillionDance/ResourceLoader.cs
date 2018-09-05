using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Mltd.Sway;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;
using UnityStudio.UnityEngine;
using UnityStudio.UnityEngine.Animation;
using UnityStudio.Utilities;

namespace MillionDance {
    internal static class ResourceLoader {

        [CanBeNull]
        public static Mesh LoadBodyMesh([NotNull] string filePath) {
            Mesh mesh = null;

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
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

        [CanBeNull]
        public static Avatar LoadBodyAvatar([NotNull] string filePath) {
            Avatar avatar = null;

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
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

        [CanBeNull]
        public static Mesh LoadHeadMesh([NotNull] string filePath) {
            var meshList = new List<Mesh>();

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
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

        [CanBeNull]
        public static Avatar LoadHeadAvatar([NotNull] string filePath) {
            Avatar avatar = null;

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
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

        [CanBeNull]
        public static CharacterImasMotionAsset LoadCamera([NotNull] string filePath) {
            CharacterImasMotionAsset cam = null;

            const string camEnds = "_cam.imo";

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            if (!behaviour.Name.EndsWith(camEnds)) {
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

        public static (CharacterImasMotionAsset, CharacterImasMotionAsset, CharacterImasMotionAsset) LoadDance([NotNull] string filePath, int songPosition) {
            CharacterImasMotionAsset dan = null, apa = null, apg = null;

            var danEnds = $"{songPosition:00}_dan.imo";
            var apaEnds = $"{songPosition:00}_apa.imo";
            var apgEnds = $"{songPosition:00}_apg.imo";

            var ser = new MonoBehaviourSerializer();

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            if (behaviour.Name.EndsWith(danEnds)) {
                                behaviour = preloadData.LoadAsMonoBehaviour(false);
                                dan = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                            } else if (behaviour.Name.EndsWith(apaEnds)) {
                                behaviour = preloadData.LoadAsMonoBehaviour(false);
                                apa = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                            } else if (behaviour.Name.EndsWith(apgEnds)) {
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

        [CanBeNull]
        public static ScenarioObject LoadScenario([NotNull] string filePath) {
            ScenarioObject result = null;

            const string scenarioEnds = "scenario_sobj";

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var asset in bundle.AssetFiles) {
                        foreach (var preloadData in asset.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            if (!behaviour.Name.EndsWith(scenarioEnds)) {
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

        public static (SwayController Body, SwayController Head) LoadSwayControllers([NotNull] string bodyFilePath, [NotNull] string headFilePath) {
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

            var body = LoadSwayController(bodyFilePath);
            var head = LoadSwayController(headFilePath);

            SwayController.FixSwayReferences(body, head);

            return (Body: body, Head: head);
        }

    }
}
