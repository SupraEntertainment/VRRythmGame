﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RhythmicVR.BeatSaber {
    public class SongLoader {

        public static string ConvertSong(string filePath, GameManager gm) {
            try {
                Song song = JsonUtility.FromJson<Song>(File.ReadAllText(filePath + Path.DirectorySeparatorChar + "info.dat"));

                RhythmicVR.Song convertedSong;
                List<RhythmicVR.Beatmap> convertedBeatmaps = new List<RhythmicVR.Beatmap>();
            
                foreach (var difficulty in song._difficultyBeatmapSets) {
                    foreach (var difficultyBeatmap in difficulty._difficultyBeatmaps) {
                    
                        var beatmapPath = filePath + Path.DirectorySeparatorChar + difficultyBeatmap._beatmapFilename;
                        string beatmapJson = File.ReadAllText(beatmapPath);
                        RhythmicVR.Beatmap bm;
                        if (difficultyBeatmap._customData._requirements.Contains("Mapping Extensions")) { //Array.IndexOf(difficultyBeatmap._customData._requirements, "Mapping Extensions") > -1) { 
                            bm = JsonUtility.FromJson<MappingExtensions.Beatmap>(beatmapJson).ToBeatmap();
                        } else if (difficultyBeatmap._customData._requirements.Contains("Noodle Extensions")) {
                            bm = JsonUtility.FromJson<NoodleExtensions.Beatmap>(beatmapJson).ToBeatmap();
                        }else {
                            bm = JsonUtility.FromJson<Beatmap>(beatmapJson).ToBeatmap();
                        }
                        convertedBeatmaps.Add(bm);
                    }
                }
                convertedSong = song.ToSong();
                Texture2D cover = new Texture2D(2, 2);
                cover.LoadImage(File.ReadAllBytes(filePath + Path.DirectorySeparatorChar + song._coverImageFilename));

                return gm.SaveSongToFile(convertedSong, convertedBeatmaps.ToArray(), cover);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}