using CommandLine;
using DereTore.Exchange.Audio.HCA;
using OpenMLTD.MiriTore.Mltd;

namespace OpenMLTD.MLTDTools.Applications.HcaDec {
    public sealed class Options {

        [Value(0)]
        public string InputFileName { get; set; } = string.Empty;

        [Option('o', "out", HelpText = "Output file name", Required = false)]
        public string OutputFileName { get; set; } = string.Empty;

        [Option('a', "key1", HelpText = "Key 1 (8 hex digits)", Required = false)]
        public string Key1 { get; set; } = MltdHcaCipher.Key1.ToString("x8");

        [Option('b', "key2", HelpText = "Key 2 (8 hex digits)", Required = false)]
        public string Key2 { get; set; } = MltdHcaCipher.Key2.ToString("x8");

        [Option("infinite", HelpText = "Enables infinite loop", Required = false, Default = false)]
        public bool InfiniteLoop { get; set; } = AudioParams.Default.InfiniteLoop;

        [Option('l', "loop", HelpText = "Number of simulated loops", Required = false, Default = 0u)]
        public uint SimulatedLoopCount { get; set; } = AudioParams.Default.SimulatedLoopCount;

        [Option('e', "no-header", HelpText = "Do not emit wave header", Required = false, Default = false)]
        public bool NoWaveHeader { get; set; }

        [Option("overrides-cipher", HelpText = "Overrides original cipher type", Required = false, Default = false)]
        public bool OverridesCipherType { get; set; }

        [Option('c', "cipher", HelpText = "Overridden cipher type", Required = false, Default = 0u)]
        public uint OverriddenCipherType { get; set; }

    }
}
