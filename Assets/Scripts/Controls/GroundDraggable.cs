using System;
using UnityEngine;
using Utils;
namespace Controls {
	public class GroundDraggable : MonoBehaviour, IMouseDraggable {

		/// <summary>
		/// Args: This Object, new position
		/// </summary>
		public event Action<GroundDraggable, Vector3> OnPositionChanged;

		private void Update () {
			RecalculateHeight();
		}

		public void UpdatePosition (Vector3 newWorldPos) {
			transform.position = newWorldPos;
			OnPositionChanged?.Invoke(this, newWorldPos);
		}
		public void RecalculateHeight () {
			if (!RaycastUtil.GroundLayerOnlyElevationRaycast(transform.position, out RaycastHit hit)) {
				return;
			}
			transform.position = hit.point;
		}
	}
}
