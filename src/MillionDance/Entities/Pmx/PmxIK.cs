using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Entities.Pmx {
    public sealed class PmxIK {

        internal PmxIK() {
        }

        public int TargetBoneIndex { get; internal set; }

        [CanBeNull]
        public PmxBone TargetBone { get; internal set; }

        public int LoopCount { get; internal set; }

        /// <summary>
        /// Angle limit, in degrees.
        /// </summary>
        public float AngleLimit { get; internal set; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<IKLink> Links { get; internal set; } = Array.Empty<IKLink>();

    }
}
