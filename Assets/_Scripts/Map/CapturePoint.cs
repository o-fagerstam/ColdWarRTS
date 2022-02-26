using System;
using Controls;
using Math;
using Singleton;
using UnityEngine;
using Utils;
namespace Map {
	public class CapturePoint : AStaticMapElement {
		private GroundDraggable _anchor;
		private void OnEnable () {
			SingletonManager.Retrieve<GameMap>().RegisterStaticMapElement(this);
			_anchor = GetComponentInChildren<GroundDraggable>();
			_anchor.OnPositionChanged += HandleAnchorPositionChanged;
			DisableHandle();
		}
		
		private void OnDisable () {
			_anchor.OnPositionChanged -= HandleAnchorPositionChanged;
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
	}
}
