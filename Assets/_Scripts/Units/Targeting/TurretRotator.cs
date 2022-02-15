using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Units.Targeting {
	public class TurretRotator : NetworkBehaviour {
		[SerializeField][Required][ChildGameObjectsOnly]
		private Transform turretPivot;
		[SerializeField][Required][ChildGameObjectsOnly]
		private Targeter targeter;
		
		[ServerCallback]
		private void Update () {
			if (!isServer) { return; }
			if (targeter.Target != null) {
				turretPivot.LookAt(targeter.Target.transform, Vector3.up);
			}
		}
	}
}
