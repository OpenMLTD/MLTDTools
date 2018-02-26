using DereTore.Exchange.Archive.ACB;
using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.AcbPack.Mltd {
    public sealed class CueTable : UtfRowBase {


        [UtfField(0)]
        public uint CueId;
        [UtfField(1, ColumnStorage.Constant)]
        public byte ReferenceType;
        [UtfField(2)]
        public ushort ReferenceIndex;
        [UtfField(3, ColumnStorage.Constant)]
        public string UserData;
        [UtfField(4, ColumnStorage.Constant)]
        public ushort Worksize;
        [UtfField(5, ColumnStorage.Constant)]
        public byte[] AisacControlMap;
        [UtfField(6)]
        public uint Length;
        [UtfField(7, ColumnStorage.Constant)]
        public byte NumAisacControlMaps;
        [UtfField(8, ColumnStorage.Constant)]
        public byte HeaderVisibility;

    }
}
