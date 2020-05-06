using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    // ReSharper disable once InconsistentNaming
    public struct AABB {

        internal AABB([NotNull] AssetStudio.AABB aabb) {
            var center = aabb.m_Center;
            var extent = aabb.m_Extent;

            (MinX, MaxX) = (center.X - extent.X / 2, center.X + extent.X / 2);
            (MinY, MaxY) = (center.Y - extent.Y / 2, center.Y + extent.Y / 2);
            (MinZ, MaxZ) = (center.Z - extent.Z / 2, center.Z + extent.Z / 2);
        }

        public float MinX, MaxX;

        public float MinY, MaxY;

        public float MinZ, MaxZ;

    }
}
