using System.Collections.Generic;
using Architecture.StateMachine;
using Constants;
using Mirror;
using Network;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
namespace Controls {
	public class UnitSelector : MonoBehaviour {

		private const float SELECTION_BOX_MIN_SIZE = 10f;

		[SerializeField] private RectTransform unitSelectionBox;

		private UnitSelectorStateMachine _stateMachine;
		private readonly HashSet<Unit> _selectedUnits = new HashSet<Unit>();
		public IEnumerable<Unit> SelectedUnits => _selectedUnits;

		private void Awake () {
			_stateMachine = new UnitSelectorStateMachine(this);
		}

		private void OnEnable () {
			_stateMachine.State = _stateMachine.CreateBaseSelectorState();
		}

		public void UpdateSelection () {
			_stateMachine.State.UpdateSelection();	
		}

		private void ClearSelection () {
			foreach (Unit selectedUnit in _selectedUnits) {
				selectedUnit.Deselect();
			}
			_selectedUnits.Clear();
		}

		private void Select (Unit unit) {
			unit.Select();
			_selectedUnits.Add(unit);
		}

		private void Deselect (Unit unit) {
			unit.Deselect();
			_selectedUnits.Remove(unit);
		}

		private void SelectDeselect (Unit unit) {
			if (_selectedUnits.Contains(unit)) {
				Deselect(unit);
			} else {
				Select(unit);
			}
		}

		private abstract class AUnitSelectorState : State<UnitSelector> {
			protected AUnitSelectorState (UnitSelector context) : base(context) {}
			public abstract void UpdateSelection ();
		}

		private class UnitSelectorStateMachine : StateMachine<UnitSelector, AUnitSelectorState> {

			public UnitSelectorStateMachine (UnitSelector context) : base(context) {}
			public BaseSelectorState CreateBaseSelectorState () => new BaseSelectorState(Context);
			public DragBoxSelectorState CreateDragBoxSelectorState (Vector2 dragBoxStartPosition) => new DragBoxSelectorState(Context, dragBoxStartPosition);
			
		}

		private class BaseSelectorState : AUnitSelectorState {
			private Vector2 _dragStartPosition;

			public BaseSelectorState (UnitSelector context) : base(context) {}
			
			public override void EnterState () {}
			public override void ExitState () {}
			
			public override void UpdateSelection () {
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					_dragStartPosition = Mouse.current.position.ReadValue();
				} else if (Mouse.current.leftButton.wasReleasedThisFrame) {
					if (!Keyboard.current.shiftKey.isPressed) {
						Context.ClearSelection();
					}

					Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

					if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMasks.unit)) {return;}
					if (!hit.collider.TryGetComponentInParent(out Unit unit)) {return;}
					if (!unit.hasAuthority) {return;}

					if (Keyboard.current.shiftKey.isPressed) {
						Context.SelectDeselect(unit);
					} else {
						Context.Select(unit);
					}

				} else if (Mouse.current.leftButton.isPressed ) {
					if ((_dragStartPosition - Mouse.current.position.ReadValue()).sqrMagnitude > SELECTION_BOX_MIN_SIZE*SELECTION_BOX_MIN_SIZE) {
						Context._stateMachine.State = Context._stateMachine.CreateDragBoxSelectorState(_dragStartPosition);
					}
				}
			}
		}

		private class DragBoxSelectorState : AUnitSelectorState {
			private readonly Vector2 _dragStartPosition;
			public DragBoxSelectorState (UnitSelector context, Vector2 dragStartPosition) : base(context) {
				_dragStartPosition = dragStartPosition;
			}
			
			public override void EnterState () {}
			public override void ExitState () {}
			
			public override void UpdateSelection () {
				RectTransform unitSelectionBox = Context.unitSelectionBox;
				unitSelectionBox.gameObject.SetActive(true);

				if (Mouse.current.leftButton.isPressed) {
					Vector2 mousePosition = Mouse.current.position.ReadValue();

					float dragHeight = mousePosition.x - _dragStartPosition.x;
					float dragWidth = mousePosition.y - _dragStartPosition.y;

					unitSelectionBox.sizeDelta = new Vector2(Mathf.Abs(dragHeight), Mathf.Abs(dragWidth));
					unitSelectionBox.anchoredPosition = _dragStartPosition + new Vector2(dragHeight/2f, dragWidth/2f);
				} else {
					if (!Keyboard.current.shiftKey.isPressed) {
						Context.ClearSelection();
					}

					Vector2 min = unitSelectionBox.anchoredPosition - (unitSelectionBox.sizeDelta/2f);
					Vector2 max = unitSelectionBox.anchoredPosition + (unitSelectionBox.sizeDelta/2f);

					Camera mainCamera = Camera.main;
					foreach (Unit unit in NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>().Units) {
						Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

						if (screenPosition.x > min.x && 
						    screenPosition.x < max.x && 
						    screenPosition.y > min.y && 
						    screenPosition.y < max.y) {
							Context.Select(unit);
						}
					}
					unitSelectionBox.gameObject.SetActive(false);
					Context._stateMachine.State = Context._stateMachine.CreateBaseSelectorState();
				}
			}
		}
	}
}
