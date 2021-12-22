using System;
using System.Collections.Generic;
using Math;
using UnityEngine;
namespace Map {
	public class ForestSection : MonoBehaviour {

		private readonly Polygon polygon = new Polygon();
		public bool IsClosed => polygon.IsClosed;
		public IEnumerable<Vector3> Points {
			get {
				List<Vector3> points = new List<Vector3>();
				foreach (Vector2 vertex in polygon.Vertices) {
					points.Add(LocalVec2ToWorldVec3(vertex));
				}
				return points;
			}
		}

		public event Action OnPolygonChanged;

		public bool AddPoint (Vector3 point) {
			bool result = polygon.AddPoint(WorldVec3ToLocalVec2(point));
			if (result) {
				OnPolygonChanged?.Invoke();
			}
			return result;
		}

		public bool ValidateAddPoint (Vector3 point) {
			return polygon.ValidateNewPoint(WorldVec3ToLocalVec2(point));
		}

		public bool Close () {
			bool result = polygon.ClosePolygon();
			if (result) {
				OnPolygonChanged?.Invoke();
			}
			return result;
		}

		public bool PointInsideSection (Vector3 point) {
			return polygon.PointInPolygon(WorldVec3ToLocalVec2(point));
		}

		private Vector2 WorldVec3ToLocalVec2 (Vector3 point) {
			Vector3 position = transform.position;
			return new Vector2(point.x - position.x, point.z - position.z);
		}

		private Vector3 LocalVec2ToWorldVec3 (Vector2 point) {
			Vector3 position = transform.position;
			return new Vector3(point.x + position.x, position.y, point.y + position.z);
		}

		private void OnDrawGizmos () {
			Gizmos.color = Color.black;
			foreach (Vector2 polygonVertex in polygon.Vertices) {
				Gizmos.DrawSphere(LocalVec2ToWorldVec3(polygonVertex), 0.2f);
			}
			foreach ((Vector2 p1, Vector2 p2) in polygon.Lines) {
				Gizmos.DrawLine(LocalVec2ToWorldVec3(p1), LocalVec2ToWorldVec3(p2));
			}
		}
	}
}
