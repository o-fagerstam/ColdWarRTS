using System;
using System.Linq;
using Constants;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls {
	public class GroundDragHandler<TMovable> where TMovable : IGroundDragMovable  {
		private readonly RuntimeSet<TMovable> _movables;
		private GroundDraggable _currentDraggedObject;
		private Vector3 _startDragPosition;
		private bool _draggingEnabled;
		private const float DRAG_SNAP_DISTANCE = 0.3f;

		public GroundDragHandler (RuntimeSet<TMovable> movables) {
			_movables = movables;
			_movables.OnElementAdded += HandleMovableAdded;
		}

		public bool IsDragging => _currentDraggedObject != null;

		public void UpdateTick (Ray mouseRay) {
			if (!_draggingEnabled) {return; }
			if (_currentDraggedObject == null) { return; }
			if (!Mouse.current.leftButton.isPressed) {
				_currentDraggedObject = null;
				return;
			}
			if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.nonRoadSurface)) { return; }

			if (_currentDraggedObject.snapTag != null) {
				float closestSqrDst = float.MaxValue;
				GroundDraggable closest = null;
				foreach (GroundDraggable draggable in _movables.SelectMany(x=>x.GroundDraggables)) {
					if (draggable == _currentDraggedObject) {
						continue;
					}
					if (_currentDraggedObject.snapTag != null && _currentDraggedObject.snapTag != draggable.snapTag) {
						continue;
					}
					if (_currentDraggedObject.snapFilterTag != null && _currentDraggedObject.snapFilterTag == draggable.snapFilterTag) {
						continue;
					}
					float sqrDst = (draggable.transform.position - hit.point).sqrMagnitude;
					if (sqrDst < closestSqrDst) {
						closest = draggable;
						closestSqrDst = sqrDst;
					}
				}
				if (closest != null && closestSqrDst <= DRAG_SNAP_DISTANCE*DRAG_SNAP_DISTANCE) {
					_currentDraggedObject.UpdatePosition(closest.transform.position);
				} else {
					_currentDraggedObject.UpdatePosition(hit.point);
				}
			} else {
				_currentDraggedObject.UpdatePosition(hit.point);
			}
			
		}

		public void SelectForDragging (GroundDraggable draggable) {
			if (_currentDraggedObject != null) {
				throw new ArgumentException("An object is already selected for dragging");
			}
			_currentDraggedObject = draggable;
			_startDragPosition = _currentDraggedObject.transform.position;
		}

		public void FinishDragging () {
			_currentDraggedObject = null;
		}

		public void CancelDragging () {
			if (_currentDraggedObject == null) {return; }
			_currentDraggedObject.transform.position = _startDragPosition;
			_currentDraggedObject = null;
		}

		public void EnableDragging () {
			_draggingEnabled = true;
			foreach (TMovable movable in _movables) {
				movable.EnableHandles();
			}
		}

		public void DisableDragging () {
			_draggingEnabled = false;
			foreach (TMovable movable in _movables) {
				movable.DisableHandles();
			}
		}

		public void HandleMovableAdded (TMovable groundDragMovable) {
			if (_draggingEnabled) {
				groundDragMovable.EnableHandles();
			} else {
				groundDragMovable.DisableHandles();
			}
		}
	}
}
