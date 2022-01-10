using System.IO;
using Map;
using UnityEngine;
namespace Persistence {
	public static class MapSaveSystem {
		private static readonly string path = Application.persistentDataPath + "/maptest.json";
		public static void SaveMap (GameMap map) {
			GameMap.GameMapData mapData = map.CreateMapData();
			string saveData = JsonUtility.ToJson(mapData, true);
			
			UnityEngine.Debug.Log($"Saving to {path}");
			using FileStream stream = new FileStream(path, FileMode.Create);
			using StreamWriter writer = new StreamWriter(stream);
			writer.Write(saveData);
			UnityEngine.Debug.Log("Finished saving");
		}

		public static void LoadMap (GameMap map) {
			using FileStream stream = new FileStream(path, FileMode.Open);
			StreamReader reader = new StreamReader(stream);
			string saveData = reader.ReadToEnd();
			GameMap.GameMapData mapData = JsonUtility.FromJson<GameMap.GameMapData>(saveData);
			map.GenerateFromMapData(mapData);
		}
	}
}
