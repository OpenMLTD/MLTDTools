using DereTore.Exchange.Archive.ACB;
using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.MLTDTools.Applications.AcbPack.Mltd {
    public sealed class AcfReferenceTable : UtfRowBase {

        [UtfField(0)]
        public byte Type;
        [UtfField(1)]
        public string Name;
        [UtfField(2, ColumnStorage.Constant)]
        public string Name2;
        [UtfField(3)]
        public uint Id;

    }
}