using System;
using YamlDotNet.Serialization;

[Serializable]
public sealed class AssetBundleManifestClass {

    public int ManifestFileVersion { get; set; }

    [YamlMember(Alias = "CRC")]
    public string Crc { get; set; }

    public ManifestHashes Hashes { get; set; }

    public int HashAppended { get; set; }

    public ClassType[] ClassTypes { get; set; }

    public string[] Assets { get; set; }

    public object[] Dependencies { get; set; }

    public sealed class ManifestHashes {

        public ManifestHash AssetFileHash { get; set; }

        public ManifestHash TypeTreeHash { get; set; }

    }

    public sealed class ManifestHash {

        [YamlMember(Alias = "serializedVersion")]
        public int SerializedVersion { get; set; }

        public string Hash { get; set; }

    }

    public sealed class ClassType {

        public int Class { get; set; }

        public object Script { get; set; }

    }

}
