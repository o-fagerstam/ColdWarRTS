using Persistence;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Managers {
	public class GameManager : MonoBehaviour {
		[SerializeField, Required, AssetsOnly] private MapSaveSystem mapSaveSystem;
		private void Start () {
			mapSaveSystem.LoadMap();
		}

	}
}
