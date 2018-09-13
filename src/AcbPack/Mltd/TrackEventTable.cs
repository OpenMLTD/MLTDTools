using DereTore.Exchange.Archive.ACB.Serialization;

namespace OpenMLTD.MLTDTools.Applications.AcbPack.Mltd {
    public sealed class TrackEventTable : UtfRowBase {

        [UtfField(0)]
        public byte[] Command;

    }
}
