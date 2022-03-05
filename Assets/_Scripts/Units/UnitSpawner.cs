using GameLogic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Units {
	public class UnitSpawner : NetworkBehaviour, IPointerClickHandler {
		[SyncVar]
		private GameObject _ownerGameObject;
		public RtsPlayer Owner => _ownerGameObject.GetComponent<RtsPlayer>();
		[SerializeField] private Unit unitPrefab;
		[SerializeField] private Transform spawnPoint;

		public void Initialize (RtsPlayer owner) {
			_ownerGameObject = owner.gameObject;
		}

		[Command]
		private void CmdSpawnUnit () {
			Unit spawnedUnit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
			spawnedUnit.Initialize(Owner);
			NetworkServer.Spawn(spawnedUnit.gameObject, connectionToClient);
		}


		public void OnPointerClick (PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) { return; }
			if (!hasAuthority) {return;}
			
			CmdSpawnUnit();
		}
	}
}
