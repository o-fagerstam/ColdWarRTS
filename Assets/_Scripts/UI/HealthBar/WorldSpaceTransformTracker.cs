using System;
using Mirror;
using Unity.Collections;
using UnityEngine;
namespace UI.HealthBar {
	public class WorldSpaceTransformTracker : MonoBehaviour {
		[ShowInInspector, ReadOnly] private RectTransform _rectTransform;
		[ShowInInspector, ReadOnly] private Transform _trackedTransform;
		[SerializeField] private Vector3 worldOffset;
		[SerializeField] private Vector3 screenOffset;

		private void Awake () {
			_rectTransform = GetComponent<RectTransform>();
		}
		
		private void Update () {
			if (_trackedTransform == null) {return;}
			Vector3 screenPos = Camera.main.WorldToScreenPoint(_trackedTransform.position + worldOffset) + screenOffset;
			_rectTransform.anchoredPosition = screenPos;
		}

		public void SetTrackedTransform (Transform transform) {
			_trackedTransform = transform;
		}

		private void OnDrawGizmos () {
			Gizmos.DrawSphere(_trackedTransform.position + worldOffset, .3f);
		}
	}
}
