using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AssetStudio;
using JetBrains.Annotations;

namespace Imas.Data.Serialized.Sway {
    public sealed class SwayController {

        internal SwayController() {
        }

        [NotNull]
        public string Top { get; internal set; } = string.Empty;

        [NotNull, ItemNotNull]
        public IReadOnlyList<SwayManager> Managers { get; internal set; } = Array.Empty<SwayManager>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<SwayCollider> Colliders { get; internal set; } = Array.Empty<SwayCollider>();

        [NotNull, ItemNotNull]
        public IReadOnlyList<SwayBone> SwayBones { get; internal set; } = Array.Empty<SwayBone>();

    }
}
