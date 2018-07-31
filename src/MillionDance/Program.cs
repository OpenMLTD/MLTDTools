using System.IO;
using MillionDance.Core;
using MillionDance.Entities.Mltd;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;
using UnityStudio.UnityEngine;
using UnityStudio.UnityEngine.Animation;

namespace MillionDance {
    internal static class Program {

        private static void Main() {
            var avatar = LoadAvatar();
            var mesh = LoadMesh();
            var newPmx = PmxCreator.Create(avatar, mesh, @"tex\body");

            using (var w = new PmxWriter(File.Open(@"C:\Users\MIC\Desktop\MikuMikuMoving64_v1275\te\mayu\mayu_gen.pmx", FileMode.Create, FileAccess.Write, FileShare.Write))) {
                w.Write(newPmx);
            }

            var (dan, _, _) = LoadDance();
            var cam = LoadCamera();
            var vmd = VmdCreator.CreateFrom(dan, cam, avatar, newPmx);

            using (var w = new VmdWriter(File.Open(@"C:\Users\MIC\Desktop\MikuMikuMoving64_v1275\te\out.vmd", FileMode.Create, FileAccess.Write, FileShare.Write))) {
                w.Write(vmd);
            }
        }

        public static Mesh LoadMesh() {
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

        private static Avatar LoadAvatar() {
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

            var ser = new MonoBehaviourSerializer();

            using (var fileStream = File.Open("Resources/dan_" + SongName + "_" + SongPosition + ".imo.unity3d", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var assetFile in bundle.AssetFiles) {
                        foreach (var preloadData in assetFile.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            switch (behaviour.Name) {
                                case "dan_" + SongName + "_" + SongPosition + "_dan.imo":
                                    behaviour = preloadData.LoadAsMonoBehaviour(false);
                                    dan = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                                    break;
                                case "dan_" + SongName + "_" + SongPosition + "_apa.imo":
                                    behaviour = preloadData.LoadAsMonoBehaviour(false);
                                    apa = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                                    break;
                                case "dan_" + SongName + "_" + SongPosition + "_apg.imo":
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

        private const string AvatarName = "ss001_015siz";
        private const string SongName = "hmt001";
        private const string SongPosition = "01";

    }
}
