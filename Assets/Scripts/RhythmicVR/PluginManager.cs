using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

namespace RhythmicVR {
	public class PluginManager {
		private readonly List<PluginBaseClass> loadedPlugins = new List<PluginBaseClass>();
		
		private readonly List<Gamemode> loadedGamemodes = new List<Gamemode>();
		private readonly List<GameObject> loadedEnvironments = new List<GameObject>();
		private readonly List<GenericTrackedObject> loadedTrackedObjects = new List<GenericTrackedObject>();
		private readonly List<TargetObject> loadedTargetObjects = new List<TargetObject>();
		private readonly List<PluginBaseClass> miscPlugins = new List<PluginBaseClass>();

		private readonly Core core;

		public PluginManager(Core core) {
			this.core = core;
		}

		/// <summary>
		/// Add a plugin to the list
		/// </summary>
		/// <param name="plugin">The plugin to add</param>
		public void AddPlugin(PluginBaseClass plugin) {
			try {
				loadedPlugins.Add(plugin);
				switch (plugin.type) {
					case AssetType.Environment:
						loadedEnvironments.Add(plugin.gameObject);
						break;
					case AssetType.Gamemode:
						loadedGamemodes.Add(plugin.GetComponentInChildren<Gamemode>());
						plugin.Init(core);
						break;
					case AssetType.Misc:
						var pl = core.SimpleInstantiate(plugin.gameObject).GetComponent<PluginBaseClass>();
						miscPlugins.Add(pl);
						pl.Init(core);
						break;
					case AssetType.TargetObject:
						loadedTargetObjects.Add(plugin.GetComponentInChildren<TargetObject>());
						break;
					case AssetType.VisualTrackedObject:
						loadedTrackedObjects.Add(plugin.GetComponentInChildren<GenericTrackedObject>());
						break;
				}
			}
			catch (Exception e) {
				Debug.Log("could not load Plugin: " + plugin.pluginName);
				Debug.Log(e);
			}
		}

		public void AddPlugins(PluginBaseClass[] plugins) {
			foreach (var plugin in plugins) {
				AddPlugin(plugin);
			}
		}

		public PluginBaseClass Find(string searchString) {
			return loadedPlugins.FirstOrDefault(plugin => plugin.pluginName == searchString);
		}
		
		public List<PluginBaseClass> GetPlugins() {
			return loadedPlugins;
		}
		
		/// <summary>
		/// Return all gamemodes
		/// </summary>
		/// <returns></returns>
		public List<Gamemode> GetAllGamemodes() {
			return loadedGamemodes;
		}
		
		/// <summary>
		/// Return all Environment Prefabs
		/// </summary>
		/// <returns></returns>
		public List<GameObject> GetAllEnvironments() {
			return loadedEnvironments;
		}
		public List<PluginBaseClass> GetAllMiscelaneousPlugins() {
			return miscPlugins;
		}
		public List<GenericTrackedObject> GetAllTrackedObjects() {
			return loadedTrackedObjects;
		}
		public List<TargetObject> GetAllTargets() {
			return loadedTargetObjects;
		}

		public void LoadPluginsFromFolder(string path) {
			List<PluginBaseClass> pluginsOut = new List<PluginBaseClass>();

			string tempPath = core.config.tempPluginRuntimePath;

			Directory.CreateDirectory(tempPath);
			

			if (Util.EnsureDirectoryIntegrity(path, true)) {
				string[] pluginPaths = Directory.GetFiles(path);
				foreach (var pluginPath in pluginPaths) {
					string destPath = tempPath + Path.GetFileName(pluginPath);
					var zip = new FastZip();
					zip.ExtractZip(pluginPath, destPath, "");
					string destPathPlatform = destPath + "/";
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
					destPathPlatform += "win64";
#elif (UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX)
					destPathPlatform += "lin64";
#endif
					destPathPlatform = Directory.GetFiles(destPathPlatform)[0];
					AssetBundle assetBundle = AssetBundle.LoadFromFile(destPathPlatform);
					string[] assetNames = assetBundle.GetAllAssetNames();
					string assetName = "";
					foreach (var asset in assetNames) {
						if (asset.Contains("plugin") && asset.Contains(".prefab")) {
							assetName = asset;
							break;
						}
					}
					GameObject assetObject = assetBundle.LoadAsset<GameObject>(assetName);
					PluginBaseClass plugin = assetObject.GetComponentInChildren<PluginBaseClass>();
					pluginsOut.Add(plugin);
				}
			}

			try {
				Directory.Delete(tempPath);
			}
			catch (Exception e) {
				Debug.Log("Could not delete temporary plugin directory");
			}

			AddPlugins(pluginsOut.ToArray());
		}
	}
}