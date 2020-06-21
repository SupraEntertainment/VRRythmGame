using System.IO;
using UnityEngine;

namespace VRRythmGame {
	public class Util {
		
        
		public static Sprite LoadSprite(string filePath) {
 
			// Load a PNG or JPG file from disk to a Sprite
			// Returns null if load fails
			
			var Tex2D = LoadTexture(filePath);
			if (Tex2D)
				return Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height),new Vector2(0,0), 100);
			return null;
		}
		
		public static Texture2D LoadTexture(string filePath) {
 
			// Load a PNG or JPG file from disk to a Texture2D
			// Returns null if load fails

			Debug.Log("loading image " + filePath);
 
			Texture2D Tex2D;
			byte[] FileData;
 
			if (File.Exists(filePath)){
				FileData = File.ReadAllBytes(filePath);
				Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
				if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
					return Tex2D;                 // If data = readable -> return texture
			}  
			return null;                     // Return null if load failed
		}
	}
}