using Constants;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class CapturePointTool : AMapEditorTool {
		[SerializeField, Required, AssetsOnly] private CapturePointRuntimeSet capturePointRuntimeSet;
		[SerializeField, Required, AssetsOnly] private CapturePoint capturePointPrefab;
		private GroundDragHandler<CapturePoint> _groundDragHandler;

		private const string TOOLTIP_BASE = "Shift-click to place capture point";
		
		public override void Activate () {
			_groundDragHandler = new GroundDragHandler<CapturePoint>(capturePointRuntimeSet);
			_groundDragHandler.EnableDragging();
			UpdateTooltip(TOOLTIP_BASE);
		}

		public override void Deactivate () {
			base.Deactivate();
			_groundDragHandler.DisableDragging();
		}

		public override void UpdateKeyboard () {}
		public override void UpdateMouse (Ray mouseRay) {
			if (_groundDragHandler.IsDragging) {
				if (Mouse.current.leftButton.wasReleasedThisFrame) {
					_groundDragHandler.FinishDragging();
					return;
				}
				if (Mouse.current.rightButton.wasPressedThisFrame) {
					_groundDragHandler.CancelDragging();
					return;
				}
				if (Mouse.current.leftButton.isPressed) {
					_groundDragHandler.UpdateTick(mouseRay);
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
					_groundDragHandler.SelectForDragging(draggedObject);
				}
			}
		}
	}
}
