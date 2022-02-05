using System;
using System.Collections.Generic;
using Constants;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
namespace Controls {
	public class UnitSelector : MonoBehaviour {

		private AUnitSelectorState state;
		private readonly HashSet<Unit> selectedUnits = new HashSet<Unit>();
		public IEnumerable<Unit> SelectedUnits => selectedUnits;

		private void OnEnable () {
			SetState(new BaseSelectorState(this));
		}

		private void SetState (AUnitSelectorState newState) {
			state = newState;
		}

		public void UpdateSelection () {
			state.UpdateSelection();	
		}

		private void ClearSelection () {
			foreach (Unit selectedUnit in selectedUnits) {
				selectedUnit.Deselect();
			}
			selectedUnits.Clear();
		}

		private void Select (Unit unit) {
			unit.Select();
			selectedUnits.Add(unit);
		}

		private void Deselect (Unit unit) {
			unit.Deselect();
			selectedUnits.Remove(unit);
		}

		private abstract class AUnitSelectorState {
			protected readonly UnitSelector outer;
			protected AUnitSelectorState (UnitSelector outer) {
				this.outer = outer;
			}
			public abstract void UpdateSelection ();
		}
		
		private class BaseSelectorState : AUnitSelectorState {

			public BaseSelectorState (UnitSelector outer) : base(outer) {}
			
			public override void UpdateSelection () {
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					// Enter drag box state
				} else if (Mouse.current.leftButton.wasReleasedThisFrame) {
					outer.ClearSelection();

					Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

					if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMasks.unit)) {return;}
					if (!hit.collider.TryGetComponentInParent(out Unit unit)) {return;}
					if (!unit.hasAuthority) {return;}
					outer.Select(unit);
				}
			}
		}
	}
}
