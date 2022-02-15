using Mirror;
using Pathfinding;
using UnityEngine;
namespace Units.Movement {
	public class UnitMovement : NetworkBehaviour {
		private Seeker _seeker;
		private IAstarAI _ai;

		private void Awake () {
			_seeker = GetComponent<Seeker>();
			_ai = GetComponent<IAstarAI>();
		}

		[Command]
		public void CmdMove (Vector3 position) {
			_ai.destination = position;
		}
	}
}
