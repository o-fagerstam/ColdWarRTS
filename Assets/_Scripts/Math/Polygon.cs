using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Math {
	
	public class Polygon {
		private readonly List<Vector2> _vertices = new List<Vector2>();
		private bool _isClosed;
		public int NumOfVertices => _vertices.Count;
		public bool IsClosed => _isClosed;
		public IEnumerable<Vector2> Vertices => _vertices;
		private IEnumerable<Tuple<Vector2, Vector2>> Lines {
			get {
				int i = 0;
				int stoppingPoint = _isClosed ? 0 : NumOfVertices - 1;
				do {
					int next = (i + 1)%NumOfVertices;
					yield return new Tuple<Vector2, Vector2>(_vertices[i], _vertices[next]);
					i = next;
				} while (i != stoppingPoint);
			}
		}

		public Vector2 BoundingBoxSize {
			get {
				if (_vertices.Count == 0) {
					throw new Exception("Cannot find bounding box of empty polygon");
				}
				float xMin = float.MaxValue;
				float yMin = float.MaxValue;
				float xMax = float.MinValue;
				float yMax = float.MinValue;

				foreach (Vector2 vertex in _vertices) {
					xMin = Mathf.Min(xMin, vertex.x);
					xMax = Mathf.Max(xMax, vertex.x);
					yMin = Mathf.Min(yMin, vertex.y);
					yMax = Mathf.Max(yMax, vertex.y);
				}

				return new Vector2(xMax - xMin, yMax - yMin);
			}
		}

		public Vector2 PolyOriginRelativeToBoundingBoxOrigin {
			get {
				if (_vertices.Count == 0) {
					throw new Exception("Cannot find bounding box of empty polygon");
				}
				float xMin = float.MaxValue;
				float yMin = float.MaxValue;

				foreach (Vector2 vertex in _vertices) {
					xMin = Mathf.Min(xMin, vertex.x);
					yMin = Mathf.Min(yMin, vertex.y);
				}

				return new Vector2(xMin, yMin);
			}
		}

		public Polygon () {}
		public Polygon (Rectangle rectangle) : this(rectangle.GetCorners().ToList(), true) {}
		private Polygon (List<Vector2> vertices, bool isClosed = false) {
			foreach (Vector2 vertex in vertices) {
				this._vertices.Add(vertex);
			}
			this._isClosed = isClosed;
		}

		public Polygon (PolygonSaveData data) {
			foreach (Vector2 vertex in data.vertices) {
				_vertices.Add(vertex);
			}
			_isClosed = data.isClosed;
		}

		public bool AddVertex (Vector2 point) {
			if (!ValidateNewPoint(point)) {
				return false;
			}
			_vertices.Add(point);
			return true;
		}

		public void RemoveLastVertex () {
			_vertices.RemoveAt(_vertices.Count-1);
		}

		public bool ClosePolygon () {
			if (!ValidateClosePolygon()) {
				return false;
			}
			_isClosed = true;
			return true;
		}

		public bool PointInPolygon (Vector2 point) {
			if (NumOfVertices < 3 || !_isClosed) {
				return false;
			}

			Vector2 extremePoint = new Vector2(1000000f, point.y);

			int intersectionCount = 0, i = 0;
			do {
				int next = (i + 1)%NumOfVertices;

				if (LinesIntersect(_vertices[i], _vertices[next], point, extremePoint)) {
					if (GetThreePointOrientation(_vertices[i], point, _vertices[next]) == Orientation.Collinear) {
						return OnCollinearSegment(_vertices[i], point, _vertices[next]);
					}
					intersectionCount++;
				}

				i = next;
			} while (i != 0);

			return intersectionCount%2 == 1;
		}

		/// <summary>
		/// Validate if a given point can be added to the polygon.
		/// </summary>
		public bool ValidateNewPoint (Vector2 point) {
			if (NumOfVertices < 2) { return true; }

			Polygon testPolygon = new Polygon(_vertices);
			testPolygon.UnvalidatedAddPoint(point);
			return !testPolygon.HasIntersectingLines();
		}
		/// <summary>
		/// Validate if the polygon can be closed.
		/// </summary>
		public bool ValidateClosePolygon () {
			if (NumOfVertices < 3) { return false; }

			Polygon testPolygon = new Polygon(_vertices, true);
			return !testPolygon.HasIntersectingLines();
		}

		private void UnvalidatedAddPoint (Vector2 point) {
			_vertices.Add(point);
		}

		private void UnvalidatedCLosePolygon () {
			_isClosed = true;
		}

		private bool HasIntersectingLines () {
			for (int i = 0; i < NumOfVertices - 3; i++) {
				for (int j = i + 2; j < NumOfVertices - 2; j++) {
					if (LinesIntersect(_vertices[i], _vertices[i + 1], _vertices[j], _vertices[j + 1])) {
						return true;
					}
				}
			}

			if (_isClosed) {
				for (int i = 1; i < NumOfVertices - 2; i++) {
					if (LinesIntersect(_vertices[i], _vertices[i + 1], _vertices[0], _vertices[NumOfVertices - 1])) {
						return true;
					}
				}
			}
			return false;
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

			if (o1 != o2 && o3 != o4) { return true; }

			return o1 == Orientation.Collinear &&
			       (OnCollinearSegment(p1, p2, q1) ||
			        OnCollinearSegment(p1, q2, q1) ||
			        OnCollinearSegment(p2, p1, q2) ||
			        OnCollinearSegment(p2, q1, q2));

		}

		/// <summary>
		/// Checks if two polygons overlap.
		/// NB Make sure that both polygons are in the same space (local/world)!
		/// </summary>
		/// <param name="other">The other polygon</param>
		public bool Overlaps (Polygon other) {
			List<Tuple<Vector2, Vector2>> thisLines = Lines.ToList();
			List<Tuple<Vector2, Vector2>> otherLines = other.Lines.ToList();

			if (PointInPolygon(otherLines[0].Item1) || other.PointInPolygon(thisLines[0].Item1)) {
				return true;
			}

			foreach ((Vector2 p1, Vector2 q1) in thisLines) {
				foreach ((Vector2 p2, Vector2 q2) in otherLines) {
					if (LinesIntersect(p1, q1, p2, q2)) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Checks if this polygon overlaps with a rectangle.
		/// NB Make sure that both polygons are in the same space (local/world)!
		/// </summary>
		/// <param name="rectangle">The rectangle to check against</param>
		public bool Overlaps (Rectangle rectangle) {
			return Overlaps(new Polygon(rectangle));
		}

		public Polygon ToWorldPolygon (Vector2 originWorldPosition) {
			List<Vector2> worldVerts = new List<Vector2>();
			foreach (Vector2 vertex in _vertices) {
				worldVerts.Add(vertex + originWorldPosition);
			}
			return new Polygon(worldVerts, _isClosed);
		}
		
		private enum Orientation {
			Clockwise, CounterClockwise, Collinear
		}

		public PolygonSaveData CreateSaveData () {
			return new PolygonSaveData(_vertices, _isClosed);
		}

		[Serializable]
		public class PolygonSaveData {
			public List<Vector2> vertices;
			public bool isClosed;

			public PolygonSaveData (List<Vector2> vertices, bool isClosed) {
				this.vertices = vertices;
				this.isClosed = isClosed;
			}
		}
	}

}
