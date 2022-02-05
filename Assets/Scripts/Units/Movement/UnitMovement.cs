using Mirror;
using Pathfinding;
using UnityEngine;
namespace Units.Movement {
	public class UnitMovement : NetworkBehaviour {
		private Seeker seeker;
		private IAstarAI ai;
		private void Awake () {
			seeker = GetComponent<Seeker>();
			ai = GetComponent<IAstarAI>();
		}

		[Command]
		public void CmdMove (Vector3 position) {
			ai.destination = position;
		}
	}
}
