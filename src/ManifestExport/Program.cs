using System;
using System.IO;
using OpenMLTD.MiriTore;
using OpenMLTD.MiriTore.Database;

namespace ManifestExport {
    internal static class Program {

        private static void Main(string[] args) {
            if (args.Length < 1) {
                PrintUsage();
                return;
            }

            var data = File.ReadAllBytes(args[0]);

            var b = AssetInfoList.TryParse(data, out var assetInfoList);

            if (!b || assetInfoList == null) {
                Console.WriteLine("Manifest parsing failed.");
                return;
            }

            string outputFilePath;

            if (args.Length > 1) {
                outputFilePath = args[1];
            } else {
                var fileInfo = new FileInfo(args[0]);
                outputFilePath = fileInfo.FullName + ".txt";
            }

            using (var writer = new StreamWriter(outputFilePath, false, MltdConstants.Utf8WithoutBom)) {
                writer.WriteLine("Asset count: {0}", assetInfoList.Assets.Count.ToString());

                foreach (var asset in assetInfoList.Assets) {
                    writer.WriteLine();
                    writer.WriteLine("Resource name: {0}", asset.ResourceName);
                    writer.WriteLine("Resource hash: {0}", asset.ContentHash);
                    writer.WriteLine("Remote name: {0}", asset.RemoteName);
                    writer.WriteLine("File size: {0} ({1})", asset.Size.ToString(), MathUtilities.GetHumanReadableFileSize(asset.Size));
                }
            }
        }

        private static void PrintUsage() {
            Console.WriteLine("Usage: ManifestExport <input manifest> [<output txt>]");
        }

    }
}
