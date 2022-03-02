using System.IO;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Persistence {
	[CreateAssetMenu(fileName = "Map Save System", menuName = "Scriptable Objects/Map/Map Save System")]
	public class MapSaveSystem : ScriptableObject {
		private string _path;
		[SerializeField] private GameMap gameMapPrefab;
		[ShowInInspector, ReadOnly] private GameMap _map;
		

		private void Awake () {
			_path = Application.persistentDataPath + "/maptest.json";
		}
		public void SaveMap () {
			GameMap.GameMapSaveData mapSaveData = _map.CreateSaveData();
			string saveData = JsonUtility.ToJson(mapSaveData, true);
			
			Debug.Log($"Saving to {_path}");
			using FileStream stream = new FileStream(_path, FileMode.Create);
			using StreamWriter writer = new StreamWriter(stream);
			writer.Write(saveData);
			Debug.Log("Finished saving");
		}

		public void LoadMap () {
			using FileStream stream = new FileStream(_path, FileMode.Open);
			StreamReader reader = new StreamReader(stream);
			string saveData = reader.ReadToEnd();
			GameMap.GameMapSaveData mapSaveData = JsonUtility.FromJson<GameMap.GameMapSaveData>(saveData);
			CreateMapIfNecessary();
			_map.GenerateFromMapData(mapSaveData);
		}

		public void NewMap () {
			CreateMapIfNecessary();
			_map.GenerateFlatMap();
		}

		private void CreateMapIfNecessary () {
			if (_map == null) {
				_map = Instantiate(gameMapPrefab, Vector3.zero, Quaternion.identity);
			}
		}
	}
}
