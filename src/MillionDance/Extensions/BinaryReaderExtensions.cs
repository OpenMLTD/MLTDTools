using System;
using System.IO;
using JetBrains.Annotations;
using OpenTK;

namespace MillionDance.Extensions {
    internal static class BinaryReaderExtensions {

        public static Vector2 ReadVector2([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();

            return new Vector2(x, y);
        }

        public static Vector3 ReadVector3([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();

            return new Vector3(x, y, z);
        }

        public static Vector4 ReadVector4([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();

            return new Vector4(x, y, z, w);
        }

        public static Quaternion ReadQuaternion([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();

            return new Quaternion(x, y, z, w);
        }

        // TODO: Not fully compatible: https://gist.github.com/felixjones/f8a06bd48f9da9a4539f#index-types
        public static int ReadVarLenIntAsInt32([NotNull] this BinaryReader reader, int size, bool unsignedUnderUInt16 = false) {
            switch (size) {
                case 1: {
                        var value = reader.ReadByte();

                        if (unsignedUnderUInt16) {
                            return value;
                        } else {
                            if (value == unchecked((byte)-1)) {
                                return -1;
                            } else {
                                return value;
                            }
                        }
                    }
                case 2: {
                        var value = reader.ReadUInt16();

                        if (unsignedUnderUInt16) {
                            return value;
                        } else {
                            if (value == unchecked((ushort)-1)) {
                                return -1;
                            } else {
                                return value;
                            }
                        }
                    }
                case 4:
                    return reader.ReadInt32();
                default:
                    return 0;
            }
        }

    }
}
