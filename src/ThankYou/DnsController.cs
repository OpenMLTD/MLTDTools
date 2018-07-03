using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using OpenMLTD.MiriTore.Logging;

namespace OpenMLTD.ThankYou {
    internal sealed class DnsController {

        public void StartDns([NotNull] DnsConfig config) {
            if (_interceptingDnsServer != null) {
                IdolLog.Info("Not started: DNS server is running.");
                return;
            }

            var patterns = new Regex[config.RedirectPatterns.Length];
            for (var i = 0; i < patterns.Length; ++i) {
                patterns[i] = new Regex(config.RedirectPatterns[i], RegexOptions.CultureInvariant);
            }

            var redirectToLocalIp = IPAddress.Parse(config.LocalIP);
            if (!CheckIP(redirectToLocalIp)) {
                IdolLog.InfoFormat(Lang.Get("ip_not_found+tpl"), redirectToLocalIp);
                return;
            }

            var upstreamDnsServerIp = IPAddress.Parse(config.UpstreamDns);
            _forwardingManager = new PortForwardingManager();
            _interceptingDnsServer = new InterceptingDnsServer(upstreamDnsServerIp, redirectToLocalIp, patterns, _forwardingManager);
            _interceptingDnsServer.Start();

            IdolLog.InfoFormat(Lang.Get("dns_started+tpl"), redirectToLocalIp);
        }

        public void StopDns() {
            if (_interceptingDnsServer == null) {
                return;
            }

            _interceptingDnsServer.Stop();
            _interceptingDnsServer = null;
            _forwardingManager = null;
        }

        private static bool CheckIP([NotNull] IPAddress localIP) {
            var ss = IPAddressHelper.GetAvailableIPAddresses();
            return ss.Any(s => s.Key.Equals(localIP));
        }

        private InterceptingDnsServer _interceptingDnsServer;
        private PortForwardingManager _forwardingManager;

    }
}
