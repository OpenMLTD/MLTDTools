using DereTore.Exchange.Archive.ACB;
using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.AcbPack.Mltd {
    public sealed class SequenceTable : UtfRowBase {

        [UtfField(0, ColumnStorage.Constant)]
        public ushort PlaybackRatio;
        [UtfField(1, ColumnStorage.Constant)]
        public ushort NumTracks;
        [UtfField(2)]
        public byte[] TrackIndex;
        [UtfField(3)]
        public ushort CommandIndex;
        [UtfField(4, ColumnStorage.Constant)]
        public byte[] LocalAisacs;
        [UtfField(5, ColumnStorage.Constant)]
        public ushort GlobalAisacStartIndex;
        [UtfField(6, ColumnStorage.Constant)]
        public ushort GlobalAisacNumRefs;
        [UtfField(7, ColumnStorage.Constant)]
        public ushort ParameterPallet;
        [UtfField(8, ColumnStorage.Constant)]
        public ushort ActionTrackStartIndex;
        [UtfField(9, ColumnStorage.Constant)]
        public ushort NumActionTracks;
        [UtfField(10, ColumnStorage.Constant)]
        public byte[] TrackValues;
        [UtfField(11, ColumnStorage.Constant)]
        public byte Type;
        [UtfField(12)]
        public ushort ControlWorkArea1;
        [UtfField(13)]
        public ushort ControlWorkArea2;

    }
}
