using System;
using System.Collections.Generic;
using System.Linq;
using Controls;
using Controls.MapEditorTools;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	public class RoadSegment : MonoBehaviour {
		[SerializeField][AssetsOnly][Required] private GroundDraggable handlePrefab;
		private static readonly float RoadWidth = ScaleUtil.GameToUnity(8f);
		private static readonly float RoadSegmentLength = ScaleUtil.GameToUnity(8f) / 4;
		private BezierCurve curve;
		private Dictionary<GroundDraggable, BezierPoint> handles; // Order: Anchor1, Anchor2, Control1, Control2;


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
				handle.transform.localScale = Vector3.one*0.2f;
				handle.OnPositionChanged += HandleHandlePositionChanged;
			}

			RecalculateMesh();
		}
		private void HandleHandlePositionChanged (GroundDraggable obj, Vector3 newWorldPos) {
			curve.MovePoint(handles[obj], VectorUtil.Flatten(newWorldPos - transform.position));
			RecalculateMesh();
		}

		private void RecalculateMesh () {
			int segmentsPerUvLoop = Mathf.RoundToInt(RoadWidth/RoadSegmentLength);
			int numSegments = Mathf.CeilToInt(curve.ApproximateLength/RoadSegmentLength);
			if (numSegments % segmentsPerUvLoop != 0) {numSegments += segmentsPerUvLoop - numSegments%segmentsPerUvLoop;}

			Vector3[] vertices = new Vector3[2*(numSegments + 1)];
			int[] triangles = new int[numSegments*6];
			Vector2[] uvs = new Vector2[vertices.Length];
			
			// vertices
			List<Vector2> twoDimPoints = curve.GetEquidistantPoints(numSegments + 1).ToList();
			Vector2 aCenter = twoDimPoints[0];
			for (int i = 0, vertIndex = 0; i < numSegments; i++) {
				Vector2 bCenter = twoDimPoints[i + 1];
				Vector2 dir = (bCenter - aCenter).normalized;
				Vector2 aLeft = aCenter + dir.Rotate(-90f)*RoadWidth;
				Vector2 aRight = aCenter + dir.Rotate(90f)*RoadWidth;
				vertices[vertIndex++] = Generate3DMeshPoint(aLeft);
				vertices[vertIndex++] = Generate3DMeshPoint(aRight);
				aCenter = bCenter;
				if (i == numSegments - 1) {
					Vector2 bLeft = bCenter + dir.Rotate(-90f)*RoadWidth;
					Vector2 bRight = bCenter + dir.Rotate(90f)*RoadWidth;
					vertices[vertIndex++] = Generate3DMeshPoint(bLeft);
					vertices[vertIndex++] = Generate3DMeshPoint(bRight);
				}
			}
			
			// triangles
			for (int i = 0, triangleIndex = 0; i < numSegments; i++) {
				int a = i*2;
				int b = i*2 + 1;
				int c = (i + 1)*2;
				int d = (i + 1)*2 + 1;

				triangles[triangleIndex++] = b;
				triangles[triangleIndex++] = c;
				triangles[triangleIndex++] = a;

				triangles[triangleIndex++] = b;
				triangles[triangleIndex++] = d;
				triangles[triangleIndex++] = c;
			}

			// uvs
			float uvStepLength = 1f/segmentsPerUvLoop;
			for (int i = 0, uvIndex = 0; i < numSegments; i++) {
				float uvFactor = (i%(segmentsPerUvLoop)) * uvStepLength;
				bool flipFactor = (i%(segmentsPerUvLoop*2)) >= segmentsPerUvLoop;
				uvFactor = flipFactor ? 1f - uvFactor : uvFactor;
				uvs[uvIndex++] = new Vector2(1f, uvFactor);
				uvs[uvIndex++] = new Vector2(0f, uvFactor);
			}
			uvs[uvs.Length - 2] = new Vector2(1f, 1f);
			uvs[uvs.Length - 1] = new Vector2(0f, 1f);
			
			// assignment
			Mesh mesh = new Mesh {
				name = "Road Mesh",
				vertices = vertices,
				triangles = triangles,
				uv = uvs
			};
			
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.RecalculateTangents();
			GetComponent<MeshFilter>().mesh = mesh;
		}
		private Vector3 Generate3DMeshPoint (Vector2 vec2) {
			Vector3 worldPoint = vec2.AddY(0f) + transform.position;
			if (!RaycastUtil.ElevationRaycast(worldPoint, out RaycastHit hit)) {
				throw new Exception($"Failed to find land over point {worldPoint}.");
			}
			Vector3 localPoint = hit.point - transform.position + new Vector3(0f, 0.001f, 0f);
			return localPoint;
		}
	}
}
