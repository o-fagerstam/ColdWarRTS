using Constants;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class PlaceRoadTool : AMapEditorTool {
		private const string TOOLTIP_BASE = "Not implemented";
		[SerializeField][AssetsOnly] RoadSegment roadPrefab;
		[ShowInInspector] [ReadOnly] private bool hasStartPoint;
		[ShowInInspector] [ReadOnly] private Vector3 startPoint;
		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
			SingletonManager.Retrieve<GroundDragManager>().CancelDragging();
			hasStartPoint = false;
		}
		
		public override void UpdateKeyboard () {}
		public override void UpdateMouse (Ray mouseRay) {
			GroundDragManager groundDragManager = SingletonManager.Retrieve<GroundDragManager>();
			if (Mouse.current.leftButton.wasReleasedThisFrame && groundDragManager.IsDragging) {
				groundDragManager.FinishDragging();
				return;
			}
			if (Mouse.current.rightButton.wasPressedThisFrame && groundDragManager.IsDragging) {
				groundDragManager.CancelDragging();
				return;
			}

			if (Mouse.current.leftButton.isPressed && groundDragManager.IsDragging) {
				groundDragManager.HandleUpdate(mouseRay);
				return;
			}
			
			if (EventSystem.current.IsPointerOverGameObject()) { return; }
			if (!Physics.Raycast(mouseRay, out RaycastHit clickHit, Mathf.Infinity, LayerMasks.anythingClickable)) { return; }
			int hitLayer = 1 << clickHit.transform.gameObject.layer;
			if (Mouse.current.leftButton.wasPressedThisFrame) {
				if (hitLayer == LayerMasks.bareGround) {
					OnLeftClickGround(clickHit);
				} else if (hitLayer == LayerMasks.mouseDraggable) {
					GroundDraggable draggedObject = clickHit.transform.GetComponent<GroundDraggable>();
					groundDragManager.SelectForDragging(draggedObject);
				}
			}
		}

		private void OnLeftClickGround (RaycastHit hit) {
			if (!hasStartPoint) {
				hasStartPoint = true;
				startPoint = hit.point;
			} else {
				hasStartPoint = false;
				GameMap map = SingletonManager.Retrieve<GameMap>();
				RoadSegment newRoadSegment = Instantiate(roadPrefab, startPoint, Quaternion.identity, map.transform);
				newRoadSegment.Initialize(startPoint, hit.point);
			}
		}

	}
}
