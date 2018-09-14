// ReSharper disable once CheckNamespace
namespace OpenMLTD.MillionDance.Entities.OpenTK {
    // ReSharper disable once InconsistentNaming
    public struct Vector2i {

        public Vector2i(int value) {
            X = value;
            Y = value;
        }

        public Vector2i(int x, int y) {
            X = x;
            Y = y;
        }

        public int X;

        public int Y;

    }
}
