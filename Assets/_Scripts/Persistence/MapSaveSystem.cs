using System;
using System.IO;
using Map;
using Singleton;
using UnityEngine;
using Utils;
namespace Persistence {
	public class MapSaveSystem : ASingletonMonoBehaviour<MapSaveSystem> {
		private string _path;
		[SerializeField] private GameMap gameMapPrefab;

		private void Awake () {
			_path = Application.persistentDataPath + "/maptest.json";
		}
		public void SaveMap () {
			GameMap.GameMapSaveData mapSaveData = SingletonManager.Retrieve<GameMap>().CreateSaveData();
			string saveData = JsonUtility.ToJson(mapSaveData, true);
			
			UnityEngine.Debug.Log($"Saving to {_path}");
			using FileStream stream = new FileStream(_path, FileMode.Create);
			using StreamWriter writer = new StreamWriter(stream);
			writer.Write(saveData);
			UnityEngine.Debug.Log("Finished saving");
		}

		public void LoadMap () {
			using FileStream stream = new FileStream(_path, FileMode.Open);
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
				SafeDestroyUtil.SafeDestroyGameObject(oldMap);
			}
			GameMap newMap = Instantiate(gameMapPrefab, Vector3.zero, Quaternion.identity);
		}
	}
}
