using System.IO;
using JetBrains.Annotations;
using MillionDance.Entities.Mltd;
using MillionDance.Entities.Pmx;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;
using UnityStudio.Unity.Animation;

namespace MillionDance {
    internal static class Program {

        private static void Main([NotNull, ItemNotNull] string[] args) {
            var avatar = LoadAvatar();
            var pmx = LoadPmxModel();
            var (dan, _, _) = LoadDance();
            var vmd = VmdCreator.FromDanceData(dan, avatar, pmx);

            using (var w = new VmdWriter(File.Open(@"C:\Users\MIC\Desktop\MikuMikuMoving64_v1275\te\out.vmd", FileMode.Create, FileAccess.Write, FileShare.Write))) {
                w.Write(vmd);
            }
        }

        private static PmxModel LoadPmxModel() {
            PmxModel model;

            using (var fileStream = File.Open("Resources/mayu/mayu.pmx", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                model = PmxReader.ReadModel(fileStream);
            }

            return model;
        }

        private static Avatar LoadAvatar() {
            Avatar avatar = null;

            using (var fileStream = File.Open("Resources/cb_ss001_015siz.unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
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

        private static (CharacterImasMotionAsset, CharacterImasMotionAsset, CharacterImasMotionAsset) LoadDance() {
            CharacterImasMotionAsset dan = null, apa = null, apg = null;

            var ser = new MonoBehaviourSerializer();

            using (var fileStream = File.Open("Resources/dan_shtstr_01.imo.unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            switch (behaviour.Name) {
                                case "dan_shtstr_01_dan.imo":
                                    behaviour = preloadData.LoadAsMonoBehaviour(false);
                                    dan = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                                    break;
                                case "dan_shtstr_01_apa.imo":
                                    behaviour = preloadData.LoadAsMonoBehaviour(false);
                                    apa = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                                    break;
                                case "dan_shtstr_01_apg.imo":
                                    behaviour = preloadData.LoadAsMonoBehaviour(false);
                                    apg = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                                    break;
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


    }
}
