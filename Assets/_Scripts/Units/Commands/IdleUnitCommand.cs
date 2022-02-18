using System;
using Units;
using Units.Movement;
namespace Controls {
	public class IdleUnitCommand : AUnitCommand {
		public IdleUnitCommand (Unit commandOwner) : base(commandOwner) {}
		public override void DoCommand () {
			_commandOwner.UnitMovement.StopMoving();
		}
	}
}
