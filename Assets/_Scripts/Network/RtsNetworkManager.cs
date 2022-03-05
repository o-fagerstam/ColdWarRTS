using GameLogic;
using Mirror;
using Units;
using UnityEngine;
namespace Network {
	public class RtsNetworkManager : NetworkManager {
		[SerializeField] private UnitSpawner unitSpawnerPrefab;
		private static int _playerIdCounter = 1;
		public override void OnServerAddPlayer (NetworkConnection conn) {
			base.OnServerAddPlayer(conn);
			
			InitializeNetworkPlayer(conn);
			CreateUnitSpawner(conn);
		}
		private static void InitializeNetworkPlayer (NetworkConnection conn) {
			RtsPlayer player = conn.identity.GetComponent<RtsPlayer>();
			RtsNetworkPlayer networkPlayer = conn.identity.GetComponent<RtsNetworkPlayer>();
			player.Initialize(_playerIdCounter++, networkPlayer);
		}
		private void CreateUnitSpawner (NetworkConnection conn) {

			Transform identityTransform = conn.identity.transform;
			UnitSpawner unitSpawnerInstance = Instantiate(unitSpawnerPrefab, identityTransform.position, identityTransform.rotation);
			unitSpawnerInstance.Initialize(conn.identity.GetComponent<RtsPlayer>());
			NetworkServer.Spawn(unitSpawnerInstance.gameObject, conn);
		}
	}
}
