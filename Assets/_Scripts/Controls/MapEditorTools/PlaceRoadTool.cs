using Constants;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class PlaceRoadTool : AMapEditorTool {
		[SerializeField][AssetsOnly] RoadSegment roadPrefab;
		[ShowInInspector][ReadOnly] private Vector3 _roadStartPoint;
		private IRoadToolState _state;
		private GroundDragHandler _groundDragHandler;
		public override void Activate () {
			_groundDragHandler = new GroundDragHandler(SingletonManager.Retrieve<GameMap>().AllRoadSegments);
			SwitchToState(new BaseState(this));
		}

		public override void Deactivate () {
			base.Deactivate();
			SwitchToState(null);
		}

		private void SwitchToState (IRoadToolState newState) {
			_state?.OnExitState();
			_state = newState;
			_state?.OnEnterState();
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
				_tool = tool;
			}
			public void OnEnterState () {
				_tool.UpdateTooltip(TOOLTIP_BASE);
				_tool._groundDragHandler.CancelDragging();
				_tool._groundDragHandler.EnableDragging();
			}
			public void UpdateMouse (Ray mouseRay) {
				if (_tool._groundDragHandler.IsDragging) {
					if (Mouse.current.leftButton.wasReleasedThisFrame) {
						_tool._groundDragHandler.FinishDragging();
						return;
					}
					if (Mouse.current.rightButton.wasPressedThisFrame) {
						_tool._groundDragHandler.CancelDragging();
						return;
					}
					if (Mouse.current.leftButton.isPressed) {
						_tool._groundDragHandler.HandleUpdate(mouseRay);
						return;
					}
				}

				if (EventSystem.current.IsPointerOverGameObject()) { return; }
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					if (Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.anyLand)) {
						_tool._roadStartPoint = hit.point;
						_tool.SwitchToState(new PlacingRoadState(_tool));
					} else if (!Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out hit, Mathf.Infinity, LayerMasks.mouseDraggable)) {
						GroundDraggable draggedObject = hit.transform.GetComponent<GroundDraggable>();
						_tool._groundDragHandler.SelectForDragging(draggedObject);
					}
				}
			}
			public void OnExitState () {
				_tool._groundDragHandler.DisableDragging();
			}
		}

		private class PlacingRoadState : IRoadToolState {
			private const string TOOLTIP_PLACING_ROAD = "Click to place road";
			private PlaceRoadTool _tool;
			public PlacingRoadState (PlaceRoadTool tool) {
				_tool = tool;
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
					_tool._groundDragHandler.Register(newRoadSegment);
					_tool.SwitchToState(new BaseState(_tool));
				}
			}
			public void OnExitState () {}
		}
	}
}
