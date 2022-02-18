using System;
using Mirror;
using UnityEngine;
using Utils;
namespace Units.Weapons {
	public class SolidProjectile : NetworkBehaviour {
		[SerializeField] private new Rigidbody rigidbody;
		private float _projectileLifetime;
		private float _fireTime;
		private int _projectileDamage;

		[ServerCallback]
		public void Update () {
			if (Time.time > _fireTime + _projectileLifetime) {
				NetworkServer.Destroy(gameObject);
			}
		}

		[Server]
		public void Fire (float launchForce, float projectileLifetime, int projectileDamage) {
			_projectileLifetime = projectileLifetime;
			_fireTime = Time.time;
			_projectileDamage = projectileDamage;
			rigidbody.velocity = transform.forward*launchForce;
		}

		[ServerCallback]
		public void OnTriggerEnter (Collider other) {
			if (other.TryGetComponentInParent(out NetworkIdentity networkIdentity)) {
				if (networkIdentity.connectionToClient == connectionToClient) { // TODO Replace with player team check
					return;
				}
			}
			NetworkServer.Destroy(gameObject);
			
			IDamageable damageable = other.GetComponentInParent<IDamageable>();
			if (damageable == null) {
				throw new MissingComponentException($"Expected {nameof(IDamageable)} component!");
			}
			damageable.DealDamage(_projectileDamage);

		}
	}
}
