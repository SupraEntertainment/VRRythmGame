﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject target;
    public static GameObject TARGET;
    public GameObject obstacle;
    public static GameObject OBSTACLE;
    
    [Header("Materials")]
    public static Material AMBIGUOUS_MAT;
    public static Material LEFT_MAT;
    public static Material RIGHT_MAT;
    public static Material CENTER_MAT;
    public Material ambiguousMaterial;
    public Material leftMaterial;
    public Material rightMaterial;
    public Material centerMaterial;

    [Header("Tracking points")] 
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform waist;
    
    [Header("Other Properties")]
    public float spawnDistance;
    public static float SPAWN_DISTANCE;
    private Config _config;

    void Start() {
        // set all static values
        TARGET = target;
        OBSTACLE = obstacle;

        SPAWN_DISTANCE = spawnDistance;
        
        AMBIGUOUS_MAT = ambiguousMaterial;
        LEFT_MAT = leftMaterial;
        RIGHT_MAT = rightMaterial;
        CENTER_MAT = centerMaterial;
        
        // load config file / create if it doesn't exist already
        try {
            _config = JsonUtility.FromJson<Config>(File.ReadAllText(Application.dataPath));
        }
        catch (Exception e) {
            Console.WriteLine(e);
            _config = new Config();
        }
        
        // create debug beatmap
        var bm = new Beatmap();
        List<TrackingPoint[]> possibleTypes = new List<TrackingPoint[]>(); 
        possibleTypes.Add(new TrackingPoint[]{TrackingPoint.LeftFoot});
        possibleTypes.Add(new TrackingPoint[]{TrackingPoint.LeftHand});
        possibleTypes.Add(new TrackingPoint[]{TrackingPoint.Waist});
        possibleTypes.Add(new TrackingPoint[]{TrackingPoint.LeftFoot});
        possibleTypes.Add(new TrackingPoint[]{TrackingPoint.LeftFoot, TrackingPoint.LeftHand, TrackingPoint.RightHand, TrackingPoint.RightFoot, TrackingPoint.Waist});
        possibleTypes.Add(new TrackingPoint[]{TrackingPoint.RightFoot});
        List<Note> notes = new List<Note>();
        for (int i = 0; i < 240; i++) {
            var note = new Note();
            note.time = Random.Range((i-0.5f)/4f,(i+0.5f)/4f);
            note.rotation = i * 6;
            note.type = possibleTypes[(int) Math.Floor(Random.value * 6)];
            note.xPos = Random.value * 2 - 1;
            note.yPos = Random.value * 2;
            notes.Add(note);
        }

        bm.notes = notes.ToArray();
        // play beatmap
        StartCoroutine(PlayBeatmap(bm));
    }

    private void FixedUpdate() {
        //SpawnTarget(1f, Random.value *2 - 1, Random.value *2, Random.value *360, /*Random.value *360*/ 0, );
    }

    // return tag string for tracker role 
    private string TrackerRoleToTag(TrackingPoint role) {
        switch (role) {
            case TrackingPoint.LeftHand:
                return "leftHand";
            case TrackingPoint.RightHand:
                return "rightHand";
            case TrackingPoint.Waist:
                return "waist";
            case TrackingPoint.LeftFoot:
                return "leftFoot";
            case TrackingPoint.RightFoot:
                return "rightFoot";
            default:
                return null;
        }
    }

    // load a song
    public void LoadSong(Song song) {
        
    }

    // start the selected beatmap
    public void StartBeatmap(Song song, Difficulty difficulty) {
        Beatmap bm = JsonUtility.FromJson<Beatmap>(File.ReadAllText(_config.SongSavePath + "/" + song.id + "_" + song.songName + "/" + difficulty.beatMapPath));
        StartCoroutine(PlayBeatmap(bm));
    }

    private static IEnumerator PlayBeatmap(Beatmap bm) {
        //var beatmapLength = bm.notes[bm.notes.Length-1].time;
        float currentTime = 0;
        for (int i = 0; i < bm.notes.Length; i++) {
            var note = bm.notes[i];
            yield return new WaitForSeconds(note.time - currentTime);
            currentTime = note.time;
            SpawnTarget(note.speed, note.xPos, note.yPos, note.cutDirection, note.rotation, note.type);
        }
    }

    // write song and all beatmaps to their files
    public void SaveSongToFile(Song songObject, Beatmap[] beatmaps) {
        string pathToSong = _config.SongSavePath + "/" + songObject.id + "_" + songObject.songName + "/";
        File.WriteAllText(pathToSong + "level.json", JsonUtility.ToJson(songObject));
        for (var index = 0; index < beatmaps.Length; index++) {
            var beatmap = beatmaps[index];
            File.WriteAllText(pathToSong + songObject.difficulties[index].beatMapPath, JsonUtility.ToJson(beatmap));
        }
    }

    
    /*
     * loads a gamemode from assetbundle
     */
    public void LoadGamemode(Gamemode gm) {
        target = gm.targetObject;
        foreach (var trackedDevicePair in gm.trackedObjects) {
            trackedDevicePair.prefab.GetComponent<GenericTrackedObject>().collider.gameObject.tag = TrackerRoleToTag(trackedDevicePair.role);
            Transform tracker;
            if (trackedDevicePair.role == TrackingPoint.LeftHand) {
                tracker = leftHand;
            } else if (trackedDevicePair.role == TrackingPoint.RightHand) {
                tracker = rightHand;
            } else if (trackedDevicePair.role == TrackingPoint.LeftFoot) {
                tracker = leftFoot;
            } else if (trackedDevicePair.role == TrackingPoint.RightFoot) {
                tracker = rightFoot;
            } else if (trackedDevicePair.role == TrackingPoint.Waist) {
                tracker = waist;
            } else {
                return;
            }
            Instantiate(trackedDevicePair.prefab, tracker);
        }
    }
    
    // spawn a target object (switching target objects for gamemodes is still missing!)
    public static void SpawnTarget(float speed, float xCoord, float yCoord, float viewRotation, float playspaceRoation, TrackingPoint[] hand) {
        GameObject cube = Instantiate(TARGET, new Vector3(xCoord, yCoord, SPAWN_DISTANCE), new Quaternion(0, 0, viewRotation, 0));
        cube.GetComponent<TargetObject>().InitNote(new Note(1, xCoord, yCoord, speed, hand, viewRotation, playspaceRoation));
    }

    // spawn a obstacle ^ same here
    public static void SpawnObstacle(float speed, float xCoord, float yCoord, float viewRotation, float playspaceRoation, float width, float height) {
        
    }
}

/*
 * config object containing all stored values
 */
internal class Config {
    public string SongSavePath { get; set; }

    public Config() {
        SongSavePath = Application.consoleLogPath;
    }
}
