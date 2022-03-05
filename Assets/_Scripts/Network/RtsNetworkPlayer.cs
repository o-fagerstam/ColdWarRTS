using System;
using GameLogic;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Network {
	[RequireComponent(typeof(RtsPlayer))]
	public class RtsNetworkPlayer : NetworkBehaviour, IPlayerNameProvider {
		[SerializeField, AssetsOnly, Required] private RtsPlayerRuntimeValue localPlayerRuntimeValue;
		
		[SyncVar]
		[SerializeField] private string _playerName;
		public string Name => _playerName;

		private RtsPlayer _player;

		private void Awake () {
			_player = GetComponent<RtsPlayer>();
		}

		public override void OnStartServer () {
			base.OnStartServer();
			_playerName = $"NameyNameson{NetworkClient.connection.connectionId}";
		}

		public override void OnStartAuthority () {
			base.OnStartAuthority();
			Debug.Log("OnStartAuthority");
			localPlayerRuntimeValue.Value = _player;
		}
	}
}
