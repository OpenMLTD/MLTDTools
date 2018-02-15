using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class BuildAssetBundles {

    [MenuItem("MLTD Tools/Build AssetBundles/Android")]
    public static void BuildAllAndroidAssetBundlesExtra() {
        PlatformBuildAllAssetBundles(BuildTarget.Android, "Android");
    }

    [MenuItem("MLTD Tools/Build AssetBundles/iOS")]
    public static void BuildAlliOSAssetBundles() {
        PlatformBuildAllAssetBundles(BuildTarget.iOS, "iOS");
    }

    private static void PlatformBuildAllAssetBundles(BuildTarget target, string platformName) {
        const string assetBundlesBasePath = "Assets/AssetBundles/";
        var platformAssetBundlesPath = assetBundlesBasePath + platformName;

        Directory.CreateDirectory(platformAssetBundlesPath);
        BuildPipeline.BuildAssetBundles(platformAssetBundlesPath, DefaultOptions, target);

        // Assuming all files has extension ".unity3d"

        var knownAssetBundleFilePaths = Directory.GetFiles(platformAssetBundlesPath, "*.unity3d", SearchOption.TopDirectoryOnly);

        var desBuilder = new DeserializerBuilder();
        var des = desBuilder.IgnoreUnmatchedProperties()
            .WithNamingConvention(new NullNamingConvention())
            .Build();

        foreach (var knownAssetBundleFilePath in knownAssetBundleFilePaths) {
            var assetBundlePath = Path.GetFullPath(knownAssetBundleFilePath);
            var manifestPath = assetBundlePath + ".manifest";

            var assetBundleFileInfo = new FileInfo(assetBundlePath);

            var manifestContent = File.ReadAllText(manifestPath, Encoding.UTF8);
            var manifest = des.Deserialize<AssetBundleManifestClass>(manifestContent);

            Debug.Log(string.Format("{0} # {1}", manifest.Hashes.AssetFileHash.Hash, assetBundleFileInfo.Name));
        }
    }

    private const BuildAssetBundleOptions DefaultOptions = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle;

}
