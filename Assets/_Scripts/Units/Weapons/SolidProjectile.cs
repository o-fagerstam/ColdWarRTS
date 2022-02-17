using System;
using Mirror;
using UnityEngine;
namespace Units.Weapons {
	public class SolidProjectile : NetworkBehaviour {
		[SerializeField] private Rigidbody rigidbody;
		private float _projectileLifetime;
		private float _fireTime;

		[ServerCallback]
		public void Update () {
			if (Time.time > _fireTime + _projectileLifetime) {
				NetworkServer.Destroy(gameObject);
			}
		}

		[Server]
		public void Fire (float launchForce, float projectileLifetime) {
			_projectileLifetime = projectileLifetime;
			_fireTime = Time.time;
			rigidbody.velocity = transform.forward*launchForce;
		}
	}
}
