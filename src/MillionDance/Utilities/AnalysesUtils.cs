using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssetStudio;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Internal;

namespace OpenMLTD.MillionDance.Utilities {
    internal static class AnalysesUtils {

        public static void SaveBodyAnimationFramesAsCsv([NotNull, ItemNotNull] KeyFrame[] frames, [NotNull] string filePath) {
            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                using (var w = new StreamWriter(fs, Encoding.ASCII)) {
                    // Header
                    var pathLookup = new Dictionary<string, int>();

                    {
                        var pathList = new List<string>();
                        var pathSet = new HashSet<string>();

                        foreach (var frame in frames) {
                            var path = frame.Path;

                            if (pathSet.Contains(path)) {
                                continue;
                            }

                            pathSet.Add(frame.Path);

                            var p = path.BreakLast('/');

                            if (frame.HasRotations) {
                                var sub = p + "-RX";
                                pathLookup[sub] = pathList.Count;
                                pathList.Add(sub);
                                sub = p + "-RY";
                                pathLookup[sub] = pathList.Count;
                                pathList.Add(sub);
                                sub = p + "-RZ";
                                pathLookup[sub] = pathList.Count;
                                pathList.Add(sub);
                            }

                            if (frame.HasPositions) {
                                var sub = p + "-TX";
                                pathLookup[sub] = pathList.Count;
                                pathList.Add(sub);
                                sub = p + "-TY";
                                pathLookup[sub] = pathList.Count;
                                pathList.Add(sub);
                                sub = p + "-TZ";
                                pathLookup[sub] = pathList.Count;
                                pathList.Add(sub);
                            }
                        }

                        var header = string.Join(",", pathList);
                        w.WriteLine(header);
                    }

                    var frameCount = frames.Length;
                    var boneCount = pathLookup.Count;

                    var rows = new float[frameCount][];

                    for (var i = 0; i < frameCount; i += 1) {
                        var row = new float[boneCount];
                        rows[i] = row;
                    }

                    // Frames
                    {
                        foreach (var frame in frames) {
                            var path = frame.Path;
                            var row = rows[frame.FrameIndex];

                            var p = path.BreakLast('/');
                            int colIndex;

                            if (frame.HasRotations) {
                                colIndex = pathLookup[p + "-RX"];
                                row[colIndex] = frame.AngleX.Value;
                                colIndex = pathLookup[p + "-RY"];
                                row[colIndex] = frame.AngleY.Value;
                                colIndex = pathLookup[p + "-RZ"];
                                row[colIndex] = frame.AngleZ.Value;
                            }

                            if (frame.HasPositions) {
                                colIndex = pathLookup[p + "-TX"];
                                row[colIndex] = frame.PositionX.Value;
                                colIndex = pathLookup[p + "-TY"];
                                row[colIndex] = frame.PositionY.Value;
                                colIndex = pathLookup[p + "-TZ"];
                                row[colIndex] = frame.PositionZ.Value;
                            }
                        }

                        foreach (var row in rows) {
                            var line = string.Join(",", row);
                            w.WriteLine(line);
                        }
                    }
                }
            }
        }

        public static void SaveBodyAnimationFramesAsCsv([NotNull] AnimationClip clip, [NotNull] string filePath) {
            var c = clip.m_MuscleClip.m_Clip.m_StreamedClip;
            var frames = c.ReadData();
            var curveCount = c.curveCount;

            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                using (var w = new StreamWriter(fs, Encoding.ASCII)) {
                    Func<StreamedClip.StreamedCurveKey, float> selector = key => key.value;

                    // Header
                    {
                        var header = string.Join(",", Enumerable.Range(1, (int)curveCount).Select(i => $"Curve {i}"));
                        w.WriteLine(header);
                    }

                    // Frames
                    foreach (var frame in frames) {
                        var line = string.Join(",", frame.keyList.Select(selector));
                        w.WriteLine(line);
                    }
                }
            }
        }

    }
}
