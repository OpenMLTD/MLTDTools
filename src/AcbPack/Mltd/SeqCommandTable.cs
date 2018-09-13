using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.MLTDTools.Applications.AcbPack.Mltd {
    [UtfTable("SequenceCommand")]
    public sealed class SeqCommandTable : UtfRowBase {

        [UtfField(0)]
        public byte[] Command;

    }
}
