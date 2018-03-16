using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        public static void Save([NotNull] ScenarioObject sobj, [NotNull] string path) {
            var desc = Describe(sobj, 0);

            File.WriteAllText(path, desc, Encoding.UTF8);

            string Describe(object s, int level) {
                if (ReferenceEquals(s, null)) {
                    return "null";
                }

                var t = s.GetType();

                if (t.IsPrimitive || t == typeof(TimeSpan) || t == typeof(string) || t == typeof(decimal) || t.IsEnum) {
                    return s.ToString();
                }

                var sb = new StringBuilder();

                if (t.IsArray) {
                    var arr = (Array)s;
                    var elemType = t.GetElementType();

                    Debug.Assert(elemType != null, nameof(elemType) + " != null");

                    sb.Append(elemType.Name);
                    sb.AppendFormat("[{0}]", arr.Length);

                    if (arr.Length > 0) {
                        sb.AppendLine(" {");
                    } else {
                        sb.Append(" {");
                    }

                    for (var i = 0; i < arr.Length; ++i) {
                        if (i > 0) {
                            sb.AppendLine(",");
                        }

                        if (level > 0) {
                            sb.Append(new string('\t', level + 1));
                        }

                        var elem = arr.GetValue(i);

                        sb.Append(Describe(elem, level + 1));
                    }

                    if (arr.Length > 0) {
                        if (level > 0) {
                            sb.Append(new string('\t', level));
                        }
                    }

                    sb.Append("}");

                    return sb.ToString();
                }

                var props = t.GetProperties();

                sb.Append(t.Name);

                if (props.Length > 0) {
                    sb.AppendLine(" {");
                } else {
                    sb.Append(" {");
                }

                for (var i = 0; i < props.Length; i++) {
                    var prop = props[i];
                    var propName = prop.Name;
                    var propValue = prop.GetValue(s);
                    var propValueStr = Describe(propValue, level + 1);

                    if (i > 0) {
                        sb.AppendLine(",");
                    }

                    if (level > 0) {
                        sb.Append(new string('\t', level + 1));
                    }

                    sb.AppendFormat("{0} = {1}", propName, propValueStr);

                    if (i == props.Length - 1) {
                        sb.AppendLine();
                    }
                }

                if (props.Length > 0) {
                    if (level > 0) {
                        sb.Append(new string('\t', level));
                    }
                }

                sb.Append("}");

                return sb.ToString();
            }
        }

    }
#endif
}
