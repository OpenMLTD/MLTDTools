using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AssetStudio;
using AssetStudio.Extended.CompositeModels;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using Imas.Data.Serialized;
using Imas.Data.Serialized.Sway;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;
using OpenMLTD.MillionDance.Entities.Mltd;

namespace OpenMLTD.MillionDance.Core.IO {
    internal static class ResourceLoader {

        [NotNull]
        public static TransformHierarchies LoadTransformHierarchies([NotNull] string filePath) {
            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            var hierarchies = TransformHierarchies.FromAssets(manager);

            return hierarchies;
        }

        [CanBeNull]
        public static MeshWrapper LoadBodyMesh([NotNull] string filePath) {
            MeshWrapper result = null;

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            var lookup = SerializedObjectsLookup.Create(manager);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Mesh) {
                        continue;
                    }

                    var mesh = obj as Mesh;

                    if (mesh == null) {
                        throw new ArgumentNullException(nameof(mesh), "Body mesh is null.");
                    }

                    result = new MeshWrapper(lookup, mesh, TexturedMaterialExtraProperties.Body);

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

            var lookup = SerializedObjectsLookup.Create(manager);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Mesh) {
                        continue;
                    }

                    var mesh = obj as Mesh;

                    if (mesh == null) {
                        throw new ArgumentNullException(nameof(mesh), "One of head meshes is null.");
                    }

                    var m = new MeshWrapper(lookup, mesh, TexturedMaterialExtraProperties.Head);
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

        [NotNull]
        public static CameraAnimationSet<CharacterImasMotionAsset> LoadCamera([NotNull] string filePath, [CanBeNull] int? desiredCameraNumber) {
            CharacterImasMotionAsset cam = null, apa = null, apg = null, bpg = null;

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            var ser = new ScriptableObjectSerializer();
            int cameraNumber;

            if (desiredCameraNumber != null) {
                do {
                    // Some songs, e.g. Alliance Stardust (alstar), has more than one cameras.
                    // In these cases the user must select the appropriate camera number.
                    if (TryLoadSpecifiedCamera(manager, ser, desiredCameraNumber.Value, ref cam, ref apa, ref apg, ref bpg)) {
                        cameraNumber = desiredCameraNumber.Value;
                        break;
                    }

                    TryLoadFirstFoundCamera(manager, ser, ref cam, ref apa, ref apg, ref bpg);
                    cameraNumber = InvalidCameraNumber;
                } while (false);
            } else {
                TryLoadFirstFoundCamera(manager, ser, ref cam, ref apa, ref apg, ref bpg);
                cameraNumber = UnspecifiedCameraNumber;
            }

            return AnimationSet.CreateCamera(cameraNumber, cam, apg, apa, bpg);
        }

