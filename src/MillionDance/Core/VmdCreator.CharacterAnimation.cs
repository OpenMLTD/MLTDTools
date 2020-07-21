using System;
using AssetStudio.Extended.CompositeModels;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Entities.Vmd;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull]
        public VmdMotion CreateCharacterAnimation([CanBeNull] IBodyAnimationSource mainDance, [NotNull] ScenarioObject baseScenario, [CanBeNull] ScenarioObject formationInfo, [CanBeNull] PrettyAvatar avatar, [CanBeNull] PmxModel mltdPmxModel, [CanBeNull] IBodyAnimationSource danceAppeal, int formationNumber, AppealType appealType) {
            VmdBoneFrame[] frames;

            if (ProcessBoneFrames && (mainDance != null && avatar != null && mltdPmxModel != null)) {
                frames = CreateBoneFrames(mainDance, avatar, mltdPmxModel, baseScenario, formationInfo, danceAppeal, formationNumber, appealType);
            } else {
                frames = Array.Empty<VmdBoneFrame>();
            }

            return new VmdMotion(ModelName, frames, null, null, null, null);
        }

    }
}
