using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AssetStudio;
using AssetStudio.Extended.CompositeModels;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using Imas.Data.Serialized;
using Imas.Data.Serialized.Sway;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Entities.Mltd;

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

                    result = new MeshWrapper(manager.assetsFileList, mesh, true);

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

                    var m = new MeshWrapper(manager.assetsFileList, mesh, false);
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

        public static (IBodyAnimationSource, IBodyAnimationSource, IBodyAnimationSource) LoadDance([NotNull] string filePath, int songPosition) {
            {
                // First try with legacy bundles
                var (dan, apa, apg) = LoadDanceLegacy(filePath, songPosition);

                if (dan != null && apa != null && apg != null) {
                    return (new LegacyBodyAnimationSource(dan), new LegacyBodyAnimationSource(apa), new LegacyBodyAnimationSource(apg));
                }
            }

            {
                // If failed, try the new one (from ~mid 2018?)
                var (dan, apa, apg) = LoadDanceCompiled(filePath, songPosition);

                if (dan != null && apa != null && apg != null) {
                    return (new CompiledBodyAnimationSource(dan), new CompiledBodyAnimationSource(apa), new CompiledBodyAnimationSource(apg));
                }
            }

            return (null, null, null);
        }

        public static (ScenarioObject, ScenarioObject, ScenarioObject) LoadScenario([NotNull] string filePath) {
            ScenarioObject main = null, landscape = null, portrait = null;

            const string mainScenarioEnds = "scenario_sobj";
            const string landscapeScenarioEnds = "scenario_yoko_sobj";
            const string portraitScenarioEnds = "scenario_tate_sobj";

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

                    if (behaviour.m_Name.EndsWith(mainScenarioEnds)) {
                        main = ser.Deserialize<ScenarioObject>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(landscapeScenarioEnds)) {
                        landscape = ser.Deserialize<ScenarioObject>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(portraitScenarioEnds)) {
                        portrait = ser.Deserialize<ScenarioObject>(behaviour);
                    }

                    if (main != null && landscape != null && portrait != null) {
                        break;
                    }
                }
            }

            return (main, landscape, portrait);
        }

        private static (CharacterImasMotionAsset, CharacterImasMotionAsset, CharacterImasMotionAsset) LoadDanceLegacy([NotNull] string filePath, int songPosition) {
            CharacterImasMotionAsset dan = null, apa = null, apg = null;

            var danComp = $"{songPosition:00}_dan.imo";
            var apaComp = $"{songPosition:00}_apa.imo";
            var apgComp = $"{songPosition:00}_apg.imo";

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

                    // Can't use EndsWith() here: some bundles have ".asset" postfixes (alstar_01.imo.unity3d: _apa.imo, _dan_imo.asset, _apg.imo.asset)
                    if (behaviour.m_Name.Contains(danComp)) {
                        dan = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.Contains(apaComp)) {
                        apa = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.Contains(apgComp)) {
                        apg = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    }

                    if (dan != null && apa != null && apg != null) {
                        break;
                    }
                }
            }

            return (dan, apa, apg);
        }

        private static (AnimationClip, AnimationClip, AnimationClip) LoadDanceCompiled([NotNull] string filePath, int songPosition) {
            AnimationClip dan = null, apa = null, apg = null;

            var danComp = $"{songPosition:00}_dan";
            var apaComp = $"{songPosition:00}_apa";
            var apgComp = $"{songPosition:00}_apg";

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.AnimationClip) {
                        continue;
                    }

                    var clip = obj as AnimationClip;

                    if (clip == null) {
                        throw new ArgumentNullException(nameof(clip), "One animation clip is null.");
                    }

                    if (clip.m_Name.Contains(danComp)) {
                        dan = clip;
                    } else if (clip.m_Name.Contains(apaComp)) {
                        apa = clip;
                    } else if (clip.m_Name.Contains(apgComp)) {
                        apg = clip;
                    }

                    if (dan != null && apa != null && apg != null) {
                        break;
                    }
                }
            }

            return (dan, apa, apg);
        }

        public static (SwayController Body, SwayController Head) LoadSwayControllers([NotNull] string bodyFilePath, [NotNull] string headFilePath) {
            var body = LoadSwayController(bodyFilePath);
            var head = LoadSwayController(headFilePath);

            SwayController.FixSwayReferences(body, head);

            return (Body: body, Head: head);
        }

        [CanBeNull]
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
