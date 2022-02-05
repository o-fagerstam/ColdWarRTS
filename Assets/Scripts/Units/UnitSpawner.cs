using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Units {
	public class UnitSpawner : NetworkBehaviour, IPointerClickHandler {
		[SerializeField] private GameObject unitPrefab;
		[SerializeField] private Transform spawnPoint;

		[Command]
		private void CmdSpawnUnit () {
			GameObject spawnedObject = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
			NetworkServer.Spawn(spawnedObject, connectionToClient);
		}


		public void OnPointerClick (PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) { return; }
			if (!hasAuthority) {return;}
			
			CmdSpawnUnit();
		}
	}
}
