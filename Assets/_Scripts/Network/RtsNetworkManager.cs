using Mirror;
using UnityEngine;
namespace Network {
	public class RtsNetworkManager : NetworkManager {
		[SerializeField] private GameObject unitSpawnerPrefab;
		public override void OnServerAddPlayer (NetworkConnection conn) {
			base.OnServerAddPlayer(conn);

			GameObject unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
			
			NetworkServer.Spawn(unitSpawnerInstance, conn);
		}
	}
}
