using Constants;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class CapturePointTool : AMapEditorTool {
		[SerializeField, Required, AssetsOnly] private CapturePoint capturePointPrefab;

		private const string TOOLTIP_BASE = "Shift-click to place capture point";
		
		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
			foreach (CapturePoint capturePoint in SingletonManager.Retrieve<GameMap>().AllCapturePoints) {
				capturePoint.EnableHandle();
			}
		}

		public override void UpdateKeyboard () {}
		public override void UpdateMouse (Ray mouseRay) {
			GroundDragManager groundDragManager = SingletonManager.Retrieve<GroundDragManager>();
			if (groundDragManager.IsDragging) {
				if (Mouse.current.leftButton.wasReleasedThisFrame) {
					groundDragManager.FinishDragging();
					return;
				}
				if (Mouse.current.rightButton.wasPressedThisFrame) {
					groundDragManager.CancelDragging();
					return;
				}
				if (Mouse.current.leftButton.isPressed) {
					groundDragManager.HandleUpdate(mouseRay);
					return;
				}
			}

			if (EventSystem.current.IsPointerOverGameObject()) { return; }
			if (Mouse.current.leftButton.wasPressedThisFrame) {
				if (Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.anyLand)) {
					GameMap map = SingletonManager.Retrieve<GameMap>();
					Instantiate(capturePointPrefab, hit.point, Quaternion.identity, map.transform);
				} else if (!Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out hit, Mathf.Infinity, LayerMasks.mouseDraggable)) {
					GroundDraggable draggedObject = hit.transform.GetComponent<GroundDraggable>();
					groundDragManager.SelectForDragging(draggedObject);
				}
			}
		}
		
	}
}
