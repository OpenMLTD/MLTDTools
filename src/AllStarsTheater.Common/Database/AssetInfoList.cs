using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using OpenMLTD.AllStarsTheater.Core;
using OpenMLTD.AllStarsTheater.Extensions;

namespace OpenMLTD.AllStarsTheater.Database {
    public sealed class AssetInfoList {

        public AssetInfoList() {
            Assets = new List<AssetInfo>();
            HeaderBytes = null;
        }

        private AssetInfoList([NotNull, ItemNotNull] List<AssetInfo> assets, [NotNull] byte[] headerBytes) {
            Assets = assets;
            HeaderBytes = headerBytes;
        }

        [NotNull, ItemNotNull]
        public List<AssetInfo> Assets { get; }

        /// <summary>
        /// Gets the read header bytes.
        /// </summary>
        [CanBeNull]
        internal byte[] HeaderBytes { get; }

#if DEBUG
        public static void Test() {
            var bytes = File.ReadAllBytes(@"Resources\Tests\db-9300.data");
            var list = Parse(bytes, MltdConstants.Utf8WithoutBom);
            bytes = list.ToBytes(MltdConstants.Utf8WithoutBom);
            File.WriteAllBytes(@"Resources\Tests\db-9300a.data", bytes);
        }
#endif

        [NotNull]
        public byte[] ToBytes([NotNull] Encoding encoding) {
            byte[] result;

            using (var stream = new MemoryStream()) {
                SaveTo(stream, encoding);

                result = stream.ToArray();
            }

            return result;
        }

        public void SaveTo([NotNull] Stream stream, [NotNull] Encoding encoding) {
            var assets = Assets;

            if (assets.Count > 0xffff) {
                throw new ArgumentOutOfRangeException(nameof(assets), "Current format does not allow storing over 65535 entries.");
            }

            stream.Write(FileHeader, 0, FileHeader.Length);

            using (var bigEndianWriter = new EndianBinaryWriter(stream, Endian.BigEndian)) {
                bigEndianWriter.Write((ushort)assets.Count);

                foreach (var asset in assets) {
                    if (asset == null) {
                        throw new ArgumentException("Assets in asset list must not be null.");
                    }

                    WriteString(stream, asset.ResourceName, encoding);
                    stream.WriteByte(asset.NameEndingByte);
                    WriteString(stream, asset.ContentHash, encoding);
                    WriteString(stream, asset.RemoteName, encoding);
                    WriteVarSize(bigEndianWriter, asset.Size);
                }
            }
        }

        public static bool TryParse([NotNull] byte[] data, [NotNull] Encoding encoding, [CanBeNull] out AssetInfoList result) {
            try {
                result = Parse(data, encoding);
                return true;
            } catch (Exception) {
                result = null;
                return false;
            }
        }

        [NotNull]
        public static AssetInfoList Parse([NotNull] byte[] data, [NotNull] Encoding encoding) {
            var assetInfoEntries = new List<AssetInfo>();
            byte[] headerBytes;
            int entryCount;

            using (var stream = new MemoryStream(data, false)) {
                var streamLength = stream.Length;

                using (var bigEndianReader = new EndianBinaryReader(stream, Endian.BigEndian)) {
                    // Assumption: "91 de" is the file header, followed by entry (entrycount+1)
                    headerBytes = stream.ReadBytes(4);

                    // Now read again.
                    stream.Position = 0;

                    var fileHeader = bigEndianReader.ReadUInt16();
                    if (fileHeader != FileHeaderInt) {
                        throw new FormatException($"File header is 0x{fileHeader:x4}, expected 0x91de.");
                    }

                    entryCount = bigEndianReader.ReadUInt16();

                    while (stream.Position < streamLength - 1) {
                        var resourceName = ReadString(stream, encoding);
                        var resourceNameEndingByte = (byte)stream.ReadByte();

                        if (resourceNameEndingByte != 0x93) {
                            throw new FormatException($"Expected name ending byte = 0x93, actual = 0x{resourceNameEndingByte:x2}.");
                        }

                        var resourceHash = ReadString(stream, encoding);
                        var remoteName = ReadString(stream, encoding);

                        var resourceSize = ReadVarSize(bigEndianReader);

                        var assetInfo = new AssetInfo(resourceName, resourceHash, remoteName, resourceSize, resourceNameEndingByte);

                        assetInfoEntries.Add(assetInfo);
                    }
                }
            }

            if (entryCount != assetInfoEntries.Count) {
                Debug.WriteLine($"Warning: recorded entry count {entryCount} does not equal to real entry count {assetInfoEntries.Count}!");
            }

            var assetInfoList = new AssetInfoList(assetInfoEntries, headerBytes);

            return assetInfoList;
        }

        [NotNull]
        private static string ReadString([NotNull] Stream stream, [NotNull] Encoding encoding) {
            var b = stream.ReadByte();
            int len;

            if (b == 0xd9) {
                // long name (>= 32 bytes)
                len = stream.ReadByte();
            } else {
                len = b & NameMask;
                var otherFlags = b & ~NameMask;
                if (otherFlags != NameFlagMask) {
                    throw new FormatException($"Invalid string record: byte1 != 0xd9 (actual: 0x{b:x2}) and otherFlags != 0x{NameFlagMask:x2}");
                }
            }

            var strBytes = stream.ReadBytes(len);

            return encoding.GetString(strBytes);
        }

        private static void WriteString([NotNull] Stream stream, [NotNull] string str, [NotNull] Encoding encoding) {
            var bytes = encoding.GetBytes(str);

            if (bytes.Length >= 256) {
                throw new FormatException("The string encoding method in AssetInfoList does not support strings longer than 256 bytes.");
            }

            if (bytes.Length >= 32) {
                stream.WriteByte(0xd9);
                stream.WriteByte((byte)bytes.Length);
            } else {
                var b = (byte)bytes.Length;
                b |= NameFlagMask;
                stream.WriteByte(b);
            }

            stream.Write(bytes, 0, bytes.Length);
        }

        private static uint ReadVarSize([NotNull] BinaryReader bigEndianReader) {
            var longSizeFlag = bigEndianReader.ReadByte();

            uint size;

            if (longSizeFlag == 0xcd || longSizeFlag == 0xce) {

                if (longSizeFlag == 0xce) {
                    size = bigEndianReader.ReadUInt32();
                } else {
                    size = bigEndianReader.ReadUInt16();
                }
            } else {
                throw new FormatException($"Invalid size flag: 0x{longSizeFlag:x2}, expected 0xcd or 0xce.");
            }

            return size;
        }

        private static void WriteVarSize([NotNull] BinaryWriter bigEndianWriter, uint size) {
            var isLongSize = size > 65535u;
            var longSizeFlag = isLongSize ? (byte)0xce : (byte)0xcd;

            bigEndianWriter.Write(longSizeFlag);

            if (isLongSize) {
                bigEndianWriter.Write(size);
            } else {
                bigEndianWriter.Write((ushort)size);
            }
        }

        private const int NameMask = 0x1f;
        private const int NameFlagMask = 0xa0; // = (read byte) & ~NameMask

        private static readonly byte[] FileHeader = { 0x91, 0xde };
        private static readonly ushort FileHeaderInt = 0x91de;

    }
}
