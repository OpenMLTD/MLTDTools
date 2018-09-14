namespace OpenMLTD.MillionDance.Viewer.Internal {
    internal interface IDrawable {

        void Draw();

        bool Visible { get; set; }

    }
}
