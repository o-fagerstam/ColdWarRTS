using Persistence;
using Singleton;
namespace GameLogic {
	public class GameManager : ASingletonMonoBehaviour<GameManager> {

		private void Start () {
			MapSaveSystem mapSaveSystem = SingletonManager.Retrieve<MapSaveSystem>();
			mapSaveSystem.LoadMap();
		}

	}
}
