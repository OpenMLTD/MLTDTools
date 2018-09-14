using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.OpenTK;

namespace OpenMLTD.MillionDance.Entities.Mvd {
    [NotNull]
    public sealed class InterpolationPair {

        internal InterpolationPair() {
        }

        internal InterpolationPair(Vector2i pointA, Vector2i pointB) {
            PointA = pointA;
            PointB = pointB;
        }

        public Vector2i PointA { get; internal set; }

        public Vector2i PointB { get; internal set; }

        [NotNull]
        public static InterpolationPair Linear() {
            var ptA = new Vector2i(0, 0);
            var ptB = new Vector2i(255, 255);
            return new InterpolationPair(ptA, ptB);
        }

    }
}
