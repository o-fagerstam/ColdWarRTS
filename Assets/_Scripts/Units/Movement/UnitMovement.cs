using System;
using Architecture.StateMachine;
using Mirror;
using Pathfinding;
using Sirenix.OdinInspector;
using Units.Targeting;
using UnityEngine;
namespace Units.Movement {
	public class UnitMovement : NetworkBehaviour {
		[SerializeField, Required, ChildGameObjectsOnly]
		private Unit unit;
		
		private Seeker _seeker;
		private IAstarAI _ai;
		private UnitMovementStateMachine _stateMachine;

		public event EventHandler OnStateFinished;

		private void Awake () {
			_seeker = GetComponent<Seeker>();
			_ai = GetComponent<IAstarAI>();
			_stateMachine = new UnitMovementStateMachine(this);
			_stateMachine.State = _stateMachine.CreateIdleState();
		}
		private void Update () {
			if (isServer) {
				_stateMachine.State.UpdateMovement();
			}
		}

		public void MoveToPosition (Vector3 position) {
			_stateMachine.State = _stateMachine.CreateMoveToPositionState(position, 0.2f);
		}

		public void MoveTowardsTargetable (Targetable targetable) {
			_stateMachine.State = _stateMachine.CreateMoveTowardsTargetableState(targetable, Mathf.Max(unit.Weapon.Range - 0.1f, 0.1f));
		}

		public void StopMoving () {
			_stateMachine.State = _stateMachine.CreateIdleState();
		}

		private void InvokeOnStateFinished () {
			OnStateFinished?.Invoke(this, EventArgs.Empty);
		}

		private abstract class AUnitMovementState : State<UnitMovement> {
			protected AUnitMovementState (UnitMovement context) : base(context) {}

			public abstract void UpdateMovement ();
		}

		private class UnitMovementStateMachine : StateMachine<UnitMovement, AUnitMovementState> {

			public UnitMovementStateMachine (UnitMovement context) : base(context) {}

			public IdleState CreateIdleState () => new IdleState(Context);
			public MoveToPositionState CreateMoveToPositionState (Vector3 position, float finishDistance) => new MoveToPositionState(Context, position, finishDistance);
			public MoveTowardsTargetableState CreateMoveTowardsTargetableState (Targetable targetable, float chaseRange) => new MoveTowardsTargetableState(Context, targetable, chaseRange);
		}

		private class IdleState : AUnitMovementState {
			public IdleState(UnitMovement other) : base(other) {}
			public override void UpdateMovement () {}
			public override void EnterState () {
				Context._ai.destination = Context.transform.position;
				Context._ai.SearchPath();
			}
			public override void ExitState () {}
		}

		private class MoveToPositionState : AUnitMovementState {
			private readonly Vector3 _position;
			private readonly float _sqrFinishDistance;

			public MoveToPositionState (UnitMovement context, Vector3 position, float finishDistance) : base(context) {
				_position = position;
				_sqrFinishDistance = finishDistance * finishDistance;
			}
			public override void UpdateMovement () {
				if ((Context.transform.position - _position).sqrMagnitude < _sqrFinishDistance) {
					Context.InvokeOnStateFinished();
				}
			}
			public override void EnterState () {
				Context._ai.destination = _position;
				Context._ai.SearchPath();
			}
			public override void ExitState () {}
		}
		private class MoveTowardsTargetableState : AUnitMovementState {
			private readonly Targetable _targetable;
			private readonly float _sqrChaseRange;
			public MoveTowardsTargetableState (UnitMovement context, Targetable targetable, float chaseRange) : base(context) {
				_targetable = targetable;
				_sqrChaseRange = chaseRange * chaseRange;
			}
			public override void UpdateMovement () {
				if ((Context.transform.position - _targetable.transform.position).sqrMagnitude > _sqrChaseRange) {
					Context._ai.destination = _targetable.transform.position;
				} else {
					Context._ai.destination = Context.transform.position;
				}
			}
			public override void EnterState () {
				Context._ai.destination = _targetable.transform.position;
				Context._ai.SearchPath();
			}
			public override void ExitState () {}
		}
	}
}
