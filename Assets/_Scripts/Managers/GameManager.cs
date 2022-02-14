using Persistence;
using Singleton;
namespace Managers {
	public class GameManager : ASingletonMonoBehaviour<GameManager> {
		
		private void Start () {
			MapSaveSystem mapSaveSystem = SingletonManager.Retrieve<MapSaveSystem>();
			mapSaveSystem.LoadMap();
		}

	}
}
