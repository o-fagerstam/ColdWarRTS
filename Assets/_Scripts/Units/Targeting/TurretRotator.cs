using System;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Units.Targeting {
	public class TurretRotator : MonoBehaviour {
		[SerializeField][Required][ChildGameObjectsOnly]
		private Transform turretPivot;
		[SerializeField][Required][ChildGameObjectsOnly]
		private Targeter targeter;

		[Server]
		private void Update () {
			if (targeter.Target != null) {
				turretPivot.LookAt(targeter.Target.transform, Vector3.up);
			}
		}
	}
}
