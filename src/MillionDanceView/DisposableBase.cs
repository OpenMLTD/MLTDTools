using System;
using System.Runtime.CompilerServices;

namespace OpenMLTD.MillionDance.Viewer {
    public abstract class DisposableBase : IDisposable {

        ~DisposableBase() {
            if (IsDisposed) {
                return;
            }

            Dispose(false);

            IsDisposed = true;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Disposed;

        public bool IsDisposed { get; private set; }

        public void Dispose() {
            if (IsDisposed) {
                return;
            }

            Dispose(true);

            IsDisposed = true;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureNotDisposed() {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }
        }

        protected virtual void Dispose(bool disposing) {
        }

    }
}
