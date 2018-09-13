using DereTore.Exchange.Archive.ACB;
using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.MLTDTools.Applications.AcbPack.Mltd {
    public sealed class TrackTable : UtfRowBase {

        [UtfField(0)]
        public ushort EventIndex;
        [UtfField(1, ColumnStorage.Constant)]
        public ushort CommandIndex;
        [UtfField(2, ColumnStorage.Constant)]
        public byte[] LocalAisacs;
        [UtfField(3, ColumnStorage.Constant)]
        public ushort GlobalAisacStartIndex;
        [UtfField(4, ColumnStorage.Constant)]
        public ushort GlobalAisacNumRefs;
        [UtfField(5, ColumnStorage.Constant)]
        public ushort ParameterPallet;
        [UtfField(6, ColumnStorage.Constant)]
        public byte TargetType;
        [UtfField(7, ColumnStorage.Constant)]
        public string TargetName;
        [UtfField(8, ColumnStorage.Constant)]
        public uint TargetId;
        [UtfField(9, ColumnStorage.Constant)]
        public string TargetAcbName;
        [UtfField(10, ColumnStorage.Constant)]
        public byte Scope;
        [UtfField(11, ColumnStorage.Constant)]
        public ushort TargetTrackNo;

    }
}
