﻿using System;
using System.Collections.Generic;
using Controls;
using Mirror;
using Sirenix.OdinInspector;
using Units.Movement;
using Units.Targeting;
using UnityEngine;
namespace Units {
	
	/// <summary>
	/// Handles the basic lifecycle and command structure of a unit.
	/// </summary>
	[RequireComponent(typeof(UnitMovement))]
	public class Unit : NetworkBehaviour {
		[SerializeField, Required] private GameObject selectionCircle;

		[SerializeField, Required, ChildGameObjectsOnly]
		private UnitMovement unitMovement;
		public UnitMovement UnitMovement => unitMovement;
		[SerializeField, Required, ChildGameObjectsOnly]
		private Targeter targeter;
		public Targeter Targeter => targeter;

		private Queue<AUnitCommand> _commandQueue = new Queue<AUnitCommand>();
		private AUnitCommand _currentCommand;
		private AUnitCommand currentCommand => _commandQueue.Count == 0 ? null : _commandQueue.Peek();

		public static event EventHandler<OnUnitSpawnedArgs> ServerOnUnitSpawned;
		public static event EventHandler<OnUnitDespawnedArgs> ServerOnUnitDespawned;		
		public static event EventHandler<OnUnitSpawnedArgs> AuthorityOnUnitSpawned;
		public static event EventHandler<OnUnitDespawnedArgs> AuthorityOnUnitDespawned;
		

		private void OnEnable () {
			unitMovement = GetComponent<UnitMovement>();
		}

		public override void OnStartServer () {
			base.OnStartServer();
			ServerOnUnitSpawned?.Invoke(this, new OnUnitSpawnedArgs{unit = this});
		}

		public override void OnStopServer () {
			base.OnStopServer();
			ServerOnUnitDespawned?.Invoke(this, new OnUnitDespawnedArgs{unit = this});
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
			if (enqueue && currentCommand == null) {
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
			AUnitCommand command = _commandQueue.Dequeue();
			command.OnCommandFinished += HandleCommandFinished;
			command.DoCommand();
		}

		[Server]
		private void AbandonCurrentCommand () {
			if (currentCommand != null) {
				currentCommand.OnCommandFinished -= HandleCommandFinished;
			}
		}
		[Server]
		private void HandleCommandFinished (object sender, AUnitCommand.OnCommandFinishedArgs e) {
			AbandonCurrentCommand();
			if (_commandQueue.Count > 0) {
				NextCommand();
			}
		}

		public override void OnStartAuthority () {
			base.OnStartAuthority();
			AuthorityOnUnitSpawned?.Invoke(this, new OnUnitSpawnedArgs(){unit =  this});
		}

		public override void OnStopAuthority () {
			base.OnStopAuthority();
			AuthorityOnUnitDespawned?.Invoke(this, new OnUnitDespawnedArgs(){unit = this});
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
	public class OnUnitSpawnedArgs : EventArgs {
		public Unit unit;
	}
	public class OnUnitDespawnedArgs : EventArgs {
		public Unit unit;
	}
}
