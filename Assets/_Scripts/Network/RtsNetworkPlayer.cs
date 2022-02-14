using System.Collections.Generic;
using Mirror;
using Units;
using UnityEngine;
namespace Network {
	public class RtsNetworkPlayer : NetworkBehaviour {

		private HashSet<Unit> _units = new HashSet<Unit>();
		public IEnumerable<Unit> Units => _units;

		private static Dictionary<int, RtsNetworkPlayer> s_players = new Dictionary<int, RtsNetworkPlayer>();
		private static int s_playerIdCounter = 1;

		[SyncVar]
		[SerializeField] private int _playerID;
		[SyncVar]
		[SerializeField] private string _playerName;
		public string PlayerName => _playerName;

		public override void OnStartServer () {
			base.OnStartServer();
			_playerID = s_playerIdCounter++;
			s_players[_playerID] = this;
			_playerName = $"NameyNameson{_playerID}";
			Unit.ServerOnUnitSpawned += HandleServerOnUnitSpawned;
			Unit.ServerOnUnitDespawned += HandleServerOnUnitDespawned;
		}

		public override void OnStopServer () {
			base.OnStopServer();
			s_players.Remove(_playerID);
			Unit.ServerOnUnitSpawned -= HandleServerOnUnitSpawned;
			Unit.ServerOnUnitDespawned -= HandleServerOnUnitDespawned;
		}

		public override void OnStartAuthority () {
			base.OnStartAuthority();
			Unit.AuthorityOnUnitSpawned += HandleAuthorityOnUnitSpawned;
			Unit.AuthorityOnUnitDespawned += HandleAuthorityOnUnitDespawned;
		}

		public override void OnStopAuthority () {
			base.OnStopAuthority();
			Unit.AuthorityOnUnitSpawned -= HandleAuthorityOnUnitSpawned;
			Unit.AuthorityOnUnitDespawned -= HandleAuthorityOnUnitDespawned;
		}

		private void HandleServerOnUnitSpawned (object sender, OnUnitSpawnedArgs e) {
			if (e.unit.connectionToClient.connectionId != connectionToClient.connectionId) {
				return;
			}
			_units.Add(e.unit);
		}
		
		private void HandleServerOnUnitDespawned (object sender, OnUnitDespawnedArgs e) {
			if (e.unit.connectionToClient.connectionId != connectionToClient.connectionId) {
				return;
			}
			_units.Remove(e.unit);
		}

		private void HandleAuthorityOnUnitSpawned (object sender, OnUnitSpawnedArgs e) {
			if (isServer) {return;}
			_units.Add(e.unit);
		}
		private void HandleAuthorityOnUnitDespawned (object sender, OnUnitDespawnedArgs e) {
			if (isServer) {return;}
			_units.Add(e.unit);
		}
	}
}
