using System;
using Mirror;
using Sirenix.OdinInspector;
using Units.Movement;
using UnityEngine;
namespace Units {
	[RequireComponent(typeof(UnitMovement))]
	public class Unit : NetworkBehaviour {
		[SerializeField][Required] private GameObject selectionCircle;

		public UnitMovement unitMovement { get; private set; }

		private void OnEnable () {
			unitMovement = GetComponent<UnitMovement>();
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
}
