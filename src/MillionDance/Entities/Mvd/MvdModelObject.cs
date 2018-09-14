namespace OpenMLTD.MillionDance.Entities.Mvd {
    public abstract class MvdModelObject {

        public string DisplayName { get; internal set; } = string.Empty;

        public string EnglishName { get; internal set; } = string.Empty;

        public int Id { get; internal set; }

    }
}
