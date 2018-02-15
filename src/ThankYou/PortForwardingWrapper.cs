using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMLTD.AllStarsTheater.Core;
using OpenMLTD.AllStarsTheater.Net;

namespace OpenMLTD.ThankYou {
    internal sealed class PortForwardingWrapper : DisposableBase {

        public PortForwardingWrapper([NotNull] Lazy<IPEndPoint> localFunc, [NotNull] Lazy<IPEndPoint> remoteFunc) {
            _cancellationToken = new CancellationTokenSource();
            _forwarder = new TcpForwarderSlim();
            Task.Factory.StartNew(_ => _forwarder.Start(localFunc, remoteFunc), _cancellationToken);
        }

        public void Stop() {
            if (_isStopped) {
                return;
            }

            _forwarder.MainSocket.Close();
            _cancellationToken.Cancel();

            _isStopped = true;
        }

        protected override void Dispose(bool disposing) {
            Stop();

            if (disposing) {
                _forwarder.Dispose();
                _cancellationToken.Dispose();
            }
        }

        private bool _isStopped;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly TcpForwarderSlim _forwarder;

    }
}