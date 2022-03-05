using System.IO;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Persistence {
	[CreateAssetMenu(fileName = "Map Save System", menuName = "Scriptable Objects/Map/Map Save System")]
	public class MapSaveSystem : ScriptableObject {
		private string _path;
		[SerializeField] private GameMap gameMapPrefab;
		[SerializeField, AssetsOnly, Required] private GameMapScriptableValue gameMapSingletonValue;

		private void Awake () {
			_path = Application.persistentDataPath + "/maptest.json";
		}
		public void SaveMap () {
			GameMap.GameMapSaveData mapSaveData = gameMapSingletonValue.Value.CreateSaveData();
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
			gameMapSingletonValue.Value.GenerateFromMapData(mapSaveData);
		}

		public void NewMap () {
			CreateMapIfNecessary();
			gameMapSingletonValue.Value.GenerateFlatMap();
		}

		private void CreateMapIfNecessary () {
			if (gameMapSingletonValue.Value == null) {
				gameMapSingletonValue.Value = Instantiate(gameMapPrefab, Vector3.zero, Quaternion.identity);
			}
		}
	}
}
