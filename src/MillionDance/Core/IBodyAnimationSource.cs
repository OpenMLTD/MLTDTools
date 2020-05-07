using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;

namespace OpenMLTD.MillionDance.Core {
    public interface IBodyAnimationSource {

        /// <summary>
        /// Converts read animation data in to internal, unified body animation data.
        /// </summary>
        /// <returns>Unified body animation data.</returns>
        [NotNull]
        BodyAnimation Convert();

    }
}
