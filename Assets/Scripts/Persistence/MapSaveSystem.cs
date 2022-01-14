using System;
using System.IO;
using Map;
using Singleton;
using UnityEngine;
using Utils;
namespace Persistence {
	public class MapSaveSystem : MonoBehaviour {
		private string path;
		[SerializeField] private GameMap gameMapPrefab;

		private void Start () {
			path = Application.persistentDataPath + "/maptest.json";
		}
		public void SaveMap () {
			GameMap.GameMapSaveData mapSaveData = SingletonManager.Retrieve<GameMap>().CreateSaveData();
			string saveData = JsonUtility.ToJson(mapSaveData, true);
			
			UnityEngine.Debug.Log($"Saving to {path}");
			using FileStream stream = new FileStream(path, FileMode.Create);
			using StreamWriter writer = new StreamWriter(stream);
			writer.Write(saveData);
			UnityEngine.Debug.Log("Finished saving");
		}

		public void LoadMap () {
			using FileStream stream = new FileStream(path, FileMode.Open);
			StreamReader reader = new StreamReader(stream);
			string saveData = reader.ReadToEnd();
			GameMap.GameMapSaveData mapSaveData = JsonUtility.FromJson<GameMap.GameMapSaveData>(saveData);
			RecreateMap();
			SingletonManager.Retrieve<GameMap>().GenerateFromMapData(mapSaveData);
		}

		public void NewMap () {
			RecreateMap();
			SingletonManager.Retrieve<GameMap>().GenerateFlatMap();
		}

		private void RecreateMap () {
			if (SingletonManager.IsRegistered<GameMap>()) {
				GameMap oldMap = SingletonManager.Retrieve<GameMap>();
				SingletonManager.Unregister(oldMap);
				SafeDestroyUtil.SafeDestroyGameObject(oldMap);
			}
			GameMap newMap = Instantiate(gameMapPrefab, Vector3.zero, Quaternion.identity);
			SingletonManager.Register(newMap);
		}
	}
}
