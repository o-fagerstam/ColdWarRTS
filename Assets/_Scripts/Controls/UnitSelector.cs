using System;
using System.Collections.Generic;
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

		private AUnitSelectorState _state;
		private readonly HashSet<Unit> _selectedUnits = new HashSet<Unit>();
		public IEnumerable<Unit> SelectedUnits => _selectedUnits;

		private void OnEnable () {
			SetState(new BaseSelectorState(this));
		}

		private void SetState (AUnitSelectorState newState) {
			_state = newState;
		}

		public void UpdateSelection () {
			_state.UpdateSelection();	
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

		private abstract class AUnitSelectorState {
			protected readonly UnitSelector Outer;
			protected AUnitSelectorState (UnitSelector outer) {
				Outer = outer;
			}
			public abstract void UpdateSelection ();
		}
		
		private class BaseSelectorState : AUnitSelectorState {
			private Vector2 _dragStartPosition;

			public BaseSelectorState (UnitSelector outer) : base(outer) {}
			
			public override void UpdateSelection () {
				if (Mouse.current.leftButton.wasPressedThisFrame) {
					_dragStartPosition = Mouse.current.position.ReadValue();
				} else if (Mouse.current.leftButton.wasReleasedThisFrame) {
					if (!Keyboard.current.shiftKey.isPressed) {
						Outer.ClearSelection();
					}

					Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

					if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMasks.unit)) {return;}
					if (!hit.collider.TryGetComponentInParent(out Unit unit)) {return;}
					if (!unit.hasAuthority) {return;}

					if (Keyboard.current.shiftKey.isPressed) {
						Outer.SelectDeselect(unit);
					} else {
						Outer.Select(unit);
					}

				} else if (Mouse.current.leftButton.isPressed ) {
					if ((_dragStartPosition - Mouse.current.position.ReadValue()).sqrMagnitude > SELECTION_BOX_MIN_SIZE*SELECTION_BOX_MIN_SIZE) {
						Outer.SetState(new DragBoxSelectorState(Outer, _dragStartPosition));
					}
				}
			}
		}

		private class DragBoxSelectorState : AUnitSelectorState {
			private Vector2 _dragStartPosition;
			public DragBoxSelectorState (UnitSelector outer, Vector2 dragStartPosition) : base(outer) {
				_dragStartPosition = dragStartPosition;
			}
			public override void UpdateSelection () {
				RectTransform unitSelectionBox = Outer.unitSelectionBox;
				unitSelectionBox.gameObject.SetActive(true);

				if (Mouse.current.leftButton.isPressed) {
					Vector2 mousePosition = Mouse.current.position.ReadValue();

					float dragHeight = mousePosition.x - _dragStartPosition.x;
					float dragWidth = mousePosition.y - _dragStartPosition.y;

					unitSelectionBox.sizeDelta = new Vector2(Mathf.Abs(dragHeight), Mathf.Abs(dragWidth));
					unitSelectionBox.anchoredPosition = _dragStartPosition + new Vector2(dragHeight/2f, dragWidth/2f);
				} else {
					if (!Keyboard.current.shiftKey.isPressed) {
						Outer.ClearSelection();
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
							Outer.Select(unit);
						}
					}
					
					unitSelectionBox.gameObject.SetActive(false);
					Outer.SetState(new BaseSelectorState(Outer));
				}
			}
		}
	}
}
