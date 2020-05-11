using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.Web {
    internal static class WebCommon {

        static WebCommon() {
            var messageHandler = new HttpClientHandler();
            InternalHttpClient = new HttpClient(messageHandler);
        }

        [NotNull]
        public static HttpClient HttpClient {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => InternalHttpClient;
        }

        [NotNull]
        private static readonly HttpClient InternalHttpClient;

    }
}
