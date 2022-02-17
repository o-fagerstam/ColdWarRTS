using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Units.Targeting {
	public class TurretRotator : NetworkBehaviour {
		[SerializeField][Required][ChildGameObjectsOnly]
		private Transform turretPivot;
		[SerializeField][Required][ChildGameObjectsOnly]
		private Targeter targeter;
		[SerializeField] private float rotationSpeed = 40f;
		
		[ServerCallback]
		private void Update () {
			Quaternion targetRotation;
			if (targeter.Target == null) {
				targetRotation = targeter.transform.rotation;
			} else {
				targetRotation = Quaternion.LookRotation(targeter.Target.transform.position - transform.position, targeter.transform.up);
			}
			turretPivot.rotation = Quaternion.RotateTowards(turretPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}
}
