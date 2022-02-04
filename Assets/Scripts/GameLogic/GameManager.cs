using System;
using Persistence;
using Singleton;
using UnityEngine;
namespace GameLogic {
	public class GameManager : ASingletonMonoBehaviour {

		private void Start () {
			MapSaveSystem mapSaveSystem = SingletonManager.Retrieve<MapSaveSystem>();
			mapSaveSystem.LoadMap();
		}

	}
}
