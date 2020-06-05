using System;
using System.Collections.Generic;
using AssetStudio.Extended.CompositeModels;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Entities.Vmd;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull]
        public VmdMotion CreateCharacterAnimation([CanBeNull] IBodyAnimationSource bodyAnimationSource, [CanBeNull] PrettyAvatar avatar, [CanBeNull] PmxModel mltdPmxModel) {
            VmdBoneFrame[] frames;

            if (ProcessBoneFrames && (bodyAnimationSource != null && avatar != null && mltdPmxModel != null)) {
                frames = CreateBoneFrames(bodyAnimationSource, avatar, mltdPmxModel);
            } else {
                frames = Array.Empty<VmdBoneFrame>();
            }

            return new VmdMotion(ModelName, frames, null, null, null, null);
        }

    }
}
