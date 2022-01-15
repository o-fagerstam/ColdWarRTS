using System;
using System.Collections.Generic;
using Constants;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls {
	public class GroundDragManager : MonoBehaviour, ISingleton {
		[ReadOnly][ShowInInspector] private HashSet<GroundDraggable> allGroundDraggables = new HashSet<GroundDraggable>();
		[ReadOnly][ShowInInspector] private GroundDraggable currentDraggedObject;
		[ReadOnly][ShowInInspector] private Vector3 startDragPosition;
		private const float DRAG_SNAP_DISTANCE = 2f;

		public bool IsDragging => currentDraggedObject != null;

		private void OnEnable () {
			SingletonManager.Register(this);
		}

		private void OnDisable () {
			SingletonManager.Unregister(this);
		}

		public void HandleUpdate (Ray mouseRay) {
			if (currentDraggedObject == null) {
				return;
			}
			if (!Mouse.current.leftButton.isPressed) {
				currentDraggedObject = null;
				return;
			}
			if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.nonRoadSurface)) { return; }

			if (currentDraggedObject.snapTag != null) {
				float closestSqrDst = float.MaxValue;
				GroundDraggable closest = null;
				foreach (GroundDraggable draggable in allGroundDraggables) {
					if (draggable == currentDraggedObject) {
						continue;
					}
					if (currentDraggedObject.snapTag != null && currentDraggedObject.snapTag != draggable.snapTag) {
						continue;
					}
					if (currentDraggedObject.snapFilterTag != null && currentDraggedObject.snapFilterTag == draggable.snapFilterTag) {
						continue;
					}
					float sqrDst = (draggable.transform.position - hit.point).sqrMagnitude;
					if (sqrDst < closestSqrDst) {
						closest = draggable;
						closestSqrDst = sqrDst;
					}
				}
				if (closest != null && closestSqrDst <= DRAG_SNAP_DISTANCE*DRAG_SNAP_DISTANCE) {
					currentDraggedObject.UpdatePosition(closest.transform.position);
				} else {
					currentDraggedObject.UpdatePosition(hit.point);
				}
			} else {
				currentDraggedObject.UpdatePosition(hit.point);
			}
			
		}

		public void SelectForDragging (GroundDraggable draggable) {
			if (currentDraggedObject != null) {
				throw new ArgumentException("An object is already selected for dragging");
			}
			currentDraggedObject = draggable;
			startDragPosition = currentDraggedObject.transform.position;
		}

		public void FinishDragging () {
			currentDraggedObject = null;
		}

		public void CancelDragging () {
			if (currentDraggedObject == null) {return; }
			currentDraggedObject.transform.position = startDragPosition;
			currentDraggedObject = null;
		}

		public void Register (GroundDraggable draggable) {
			allGroundDraggables.Add(draggable);
		}

		public void Deregister (GroundDraggable draggable) {
			allGroundDraggables.Remove(draggable);
		}
	}
}
