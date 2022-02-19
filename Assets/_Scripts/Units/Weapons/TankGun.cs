using Mirror;
using Sirenix.OdinInspector;
using Units.Targeting;
using UnityEngine;
namespace Units.Weapons {
	public class TankGun : NetworkBehaviour {
		[SerializeField, Required, ChildGameObjectsOnly]
		private Targeter targeter;
		[SerializeField, Required, ChildGameObjectsOnly]
		private Transform bulletSpawnPoint;
		[SerializeField, Required, ChildGameObjectsOnly]
		private Transform weaponForwardPivot;
		[SerializeField, Required, AssetsOnly]
		private SolidProjectile projectilePrefab;
		[SerializeField] private float weaponRange = 5f;
		public float Range => weaponRange;
		[SerializeField] private float rateOfFire = 1f;
		[SerializeField] private float projectileLaunchForce = 10f;
		[SerializeField] private float projectileLifetime = 5f;
		[SerializeField] private int projectileDamage = 3000;
		private float _lastFireTime = Mathf.NegativeInfinity;
		
		[ServerCallback]
		private void Update () {
			if (targeter.Target == null) {return;}
			if (!CanFireAtTarget()) {return;}

			Fire();
		}

		[Server]
		private void Fire () {
			SolidProjectile projectileInstance = Instantiate(projectilePrefab, bulletSpawnPoint.position, weaponForwardPivot.rotation);
			NetworkServer.Spawn(projectileInstance.gameObject, connectionToClient); //TODO Set player ID in bullet instead of giving authority over it
			projectileInstance.Fire(projectileLaunchForce, projectileLifetime, projectileDamage);
			_lastFireTime = Time.time;
		}

		[Server]
		private bool CanFireAtTarget () {
			Vector3 vectorToTarget = targeter.Target.transform.position - transform.position;
			return TargetInRange(vectorToTarget) &&
			       WeaponHasAngleOnTarget(vectorToTarget) &&
			       WeaponIsLoaded();
		}
		[Server]
		private bool TargetInRange (Vector3 vectorToTarget) => vectorToTarget.sqrMagnitude < weaponRange*weaponRange;
		[Server]
		private bool WeaponHasAngleOnTarget (Vector3 vectorToTarget) => Vector3.Angle(vectorToTarget, weaponForwardPivot.forward) < 2f;
		[Server]
		private bool WeaponIsLoaded () => Time.time > _lastFireTime + (1f/rateOfFire);
	}
}
