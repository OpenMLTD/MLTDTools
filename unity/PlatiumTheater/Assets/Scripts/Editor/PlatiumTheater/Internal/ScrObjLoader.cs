using System;
using System.IO;
using Imas.Live;
using Imas.Live.Scrobj;
using JetBrains.Annotations;
using UnityEngine;

namespace PlatiumTheater.Internal {
    internal static class ScrObjLoader {

        internal static void LoadBeatmap([NotNull] NoteScrObj obj, [NotNull] string path) {
            var lines = File.ReadAllLines(path);

            var index = 8;

            obj.evts = ReadEventNoteDataList(lines, ref index);
            obj.ct = ReadEventConductorDataList(lines, ref index);
            obj.scoreSpeed = ReadFloatList(lines, ref index, 0);
            obj.judgeRange = ReadFloatList(lines, ref index, 0);

            obj.BGM_offset = lines[index++].ConvSingle(19);
        }

        internal static void LoadScenario([NotNull] ScenarioScrObj obj, [NotNull] string path) {
            var lines = File.ReadAllLines(path);

            var index = 8;

            //try {
            obj.scenario = ReadEventScenarioDataList(lines, ref index);
            obj.texs = ReadTexTargetNameList(lines, ref index);
            obj.ap_st = ReadEventScenarioData(lines, ref index, false);
            obj.ap_pose = ReadEventScenarioData(lines, ref index, false);
            obj.ap_end = ReadEventScenarioData(lines, ref index, false);
            obj.fine_ev = ReadEventScenarioData(lines, ref index, false);
            //} catch (Exception ex) {
            //    Debug.LogError("Exception happend at line " + (index + 1).ToString());
            //    Debug.LogError(ex);
            //}
        }

        private static EventNoteData[] ReadEventNoteDataList([NotNull] string[] lines, ref int index) {
            index += 2;

            var evtsCount = lines[index++].ConvInt32(12);
            var result = new EventNoteData[evtsCount];

            for (var i = 0; i < evtsCount; ++i) {
                var eventNoteData = ReadEventNoteData(lines, ref index);
                result[i] = eventNoteData;
            }

            return result;
        }

        private static EventNoteData ReadEventNoteData([NotNull] string[] lines, ref int index) {
            index += 2;

            var result = new EventNoteData();

            result.absTime = lines[index++].ConvDouble(20);
            result.selected = lines[index++].ConvBoolean(20);
            result.tick = lines[index++].ConvInt64(17);
            result.measure = lines[index++].ConvInt32(17);
            result.beat = lines[index++].ConvInt32(14);
            result.track = lines[index++].ConvInt32(15);
            result.type = lines[index++].ConvInt32(14);
            result.startPosx = lines[index++].ConvSingle(21);
            result.endPosx = lines[index++].ConvSingle(19);
            result.speed = lines[index++].ConvSingle(17);
            result.duration = lines[index++].ConvInt32(18);

            result.poly = ReadPolyPoints(lines, ref index);

            result.endType = lines[index++].ConvInt32(17);
            result.leadTime = lines[index++].ConvDouble(21);

            return result;
        }

        private static PolyPoint[] ReadPolyPoints([NotNull] string[] lines, ref int index) {
            index += 2;

            var polyCount = lines[index++].ConvInt32(15);

            if (polyCount == 0) {
                return null;
            }

            var result = new PolyPoint[polyCount];

            for (var i = 0; i < polyCount; ++i) {
                index += 2;

                var subtick = lines[index++].ConvInt32(20);
                var posx = lines[index++].ConvSingle(19);

                result[i] = new PolyPoint {
                    subtick = subtick,
                    posx = posx
                };
            }

            return result;
        }

        private static EventConductorData[] ReadEventConductorDataList([NotNull] string[] lines, ref int index) {
            index += 2;
        
            var cc = lines[index++].ConvInt32(12);
            var result = new EventConductorData[cc];

            for (var i = 0; i < cc; ++i) {
                result[i] = ReadEventConductorData(lines, ref index);
            }

            return result;
        }

