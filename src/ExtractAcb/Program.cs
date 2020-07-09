using System;
using System.Diagnostics;
using System.IO;
using AssetStudio;
using JetBrains.Annotations;

namespace ExtractAcb {
    internal static class Program {

        public static void Main([NotNull, ItemNotNull] string[] args) {
            foreach (var name in args) {
                if (File.Exists(name)) {
                    if (IsAcbBundle(name)) {
                        ExtractSingleAcb(name);
                    } else {
                        Console.Error.WriteLine("Not a ACB bundle: {0}", name);
                    }
                } else if (Directory.Exists(name)) {
                    ExtractDir(name);
                } else {
                    Console.Error.WriteLine("Invalid entry: {0}", name);
                }
            }
        }

        private static void ExtractDir([NotNull] string dir) {
            foreach (var fileName in Directory.EnumerateFiles(dir)) {
                if (IsAcbBundle(fileName)) {
                    ExtractSingleAcb(fileName);
                }
            }

            foreach (var d in Directory.EnumerateDirectories(dir)) {
                ExtractDir(d);
            }
        }

        private static void ExtractSingleAcb([NotNull] string assetBundlePath) {
            Console.WriteLine("Extracting: {0}", assetBundlePath);

            var dir = new FileInfo(assetBundlePath).DirectoryName;
            Debug.Assert(dir != null);

            var manager = new AssetsManager();
            manager.LoadFiles(assetBundlePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var o in assetFile.Objects) {
                    if (o.type != ClassIDType.TextAsset) {
                        continue;
                    }

                    var a = (TextAsset)o;

                    var acbName = a.m_Name;
                    var acbData = a.m_Script;
                    var acbPath = Path.Combine(dir, acbName);

                    using (var file = File.Open(acbPath, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                        file.Write(acbData, 0, acbData.Length);
                    }
                }
            }
        }

        private static bool IsAcbBundle([NotNull] string fileName) {
            return fileName.EndsWith(".acb.unity3d");
        }

    }
}
