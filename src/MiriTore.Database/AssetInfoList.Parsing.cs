using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using MessagePack;

namespace OpenMLTD.MiriTore.Database {
    partial class AssetInfoList {

        [NotNull]
        public static AssetInfoList Parse([NotNull] byte[] data) {
            var deserialized = MessagePackSerializer.Deserialize<object>(data);

            var des = deserialized as object[];
            Debug.Assert(des != null, nameof(des) + " != null");
            Debug.Assert(des.Length == 1, nameof(des) + ".Length == 1");

            var dict = des[0] as IDictionary<object, object>;

            Debug.Assert(dict != null, nameof(dict) + " != null");

            var assetList = new List<AssetInfo>();

            foreach (var kv in dict) {
                var resourceName = kv.Key as string;
                Debug.Assert(resourceName != null, nameof(resourceName) + " != null");

                var arr = kv.Value as object[];
                Debug.Assert(arr != null, nameof(arr) + " != null");
                Debug.Assert(arr.Length == 3, nameof(arr) + ".Length == 3");

                var contentHash = arr[0] as string;
                Debug.Assert(contentHash != null, nameof(contentHash) + " != null");

                var remoteName = arr[1] as string;
                Debug.Assert(remoteName != null, nameof(remoteName) + " != null");

                uint size;

                switch (arr[2]) {
                    case uint u32:
                        size = u32;
                        break;
                    case ushort u16:
                        size = u16;
                        break;
                    default:
                        throw new InvalidCastException("Type mismatch at AssetInfo.Size field.");
                }

                var assetInfo = new AssetInfo(resourceName, contentHash, remoteName, size);

                assetList.Add(assetInfo);
            }

            return new AssetInfoList(assetList);
        }

        public static bool TryParse([NotNull] byte[] data, [CanBeNull] out AssetInfoList result) {
            try {
                result = Parse(data);
                return true;
            } catch (Exception) {
                result = null;
                return false;
            }
        }

    }
}
