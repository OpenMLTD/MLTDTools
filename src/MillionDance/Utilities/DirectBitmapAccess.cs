using System;
using System.Drawing;
using System.Drawing.Imaging;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Utilities {
    internal sealed class DirectBitmapAccess : IDisposable {

        public DirectBitmapAccess([NotNull] Bitmap bitmap, ImageLockMode lockMode, bool premultiplied) {
            _bitmap = bitmap;
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var format = premultiplied ? PixelFormat.Format32bppPArgb : PixelFormat.Format32bppArgb;
            _bitmapData = bitmap.LockBits(rect, lockMode, format);
        }

        ~DirectBitmapAccess() {
            Dispose(false);
        }

        public bool IsDisposed { get; private set; }

        public int Width {
            get => _bitmapData.Width;
        }

        public int Height {
            get => _bitmapData.Height;
        }

        public void SetPixel(int x, int y, in Color color) {
            EnsureNotDisposed();

            if (x < 0 || x >= Width) {
                return;
            }

            if (y < 0 || y >= Height) {
                return;
            }

            var c = color.ToArgb();

            unsafe {
                var bytePtr = (byte*)_bitmapData.Scan0;
                var intPtr = (int*)(bytePtr + (y * _bitmapData.Stride)) + x;

                *intPtr = c;
            }
        }

        public Color GetPixel(int x, int y) {
            EnsureNotDisposed();

            if (x < 0 || x >= Width) {
                return TransparentBlack;
            }

            if (y < 0 || y >= Height) {
                return TransparentBlack;
            }

            int c;

            unsafe {
                var bytePtr = (byte*)_bitmapData.Scan0;
                var intPtr = (int*)(bytePtr + (y * _bitmapData.Stride)) + x;

                c = *intPtr;
            }

            return Color.FromArgb(c);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }

            IsDisposed = true;

            _bitmap.UnlockBits(_bitmapData);
        }

        private void EnsureNotDisposed() {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }
        }

        [NotNull]
        private readonly Bitmap _bitmap;

        [NotNull]
        private readonly BitmapData _bitmapData;

        private static readonly Color TransparentBlack = Color.FromArgb(0, 0, 0, 0);

    }
}
