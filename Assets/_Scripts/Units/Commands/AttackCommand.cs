using Units;
using Units.Movement;
using Units.Targeting;
namespace Controls {
	public class AttackCommand : AUnitCommand {
		private Targetable _target;
		public AttackCommand (Unit commandOwner, Targetable target) : base(commandOwner) {
			_target = target;
		}
		public override void DoCommand () {
			_commandOwner.UnitMovement.SetState(new UnitMovement.MoveTowardsTargetableState(_commandOwner.UnitMovement, _target, 4.8f)); //TODO Make chase range depend on weapon range
			_commandOwner.Targeter.SetTarget(_target);
		}
	}
}
