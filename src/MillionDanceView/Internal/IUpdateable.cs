namespace MillionDanceView.Internal {
    internal interface IUpdateable {

        void Update();

        bool Enabled { get; set; }

    }
}