        private static EventConductorData ReadEventConductorData([NotNull] string[] lines, ref int index) {
            index += 2;

            var result = new EventConductorData();

            result.absTime = lines[index++].ConvDouble(20);
            result.selected = lines[index++].ConvBoolean(20);
            result.tick = lines[index++].ConvInt64(17);
            result.measure = lines[index++].ConvInt32(17);
            result.beat = lines[index++].ConvInt32(14);
            result.track = lines[index++].ConvInt32(15);
            result.tempo = lines[index++].ConvDouble(18);
            result.tsigNumerator = lines[index++].ConvInt32(23);
            result.tsigDemoninator = lines[index++].ConvInt32(25);

            index++;
            result.marker = string.Empty;

            return result;
        }

        private static EventScenarioData[] ReadEventScenarioDataList([NotNull] string[] lines, ref int index) {
            index += 2;

            var cc = lines[index++].ConvInt32(12);
            var result = new EventScenarioData[cc];

            for (var i = 0; i < cc; ++i) {
                result[i] = ReadEventScenarioData(lines, ref index, true);
            }

            return result;
        }

        private static EventScenarioData ReadEventScenarioData([NotNull] string[] lines, ref int index, bool isArrayElement) {
            index += isArrayElement ? 2 : 1;

            var outerOffset = isArrayElement ? 0 : -2;

            var result = new EventScenarioData();

            result.absTime = lines[index++].ConvDouble(20 + outerOffset);
            result.selected = lines[index++].ConvBoolean(20 + outerOffset);
            result.tick = lines[index++].ConvInt64(17 + outerOffset);
            result.measure = lines[index++].ConvInt32(17 + outerOffset);
            result.beat = lines[index++].ConvInt32(14 + outerOffset);
            result.track = lines[index++].ConvInt32(15 + outerOffset);
            result.type = lines[index++].ConvInt32(14 + outerOffset);
            result.param = lines[index++].ConvInt32(15 + outerOffset);
            result.target = lines[index++].ConvInt32(16 + outerOffset);
            result.duration = lines[index++].ConvInt64(21 + outerOffset);

            ++index;
            result.str = string.Empty;
            ++index;
            result.info = string.Empty;

            result.on = lines[index++].ConvInt32(12 + outerOffset);
            result.on2 = lines[index++].ConvInt32(13 + outerOffset);

            result.col = ReadColorRgba(lines, ref index, isArrayElement, false);
            result.col2 = ReadColorRgba(lines, ref index, isArrayElement, false);

            result.cols = ReadFloatList(lines, ref index, isArrayElement ? 3 : 1);

            index += 3;
            result.tex = null;

            result.texInx = lines[index++].ConvInt32(16 + outerOffset);
            result.trig = lines[index++].ConvInt32(14 + outerOffset);
            result.speed = lines[index++].ConvSingle(17 + outerOffset);
            result.idol = lines[index++].ConvInt32(14 + outerOffset);

            result.mute = ReadBooleanList(lines, ref index, isArrayElement ? 3 : 1);

            result.addf = lines[index++].ConvBoolean(16 + outerOffset);
            result.eye_x = lines[index++].ConvSingle(17 + outerOffset);
            result.eye_y = lines[index++].ConvSingle(17 + outerOffset);

            result.formation = ReadVector4List(lines, ref index, isArrayElement ? 3 : 1);

            result.appeal = lines[index++].ConvBoolean(18 + outerOffset);
            result.cheeklv = lines[index++].ConvInt32(17 + outerOffset);
            result.eyeclose = lines[index++].ConvBoolean(20 + outerOffset);
            result.talking = lines[index++].ConvBoolean(19 + outerOffset);
            result.delay = lines[index++].ConvBoolean(17 + outerOffset);

            result.clratio = ReadInt32List(lines, ref index, isArrayElement ? 3 : 1);
            result.clcols = ReadInt32List(lines, ref index, isArrayElement ? 3 : 1);

            result.camcut = lines[index++].ConvInt32(16 + outerOffset);

            result.vjparam = ReadVjParam(lines, ref index, isArrayElement);

            return result;
        }

