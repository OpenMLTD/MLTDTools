using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.MLTDTools.Applications.AcbPack.Mltd {
    [UtfTable("Strings")]
    public sealed class StringValueTable : UtfRowBase {

        [UtfField(0)]
        public string StringValue;

    }
}
