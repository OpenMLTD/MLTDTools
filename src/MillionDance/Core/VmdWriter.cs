using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using MillionDance.Entities.Vmd;
using MillionDance.Extensions;

namespace MillionDance.Core {
    public sealed class VmdWriter : DisposableBase {

        public VmdWriter([NotNull] Stream stream) {
            _writer = new BinaryWriter(stream);
        }

        public void Write([NotNull] VmdMotion motion) {
            WriteMotion(motion);
        }

        protected override void Dispose(bool disposing) {
            _writer?.Dispose();
            _writer = null;

            base.Dispose(disposing);
        }

        private void WriteMotion([NotNull] VmdMotion motion) {
            WriteString($"Vocaloid Motion Data {motion.Version:0000}", 30);

            WriteString(motion.ModelName, 20);

            WriteBoneFrames();
            WriteFacialFrames();
            WriteCameraFrames();
            WriteLightFrames();

            _writer.Write(new byte[4]);

            WriteIkFrames();

            void WriteBoneFrames() {
                _writer.Write(motion.BoneFrames.Count);

                foreach (var frame in motion.BoneFrames) {
                    WriteBoneFrame(frame);
                }
            }

            void WriteFacialFrames() {
                _writer.Write(motion.FacialFrames.Count);

                foreach (var frame in motion.FacialFrames) {
                    WriteFacialFrame(frame);
                }
            }

            void WriteCameraFrames() {
                _writer.Write(motion.CameraFrames.Count);

                foreach (var frame in motion.CameraFrames) {
                    WriteCameraFrame(frame);
                }
            }

            void WriteLightFrames() {
                _writer.Write(motion.LightFrames.Count);

                foreach (var frame in motion.LightFrames) {
                    WriteLightFrame(frame);
                }
            }

            void WriteIkFrames() {
                if (motion.IkFrames == null) {
                    return;
                }

                _writer.Write(motion.IkFrames.Count);

                foreach (var frame in motion.IkFrames) {
                    WriteIkFrame(frame);
                }
            }
        }

        private void WriteBoneFrame([NotNull] VmdBoneFrame frame) {
            WriteString(frame.Name, 15);
            _writer.Write(frame.FrameIndex);
            _writer.Write(frame.Position);
            _writer.Write(frame.Rotation);
            WriteMultidimArray(frame.Interpolation);
        }

        private void WriteFacialFrame([NotNull] VmdFacialFrame frame) {
            WriteString(frame.FacialExpressionName, 15);
            _writer.Write(frame.FrameIndex);
            _writer.Write(frame.Weight);
        }

        private void WriteCameraFrame([NotNull] VmdCameraFrame frame) {
            _writer.Write(frame.FrameIndex);
            _writer.Write(frame.Length);
            _writer.Write(frame.Position);
            _writer.Write(frame.Orientation);
            WriteMultidimArray(frame.Interpolation);
            _writer.Write(frame.FieldOfView);
            WriteMultidimArray(frame.Unknown);
        }

        private void WriteLightFrame([NotNull] VmdLightFrame frame) {
            _writer.Write(frame.FrameIndex);
            _writer.Write(frame.Color);
            _writer.Write(frame.Position);
        }

        private void WriteIkControl([NotNull] IkControl ik) {
            WriteString(ik.Name, 20);
            _writer.Write(ik.Enabled);
        }

        private void WriteIkFrame([NotNull] VmdIkFrame frame) {
            _writer.Write(frame.FrameIndex);
            _writer.Write(frame.Visible);
            _writer.Write(frame.IkControls.Count);

            foreach (var ik in frame.IkControls) {
                WriteIkControl(ik);
            }
        }

        private void WriteString([NotNull] string str, int expectedLength) {
            var bytes = ShiftJisEncoding.GetBytes(str);
            byte[] result;

            if (bytes.Length < expectedLength) {
                result = new byte[expectedLength];
                Array.Copy(bytes, 0, result, 0, bytes.Length);
            } else if (bytes.Length == expectedLength) {
                result = bytes;
            } else {
                throw new ArgumentOutOfRangeException($"The string is too long. Should be at most {expectedLength} bytes.");
            }

            _writer.Write(result);
        }

        private void WriteMultidimArray([NotNull] Array array) {
            var len = array.Length;
            var buf = new byte[len];
            Buffer.BlockCopy(array, 0, buf, 0, len);
            _writer.Write(buf);
        }

        private static readonly Encoding ShiftJisEncoding = Encoding.GetEncoding("Shift-JIS");

        private BinaryWriter _writer;

    }
}
