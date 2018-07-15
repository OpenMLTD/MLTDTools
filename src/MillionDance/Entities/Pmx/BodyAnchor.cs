using System;
using System.Diagnostics.CodeAnalysis;

namespace MillionDance.Entities.Pmx {
    public sealed class BodyAnchor : IComparable<BodyAnchor>, IEquatable<BodyAnchor> {

        internal BodyAnchor() {
        }

        public int BodyIndex { get; internal set; }

        public bool IsNear { get; internal set; }

        public int VertexIndex { get; internal set; }

        public int CompareTo(BodyAnchor other) {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (ReferenceEquals(null, other)) {
                return 1;
            }

            var bodyIndexComparison = BodyIndex.CompareTo(other.BodyIndex);

            if (bodyIndexComparison != 0) {
                return bodyIndexComparison;
            }

            return VertexIndex.CompareTo(other.VertexIndex);
        }

        #region IEquatable
        public bool Equals(BodyAnchor other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return BodyIndex == other.BodyIndex && IsNear == other.IsNear && VertexIndex == other.VertexIndex;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is BodyAnchor && Equals((BodyAnchor)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode() {
            unchecked {
                var hashCode = BodyIndex;
                hashCode = (hashCode * 397) ^ IsNear.GetHashCode();
                hashCode = (hashCode * 397) ^ VertexIndex;
                return hashCode;
            }
        }

        public static bool operator ==(BodyAnchor left, BodyAnchor right) {
            return Equals(left, right);
        }

        public static bool operator !=(BodyAnchor left, BodyAnchor right) {
            return !Equals(left, right);
        }
        #endregion

    }
}
