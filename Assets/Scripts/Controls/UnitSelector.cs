using System;
using System.Collections.Generic;
using Constants;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
namespace Controls {
	public class UnitSelector : MonoBehaviour {

		private AUnitSelectorState _state;
		private readonly HashSet<Unit> _selectedUnits = new HashSet<Unit>();
		public IEnumerable<Unit> SelectedUnits => _selectedUnits;

		private void OnEnable () {
			SetState(new BaseSelectorState(this));
		}

		private void SetState (AUnitSelectorState newState) {
			_state = newState;
		}

		public void UpdateSelection () {
			_state.UpdateSelection();	
		}

		private void ClearSelection () {
			foreach (Unit selectedUnit in _selectedUnits) {
				selectedUnit.Deselect();
			}
			_selectedUnits.Clear();
		}

		private void Select (Unit unit) {
			unit.Select();
			_selectedUnits.Add(unit);
		}

		private void Deselect (Unit unit) {
			unit.Deselect();
			_selectedUnits.Remove(unit);
		}

		private abstract class AUnitSelectorState {
			protected readonly UnitSelector Outer;
			protected AUnitSelectorState (UnitSelector outer) {
				this.Outer = outer;
			}
			public abstract void UpdateSelection ();
		}
		
		private class BaseSelectorState : AUnitSelectorState {

			public BaseSelectorState (UnitSelector outer) : base(outer) {}
			
			public override void UpdateSelection () {
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					// Enter drag box state
				} else if (Mouse.current.leftButton.wasReleasedThisFrame) {
					Outer.ClearSelection();

					Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

					if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMasks.unit)) {return;}
					if (!hit.collider.TryGetComponentInParent(out Unit unit)) {return;}
					if (!unit.hasAuthority) {return;}
					Outer.Select(unit);
				}
			}
		}
	}
}
