namespace OpenMLTD.MillionDance.Viewer.Internal {
    internal interface IUpdateable {

        void Update();

        bool Enabled { get; set; }

    }
}
