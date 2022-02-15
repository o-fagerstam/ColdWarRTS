using Constants;
using Units;
using Units.Targeting;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
namespace Controls {
	public class UnitCommandGiver : MonoBehaviour {
		private UnitSelector _unitSelector;

		private void Awake () {
			_unitSelector = GetComponent<UnitSelector>();
		}

		public void UpdateUnitCommands () {
			if (!Mouse.current.rightButton.wasPressedThisFrame) {
				return;
			}

			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMasks.anySurfaceOrUnit)) { return; }

			if (hit.collider.TryGetComponentInParent(out Targetable targetable) &&
			    !targetable.hasAuthority) { //TODO replace with team logic
				GiveTargetOrders(targetable);
			} else {
				GiveMoveOrders(hit.point);
			}


		}
		private void GiveMoveOrders (Vector3 point) {
			foreach (Unit unit in _unitSelector.SelectedUnits) {
				unit.UnitMovement.CmdMove(point);
			}
		}

		private void GiveTargetOrders (Targetable target) {
			foreach (Unit unit in _unitSelector.SelectedUnits) {
				unit.Targeter.CmdSetTarget(target.gameObject);
			}
		}
	}
}
