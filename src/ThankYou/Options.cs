using CommandLine;

namespace OpenMLTD.ThankYou {
    public sealed class Options {

        [Option('i', Default = false)]
        public bool ShowInfo { get; set; }

        [Option("debug", Default = false)]
        public bool IsDebug { get; set; }

    }
}
