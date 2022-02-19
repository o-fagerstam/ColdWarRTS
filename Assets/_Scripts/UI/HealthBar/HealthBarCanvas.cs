using System;
using System.Collections.Generic;
using Singleton;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
namespace UI.HealthBar {
	[RequireComponent(typeof(Canvas))]
	public class HealthBarCanvas : MonoBehaviour {
		[SerializeField, Required, AssetsOnly] private HealthBar healthBarPrefab;

		private Dictionary<Unit, HealthBar> _healthBars = new Dictionary<Unit, HealthBar>();

		private void Awake () {
			Unit.ClientOnUnitSpawned += HandleUnitSpawned;
			Unit.ClientOnUnitDespawned += HandleUnitDespawned;
		}
		private void HandleUnitSpawned (object sender, OnUnitSpawnedArgs e) {
			CreateHealthBar(e.unit);
		}

		private void HandleUnitDespawned (object sender, OnUnitDespawnedArgs e) {
			DestroyHealthBar(e.unit);
		}

		private void CreateHealthBar (Unit unit) {
			HealthBar newHealthBar = Instantiate(healthBarPrefab, transform);
			newHealthBar.BeginTracking(unit);
			_healthBars[unit] = newHealthBar;
		}
		
		private void DestroyHealthBar (Unit unit) {

			HealthBar healthBar = _healthBars[unit];
			_healthBars.Remove(unit);
			Destroy(healthBar.gameObject);
		}

		private void OnDestroy () {
			Unit.ClientOnUnitSpawned -= HandleUnitSpawned;
		}
	}
}
