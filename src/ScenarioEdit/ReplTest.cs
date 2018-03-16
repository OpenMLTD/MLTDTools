using System.IO;
using JetBrains.Annotations;
using OpenMLTD.ScenarioEdit.Entities;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;

namespace OpenMLTD.ScenarioEdit {
#if DEBUG
    public static class ReplTest {

        [CanBeNull]
        public static ScenarioObject Load([NotNull] string path) {
            ScenarioObject result = null;

            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bundle = new BundleFile(fileStream, false)) {
                    foreach (var asset in bundle.AssetFiles) {
                        foreach (var preloadData in asset.PreloadDataList) {
                            if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                continue;
                            }

                            var behaviour = preloadData.LoadAsMonoBehaviour(true);

                            if (!behaviour.Name.EndsWith("scenario_sobj")) {
                                continue;
                            }

                            behaviour = preloadData.LoadAsMonoBehaviour(false);

                            var serializer = new MonoBehaviourSerializer();
                            result = serializer.Deserialize<ScenarioObject>(behaviour);
                            break;
                        }
                    }
                }
            }

            return result;
        }

    }
#endif
}
