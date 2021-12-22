using System;
using System.Collections.Generic;
using UnityEngine;
namespace Math {
	public class Polygon {
		private List<Vector2> vertices = new List<Vector2>();
		public int NumOfVertices => vertices.Count;
		public IEnumerable<Vector2> Vertices => vertices;
		public IEnumerable<Tuple<Vector2, Vector2>> Lines {
			get {
				List<Tuple<Vector2, Vector2>> lines = new List<Tuple<Vector2, Vector2>>();
				int i = 0;
				do {
					int next = (i + 1)%NumOfVertices;
					lines.Add(new Tuple<Vector2, Vector2>(vertices[i], vertices[next]));
					i = next;
				} while (i != 0);
				return lines;
			}
		}

		public bool AddPoint (Vector2 point) {
			if (!ValidateNewPoint(point)) {
				return false;
			}
			vertices.Add(point);
			return true;
		}

		public bool PointInPolygon (Vector2 point) {
			if (NumOfVertices < 3) {
				return false;
			}

			Vector2 extremePoint = new Vector2(Mathf.Infinity, point.y);

			int intersectionCount = 0, i = 0;
			do {
				int next = (i + 1)%NumOfVertices;

				if (LinesIntersect(vertices[i], vertices[next], point, extremePoint)) {
					if (GetThreePointOrientation(vertices[i], point, vertices[next]) == Orientation.Collinear) {
						return OnCollinearSegment(vertices[i], point, vertices[next]);
					}
					intersectionCount++;
				}
				
				i = next;
			} while (i != 0);

			return intersectionCount%2 == 1;
		}

		/// <summary>
		/// Check if it would be valid to add a point to the polygon.
		/// </summary>
		public bool ValidateNewPoint (Vector2 point) {
			if (NumOfVertices < 2) { return true; }

			Vector2 lastPoint = vertices[NumOfVertices - 1];

			for (int i = 0; i < NumOfVertices-2; i++) {
				if (LinesIntersect(vertices[i], vertices[i + 1], lastPoint, point)) {
					Debug.Log($"Found intersecting line {i}, {i+1}");

					Vector3 vert1 = new Vector3(vertices[i].x, .1f, vertices[i].y);
					Vector3 vert2 = new Vector3(vertices[i+1].x, .1f, vertices[i+1].y);
					Debug.DrawLine(vert1, vert2, Color.red, 4f);
					return false;
				}
			}
			return true;
		}
		

		private Orientation GetThreePointOrientation (Vector2 p, Vector2 q, Vector2 r) {
			float val = (q.y - p.y)*(r.x - q.x) - (r.y - q.y)*(q.x - p.x);

			if (val > 0) {
				return Orientation.Clockwise;
			} else if (val < 0) {
				return Orientation.CounterClockwise;
			} else {
				return Orientation.Collinear;
			}
		}

		private bool OnCollinearSegment (Vector2 p, Vector3 q, Vector3 r) {
			return q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
			        q.y <= Mathf.Min(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y);
		}
		
		private bool LinesIntersect (Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
			Orientation o1 = GetThreePointOrientation(p1, q1, p2);
			Orientation o2 = GetThreePointOrientation(p1, q1, q2);
			Orientation o3 = GetThreePointOrientation(p2, q2, p1);
			Orientation o4 = GetThreePointOrientation(p2, q2, q1);

			if (o1 != o2 && o3 != o4) { return true;}
			
			return o1 == Orientation.Collinear &&
			       (OnCollinearSegment(p1, p2, q1) ||
			        OnCollinearSegment(p1, q2, q1) ||
			        OnCollinearSegment(p2, p1, q2) ||
			        OnCollinearSegment(p2, q1, q2));

		}

		private enum Orientation {
			Clockwise, CounterClockwise, Collinear
		}
	}
}
