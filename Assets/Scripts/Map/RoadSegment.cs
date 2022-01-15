using System;
using System.Collections.Generic;
using System.Linq;
using Controls;
using Math;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class RoadSegment : AStaticMapElement {
		[SerializeField][AssetsOnly][Required] private GroundDraggable handlePrefab;
		private static readonly float RoadHeightOverGround = ScaleUtil.GameToUnity(.5f);
		private static readonly float RoadWidth = ScaleUtil.GameToUnity(8f);
		private static readonly float RoadSegmentLength = ScaleUtil.GameToUnity(2f);
		private BezierCurve curve;
		private Dictionary<GroundDraggable, BezierPoint> handles; // Order: Anchor1, Anchor2, Control1, Control2;
		[ShowInInspector] [ReadOnly] private List<(Vector3 left, Vector3 right)> meshPoints = new List<(Vector3 left, Vector3 right)>();
		private const string ANCHOR_TAG = "RoadAnchor";

		public void Initialize (Vector3 worldAnchor1, Vector3 worldAnchor2) {
			Vector3 position = transform.position;
			Vector3 localAnchor1 = worldAnchor1 - position;
			Vector3 localAnchor2 = worldAnchor2 - position;
			Vector3 dir = localAnchor2 - localAnchor1;
			Vector3 localControl1 = localAnchor1 + dir*0.33f;
			Vector3 localControl2 = localAnchor2 - dir*0.33f;
			GenerateCurve(localAnchor1, localAnchor2, localControl1, localControl2, position);
		}

		public void CreateFromSaveData (RoadSegmentSaveData data) {
			transform.position = data.position;
			GenerateCurve(data.anchor1, data.anchor2, data.control1, data.control2, data.position);
		}
		private void GenerateCurve (Vector3 localAnchor1, Vector3 localAnchor2, Vector3 localControl1, Vector3 localControl2, Vector3 position) {
			curve = new BezierCurve(
				VectorUtil.Flatten(localAnchor1),
				VectorUtil.Flatten(localAnchor2),
				VectorUtil.Flatten(localControl1),
				VectorUtil.Flatten(localControl2));

			Vector3 worldControl1 = localControl1 + position;
			Vector3 worldControl2 = localControl2 + position;
			Vector3 worldAnchor1 = localAnchor1 + position;
			Vector3 worldAnchor2 = localAnchor2 + position;
			GroundDraggable anchor1Handle = Instantiate(handlePrefab, worldAnchor1, Quaternion.identity, transform);
			anchor1Handle.snapTag = ANCHOR_TAG;
			anchor1Handle.snapFilterTag = gameObject.GetInstanceID().ToString();
			GroundDraggable anchor2Handle = Instantiate(handlePrefab, worldAnchor2, Quaternion.identity, transform);
			anchor2Handle.snapTag = ANCHOR_TAG;
			anchor2Handle.snapFilterTag = gameObject.GetInstanceID().ToString();
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
			SingletonManager.Retrieve<GameMap>().RegisterStaticMapElement(this);
			InvokeShapeChanged();
		}
		private void HandleHandlePositionChanged (GroundDraggable obj, Vector3 newWorldPos) {
			curve.MovePoint(handles[obj], (newWorldPos - transform.position).Flatten());
			RecalculateMesh();
			InvokeShapeChanged();
		}

		private void RecalculateMesh () {
			int segmentsPerUvLoop = Mathf.RoundToInt(RoadWidth/RoadSegmentLength);
			int numSegments = Mathf.CeilToInt(curve.ApproximateLength/RoadSegmentLength);
			if (numSegments % segmentsPerUvLoop != 0) {numSegments += segmentsPerUvLoop - numSegments%segmentsPerUvLoop;}

			Vector3[] vertices = new Vector3[2*(numSegments + 1)];
			int vertIndex = 0;
			meshPoints = CreateMeshPoints(numSegments);
			foreach ((Vector3 left, Vector3 right) in meshPoints) {
				vertices[vertIndex++] = left;
				vertices[vertIndex++] = right;
			}
			int[] triangles = new int[numSegments*6];
			Vector2[] uvs = new Vector2[vertices.Length];

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
			GetComponent<MeshCollider>().sharedMesh = mesh;
		}

		private List<(Vector3, Vector3)> CreateMeshPoints (int numSegments) {
			List<Vector2> twoDimPoints = curve.GetEquidistantPoints(numSegments + 1).ToList();
			List<(Vector3, Vector3)> meshPoints = new List<(Vector3, Vector3)>();
			
			Vector2 aCenter = twoDimPoints[0];
			for (int i = 0; i < numSegments; i++) {
				Vector2 bCenter = twoDimPoints[i + 1];
				Vector2 dir = (bCenter - aCenter).normalized;
				Vector2 aLeft = aCenter + dir.Rotate(-90f)*RoadWidth;
				Vector2 aRight = aCenter + dir.Rotate(90f)*RoadWidth;
				meshPoints.Add(GenerateMeshPointPair(aLeft, aRight));
				aCenter = bCenter;
				if (i == numSegments - 1) {
					Vector2 bLeft = bCenter + dir.Rotate(-90f)*RoadWidth;
					Vector2 bRight = bCenter + dir.Rotate(90f)*RoadWidth;
					meshPoints.Add(GenerateMeshPointPair(bLeft, bRight));
				}
			}
			return meshPoints;
		}
		private Vector3 Generate3DMeshPoint (Vector2 vec2) {
			Vector3 worldPoint = vec2.AddY(0f) + transform.position;
			if (!RaycastUtil.GroundLayerOnlyElevationRaycast(worldPoint, out RaycastHit hit)) {
				throw new Exception($"Failed to find land over point {worldPoint}.");
			}
			Vector3 localPoint = hit.point - transform.position + new Vector3(0f, RoadHeightOverGround, 0f);
			return localPoint;
		}

		private (Vector3 pointLeft, Vector3 pointRight) GenerateMeshPointPair (Vector2 flatPosLeft, Vector2 flatPosRight) {
			Vector3 pointLeft = Generate3DMeshPoint(flatPosLeft);
			Vector3 pointRight = Generate3DMeshPoint(flatPosRight);
			if (pointLeft.y > pointRight.y) {
				pointRight.y = pointLeft.y;
			} else {
				pointLeft.y = pointRight.y;
			}
			return (pointLeft, pointRight);
		}
		protected override void UpdateElementVisuals () {
			RecalculateMesh();
		}
		public override bool Overlaps (Rectangle worldRectangle) {
			Vector2 localOffset = transform.position.Flatten();
			Rectangle localRectangle = new Rectangle(worldRectangle.Center - localOffset, worldRectangle.Dimension);

			foreach ((Vector3 left, Vector3 right) in meshPoints) {
				if (localRectangle.ContainsPoint(left.Flatten()) || localRectangle.ContainsPoint(right.Flatten())) {
					return true;
				}
			}
			return false;
		}

		public RoadSegmentSaveData CreateSaveData () {
			return new RoadSegmentSaveData(
				transform.position,
				curve.anchor1.AddY(),
				curve.anchor2.AddY(),
				curve.control1.AddY(),
				curve.control2.AddY()
			);
		}

		[Serializable]
		public class RoadSegmentSaveData {
			public Vector3 position;
			public Vector3 anchor1;
			public Vector3 anchor2;
			public Vector3 control1;
			public Vector3 control2;
			
			public RoadSegmentSaveData (Vector3 position, Vector3 anchor1, Vector3 anchor2, Vector3 control1, Vector3 control2) {
				this.position = position;
				this.anchor1 = anchor1;
				this.anchor2 = anchor2;
				this.control1 = control1;
				this.control2 = control2;
			}
		}
	}
}
