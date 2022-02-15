using System;
using Mirror;
using Sirenix.OdinInspector;
using Units.Movement;
using Units.Targeting;
using UnityEngine;
namespace Units {
	[RequireComponent(typeof(UnitMovement))]
	public class Unit : NetworkBehaviour {
		[SerializeField, Required] private GameObject selectionCircle;

		[SerializeField, Required, ChildGameObjectsOnly]
		private UnitMovement unitMovement;
		public UnitMovement UnitMovement => unitMovement;
		[SerializeField, Required, ChildGameObjectsOnly]
		private Targeter targeter;
		public Targeter Targeter => targeter;

		public static event EventHandler<OnUnitSpawnedArgs> ServerOnUnitSpawned;
		public static event EventHandler<OnUnitDespawnedArgs> ServerOnUnitDespawned;		
		public static event EventHandler<OnUnitSpawnedArgs> AuthorityOnUnitSpawned;
		public static event EventHandler<OnUnitDespawnedArgs> AuthorityOnUnitDespawned;
		

		private void OnEnable () {
			unitMovement = GetComponent<UnitMovement>();
		}

		public override void OnStartServer () {
			base.OnStartServer();
			ServerOnUnitSpawned?.Invoke(this, new OnUnitSpawnedArgs{unit = this});
		}

		public override void OnStopServer () {
			base.OnStopServer();
			ServerOnUnitDespawned?.Invoke(this, new OnUnitDespawnedArgs{unit = this});
		}

		public override void OnStartAuthority () {
			base.OnStartAuthority();
			AuthorityOnUnitSpawned?.Invoke(this, new OnUnitSpawnedArgs(){unit =  this});
		}

		public override void OnStopAuthority () {
			base.OnStopAuthority();
			AuthorityOnUnitDespawned?.Invoke(this, new OnUnitDespawnedArgs(){unit = this});
		}

		[Client]
		public void Select () {
			if (!hasAuthority) {return;}
			selectionCircle.SetActive(true);
		}

		[Client]
		public void Deselect () {
			if (!hasAuthority) {return;}
			selectionCircle.SetActive(false);
		}
	}
	public class OnUnitSpawnedArgs : EventArgs {
		public Unit unit;
	}
	public class OnUnitDespawnedArgs : EventArgs {
		public Unit unit;
	}
}
