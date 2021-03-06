﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RhythmicVR.BeatSaber {
    /// <summary>
    /// Beat saber song converter
    /// ----
    /// converts beatsaber song files and beatmaps as a whole into RhythmicVR songs and beatmaps.
    /// Supports mapping extensions and noodle extensions
    /// </summary>
    public class SongLoader {

        public static string ConvertSong(string filePath, Core core) {
            try {
                Song song = JsonUtility.FromJson<Song>(File.ReadAllText(filePath + Path.DirectorySeparatorChar + "info.dat"));

                RhythmicVR.Song convertedSong;
                List<BlockSong.Beatmap> convertedBeatmaps = new List<BlockSong.Beatmap>();
            
                foreach (var difficulty in song._difficultyBeatmapSets) {
                    foreach (var difficultyBeatmap in difficulty._difficultyBeatmaps) {
                    
                        var beatmapPath = filePath + Path.DirectorySeparatorChar + difficultyBeatmap._beatmapFilename;
                        string beatmapJson = File.ReadAllText(beatmapPath);
                        BlockSong.Beatmap bm;
                        if (difficultyBeatmap._customData._requirements.Contains("Mapping Extensions")) { //Array.IndexOf(difficultyBeatmap._customData._requirements, "Mapping Extensions") > -1) { 
                            bm = JsonUtility.FromJson<BeatSaberImporter.MappingExtensions.Beatmap>(beatmapJson).ToBeatmap();
                        } else if (difficultyBeatmap._customData._requirements.Contains("Noodle Extensions")) {
                            bm = JsonUtility.FromJson<BeatSaberImporter.NoodleExtensions.Beatmap>(beatmapJson).ToBeatmap();
                        }else {
                            bm = JsonUtility.FromJson<BeatSaberImporter.Beatmap>(beatmapJson).ToBeatmap();
                        }

                        foreach (var note in bm.notes) {
                            note.time = (note.time / song._beatsPerMinute) * 60;
                        }
                        convertedBeatmaps.Add(bm);
                    }
                }
                convertedSong = song.ToSong();
                var coverPath = filePath + Path.DirectorySeparatorChar + song._coverImageFilename;
                var audioPath = filePath + Path.DirectorySeparatorChar + song._songFilename;

                Debug.Log("Imported Beatsaber Song successfully.");
                Debug.Log(convertedSong.songName + " - " + convertedSong.songAuthorName + " - " + convertedSong.albumName);
                return core.SaveSongToFile(convertedSong, convertedBeatmaps.ToArray(), coverFilePath:coverPath, audioFilePath:audioPath);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
