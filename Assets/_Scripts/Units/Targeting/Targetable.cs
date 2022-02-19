using System;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Units.Targeting {
	public class Targetable : NetworkBehaviour {
		[SerializeField] [Required] [ChildGameObjectsOnly] private Transform aimAtPoint;
		public Transform AimAtPoint => aimAtPoint;

		public event EventHandler OnBecomeUntargetable;

		public override void OnStopServer () {
			base.OnStopServer();
			OnBecomeUntargetable?.Invoke(this, EventArgs.Empty);
		}
	}
}
