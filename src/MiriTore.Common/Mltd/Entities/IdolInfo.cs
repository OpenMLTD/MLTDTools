namespace OpenMLTD.MiriTore.Mltd.Entities {
    /// <summary>
    /// Basic idol info.
    /// </summary>
    public sealed class IdolInfo {

        public IdolInfo(string name, int idolID, string abbreviation, ColorType color) {
            Name = name;
            IdolID = idolID;
            Abbreviation = abbreviation;
            Color = color;
        }

        /// <summary>
        /// Idol ID, starting from 1.
        /// May be not-continuous, e.g. ..., 51, 52, 101, 102, 201, 202.
        /// </summary>
        public int IdolID { get; }

        /// <summary>
        /// Name in Japanese.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 3-letter abbreviation.
        /// </summary>
        public string Abbreviation { get; }

        /// <summary>
        /// Color attribute.
        /// </summary>
        public ColorType Color { get; }

    }
}
