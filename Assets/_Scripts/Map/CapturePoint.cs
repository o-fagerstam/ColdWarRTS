using System;
using System.Collections.Generic;
using System.Linq;
using Controls;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	public class CapturePoint : AStaticMapElement, IGroundDragMovable {
		[SerializeField, AssetsOnly, Required] private CapturePointRuntimeSet capturePointRuntimeSet;
		private GroundDraggable _anchor;
		private void OnEnable () {
			_anchor = GetComponentInChildren<GroundDraggable>();
			_anchor.OnPositionChanged += HandleAnchorPositionChanged;
			capturePointRuntimeSet.Add(this);
		}
		
		private void OnDisable () {
			_anchor.OnPositionChanged -= HandleAnchorPositionChanged;
			capturePointRuntimeSet.Remove(this);
		}
		private void HandleAnchorPositionChanged (GroundDraggable anchor, Vector3 newPosition) {
			transform.position = newPosition;
			anchor.transform.position = newPosition;
		}

		public void EnableHandle () {
			_anchor.gameObject.SetActive(true);
		}
		public void DisableHandle () {
			_anchor.gameObject.SetActive(false);
		}
		public float Radius { get; private set; } = 3f;
		protected override void UpdateElementVisuals () {
			if (!RaycastUtil.NonRoadSurfaceElevationRaycast(transform.position, out RaycastHit hit)) {
				throw new GroundRaycastException($"Failed to find land over point {transform.position}.");
			}
			transform.position = hit.point;
		}
		public override bool Overlaps (Rectangle worldRectangle) {
			Rectangle thisWorldRectangle = new Rectangle(transform.position.Flatten(), Vector2.one*Radius *2);
			return thisWorldRectangle.Overlaps(worldRectangle);
		}

		public void RestoreFromSaveData (CapturePointData data) {
			transform.position = data.position;
		}

		public CapturePointData CreateSaveData () {
			return new CapturePointData(transform.position);
		}

		[Serializable]
		public class CapturePointData {
			public Vector3 position;

			public CapturePointData (Vector3 position) {
				this.position = position;
			}
		}
		public IEnumerable<GroundDraggable> GroundDraggables => Enumerable.Repeat(_anchor, 1);
		public void EnableHandles () {
			_anchor.gameObject.SetActive(true);
		}
		public void DisableHandles () {
			if (_anchor != null) {
				_anchor.gameObject.SetActive(false);
			}
		}
	}
}
