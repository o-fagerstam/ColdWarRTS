using Constants;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class PlaceRoadTool : AMapEditorTool {
		[SerializeField][AssetsOnly] RoadSegment roadPrefab;
		[ShowInInspector] [ReadOnly] private Vector3 _roadStartPoint;
		private IRoadToolState _state;
		public override void Activate () {
			SwitchToState(new BaseState(this));
		}

		private void SwitchToState (IRoadToolState newState) {
			_state?.OnExitState();
			_state = newState;
			_state.OnEnterState();
		}
		
		public override void UpdateKeyboard () {}
		public override void UpdateMouse (Ray mouseRay) {
			_state.UpdateMouse(mouseRay);
		}

		private interface IRoadToolState {
			public void OnEnterState ();
			public void UpdateMouse (Ray mouseRay);

			public void OnExitState ();
		}

		private class BaseState : IRoadToolState {
			private const string TOOLTIP_BASE = "Shift-click to start placing road";
			private PlaceRoadTool _tool;
			public BaseState (PlaceRoadTool tool) {
				this._tool = tool;
			}
			public void OnEnterState () {
				_tool.UpdateTooltip(TOOLTIP_BASE);
				SingletonManager.Retrieve<GroundDragManager>().CancelDragging();
				foreach (RoadSegment roadSegment in SingletonManager.Retrieve<GameMap>().AllRoadSegments) {
					roadSegment.EnableAnchors();
				}
			}
			public void UpdateMouse (Ray mouseRay) {
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
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					if (Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.anyLand)) {
						_tool._roadStartPoint = hit.point;
						_tool.SwitchToState(new PlacingRoadState(_tool));
					} else if (!Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out hit, Mathf.Infinity, LayerMasks.mouseDraggable)) {
						GroundDraggable draggedObject = hit.transform.GetComponent<GroundDraggable>();
						groundDragManager.SelectForDragging(draggedObject);
					}
				}
			}
			public void OnExitState () {
				foreach (RoadSegment roadSegment in SingletonManager.Retrieve<GameMap>().AllRoadSegments) {
					roadSegment.DisableAnchors();
				}
			}
		}

		private class PlacingRoadState : IRoadToolState {
			private const string TOOLTIP_PLACING_ROAD = "Click to place road";
			private PlaceRoadTool _tool;
			public PlacingRoadState (PlaceRoadTool tool) {
				this._tool = tool;
			}

			public void OnEnterState () {
				_tool.UpdateTooltip(TOOLTIP_PLACING_ROAD);
			}
			public void UpdateMouse (Ray mouseRay) {
				if (EventSystem.current.IsPointerOverGameObject()) { return; }
				if (!Physics.Raycast(mouseRay, out RaycastHit clickHit, Mathf.Infinity, LayerMasks.anyLand)) { return; }

				if (Mouse.current.leftButton.wasPressedThisFrame) {
					GameMap map = SingletonManager.Retrieve<GameMap>();
					RoadSegment newRoadSegment = Instantiate(_tool.roadPrefab, _tool._roadStartPoint, Quaternion.identity, map.transform);
					newRoadSegment.Initialize(_tool._roadStartPoint, clickHit.point);
					newRoadSegment.EnableAnchors();
					_tool.SwitchToState(new BaseState(_tool));
				}
			}
			public void OnExitState () {}
		}
	}
}
