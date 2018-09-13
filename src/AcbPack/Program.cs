using System;
using System.IO;
using DereTore.Exchange.Archive.ACB.Serialization;
using DereTore.Exchange.Audio.HCA;
using OpenMLTD.MLTDTools.Applications.AcbPack.Mltd;

namespace OpenMLTD.MLTDTools.Applications.AcbPack {
    internal static class Program {

        [STAThread]
        private static int Main(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine(HelpMessage);
                return -1;
            }

            var inputHcaFileName = args[0];
            var outputFileName = args[1];
            var songName = DefaultSongName;

            for (var i = 2; i < args.Length; ++i) {
                var arg = args[i];
                if (arg[0] == '-' || arg[0] == '/') {
                    switch (arg.Substring(1)) {
                        case "n":
                            if (i < args.Length - 1) {
                                songName = args[++i];
                            }
                            break;
                    }
                }
            }

            try {
                var header = GetFullTable(inputHcaFileName, songName);
                var table = new UtfRowBase[] { header };
                var serializer = new AcbSerializer();

                using (var fs = File.Open(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                    serializer.Serialize(table, fs);
                }
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());

                return -2;
            }

            return 0;
        }

        private static HeaderTable GetFullTable(string hcaFileName, string songName) {
            HcaInfo info;
            uint lengthInSamples;
            float lengthInSeconds;
            using (var fileStream = File.Open(hcaFileName, FileMode.Open, FileAccess.Read)) {
                var decoder = new OneWayHcaDecoder(fileStream);
                info = decoder.HcaInfo;
                lengthInSamples = decoder.LengthInSamples;
                lengthInSeconds = decoder.LengthInSeconds;
            }
            var cue = new[] {
                new CueTable {
                    AisacControlMap = null,
                    CueId = 0,
                    HeaderVisibility = 1,
                    NumAisacControlMaps = 0,
                    ReferenceType = 3,
                    UserData = string.Empty,
                    ReferenceIndex = 0,
                    Worksize = 0,
                    Length = (uint)Math.Round(lengthInSeconds * 1000)
               },
                new CueTable {
                    AisacControlMap = null,
                    CueId = 1,
                    HeaderVisibility = 1,
                    NumAisacControlMaps = 0,
                    ReferenceType = 3,
                    UserData = string.Empty,
                    ReferenceIndex = 1,
                    Worksize = 0,
                    Length = 0xffffffff
                },
                new CueTable {
                    AisacControlMap = null,
                    CueId = 2,
                    HeaderVisibility = 1,
                    NumAisacControlMaps = 0,
                    ReferenceType = 3,
                    UserData = string.Empty,
                    ReferenceIndex = 2,
                    Worksize = 0,
                    Length = 0xffffffff
                }
            };
            var cueName = new[] {
                new CueNameTable {
                    CueName = songName + "_bgm",
                    CueIndex = 0
                },
                new CueNameTable {
                    CueName = songName + "_bgm_preview",
                    CueIndex = 1
                },
                new CueNameTable {
                    CueName = songName + "_bgm_soundcheck",
                    CueIndex = 2
                }
            };
            var waveform = new[] {
                new WaveformTable {
                    MemoryAwbId = 0,
                    EncodeType = 2, // HCA
                    Streaming = 0,
                    NumChannels = (byte)info.ChannelCount,
                    LoopFlag = 1,
                    SamplingRate = (ushort)info.SamplingRate,
                    NumSamples = lengthInSamples,
                    ExtensionData = ushort.MaxValue,
                    StreamAwbPortNo = ushort.MaxValue,
                    StreamAwbId = ushort.MaxValue
               }
            };
            var synth = new[] {
                new SynthTable {
                    VoiceLimitGroupName = string.Empty,
                    Type = 0,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    ActionTrackStartIndex = ushort.MaxValue,
                    NumActionTracks = 0,
                    CommandIndex = 65535,
                    ControlWorkArea1 = 0,
                    ControlWorkArea2 = 0,
                    LocalAisacs = null,
                    TrackValues = null,
                    ReferenceItems = new byte[] {0x00, 0x01, 0x00, 0x00}
                },
                new SynthTable {
                    VoiceLimitGroupName = string.Empty,
                    Type = 0,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    ActionTrackStartIndex = ushort.MaxValue,
                    NumActionTracks = 0,
                    CommandIndex = 0,
                    ControlWorkArea1 = 1,
                    ControlWorkArea2 = 1,
                    LocalAisacs = null,
                    TrackValues = null,
                    ReferenceItems = new byte[] {0x00, 0x01, 0x00, 0x00}
                },
                new SynthTable {
                    VoiceLimitGroupName = string.Empty,
                    Type = 0,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    ActionTrackStartIndex = ushort.MaxValue,
                    NumActionTracks = 0,
                    CommandIndex = 1,
                    ControlWorkArea1 = 2,
                    ControlWorkArea2 = 2,
                    LocalAisacs = null,
                    TrackValues = null,
                    ReferenceItems = new byte[] {0x00, 0x01, 0x00, 0x00}
                }
            };
            var seqCommand = new[] {
                new SeqCommandTable {
                    Command = new byte[29] {
                        0x00, 0x41, 0x0c, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x0a, 0x00,
                        0x6f, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x6f, 0x04, 0x00, 0x03, 0x27, 0x10
                    }
                },
                new SeqCommandTable {
                    Command = new byte[31] {
                        0x00, 0x41, 0x0c, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x09, 0x00,
                        0x6e, 0x01, 0x01, 0x00, 0x57, 0x02, 0x00, 0x23, 0x00, 0x6f, 0x04, 0x00, 0x00, 0x27, 0x10
                    }
                }
            };
            var track = new[] {
                new TrackTable {
                    CommandIndex = ushort.MaxValue,
                    EventIndex = 0,
                    LocalAisacs = null,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    TargetType = 0,
                    TargetName = string.Empty,
                    TargetId = uint.MaxValue,
                    TargetAcbName = string.Empty,
                    Scope = 0,
                    TargetTrackNo = ushort.MaxValue
                },
                new TrackTable {
                    CommandIndex = ushort.MaxValue,
                    EventIndex = 1,
                    LocalAisacs = null,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    TargetType = 0,
                    TargetName = string.Empty,
                    TargetId = uint.MaxValue,
                    TargetAcbName = string.Empty,
                    Scope = 0,
                    TargetTrackNo = ushort.MaxValue
                },
                new TrackTable {
                    CommandIndex = ushort.MaxValue,
                    EventIndex = 2,
                    LocalAisacs = null,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    TargetType = 0,
                    TargetName = string.Empty,
                    TargetId = uint.MaxValue,
                    TargetAcbName = string.Empty,
                    Scope = 0,
                    TargetTrackNo = ushort.MaxValue
                }
            };
            var sequence = new[] {
                new SequenceTable {
                    PlaybackRatio = 100,
                    NumTracks = 1,
                    TrackIndex = new byte[] {0x00, 0x00},
                    CommandIndex = 0,
                    LocalAisacs = null,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    ActionTrackStartIndex = ushort.MaxValue,
                    NumActionTracks = 0,
                    TrackValues = null,
                    Type = 0,
                    ControlWorkArea1 = 0,
                    ControlWorkArea2 = 0
                },
                new SequenceTable {
                    PlaybackRatio = 100,
                    NumTracks = 1,
                    TrackIndex = new byte[] {0x00, 0x01},
                    CommandIndex = 1,
                    LocalAisacs = null,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    ActionTrackStartIndex = ushort.MaxValue,
                    NumActionTracks = 0,
                    TrackValues = null,
                    Type = 0,
                    ControlWorkArea1 = 1,
                    ControlWorkArea2 = 1
                },
                new SequenceTable {
                    PlaybackRatio = 100,
                    NumTracks = 1,
                    TrackIndex = new byte[] {0x00, 0x02},
                    CommandIndex = 1,
                    LocalAisacs = null,
                    GlobalAisacStartIndex = ushort.MaxValue,
                    GlobalAisacNumRefs = 0,
                    ParameterPallet = ushort.MaxValue,
                    ActionTrackStartIndex = ushort.MaxValue,
                    NumActionTracks = 0,
                    TrackValues = null,
                    Type = 0,
                    ControlWorkArea1 = 2,
                    ControlWorkArea2 = 2
                }
            };
            var stringValue = new[] {
                new StringValueTable {
                    StringValue = "MasterOut"
                },
                new StringValueTable {
                    StringValue = "bus_reverb"
                },
                new StringValueTable {
                    StringValue = "bus_voice_rip"
                },
                new StringValueTable {
                    StringValue = "stage_master"
                },
                new StringValueTable {
                    StringValue = string.Empty
                }
            };
            var acfReference = new[] {
                new AcfReferenceTable {
                    Type = 3,
                    Name = "master",
                    Name2 = string.Empty,
                    Id = 1
                },
                new AcfReferenceTable {
                    Type = 3,
                    Name = "song_option",
                    Name2 = string.Empty,
                    Id = 4,
                },
                new AcfReferenceTable {
                    Type = 3,
                    Name = "song_submix",
                    Name2 = string.Empty,
                    Id = 10
                },
                new AcfReferenceTable {
                    Type = 9,
                    Name = "MasterOut",
                    Name2 = string.Empty,
                    Id = uint.MaxValue
                },
                new AcfReferenceTable {
                    Type = 9,
                    Name = "bus_reverb",
                    Name2 = string.Empty,
                    Id = uint.MaxValue
                },
                new AcfReferenceTable {
                    Type = 9,
                    Name = "bus_voice_rip",
                    Name2 = string.Empty,
                    Id = uint.MaxValue
                },
                new AcfReferenceTable {
                    Type = 9,
                    Name = "stage_master",
                    Name2 = string.Empty,
                    Id = uint.MaxValue
                },
                new AcfReferenceTable {
                    Type = 9,
                    Name = string.Empty,
                    Name2 = string.Empty,
                    Id = uint.MaxValue
                },
                new AcfReferenceTable {
                    Type = 3,
                    Name = "bgm_option",
                    Name2 = string.Empty,
                    Id = 3
                },
                new AcfReferenceTable {
                    Type = 3,
                    Name = "bgm_submix",
                    Name2 = string.Empty,
                    Id = 9
                }
            };
            var synthCommand = new[] {
                new SynthCommandTable {
                    Command = new byte[14] {
                        0x00, 0x24, 0x04, 0x05, 0xdc, 0x01, 0xc8, 0x00, 0x28, 0x04, 0x05, 0xeb, 0x02, 0xc8
                    }
                },
                new SynthCommandTable {
                    Command = new byte[7] {
                        0x00, 0x28, 0x04, 0x05, 0xeb, 0x02, 0xc8
                    }
                }
            };
            var trackEvent = new[] {
                new TrackEventTable {
                    Command = new byte[10] {
                        0x07, 0xd0, 0x04, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00
                    }
                },
                new TrackEventTable {
                    Command = new byte[27] {
                        0x03, 0xe7, 0x04, 0x00, 0x01, 0x28, 0xe0, 0x07, 0xd0, 0x04, 0x00, 0x02, 0x00, 0x01, 0x07, 0xd1,
                        0x04, 0x00, 0x01, 0x77, 0x00, 0x0f, 0xa0, 0x00, 0x00, 0x00, 0x00
                    }
                },
                new TrackEventTable {
                    Command = new byte[10] {
                        0x07, 0xd0, 0x04, 0x00, 0x02, 0x00, 0x02, 0x00, 0x00, 0x00
                    }
                }
            };
            var hcaData = File.ReadAllBytes(hcaFileName);
            var header = new HeaderTable {
                FileIdentifier = 0,
                Size = 0,
                Version = 0x01290000,
                Type = 0,
                Target = 0,
                CategoryExtension = 0,
                NumCueLimitListWorks = 0,
                NumCueLimitNodeWorks = 0,
                AcbVolume = 1,
                CharacterEncodingType = 0,
                CuePriorityType = byte.MaxValue,
                NumCueLimit = 0,
                VersionString = StandardAcbVersionString,
                Name = songName,
                AcfMd5Hash = Guid.NewGuid().ToByteArray(),
                AisacTable = null,
                GraphTable = null,
                GlobalAisacReferenceTable = null,
                AisacNameTable = null,
                AisacControlNameTable = null,
                AutoModulationTable = null,
                StreamAwbTocWorkOld = null,
                CueLimitWorkTable = null,
                StreamAwbTocWork_Old = null,
                OutsideLinkTable = null,
                BlockSequenceTable = null,
                BlockTable = null,
                EventTable = null,
                ActionTrackTable = null,
                WaveformExtensionDataTable = null,
                BeatSyncInfoTable = null,
                TrackCommandTable = null,
                SeqParameterPalletTable = null,
                TrackParameterPalletTable = null,
                SynthParameterPalletTable = null,
                PaddingArea = null,
                StreamAwbTocWork = null,
                StreamAwbAfs2Header = null,
                CueTable = cue,
                CueNameTable = cueName,
                WaveformTable = waveform,
                SynthTable = synth,
                SeqCommandTable = seqCommand,
                TrackTable = track,
                SequenceTable = sequence,
                AwbFile = hcaData,
                AcbGuid = Guid.NewGuid().ToByteArray(),
                StreamAwbHash = Guid.Empty.ToByteArray(),
                StringValueTable = stringValue,
                AcfReferenceTable = acfReference,
                SynthCommandTable = synthCommand,
                TrackEventTable = trackEvent
            };
            return header;
        }

        private const string StandardAcbVersionString = "\nACB Format/PC Ver.1.29.00 Build:\n";
        private const string DefaultSongName = "song3_bnthtr";

        private static readonly byte[] StandardAcfHash = { 0x0e, 0xf7, 0x50, 0x41, 0x55, 0x0d, 0xda, 0xda, 0x89, 0xd0, 0x4e, 0x74, 0xbc, 0x91, 0x32, 0x2c };

        private static readonly string HelpMessage = "Usage: AcbPack.exe <HCA live music file> <output ACB> [-n <song name>]";

    }
}
