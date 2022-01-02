using Constants;
using Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class EditElevationTool : AMapEditorTool {
		private const string TOOLTIP_BASE = "Left click: Raise, Right click: Lower, Scroll: Change brush size, Space: Exit";
		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
		}
		public override void UpdateKeyboard () {}
		public override void UpdateMouse (Ray mouseRay) {
			if (!Mouse.current.leftButton.isPressed && !Mouse.current.rightButton.isPressed) {return;}
			if (EventSystem.current.IsPointerOverGameObject()) { return; }
			if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.ground)) { return; }

			if (Mouse.current.leftButton.wasPressedThisFrame) {
				OnLeftClickGround(hit);
			}
			if (Mouse.current.rightButton.wasPressedThisFrame) {
				OnRightClickGround(hit);
			}

		}

		public void OnLeftClickGround (RaycastHit hit) {
			EditMapElevation(hit, 2f);
		}

		public void OnRightClickGround (RaycastHit hit) {
			EditMapElevation(hit, -2f);
		}

		public void EditMapElevation (RaycastHit hit, float magnitude) {
			MapChunk chunk = hit.transform.GetComponent<MapChunk>();
			GameMap map = chunk.Map;
			map.EditElevation(hit.point, 5f, magnitude);
		}
	}
}
