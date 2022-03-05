using System.Collections.Generic;
using Mirror;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
namespace GameLogic {
	public class RtsPlayer : NetworkBehaviour {
		[SerializeField, AssetsOnly, Required] private RtsPlayerRuntimeSet allPlayers;
		[SerializeField, AssetsOnly, Required] private UnitRuntimeSet allUnits;
		public int PlayerId { get; private set; }
		
		private IPlayerNameProvider _playerNameProvider;
		public string Name => _playerNameProvider.Name;
		
		private HashSet<Unit> _units = new HashSet<Unit>();
		public IEnumerable<Unit> Units => _units;

		public void Initialize (int playerId, IPlayerNameProvider playerNameProvider) {
			PlayerId = playerId;
			_playerNameProvider = playerNameProvider;
		}

		private void OnEnable () {
			allUnits.OnElementAdded += HandleUnitAdded;
			allUnits.OnElementRemoved += HandleUnitRemoved;
			
			allPlayers.Add(this);
		}

		private void OnDisable () {
			allPlayers.Remove(this);

			allUnits.OnElementAdded -= HandleUnitAdded;
			allUnits.OnElementRemoved -= HandleUnitRemoved;
		}

		private void HandleUnitAdded (Unit unit) {
			if (unit.Owner == this) {
				_units.Add(unit);
			}
		}

		private void HandleUnitRemoved (Unit unit) {
			if (unit.Owner == this) {
				_units.Remove(unit);
			}
		}
	}
}
