using System;
using UnityEngine;
namespace Units.Commands {
	public class MoveUnitCommand : AUnitCommand {
		private readonly Vector3 _point;
		public MoveUnitCommand (Unit commandOwner, Vector3 point) : base(commandOwner) {
			_point = point;
		}
		public override void DoCommand () {
			_commandOwner.UnitMovement.MoveToPosition(_point);
			_commandOwner.UnitMovement.OnStateFinished += HandlePositionReached;
		}
		private void HandlePositionReached (object sender, EventArgs e) {
			_commandOwner.UnitMovement.OnStateFinished -= HandlePositionReached;
			Debug.Log("Finished Move command");
			InvokeOnCommandFinished();
		}
	}
}