        [NotNull]
        public static DanceAnimationSet<IBodyAnimationSource> LoadDance([NotNull] string filePath, int motionNumber, int formationNumber) {
            IBodyAnimationSource defaultSource = null, anotherSource = null, specialSource = null, gorgeousSource = null;
            bool anyAnimationLoaded;
            int suggestedPosition;

            // About number of main dance animations and special/another appeal animations:
            // Most songs have 1 dance animation (i.e. animation for 1 idol, multiplied by 3/4/5 etc.) and 1 appeal animation
            // (i.e. for 1 idol when player completes FC before the big note). Or, n dance animation and n appeal animations
            // (e.g. 虹色letters [nijile], n=2 for position 1 and 2). Some songs have 1 dance animation and n appeal animations
            // (e.g. クルリウタ [kururi], n=3 for position 1, 2 and 3). Rarely a song has n dance animations and 1 appeal animation
            // (i.e. RE@DY!! [ready0], n=5). For each dance animation, there is a dan_ object; for each appeal animation, there
            // is an apa_ or apg_ object. So there isn't really a guarantee when dan, apa or apg is non-null.

            {
                // First try with legacy bundles
                var loaded = LoadDanceLegacy(filePath, motionNumber, formationNumber);

                suggestedPosition = loaded.SuggestedPosition;
                anyAnimationLoaded = loaded.Default != null || loaded.Special != null || loaded.Another != null || loaded.Gorgeous != null;

                if (loaded.Default != null) {
                    defaultSource = new LegacyBodyAnimationSource(loaded.Default);
                }

                if (loaded.Special != null) {
                    specialSource = new LegacyBodyAnimationSource(loaded.Special);
                }

                if (loaded.Another != null) {
                    anotherSource = new LegacyBodyAnimationSource(loaded.Another);
                }

                if (loaded.Gorgeous != null) {
                    gorgeousSource = new LegacyBodyAnimationSource(loaded.Gorgeous);
                }
            }

            if (!anyAnimationLoaded) {
                // If failed, try the new one (from ~mid 2018?)
                var loaded = LoadDanceCompiled(filePath, motionNumber, formationNumber);

                suggestedPosition = loaded.SuggestedPosition;

                if (loaded.Default != null) {
                    defaultSource = new CompiledBodyAnimationSource(loaded.Default);
                }

                if (loaded.Special != null) {
                    specialSource = new CompiledBodyAnimationSource(loaded.Special);
                }

                if (loaded.Another != null) {
                    anotherSource = new CompiledBodyAnimationSource(loaded.Another);
                }

                if (loaded.Gorgeous != null) {
                    gorgeousSource = new CompiledBodyAnimationSource(loaded.Gorgeous);
                }
            }

            var animationSet = AnimationSet.CreateDance(suggestedPosition, defaultSource, specialSource, anotherSource, gorgeousSource);

            return animationSet;
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

        [NotNull]
        private static DanceAnimationSet<CharacterImasMotionAsset> LoadDanceLegacy([NotNull] string filePath, int motionNumber, int formationNumber) {
            CharacterImasMotionAsset dan = null, apa = null, apg = null, bpg = null;

            var danComp = $"{motionNumber:00}_dan.imo";
            var apaComp = $"{formationNumber:00}_apa.imo";
            var apgComp = $"{formationNumber:00}_apg.imo";
            var bpgComp = $"{formationNumber:00}_bpg.imo";

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
                    } else if (behaviour.m_Name.Contains(bpgComp)) {
                        bpg = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    }

                    if (dan != null && apa != null && apg != null && bpg != null) {
                        break;
                    }
                }
            }

            var suggestedPosition = GetSuggestedDancePosition(manager);

            return AnimationSet.CreateDance(suggestedPosition, dan, apg, apa, bpg);
        }

        [NotNull]
        private static DanceAnimationSet<AnimationClip> LoadDanceCompiled([NotNull] string filePath, int motionNumber, int formationNumber) {
            AnimationClip dan = null, apa = null, apg = null, bpg = null;

            var danComp = $"{motionNumber:00}_dan";
            var apaComp = $"{formationNumber:00}_apa";
            var apgComp = $"{formationNumber:00}_apg";
            var bpgComp = $"{formationNumber:00}_bpg";

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
                    } else if (clip.m_Name.Contains(bpgComp)) {
                        bpg = clip;
                    }

                    if (dan != null && apa != null && apg != null && bpg != null) {
                        break;
                    }
                }
            }

            var suggestedPosition = GetSuggestedDancePosition(manager);

            return AnimationSet.CreateDance(suggestedPosition, dan, apg, apa, bpg);
        }

        private static bool TryLoadSpecifiedCamera([NotNull] AssetsManager manager, [NotNull] ScriptableObjectSerializer serializer, int cameraNumber, [CanBeNull] ref CharacterImasMotionAsset cam, [CanBeNull] ref CharacterImasMotionAsset apa, [CanBeNull] ref CharacterImasMotionAsset apg, [CanBeNull] ref CharacterImasMotionAsset bpg) {
            var camNumStr = cameraNumber.ToString("00");
            var defCamEnds = $"_{camNumStr}_cam.imo";
            var apaCamEnds = $"_{camNumStr}_apa.imo";
            var apgCamEnds = $"_{camNumStr}_apg.imo";
            var bpgCamEnds = $"_{camNumStr}_bpg.imo";
            // var apaPortraitCamEnds = $"_{camNumStr}_tate_apa.imo";
            // var apgPortraitCamEnds = $"_{camNumStr}_tate_apg.imo";
            // var bpgPortraitCamEnds = $"_{camNumStr}_tate_bpg.imo"; // should not exist

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    if (behaviour == null) {
                        throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                    }

                    if (behaviour.m_Name.EndsWith(defCamEnds)) {
                        cam = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apaCamEnds)) {
                        apa = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apgCamEnds)) {
                        apg = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(bpgCamEnds)) {
                        bpg = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    }

                    if (cam != null && apa != null && apg != null && bpg != null) {
                        break;
                    }
                }
            }

            return cam != null && apa != null && apg != null && bpg != null;
        }

        private static bool TryLoadFirstFoundCamera([NotNull] AssetsManager manager, [NotNull] ScriptableObjectSerializer serializer, [CanBeNull] ref CharacterImasMotionAsset cam, [CanBeNull] ref CharacterImasMotionAsset apa, [CanBeNull] ref CharacterImasMotionAsset apg, [CanBeNull] ref CharacterImasMotionAsset bpg) {
            const string defCamEnds = "_cam.imo";
            const string apaCamEnds = "_apa.imo";
            const string apgCamEnds = "_apg.imo";
            const string bpgCamEnds = "_bpg.imo";
            const string apaPortraitCamEnds = "_tate_apa.imo";
            const string apgPortraitCamEnds = "_tate_apg.imo";
            const string bpgPortraitCamEnds = "_tate_bpg.imo"; // should not exist

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    if (behaviour == null) {
                        throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                    }

                    if (behaviour.m_Name.EndsWith(defCamEnds)) {
                        cam = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apaCamEnds) && !behaviour.m_Name.EndsWith(apaPortraitCamEnds)) {
                        apa = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apgCamEnds) && !behaviour.m_Name.EndsWith(apgPortraitCamEnds)) {
                        apg = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(bpgCamEnds) && !behaviour.m_Name.EndsWith(bpgPortraitCamEnds)) {
                        bpg = serializer.Deserialize<CharacterImasMotionAsset>(behaviour);
                    }

                    if (cam != null && apa != null && apg != null && bpg != null) {
                        break;
                    }
                }
            }

            return cam != null && apa != null && apg != null && bpg != null;
        }

        public static (SwayController Body, SwayController Head) LoadSwayControllers([NotNull] string bodyFilePath, [NotNull] string headFilePath) {
            var body = LoadSwayController(bodyFilePath);

            if (body == null) {
                throw new ArgumentNullException(nameof(body), "Body sway is null.");
            }

            var head = LoadSwayController(headFilePath);

            if (head == null) {
                throw new ArgumentNullException(nameof(head), "Head sway is null.");
            }

            SwayAsset.FixSwayReferences(body, head);

            return (Body: body, Head: head);
        }

        internal const int InvalidDancePosition = -1;

        internal const int UnspecifiedCameraNumber = 0;

        internal const int InvalidCameraNumber = -1;

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

                    result = SwayAsset.Parse(str);

                    break;
                }
            }

            return result;
        }

        private static int GetSuggestedDancePosition([NotNull] AssetsManager manager) {
            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    Debug.Assert(behaviour != null);

                    var match = DanceAssetVaguePattern.Match(behaviour.m_Name);

                    if (match.Success) {
                        var posStr = match.Groups["position"].Value;
                        var pos = Convert.ToInt32(posStr);
                        return pos;
                    }
                }
            }

            return InvalidDancePosition;
        }

        [NotNull]
        private static readonly Regex ReplaceNewLine = new Regex("(?<!\r)\n", RegexOptions.CultureInvariant);

        [NotNull]
        private static readonly Regex DanceAssetVaguePattern = new Regex(@"(?<position>\d{2})_dan", RegexOptions.CultureInvariant);

    }
}
