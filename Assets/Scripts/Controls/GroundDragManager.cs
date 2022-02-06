using System;
using System.Collections.Generic;
using Constants;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls {
	public class GroundDragManager : ASingletonMonoBehaviour<GroundDragManager> {
		[ReadOnly][ShowInInspector] private HashSet<GroundDraggable> _allGroundDraggables = new HashSet<GroundDraggable>();
		[ReadOnly][ShowInInspector] private GroundDraggable _currentDraggedObject;
		[ReadOnly][ShowInInspector] private Vector3 _startDragPosition;
		private const float DRAG_SNAP_DISTANCE = 0.3f;

		public bool IsDragging => _currentDraggedObject != null;

		public void HandleUpdate (Ray mouseRay) {
			if (_currentDraggedObject == null) {
				return;
			}
			if (!Mouse.current.leftButton.isPressed) {
				_currentDraggedObject = null;
				return;
			}
			if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.nonRoadSurface)) { return; }

			if (_currentDraggedObject.snapTag != null) {
				float closestSqrDst = float.MaxValue;
				GroundDraggable closest = null;
				foreach (GroundDraggable draggable in _allGroundDraggables) {
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

		public void Register (GroundDraggable draggable) {
			_allGroundDraggables.Add(draggable);
		}

		public void Deregister (GroundDraggable draggable) {
			_allGroundDraggables.Remove(draggable);
		}
	}
}
