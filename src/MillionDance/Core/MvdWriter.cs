using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Mvd;
using OpenMLTD.MillionDance.Extensions;

namespace OpenMLTD.MillionDance.Core {
    public sealed class MvdWriter : DisposableBase {

        public MvdWriter([NotNull] Stream stream) {
            _writer = new BinaryWriter(stream);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write([NotNull] MvdMotion motion) {
            WriteMotion(motion);
        }

        protected override void Dispose(bool disposing) {
            _writer?.Dispose();
            _writer = null;

            base.Dispose(disposing);
        }

        private void WriteMotion([NotNull] MvdMotion motion) {
            WriteFixedLengthString(MvdHeaderString, Utf8WithoutBom, 30);

            _writer.Write(1.0f);
            _writer.Write((byte)1); // 0=unicode 1=utf-8

            // Now the main part.
            WriteMvdCameras(motion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteMvdCameras([NotNull] MvdMotion motion) {
            foreach (var m in motion.CameraMotions) {
                void WriteCameraMotion(MvdCameraMotion cameraMotion) {
                    WriteVariableLengthString(cameraMotion.DisplayName, Utf8WithoutBom, 30);
                    WriteVariableLengthString(cameraMotion.EnglishName, Utf8WithoutBom, 30);

                    _writer.Write(motion.Fps);

                    _writer.Write(0);
                    _writer.Write(false);
                    _writer.Write(false);
                    _writer.Write(0);
                    _writer.Write(0);

                    var startId = 0;
                    var stageCount = 0;

                    _writer.Write((long)1); // because we only export 1 camera

                    startId = WriteNameList(cameraMotion.Camera, startId, out var count);
                    stageCount += count;

                    startId = 0;

                    void WriteCamera(BinaryWriter writer, int id) {
                        writer.Write((byte)96);
                        writer.Write((byte)3);
                        writer.Write(id);
                        writer.Write(64);

                        writer.Write(cameraMotion.CameraFrames.Count);

                        writer.Write(4);
                        // stream.Write(BitConverter.GetBytes(cameraSequence.motiondata[boneindex].Count), 0, 4);
                        // It seems the value is always 1 in our case...
                        writer.Write(1);

                        var i = 0;

                        foreach (var frame in cameraMotion.CameraFrames) {
                            writer.Write(i); // layer index
                            writer.Write(frame.FrameNumber);

                            writer.Write(frame.Distance);
                            writer.Write(frame.Position);
                            writer.Write(frame.Rotation);
                            writer.Write(frame.FieldOfView);
                            writer.Write(frame.IsSpline);

                            WriteZeroBlocks(writer, 3);

                            writer.Write(frame.TranslationInterpolation);
                            writer.Write(frame.RotationInterpolation);
                            writer.Write(frame.DistanceInterpolation);
                            writer.Write(frame.FovInterpolation);
                        }

                        ++i;
                    }

                    void WriteCameraProperty(BinaryWriter writer, int id) {
                        writer.Write((byte)104);
                        writer.Write((byte)2);
                        writer.Write(id);
                        writer.Write(32);

                        writer.Write(cameraMotion.CameraPropertyFrames.Count); // 1 frame
                        writer.Write(0);

                        foreach (var frame in cameraMotion.CameraPropertyFrames) {
                            writer.Write(frame.FrameNumber);
                            writer.Write(frame.Enabled);
                            writer.Write(frame.IsPerspective);
                            writer.Write(frame.Alpha);
                            writer.Write(frame.EffectEnabled);
                            writer.Write(frame.DynamicFovEnabled);
                            writer.Write(frame.DynamicFovRate);
                            writer.Write(frame.DynamicFovCoefficient);
                            writer.Write(frame.RelatedModelId);
                            writer.Write(frame.RelatedBoneId);
                        }
                    }

                    WriteCamera(_writer, startId);
                    ++startId;
                    WriteCameraProperty(_writer, startId);
                    ++startId;

                    // Write EOF
                    _writer.Write((byte)0xff);
                    _writer.Write((byte)0x00);
                }

                WriteCameraMotion(m);
            }
        }

        private int WriteNameList([NotNull] MvdCameraObject camera, int startId, out int count) {
            count = 0;

            _writer.Write(startId);

            WriteVariableLengthString(camera.DisplayName, Utf8WithoutBom);

            ++count;

            // Omitted...

            return startId + 1;
        }

        private void WriteFixedLengthString([CanBeNull] string str, [NotNull] Encoding encoding, int length) {
            Debug.Assert(length > 0);

            byte[] arr;

            if (string.IsNullOrEmpty(str)) {
                arr = new byte[length];
                _writer.Write(arr);
            } else {
                var bytes = encoding.GetBytes(str);

                if (bytes.Length > length) {
                    throw new ArgumentOutOfRangeException("The string to write is too long.");
                }

                arr = new byte[length];

                Array.ConstrainedCopy(bytes, 0, arr, 0, bytes.Length);

                _writer.Write(arr);
            }
        }

        private void WriteVariableLengthString([CanBeNull] string str, [NotNull] Encoding encoding, int advicedLength = 0) {
            if (string.IsNullOrEmpty(str)) {
                _writer.Write(0);
            } else {
                var bytes = encoding.GetBytes(str);

                if (advicedLength > 0 && bytes.Length > advicedLength) {
                    Debug.Print("Warning: string length may be too long.");
                }

                _writer.Write(bytes.Length);
                _writer.Write(bytes);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteZeroBlocks([NotNull] BinaryWriter writer, int length) {
            var bytes = new byte[length];
            writer.Write(bytes);
        }

        private const string MvdHeaderString = "Motion Vector Data file";

        [NotNull]
        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

        private BinaryWriter _writer;

    }
}
