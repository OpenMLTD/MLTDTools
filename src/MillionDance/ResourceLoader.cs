using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AssetStudio;
using AssetStudio.Extended.CompositeModels;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mltd;
using OpenMLTD.MillionDance.Entities.Mltd.Sway;

namespace OpenMLTD.MillionDance {
    internal static class ResourceLoader {

        [CanBeNull]
        public static MeshWrapper LoadBodyMesh([NotNull] string filePath) {
            MeshWrapper result = null;

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Mesh) {
                        continue;
                    }

                    var mesh = obj as Mesh;

                    if (mesh == null) {
                        throw new ArgumentNullException(nameof(mesh), "Body mesh is null.");
                    }

                    result = new MeshWrapper(mesh);

                    break;
                }
            }

            return result;
        }

        [CanBeNull]
        public static AvatarWrapper LoadBodyAvatar([NotNull] string filePath) {
            AvatarWrapper result = null;

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Avatar) {
                        continue;
                    }

                    var avatar = obj as Avatar;

                    if (avatar == null) {
                        throw new ArgumentNullException(nameof(avatar), "Body avatar is null.");
                    }

                    result = new AvatarWrapper(avatar);

                    break;
                }
            }

            return result;
        }

        [CanBeNull]
        public static CompositeMesh LoadHeadMesh([NotNull] string filePath) {
            var meshList = new List<PrettyMesh>();

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Mesh) {
                        continue;
                    }

                    var mesh = obj as Mesh;

                    if (mesh == null) {
                        throw new ArgumentNullException(nameof(mesh), "One of head meshes is null.");
                    }

                    var m = new MeshWrapper(mesh);
                    meshList.Add(m);
                }
            }

            var result = CompositeMesh.FromMeshes(meshList.ToArray());

            return result;
        }

        [CanBeNull]
        public static AvatarWrapper LoadHeadAvatar([NotNull] string filePath) {
            AvatarWrapper result = null;

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Avatar) {
                        continue;
                    }

                    var avatar = obj as Avatar;

                    if (avatar == null) {
                        throw new ArgumentNullException(nameof(avatar), "Head avatar is null.");
                    }

                    result = new AvatarWrapper(avatar);

                    break;
                }
            }

            return result;
        }

        [CanBeNull]
        public static CharacterImasMotionAsset LoadCamera([NotNull] string filePath) {
            CharacterImasMotionAsset cam = null;

            const string camEnds = "_cam.imo";

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    if (behaviour == null) {
                        throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                    }

                    if (!behaviour.m_Name.EndsWith(camEnds)) {
                        continue;
                    }

                    var ser = new ScriptableObjectSerializer();

                    cam = ser.Deserialize<CharacterImasMotionAsset>(behaviour);

                    break;
                }
            }

            return cam;
        }

        public static (CharacterImasMotionAsset, CharacterImasMotionAsset, CharacterImasMotionAsset) LoadDance([NotNull] string filePath, int songPosition) {
            CharacterImasMotionAsset dan = null, apa = null, apg = null;

            var danEnds = $"{songPosition:00}_dan.imo";
            var apaEnds = $"{songPosition:00}_apa.imo";
            var apgEnds = $"{songPosition:00}_apg.imo";

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            var ser = new ScriptableObjectSerializer();

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    if (behaviour == null) {
                        throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                    }

                    if (behaviour.m_Name.EndsWith(danEnds)) {
                        dan = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apaEnds)) {
                        apa = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apgEnds)) {
                        apg = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    }

                    if (dan != null && apa != null && apg != null) {
                        break;
                    }
                }
            }

            return (dan, apa, apg);
        }

        [CanBeNull]
        public static ScenarioObject LoadScenario([NotNull] string filePath) {
            ScenarioObject result = null;

            const string scenarioEnds = "scenario_sobj";

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    if (behaviour == null) {
                        throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                    }

                    if (!behaviour.m_Name.EndsWith(scenarioEnds)) {
                        continue;
                    }

                    var ser = new ScriptableObjectSerializer();

                    result = ser.Deserialize<ScenarioObject>(behaviour);

                    break;
                }
            }

            return result;
        }

        public static (SwayController Body, SwayController Head) LoadSwayControllers([NotNull] string bodyFilePath, [NotNull] string headFilePath) {
            var body = LoadSwayController(bodyFilePath);
            var head = LoadSwayController(headFilePath);

            SwayController.FixSwayReferences(body, head);

            return (Body: body, Head: head);
        }

        private static SwayController LoadSwayController([NotNull] string filePath) {
            SwayController result = null;

            const string swayEnds = "_sway";

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.TextAsset) {
                        continue;
                    }

                    var textAsset = obj as TextAsset;

                    if (textAsset == null) {
                        throw new ArgumentNullException(nameof(textAsset), "Sway data is null.");
                    }

                    if (!textAsset.m_Name.EndsWith(swayEnds)) {
                        continue;
                    }

                    var raw = textAsset.m_Script;
                    var str = Encoding.UTF8.GetString(raw);

                    str = ReplaceNewLine.Replace(str, "\n");

                    result = SwayController.Parse(str);

                    break;
                }
            }

            return result;
        }

        [NotNull]
        private static readonly Regex ReplaceNewLine = new Regex("(?<!\r)\n");

    }
}
