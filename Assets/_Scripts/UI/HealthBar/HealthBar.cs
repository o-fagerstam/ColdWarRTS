using System;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
using UnityEngine.UI;
namespace UI.HealthBar {
	[RequireComponent(typeof(WorldSpaceTransformTracker))]
	public class HealthBar : MonoBehaviour {
		[ShowInInspector, ReadOnly] private UnitHealth _health;
		[ShowInInspector, ReadOnly] private WorldSpaceTransformTracker _tracker;
		[SerializeField, Required, ChildGameObjectsOnly] private Image healthBarImage;

		private void Awake () {
			_tracker = GetComponent<WorldSpaceTransformTracker>();
		}

		public void BeginTracking (Unit unit) {
			if (_health != null) {
				_health.ClientOnHealthUpdated -= HandleHealthUpdated;
			}
			_health = unit.Health; 
			_health.ClientOnHealthUpdated += HandleHealthUpdated;
			UpdateHealthBar(_health.CurrentHealth, _health.MaxHealth);

			_tracker.SetTrackedTransform(unit.transform);
		}

		public void OnDestroy () {
			_health.ClientOnHealthUpdated -= HandleHealthUpdated;
		}

		private void HandleHealthUpdated (object sender, ClientOnHealthUpdateArgs e) {
			UpdateHealthBar(e.newHealth, e.maxHealth);
		}
		private void UpdateHealthBar (int currentHealth, int maxHealth) {

			float healthPercentage = (float)currentHealth/maxHealth;
			healthBarImage.fillAmount = healthPercentage;
		}
	}
}
