﻿using UnityEngine;

namespace RhythmicVR {
	[System.Serializable]
	public class Song {
		public int id;
		public string formatVersion;
		public string songName;
		public string songSubName;
		public string songAuthorName;
		public string albumName;
		public string levelAuthorName;
		public float beatsPerMinute;
		public float previewStartTime;
		public string songFile = "song.ogg";
		public string coverImageFile = "cover.jpg";
		public float startTimeOffset;
		public TrackingPoint[] trackingPoints;
		public string leftHandTool;
		public string rightHandTool;
		public string waistTool;
		public string leftFootTool;
		public string rightFootTool;
		public string environment;
		public Difficulty[] difficulties;
		[System.NonSerialized] public string pathToDir;
		[System.NonSerialized] public GameObject uiPanel;

	}
}
