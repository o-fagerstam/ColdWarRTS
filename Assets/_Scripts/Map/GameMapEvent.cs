using System;
using ScriptableObjectArchitecture;
using UnityEngine;
namespace Map {
	[CreateAssetMenu(fileName = "Game Map Event", menuName = "Scriptable Objects/Map/Game Map Event")]
	public class GameMapEvent  : ScriptableEvent<GameMapEventArgs> {}
	public class GameMapEventArgs : EventArgs {
		public GameMap Map;
	}
}
