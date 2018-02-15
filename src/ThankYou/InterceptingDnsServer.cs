using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ARSoft.Tools.Net.Dns;
using JetBrains.Annotations;
using OpenMLTD.AllStarsTheater.Logging;

namespace OpenMLTD.ThankYou {
    public sealed class InterceptingDnsServer {

        public InterceptingDnsServer([NotNull] IPAddress upstreamDnsServerIP, [NotNull] IPAddress redirectToLocalIP, [NotNull, ItemNotNull] IReadOnlyList<Regex> redirectPatterns, [CanBeNull] PortForwardingManager manager) {
            _upstreamDnsServerIp = upstreamDnsServerIP;
            _redirectToLocalIp = redirectToLocalIP;
            _redirectPatterns = redirectPatterns;

            _state = DnsServerState.Stopped;

            _portForwardingManager = manager;

            _upstreamRelayClient = new DnsClient(new[] { upstreamDnsServerIP }, (int)NormalDnsQueryTimeout.TotalMilliseconds);
        }

        public void Start() {
            if (_state == DnsServerState.NotInitialized) {
                throw new InvalidOperationException("You have to initialize InterceptingDnsServer before using it.");
            }

            if (IsRunning) {
                return;
            }

            var dnsIPAddress = IPAddress.Any;
            var dnsServer = new DnsServer(dnsIPAddress, 10, 10);
            _dnsServer = dnsServer;
            dnsServer.QueryReceived += OnQueryReceived;

            dnsServer.Start();

            _state = DnsServerState.Running;
        }

        public void Stop() {
            if (!IsRunning) {
                return;
            }

            if (_dnsServer != null) {
                _dnsServer.Stop();
                _dnsServer.QueryReceived -= OnQueryReceived;
            }

            _state = DnsServerState.Stopped;
        }

        public bool IsRunning => _state == DnsServerState.Running;

        private bool IsAccessAllowed([NotNull] IPEndPoint clientEndPoint, [NotNull] DnsQuestion question) {
            return true;
        }

        private async Task OnQueryReceived(object sender, QueryReceivedEventArgs e) {
            await Task.Run(() => {
                var response = new DnsMessage {
                    ReturnCode = ReturnCode.ServerFailure
                };

                if (!(e.Query is DnsMessage query)) {
                    e.Response = response;
                    return;
                }

                query.IsQuery = false;

                var question = query.Questions.First();

                //Fantasy.Info($"New question: {question.Name.ToString()}");

                var accessAllowed = IsAccessAllowed(e.RemoteEndpoint, question);

                if (!accessAllowed) {
                    e.Response = response;
                    return;
                }

                var answer = ResolveDnsQuery(question);

                if (ShouldRedirect(question)) {
                    // Resolved with redirection
                    IdolLog.InfoFormat(Lang.Get("redirecting+tpl"), question.Name.ToString());

                    SetupReponse(query, question);

                    if (_portForwardingManager != null) {
                        SetupForwarding(answer);
                    }

                    e.Response = query;

                    return;
                }

                if (query.Questions.Count == 1 && answer != null) {
                    foreach (var record in answer.AnswerRecords) {
                        query.AnswerRecords.Add(record);
                    }

                    foreach (var record in answer.AdditionalRecords) {
                        query.AnswerRecords.Add(record);
                    }

                    // Resolved without redirection

                    query.ReturnCode = ReturnCode.NoError;

                    e.Response = query;

                    return;
                }

                // Failed to resolve ...
                query.ReturnCode = ReturnCode.ServerFailure;

                e.Response = query;
            });
        }

        [CanBeNull]
        private DnsMessage ResolveDnsQuery([NotNull] DnsQuestion question) {
            var answer = _upstreamRelayClient.Resolve(question.Name, question.RecordType, question.RecordClass);

            if (answer == null) {
                // Failed...
            }

            return answer;
        }

        private bool ShouldRedirect([NotNull] DnsQuestion question) {
            if (_redirectPatterns.Count == 0) {
                return question.RecordType == RecordType.A;
            }

            var domainName = question.Name.ToString();

            return _redirectPatterns.Any(re => re.IsMatch(domainName));
        }

        private void SetupReponse([NotNull] DnsMessage query, [NotNull] DnsQuestion question) {
            var record = new ARecord(question.Name, 100, _redirectToLocalIp);
            query.AnswerRecords.Add(record);
            query.ReturnCode = ReturnCode.NoError;
        }

        private void SetupForwarding([CanBeNull] DnsMessage answer) {
            if (_portForwardingManager == null) {
                throw new InvalidOperationException();
            }

            var response = answer?.AnswerRecords.OfType<ARecord>().FirstOrDefault();

            if (response == null || !response.Address.Equals(_redirectIP)) {
                return;
            }

            //Fantasy.InfoFormat("{0} : Found a record to proxy with https", response.Name);

            _redirectIP = response.Address;
            var ep = new IPEndPoint(_redirectIP, HttpsPort);
            _portForwardingManager.StartForwarding(ep);
        }

        public const int HttpsPort = 443;

        private static readonly TimeSpan NormalDnsQueryTimeout = TimeSpan.FromSeconds(8);

        private readonly IPAddress _upstreamDnsServerIp;
        private readonly IPAddress _redirectToLocalIp;

        // ???????
        // It requires deeper understading... What does it mean?
        private IPAddress _redirectIP;

        [NotNull, ItemNotNull]
        private readonly IReadOnlyList<Regex> _redirectPatterns;

        [CanBeNull]
        private readonly PortForwardingManager _portForwardingManager;

        [CanBeNull]
        private DnsServer _dnsServer;

        [NotNull]
        private readonly DnsClient _upstreamRelayClient;

        private DnsServerState _state;

        private enum DnsServerState {

            NotInitialized,
            Stopped,
            Running

        }

    }
}