using System;
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
			_commandOwner.UnitMovement.MoveTowardsTargetable(_target); //TODO Make chase range depend on weapon range
			_commandOwner.Targeter.SetTarget(_target);
			_target.OnBecomeUntargetable += HandleTargetableBecomeUntargetable;
		}
		private void HandleTargetableBecomeUntargetable (object sender, EventArgs e) {
			_target.OnBecomeUntargetable -= HandleTargetableBecomeUntargetable;
			InvokeOnCommandFinished();
		}
	}
}
