using System;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class VertexPin : IComparable<VertexPin>, IEquatable<VertexPin> {

        internal VertexPin() {
        }

        public int VertexIndex { get; internal set; }

        public int CompareTo(VertexPin other) {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (ReferenceEquals(null, other)) {
                return 1;
            }

            return VertexIndex.CompareTo(other.VertexIndex);
        }

        #region IEquatable
        public bool Equals(VertexPin other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return VertexIndex == other.VertexIndex;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is VertexPin && Equals((VertexPin)obj);
        }

        public override int GetHashCode() {
            return VertexIndex;
        }

        public static bool operator ==(VertexPin left, VertexPin right) {
            return Equals(left, right);
        }

        public static bool operator !=(VertexPin left, VertexPin right) {
            return !Equals(left, right);
        }
        #endregion

    }
}
