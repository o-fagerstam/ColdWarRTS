using System;
using UnityEngine;
namespace Controls {
	public class GroundDraggable : MonoBehaviour, IMouseDraggable {

		/// <summary>
		/// Args: This Object, new position
		/// </summary>
		public event Action<GroundDraggable, Vector3> OnPositionChanged;

		public void UpdatePosition (Vector3 newWorldPos) {
			transform.position = newWorldPos;
			OnPositionChanged?.Invoke(this, newWorldPos);
		}
	}
}
