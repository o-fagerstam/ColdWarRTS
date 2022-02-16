using System;
using Mirror;
using Pathfinding;
using Units.Targeting;
using UnityEngine;
namespace Units.Movement {
	public class UnitMovement : NetworkBehaviour {
		private Seeker _seeker;
		private IAstarAI _ai;
		private AUnitMovementState _movementState;

		private void Awake () {
			_seeker = GetComponent<Seeker>();
			_ai = GetComponent<IAstarAI>();
			SetState(new IdleState(this));
		}

		private void Update () {
			if (isServer) {
				_movementState.UpdateMovement();
			}
		}

		public void SetState (AUnitMovementState state) {
			_movementState = state;
			_movementState.OnEnterState();
		}

		public abstract class AUnitMovementState {
			protected UnitMovement Outer;
			protected AUnitMovementState (UnitMovement outer) {
				Outer = outer;
			}

			public abstract void UpdateMovement ();
			public abstract void OnEnterState ();
		}

		private class IdleState : AUnitMovementState {
			public IdleState(UnitMovement other) : base(other) {}
			public override void UpdateMovement () {}
			public override void OnEnterState () {
				Outer._ai.destination = Outer.transform.position;
				Outer._ai.SearchPath();
			}
		}

		public class MoveToPositionState : AUnitMovementState {
			private Vector3 _position;
			private float _sqrFinishDistance;

			public event EventHandler<OnPositionReachedArgs> OnPositionReached;

			public class OnPositionReachedArgs : EventArgs {
				public UnitMovement UnitMovement;
			}
			
			public MoveToPositionState (UnitMovement outer, Vector3 position, float finishDistance) : base(outer) {
				_position = position;
				_sqrFinishDistance = finishDistance * finishDistance;
			}
			public override void UpdateMovement () {
				if ((Outer.transform.position - _position).sqrMagnitude < _sqrFinishDistance) {
					OnPositionReached?.Invoke(this, new OnPositionReachedArgs(){UnitMovement = Outer});
					Outer.SetState(new IdleState(Outer));
				}
			}
			public override void OnEnterState () {
				Outer._ai.destination = _position;
				Outer._ai.SearchPath();
			}
		}
		public class MoveTowardsTargetableState : AUnitMovementState {
			private Targetable _targetable;
			private float _sqrChaseRange;
			public MoveTowardsTargetableState (UnitMovement outer, Targetable targetable, float chaseRange) : base(outer) {
				_targetable = targetable;
				_sqrChaseRange = chaseRange * chaseRange;
			}
			public override void UpdateMovement () {
				if ((Outer.transform.position - _targetable.transform.position).sqrMagnitude > _sqrChaseRange) {
					Outer._ai.destination = _targetable.transform.position;
				} else {
					Outer._ai.destination = Outer.transform.position;
				}
			}
			public override void OnEnterState () {
				// TODO Subscribe to OnUnitDeath
				Outer._ai.destination = _targetable.transform.position;
				Outer._ai.SearchPath();
			}
		}
	}
}
