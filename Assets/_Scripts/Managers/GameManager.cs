using Persistence;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Managers {
	public class GameManager : ASingletonMonoBehaviour<GameManager> {
		[SerializeField, Required, AssetsOnly] private MapSaveSystem mapSaveSystem;
		private void Start () {
			mapSaveSystem.LoadMap();
		}

	}
}
