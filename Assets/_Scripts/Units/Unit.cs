using System;
using System.Collections.Generic;
using GameLogic;
using Mirror;
using Network;
using Sirenix.OdinInspector;
using Units.Commands;
using Units.Movement;
using Units.Targeting;
using Units.Weapons;
using UnityEngine;
namespace Units {
	
	/// <summary>
	/// Handles the basic lifecycle and command structure of a unit.
	/// </summary>
	[RequireComponent(typeof(UnitMovement))]
	public class Unit : NetworkBehaviour {
		[SerializeField, AssetsOnly, Required] private UnitRuntimeSet allUnits;

		[Title("Components")]
		[SerializeField, Required] private GameObject selectionCircle;

		[SerializeField, Required, ChildGameObjectsOnly]
		private UnitMovement unitMovement;
		public UnitMovement UnitMovement => unitMovement;
		[SerializeField, Required, ChildGameObjectsOnly]
		private Targeter targeter;
		public Targeter Targeter => targeter;
		[SerializeField, Required, ChildGameObjectsOnly]
		private UnitHealth health;
		public UnitHealth Health => health;
		[SerializeField, Required, ChildGameObjectsOnly]
		private TankGun weapon;
		public TankGun Weapon => weapon;

		private Queue<AUnitCommand> _commandQueue = new Queue<AUnitCommand>();
		private AUnitCommand _currentCommand;

		[SyncVar]
		private GameObject _ownerGameObject;
		public RtsPlayer Owner => _ownerGameObject.GetComponent<RtsPlayer>();

		private bool _hasBeenInitialized;

		public void Initialize (RtsPlayer owner) {
			_ownerGameObject = owner.gameObject;
			allUnits.Add(this);
			_hasBeenInitialized = true;
		}

		private void OnEnable () {
			unitMovement = GetComponent<UnitMovement>();
			if (_hasBeenInitialized) {
				allUnits.Add(this);
			}
		}

		private void OnDisable () {
			allUnits.Remove(this);
		}

		[Command]
		public void CmdGiveMoveCommand (Vector3 point, bool enqueue) {
			GiveCommand(new MoveUnitCommand(this, point), enqueue);
		}
		[Command]
		public void CmdGiveAttackCommand (GameObject target, bool enqueue) {
			Targetable targetable = target.GetComponent<Targetable>();
			GiveCommand(new AttackCommand(this, targetable), enqueue);
		}

		[Server]
		private void GiveCommand (AUnitCommand command, bool enqueue) {
			if (enqueue && _currentCommand.GetType() != typeof(IdleUnitCommand)) {
				EnqueueCommand(command);
			} else {
				ImmediateCommand(command);
			}
		}

		[Server]
		private void ImmediateCommand (AUnitCommand command) {
			AbandonCurrentCommand();
			_commandQueue.Clear();
			_commandQueue.Enqueue(command);
			NextCommand();
		}
		[Server]
		private void EnqueueCommand (AUnitCommand command) {
			_commandQueue.Enqueue(command);
		}

		[Server]
		private void NextCommand () {
			if (_commandQueue.Count > 0) {
				_currentCommand = _commandQueue.Dequeue();
				_currentCommand.OnCommandFinished += HandleCommandFinished;
			} else {
				_currentCommand = new IdleUnitCommand(this);
			}
			_currentCommand.DoCommand();
		}

		[Server]
		private void AbandonCurrentCommand () {
			if (_currentCommand != null) {
				_currentCommand.OnCommandFinished -= HandleCommandFinished;
			}
		}
		[Server]
		private void HandleCommandFinished (object sender, AUnitCommand.OnCommandFinishedArgs e) {
			AbandonCurrentCommand();
			NextCommand();
		}
		
		[Server]
		public void ServerDie () {
			NetworkServer.Destroy(gameObject);
		}

		[Client]
		public void Select () {
			if (!hasAuthority) {return;}
			selectionCircle.SetActive(true);
		}

		[Client]
		public void Deselect () {
			if (!hasAuthority) {return;}
			selectionCircle.SetActive(false);
		}

	}
}
