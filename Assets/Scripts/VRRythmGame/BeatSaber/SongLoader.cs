﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VRRythmGame.BeatSaber.MappingExtensions;
using VRRythmGame.BeatSaber.NoodleExtensions;

namespace VRRythmGame.BeatSaber {
    public class SongLoader {

        public static string ConvertSong(string filePath, GameManager gm) {
            try {
                Song song = JsonUtility.FromJson<Song>(File.ReadAllText(filePath + Path.DirectorySeparatorChar + "info.dat"));

                VRRythmGame.Song convertedSong;
                List<VRRythmGame.Beatmap> convertedBeatmaps = new List<VRRythmGame.Beatmap>();
            
                foreach (var difficulty in song._difficultyBeatmapSets) {
                    foreach (var difficultyBeatmap in difficulty._difficultyBeatmaps) {
                    
                        var beatmapPath = filePath + Path.DirectorySeparatorChar + difficultyBeatmap._beatmapFilename;
                        string beatmapJson = File.ReadAllText(beatmapPath);
                        VRRythmGame.Beatmap bm;
                        if (difficultyBeatmap._customData[0]._requirements.Contains("Mapping Extensions")) { //Array.IndexOf(difficultyBeatmap._customData[0]._requirements, "Mapping Extensions") > -1) {
                            bm = LoadMappingExtensionsSong(beatmapJson);
                        } else if (difficultyBeatmap._customData[0]._requirements.Contains("Noodle Extensions")) {
                            bm = LoadNoodleExtensionsSong(beatmapJson);
                        }else {
                            bm = LoadDefaultSong(beatmapJson);
                        }
                        convertedBeatmaps.Add(bm);
                    }
                }
                convertedSong = song.ToSong();

                return gm.SaveSongToFile(convertedSong, convertedBeatmaps.ToArray());
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }
        
        public static VRRythmGame.Beatmap LoadDefaultSong(string jsonString) {
            return JsonUtility.FromJson<Beatmap>(jsonString).ToBeatmap();
        }
    
        public static VRRythmGame.Beatmap LoadMappingExtensionsSong(string jsonString) {
            return JsonUtility.FromJson<MappingExtensions.Beatmap>(jsonString).ToBeatmap();
        }
    
        public static VRRythmGame.Beatmap LoadNoodleExtensionsSong(string jsonString) {
            return JsonUtility.FromJson<NoodleExtensions.Beatmap>(jsonString).ToBeatmap();
        }
    }
}