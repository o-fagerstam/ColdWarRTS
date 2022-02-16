using System;
using Units;
using Units.Movement;
using UnityEngine;
namespace Controls {
	public class MoveUnitCommand : AUnitCommand {
		private readonly Vector3 _point;
		private UnitMovement.MoveToPositionState _trackedState;
		public MoveUnitCommand (Unit commandOwner, Vector3 point) : base(commandOwner) {
			_point = point;
		}
		public override void DoCommand () {
			_trackedState = new UnitMovement.MoveToPositionState(_commandOwner.UnitMovement, _point, 1f);
			_trackedState.OnPositionReached += HandlePositionReached;
			_commandOwner.UnitMovement.SetState(_trackedState);
		}
		private void HandlePositionReached (object sender, UnitMovement.MoveToPositionState.OnPositionReachedArgs onPositionReachedArgs) {
			_trackedState.OnPositionReached -= HandlePositionReached;
			Debug.Log("Finished Move command");
			InvokeOnCommandFinished();
		}
	}
}
