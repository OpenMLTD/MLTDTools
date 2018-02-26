using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.AcbPack.Mltd {
    [UtfTable("Strings")]
    public sealed class StringValueTable : UtfRowBase {

        [UtfField(0)]
        public string StringValue;

    }
}