        private static VjParam ReadVjParam([NotNull] string[] lines, ref int index, bool isInArrayScenarioData) {
            ++index;

            var outerOffset = isInArrayScenarioData ? 0 : -2;

            var result = new VjParam();

            result.use = lines[index++].ConvBoolean(16 + outerOffset);
            result.renderTex = lines[index++].ConvBoolean(22 + outerOffset);
            result.col = lines[index++].ConvInt32(14 + outerOffset);
            result.row = lines[index++].ConvInt32(14 + outerOffset);
            result.begin = lines[index++].ConvInt32(16 + outerOffset);
            result.speed = lines[index++].ConvInt32(16 + outerOffset);

            result.color = ReadColorRgba(lines, ref index, isInArrayScenarioData, true);

            return result;
        }

        private static ColorRGBA ReadColorRgba([NotNull] string[] lines, ref int index, bool isInArrayScenarioData, bool isInVjParam) {
            ++index;

            var pos = 12;

            if (isInArrayScenarioData) {
                pos += 2;
            }

            if (isInVjParam) {
                pos += 1;
            }

            var result = new ColorRGBA();

            result.r = lines[index++].ConvSingle(pos);
            result.g = lines[index++].ConvSingle(pos);
            result.b = lines[index++].ConvSingle(pos);
            result.a = lines[index++].ConvSingle(pos);

            return result;
        }

        private static TexTargetName[] ReadTexTargetNameList([NotNull] string[] lines, ref int index) {
            index += 2;

            var count = lines[index++].ConvInt32(12);

            if (count == 0) {
                return null;
            }

            var result = new TexTargetName[count];

            for (var i = 0; i < count; ++i) {
                index += 2;

                var target = lines[index++].ConvInt32(16);
                var curLine = lines[index++];
                var name = curLine.Substring(18, curLine.Length - 19);

                result[i] = new TexTargetName {
                    target = target,
                    name = name
                };
            }

            return result;
        }

        private static float[] ReadFloatList([NotNull] string[] lines, ref int index, int extraOffset) {
            index += 2;

            var count = lines[index++].ConvInt32(12 + extraOffset);

            if (count == 0) {
                return null;
            }

            var result = new float[count];

            for (var i = 0; i < count; ++i) {
                ++index;

                result[i] = lines[index++].ConvSingle(15 + extraOffset);
            }

            return result;
        }

        private static int[] ReadInt32List([NotNull] string[] lines, ref int index, int extraOffset) {
            index += 2;

            var count = lines[index++].ConvInt32(12 + extraOffset);

            if (count == 0) {
                return null;
            }

            var result = new int[count];

            for (var i = 0; i < count; ++i) {
                ++index;

                result[i] = lines[index++].ConvInt32(13 + extraOffset);
            }

            return result;
        }

        private static Vector4[] ReadVector4List([NotNull] string[] lines, ref int index, int extraOffset) {
            index += 2;

            var count = lines[index++].ConvInt32(12 + extraOffset);

            if (count == 0) {
                return null;
            }

            var result = new Vector4[count];

            for (var i = 0; i < count; ++i) {
                index += 2;

                var x = lines[index++].ConvSingle(13 + extraOffset);
                var y = lines[index++].ConvSingle(13 + extraOffset);
                var z = lines[index++].ConvSingle(13 + extraOffset);
                var w = lines[index++].ConvSingle(13 + extraOffset);

                result[i] = new Vector4(x, y, z, w);
            }

            return result;
        }

        private static bool[] ReadBooleanList([NotNull] string[] lines, ref int index, int extraOffset) {
            index += 2;

            var count = lines[index++].ConvInt32(12 + extraOffset);

            if (count == 0) {
                return null;
            }

            var result = new bool[count];

            for (var i = 0; i < count; ++i) {
                ++index;

                result[i] = lines[index++].ConvBoolean(15 + extraOffset);
            }

            return result;
        }

        private static int ConvInt32([NotNull] this string str, int startIndex) {
            return Convert.ToInt32(str.Substring(startIndex));
        }

        private static long ConvInt64([NotNull] this string str, int startIndex) {
            return Convert.ToInt64(str.Substring(startIndex));
        }

        private static float ConvSingle([NotNull] this string str, int startIndex) {
            return Convert.ToSingle(str.Substring(startIndex));
        }

        private static double ConvDouble([NotNull] this string str, int startIndex) {
            return Convert.ToDouble(str.Substring(startIndex));
        }

        private static bool ConvBoolean([NotNull] this string str, int startIndex) {
            var b = Convert.ToByte(str.Substring(startIndex));
            return b != 0;
        }

    }
}
