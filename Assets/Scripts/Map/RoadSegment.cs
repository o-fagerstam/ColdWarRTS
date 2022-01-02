using System;
using System.Collections.Generic;
using Controls;
using Controls.MapEditorTools;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class RoadSegment : MonoBehaviour {
		private BezierCurve curve;
		private Dictionary<GroundDraggable, BezierPoint> handles; // Order: Anchor1, Anchor2, Control1, Control2;
		[SerializeField][AssetsOnly][Required] private GroundDraggable handlePrefab;

		public void Initialize (Vector3 worldAnchor1, Vector3 worldAnchor2) {
			Vector3 position = transform.position;
			Vector3 localAnchor1 = worldAnchor1 - position;
			Vector3 localAnchor2 = worldAnchor2 - position;
			Vector3 dir = localAnchor2 - localAnchor1;
			Vector3 localControl1 = localAnchor1 + dir*0.33f;
			Vector3 localControl2 = localAnchor2 - dir*0.33f;
			curve = new BezierCurve(
				VectorUtil.Flatten(localAnchor1),
				VectorUtil.Flatten(localAnchor2),
				VectorUtil.Flatten(localControl1),
				VectorUtil.Flatten(localControl2));

			Vector3 worldControl1 = localControl1 + position;
			Vector3 worldControl2 = localControl2 + position;
			GroundDraggable anchor1Handle = Instantiate(handlePrefab, worldAnchor1, Quaternion.identity, transform);
			GroundDraggable anchor2Handle = Instantiate(handlePrefab, worldAnchor2, Quaternion.identity, transform);
			GroundDraggable control1Handle = Instantiate(handlePrefab, worldControl1, Quaternion.identity, transform);
			GroundDraggable control2Handle = Instantiate(handlePrefab, worldControl2, Quaternion.identity, transform);
			handles = new Dictionary<GroundDraggable, BezierPoint> {
				[anchor1Handle] = BezierPoint.Anchor1,
				[anchor2Handle] = BezierPoint.Anchor2,
				[control1Handle] = BezierPoint.Control1,
				[control2Handle] = BezierPoint.Control2
			};
			foreach (GroundDraggable handle in handles.Keys) {
				handle.OnPositionChanged += HandleHandlePositionChanged;
			}
		}
		private void HandleHandlePositionChanged (GroundDraggable obj, Vector3 newWorldPos) {
			curve.MovePoint(handles[obj], VectorUtil.Flatten(newWorldPos - transform.position));
		}
		
		// TODO Draw functions
		private void OnDrawGizmos () {
			Gizmos.color = Color.black;
			Gizmos.matrix = transform.localToWorldMatrix;
			for (int i = 0; i <= 10; i++) {
				float t = (float)i/10;
				Vector3 point = VectorUtil.AddY(curve.GetPointOnCurve(t), 0f);
				Gizmos.DrawSphere(point, 0.1f);
			}
			Gizmos.color = Color.white;
			foreach (Vector2 point in curve.GetEquidistantPoints(20)) {
				Gizmos.DrawSphere(VectorUtil.AddY(point), 0.1f);
			}
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(VectorUtil.AddY(curve.anchor1), 0.3f);
			Gizmos.DrawSphere(VectorUtil.AddY(curve.anchor2), 0.3f);
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(VectorUtil.AddY(curve.control1), 0.3f);
			Gizmos.DrawSphere(VectorUtil.AddY(curve.control2), 0.3f);
		}
	}
}
