using System;
using Singleton;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
namespace UI.HealthBar {
	[RequireComponent(typeof(Canvas))]
	public class HealthBarCanvas : MonoBehaviour {
		[SerializeField, Required, AssetsOnly] private HealthBar healthBarPrefab;

		private void Awake () {
			Unit.ClientOnUnitSpawned += HandleUnitSpawned;
		}
		private void HandleUnitSpawned (object sender, OnUnitSpawnedArgs e) {
			CreateHealthBar(e.unit);
		}

		private void CreateHealthBar (Unit unit) {
			HealthBar newHealthBar = Instantiate(healthBarPrefab, transform);
			newHealthBar.BeginTracking(unit);
		}

		private void OnDestroy () {
			Unit.ClientOnUnitSpawned -= HandleUnitSpawned;
		}
	}
}
