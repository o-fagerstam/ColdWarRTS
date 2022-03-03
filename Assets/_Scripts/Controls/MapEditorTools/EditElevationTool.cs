using Constants;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class EditElevationTool : AMapEditorTool {
		private const string TOOLTIP_BASE = "Left click: Raise, Right click: Lower, Scroll: Change brush size, Space: Exit";

		[SerializeField, Required, AssetsOnly] private GameMapScriptableValue gameMap;
		
		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
		}
		public override void UpdateKeyboard () {}
		public override void UpdateMouse (Ray mouseRay) {
			if (!Mouse.current.leftButton.isPressed && !Mouse.current.rightButton.isPressed) {return;}
			if (EventSystem.current.IsPointerOverGameObject()) { return; }
			if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.anySurface)) { return; }

			if (Mouse.current.leftButton.wasPressedThisFrame) {
				OnLeftClickGround(hit);
			}
			if (Mouse.current.rightButton.wasPressedThisFrame) {
				OnRightClickGround(hit);
			}

		}

		private void OnLeftClickGround (RaycastHit hit) {
			EditMapElevation(hit, 2f);
		}

		private void OnRightClickGround (RaycastHit hit) {
			EditMapElevation(hit, -2f);
		}

		public void EditMapElevation (RaycastHit hit, float magnitude) {
			gameMap.value.EditElevation(hit.point, 5f, magnitude);
		}
	}
}
