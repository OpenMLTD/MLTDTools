using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.Web.TDAssets {
    // ReSharper disable once InconsistentNaming
    internal static class TDDownloader {

        [NotNull]
        public static Task<byte[]> DownloadData(int resourceVersion, [NotNull] string assetName) {
            return DownloadData(resourceVersion, assetName, TDAssetsApi.UnityVersion_Default);
        }

        [NotNull]
        public static async Task<byte[]> DownloadData(int resourceVersion, [NotNull] string assetName, [NotNull] string unityVersion) {
            var url = TDAssetsApi.AssetUrl(TDAssetsApi.Host_BN765, unityVersion, UnityMobilePlatform.Android, resourceVersion, assetName);
            var data = await WebCommon.HttpClient.GetByteArrayAsync(url);

            return data;
        }

        [NotNull]
        public static Task DownloadToFile(int resourceVersion, [NotNull] string assetName, [NotNull] string destinationPath) {
            return DownloadToFile(resourceVersion, assetName, destinationPath, TDAssetsApi.UnityVersion_Default);
        }

        [NotNull]
        public static async Task DownloadToFile(int resourceVersion, [NotNull] string assetName, [NotNull] string destinationPath, [NotNull] string unityVersion) {
            var data = await DownloadData(resourceVersion, assetName, unityVersion);

            var tmpFileName = Path.GetTempFileName();
            File.WriteAllBytes(tmpFileName, data);

            // TODO: some other exception handling (e.g. a file with desired dir name exist...)
            {
                var fileInfo = new FileInfo(destinationPath);
                var dir = fileInfo.DirectoryName;

                if (dir != null) {
                    if (!Directory.Exists(dir)) {
                        Directory.CreateDirectory(dir);
                    }
                }
            }

            File.Copy(tmpFileName, destinationPath, true);

            File.Delete(tmpFileName);
        }

    }
}
