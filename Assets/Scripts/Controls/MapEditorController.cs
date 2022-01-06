using Constants;
using Controls.MapEditorTools;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls {
	public class MapEditorController : ARtsController {
		[Title("Map Editor Controller")]
		[SceneObjectsOnly]
		[SerializeField] private GameMap map;
		[SerializeField] private float scrollSpeed = 5f;
		[Title("Debug")]
		[ReadOnly][ShowInInspector] private AMapEditorTool currentTool;
		[ReadOnly][ShowInInspector] private IMouseDraggable currentDraggedObject;

		public void SelectTool (AMapEditorTool tool) {
			currentTool = tool;
			tool.Activate();
		}

		protected virtual void Update () {
			UpdateKeyboardControl();
			UpdateMouseControl();
		}

		private void UpdateKeyboardControl () {
			UpdateKeyboardMovement();
			if (currentTool != null) {
				if (Keyboard.current.escapeKey.wasPressedThisFrame) {
					ClearTool();
				} else {
					currentTool.UpdateKeyboard();
				}
			}
		}

		private void UpdateKeyboardMovement () {
			if (Keyboard.current.wKey.isPressed) {
				transform.position += Vector3.forward*scrollSpeed*Time.deltaTime;
			}
			if (Keyboard.current.sKey.isPressed) {
				transform.position += Vector3.back*scrollSpeed*Time.deltaTime;
			}
			if (Keyboard.current.aKey.isPressed) {
				transform.position += Vector3.left*scrollSpeed*Time.deltaTime;
			}
			if (Keyboard.current.dKey.isPressed) {
				transform.position += Vector3.right*scrollSpeed*Time.deltaTime;
			}
		}

		private void UpdateMouseControl () {
			Camera mainCamera = Camera.main;
			Ray mouseRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (currentTool != null) {
				currentTool.UpdateMouse(mouseRay);
				return;
			}
			
			UpdateMouseRelease();
			UpdateMouseDrag(mouseRay);
			UpdateMouseDown(mouseRay);
		}
		private void UpdateMouseRelease () {
			if (!Mouse.current.leftButton.wasReleasedThisFrame) { return; }
			currentDraggedObject = null;
		}
		private void UpdateMouseDrag (Ray mouseRay) {
			if (!Mouse.current.leftButton.isPressed) { return; }
			if (currentDraggedObject == null) { return; }
			if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.bareGround)) { return; }
			currentDraggedObject.UpdatePosition(hit.point);
		}

		private void UpdateMouseDown (Ray mouseRay) {
			if (!Mouse.current.leftButton.wasPressedThisFrame &&
			    !Mouse.current.rightButton.wasPressedThisFrame) { return; }

			if (EventSystem.current.IsPointerOverGameObject()) { return; }

			if (!Physics.Raycast(mouseRay, out RaycastHit raycastHit, Mathf.Infinity, LayerMasks.anythingClickable)) { return; }

			if (1 << raycastHit.transform.gameObject.layer == LayerMasks.mouseDraggable) {
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					bool found = raycastHit.transform.TryGetComponent(out IMouseDraggable draggableComponent);
					if (!found) {
						throw new MissingComponentException(
							$"Object {raycastHit.transform.gameObject} is has no {nameof(IMouseDraggable)} component.");
					}
					currentDraggedObject = draggableComponent;
				}
			}
		}
		public void ClearTool () {
			currentTool.Deactivate();
			currentTool = null;
		}
	}
}
