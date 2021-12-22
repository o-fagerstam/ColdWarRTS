using System;
using Math;
using UnityEngine;
namespace Map {
	public class ForestSection : MonoBehaviour {

		private readonly Polygon polygon = new Polygon();

		public bool AddPoint (Vector3 point) {
			return polygon.AddPoint(RemoveY(point));
		}

		public bool ValidateAddPoint (Vector3 point) {
			return polygon.ValidateNewPoint(RemoveY(point));
		}

		public bool PointInsideSection (Vector3 point) {
			return polygon.PointInPolygon(RemoveY(point));
		}

		private Vector2 RemoveY (Vector3 point) {
			return new Vector2(point.x, point.z);
		}

		private Vector3 AddY (Vector2 point) {
			return new Vector3(point.x, 0, point.y);
		}

		private void OnDrawGizmos () {
			Gizmos.color = Color.black;
			foreach (Vector2 polygonVertex in polygon.Vertices) {
				Gizmos.DrawSphere(AddY(polygonVertex), 0.2f);
			}
			foreach ((Vector2 p1, Vector2 p2) in polygon.Lines) {
				Gizmos.DrawLine(AddY(p1), AddY(p2));
			}
		}
	}
}
