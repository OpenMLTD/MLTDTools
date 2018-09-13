using DereTore.Exchange.Archive.ACB;
using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.MLTDTools.Applications.AcbPack.Mltd {
    public sealed class SynthTable : UtfRowBase {

        [UtfField(0, ColumnStorage.Constant)]
        public byte Type;
        [UtfField(1, ColumnStorage.Constant)]
        public string VoiceLimitGroupName;
        [UtfField(2)]
        public ushort CommandIndex;
        [UtfField(3)]
        public byte[] ReferenceItems;
        [UtfField(4, ColumnStorage.Constant)]
        public byte[] LocalAisacs;
        [UtfField(5, ColumnStorage.Constant)]
        public ushort GlobalAisacStartIndex;
        [UtfField(6, ColumnStorage.Constant)]
        public ushort GlobalAisacNumRefs;
        [UtfField(7)]
        public ushort ControlWorkArea1;
        [UtfField(8)]
        public ushort ControlWorkArea2;
        [UtfField(9, ColumnStorage.Constant)]
        public byte[] TrackValues;
        [UtfField(10, ColumnStorage.Constant)]
        public ushort ParameterPallet;
        [UtfField(11, ColumnStorage.Constant)]
        public ushort ActionTrackStartIndex;
        [UtfField(12, ColumnStorage.Constant)]
        public ushort NumActionTracks;

    }
}
