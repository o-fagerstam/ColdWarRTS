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
			// TODO Move input handling into its own class on the controller
			if (!Mouse.current.rightButton.wasPressedThisFrame) {
				return;
			}

			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMasks.anySurfaceOrUnit)) { return; }

			if (hit.collider.TryGetComponentInParent(out Targetable targetable) &&
			    !targetable.hasAuthority) { //TODO replace with team logic
				GiveAttackOrders(targetable);
			} else {
				GiveMoveOrders(hit.point);
			}


		}
		private void GiveMoveOrders (Vector3 point) {
			foreach (Unit unit in _unitSelector.SelectedUnits) {
				unit.CmdGiveMoveCommand(point, false);
			}
		}

		private void GiveAttackOrders (Targetable target) {
			foreach (Unit unit in _unitSelector.SelectedUnits) {
				unit.CmdGiveAttackCommand(target.gameObject, false);
			}
		}
	}
}
