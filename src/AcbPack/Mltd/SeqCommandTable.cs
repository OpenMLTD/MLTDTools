using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.AcbPack.Mltd {
    [UtfTable("SequenceCommand")]
    public sealed class SeqCommandTable : UtfRowBase {

        [UtfField(0)]
        public byte[] Command;

    }
}
