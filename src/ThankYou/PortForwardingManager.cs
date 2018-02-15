using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace OpenMLTD.ThankYou {
    public sealed class PortForwardingManager {

        internal PortForwardingWrapper StartForwarding([NotNull] IPEndPoint remoteEndPoint) {
            CancelForwarders();

            var remoteFunc = new Lazy<IPEndPoint>(() => remoteEndPoint);
            var localFunc = new Lazy<IPEndPoint>(() => new IPEndPoint(IPAddress.Any, remoteEndPoint.Port));
            var wrapper = new PortForwardingWrapper(localFunc, remoteFunc);
            _currentForwarders.Add(wrapper);
            return wrapper;
        }

        private void CancelForwarders() {
            foreach (var forwarder in _currentForwarders) {
                forwarder.Stop();
            }
        }

        private readonly List<PortForwardingWrapper> _currentForwarders = new List<PortForwardingWrapper>();

    }
}