using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MessagePack;

namespace OpenMLTD.MiriTore.Database {
    public static class AssetInfoListExtensions {

        public static void SaveTo([NotNull] this AssetInfoList list, [NotNull] Stream stream) {
            var dict = new Dictionary<string, object[]>();

            foreach (var assetInfo in list.Assets) {
                var key = assetInfo.ResourceName;
                var arr = new object[3];

                arr[0] = assetInfo.ContentHash;
                arr[1] = assetInfo.RemoteName;

                if (assetInfo.Size > ushort.MaxValue) {
                    arr[2] = assetInfo.Size;
                } else {
                    arr[2] = (ushort)assetInfo.Size;
                }

                dict.Add(key, arr);
            }

            var databaseObject = new object[] {
                dict
            };

            MessagePackSerializer.Serialize(stream, databaseObject);
        }

        [NotNull]
        public static byte[] ToBytes([NotNull] this AssetInfoList list) {
            byte[] result;

            using (var stream = new MemoryStream()) {
                list.SaveTo(stream);

                result = stream.ToArray();
            }

            return result;
        }

    }
}
