using Constants;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
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
			if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMasks.anySurface)) {return;}

			GiveMoveOrders(hit.point);
		}
		private void GiveMoveOrders (Vector3 point) {

			foreach (Unit unit in _unitSelector.SelectedUnits) {
				unit.unitMovement.CmdMove(point);
			}
			
		}
	}
}
