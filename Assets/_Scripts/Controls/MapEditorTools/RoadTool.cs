using Architecture.StateMachine;
using Constants;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public class RoadTool : AMapEditorTool {
		[SerializeField][AssetsOnly] RoadSegment roadPrefab;
		[ShowInInspector][ReadOnly] private Vector3 _roadStartPoint;
		private GroundDragHandler _groundDragHandler;
		private RoadToolStateMachine _stateMachine;

		private void Awake () {
			_stateMachine = new RoadToolStateMachine(this);
		}

		public override void Activate () {
			_groundDragHandler = new GroundDragHandler(SingletonManager.Retrieve<GameMap>().AllRoadSegments);
			_stateMachine.State = _stateMachine.CreatePlacingRoadState();
		}

		public override void Deactivate () {
			base.Deactivate();
			_stateMachine.State = null;
		}
		

		public override void UpdateKeyboard () {}
		public override void UpdateMouse (Ray mouseRay) {
			_stateMachine.State.UpdateMouse(mouseRay);
		}

		private abstract class RoadToolState : State<RoadTool> {
			protected RoadToolState (RoadTool context) : base(context) {}
			public abstract void UpdateMouse (Ray mouseRay);
		}
		
		private class RoadToolStateMachine : StateMachine<RoadTool, RoadToolState> {
			public RoadToolStateMachine (RoadTool context) : base(context) {}

			public RoadToolBaseState CreateBaseState () { return new RoadToolBaseState(Context); }
			public PlacingRoadState CreatePlacingRoadState () { return new PlacingRoadState(Context); }
		}

		private class RoadToolBaseState : RoadToolState {
			private const string TOOLTIP_BASE = "Shift-click to start placing road";
			public RoadToolBaseState (RoadTool context) : base(context) {
				Context = context;
			}
			public override void EnterState () {
				Context.UpdateTooltip(TOOLTIP_BASE);
				Context._groundDragHandler.CancelDragging();
				Context._groundDragHandler.EnableDragging();
			}
			public override void UpdateMouse (Ray mouseRay) {
				if (Context._groundDragHandler.IsDragging) {
					if (Mouse.current.leftButton.wasReleasedThisFrame) {
						Context._groundDragHandler.FinishDragging();
						return;
					}
					if (Mouse.current.rightButton.wasPressedThisFrame) {
						Context._groundDragHandler.CancelDragging();
						return;
					}
					if (Mouse.current.leftButton.isPressed) {
						Context._groundDragHandler.HandleUpdate(mouseRay);
						return;
					}
				}

				if (EventSystem.current.IsPointerOverGameObject()) { return; }
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					if (Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.anyLand)) {
						Context._roadStartPoint = hit.point;
						Context._stateMachine.State = new PlacingRoadState(Context);
					} else if (!Keyboard.current.leftShiftKey.isPressed && Physics.Raycast(mouseRay, out hit, Mathf.Infinity, LayerMasks.mouseDraggable)) {
						GroundDraggable draggedObject = hit.transform.GetComponent<GroundDraggable>();
						Context._groundDragHandler.SelectForDragging(draggedObject);
					}
				}
			}
			public override void ExitState () {
				Context._groundDragHandler.DisableDragging();
			}
		}

		private class PlacingRoadState : RoadToolState {
			private const string TOOLTIP_PLACING_ROAD = "Click to place road";
			public PlacingRoadState (RoadTool context) : base(context) {
				Context = context;
			}

			public override void EnterState () {
				Context.UpdateTooltip(TOOLTIP_PLACING_ROAD);
			}
			public override void UpdateMouse (Ray mouseRay) {
				if (EventSystem.current.IsPointerOverGameObject()) { return; }
				if (!Physics.Raycast(mouseRay, out RaycastHit clickHit, Mathf.Infinity, LayerMasks.anyLand)) { return; }

				if (Mouse.current.leftButton.wasPressedThisFrame) {
					GameMap map = SingletonManager.Retrieve<GameMap>();
					RoadSegment newRoadSegment = Instantiate(Context.roadPrefab, Context._roadStartPoint, Quaternion.identity, map.transform);
					newRoadSegment.Initialize(Context._roadStartPoint, clickHit.point);
					Context._groundDragHandler.Register(newRoadSegment);
					Context._stateMachine.State = Context._stateMachine.CreateBaseState();
				}
			}
			public override void ExitState () {}
		}


	}
}
