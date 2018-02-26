using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.AcbPack.Mltd {
    public sealed class WaveformTable : UtfRowBase {

        [UtfField(0)]
        public ushort MemoryAwbId;
        [UtfField(1)]
        public byte EncodeType;
        [UtfField(2)]
        public byte Streaming;
        [UtfField(3)]
        public byte NumChannels;
        [UtfField(4)]
        public byte LoopFlag;
        [UtfField(5)]
        public ushort SamplingRate;
        [UtfField(6)]
        public uint NumSamples;
        [UtfField(7)]
        public ushort ExtensionData;
        [UtfField(8)]
        public ushort StreamAwbPortNo;
        [UtfField(9)]
        public ushort StreamAwbId;

    }
}
