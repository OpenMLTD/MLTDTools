using System;
using System.IO;
using JetBrains.Annotations;
using MillionDance.Entities.Mvd;
using OpenTK;

namespace MillionDance.Extensions {
    internal static class BinaryWriterExtensions {

        public static void Write([NotNull] this BinaryWriter writer, Vector2 vector) {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }

        public static void Write([NotNull] this BinaryWriter writer, Vector3 vector) {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        public static void Write([NotNull] this BinaryWriter writer, Vector4 vector) {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
            writer.Write(vector.W);
        }

        public static void Write([NotNull] this BinaryWriter writer, Quaternion quaternion) {
            writer.Write(quaternion.X);
            writer.Write(quaternion.Y);
            writer.Write(quaternion.Z);
            writer.Write(quaternion.W);
        }

        public static void Write([NotNull] this BinaryWriter writer, [NotNull] InterpolationPair interpolation) {
            writer.Write((byte)interpolation.PointA.X);
            writer.Write((byte)interpolation.PointA.Y);
            writer.Write((byte)interpolation.PointB.X);
            writer.Write((byte)interpolation.PointB.Y);
        }

        public static void WriteInt32AsVarLenInt([NotNull] this BinaryWriter writer, int value, int size, bool unsignedUnderUInt16 = false) {
            switch (size) {
                case 1: {
                        if (unsignedUnderUInt16) {
                            writer.Write((byte)value);
                        } else {
                            if (value == -1) {
                                writer.Write(unchecked((byte)-1));
                            } else {
                                writer.Write((byte)value);
                            }
                        }

                        break;
                    }
                case 2: {
                        if (unsignedUnderUInt16) {
                            writer.Write((ushort)value);
                        } else {
                            if (value == -1) {
                                writer.Write(unchecked((uint)-1));
                            } else {
                                writer.Write((ushort)value);
                            }
                        }
                        break;
                    }
                case 4:
                    writer.Write(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }
        }

    }
}
