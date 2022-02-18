using System;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Units {
	public class UnitHealth : NetworkBehaviour, IDamageable {

		[SerializeField, Required, ChildGameObjectsOnly]
		private Unit unit;

		[SerializeField] private int maxHealth = 10000;
		public int MaxHealth => maxHealth;
		[SyncVar(hook = nameof(HandleHealthUpdated))] private int _currentHealth;
		public int CurrentHealth => _currentHealth;

		public event EventHandler<ClientOnHealthUpdateArgs> ClientOnHealthUpdated; 

		public override void OnStartServer () {
			base.OnStartServer();
			_currentHealth = maxHealth;
		}

		[Server]
		public void DealDamage (int damageAmount) {
			if (_currentHealth == 0) {return;}

			_currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

			if (_currentHealth == 0) {
				unit.ServerDie();
				Debug.Log("Dead!");
			}
		}

		[Client]
		private void HandleHealthUpdated (int oldHealth, int newHealth) {
			ClientOnHealthUpdated?.Invoke(this, new ClientOnHealthUpdateArgs(){oldHealth =  oldHealth, newHealth = newHealth, maxHealth = maxHealth});
		}
	}

	public class ClientOnHealthUpdateArgs : EventArgs {
		public int maxHealth;
		public int oldHealth;
		public int newHealth;
	}
}
