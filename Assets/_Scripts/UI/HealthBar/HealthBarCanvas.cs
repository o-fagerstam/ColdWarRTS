using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
namespace UI.HealthBar {
	[RequireComponent(typeof(Canvas))]
	public class HealthBarCanvas : MonoBehaviour {
		[SerializeField, Required, AssetsOnly] private HealthBar healthBarPrefab;
		[SerializeField, Required, AssetsOnly] private UnitRuntimeSet allUnits;

		private readonly Dictionary<Unit, HealthBar> _healthBars = new Dictionary<Unit, HealthBar>();

		private void OnEnable () {
			allUnits.OnElementAdded += HandleUnitSpawned;
			allUnits.OnElementRemoved += HandleUnitDespawned;
		}

		private void OnDisable () {
			allUnits.OnElementAdded -= HandleUnitSpawned;
			allUnits.OnElementRemoved -= HandleUnitDespawned;
		}
		private void HandleUnitSpawned (Unit unit) {
			CreateHealthBar(unit);
		}

		private void HandleUnitDespawned (Unit unit) {
			DestroyHealthBar(unit);
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


	}
}
