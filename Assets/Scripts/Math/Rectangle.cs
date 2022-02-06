using System.Collections.Generic;
using UnityEngine;
namespace Math {
	public readonly struct Rectangle {
		private readonly Vector2 _center; //Position of center
		private readonly Vector2 _dimension; //Length of sides

		public Vector2 Center => _center;
		public Vector2 Dimension => _dimension;
		public IEnumerable<Vector2> GetCorners () {
			yield return new Vector2(xMin, yMin);
			yield return new Vector2(xMin, yMax);
			yield return new Vector2(xMax, yMax);
			yield return new Vector2(xMax, yMin);
		}
		public float xMin => _center.x - _dimension.x/2f;
		public float xMax => _center.x + _dimension.x/2f;
		public float yMin => _center.y - _dimension.y/2f;
		public float yMax => _center.y + _dimension.y/2f;
		
		public Rectangle (Vector2 center, Vector2 dimension) {
			this._center = center;
			this._dimension = dimension;
		}

		public bool Overlaps (Rectangle other) {
			if (xMin >= other.xMax || other.xMin >= xMax) {
				return false;
			}
			if (yMin >= other.yMax || other.yMin >= yMax) {
				return false;
			}
			return true;
		}

		public bool Overlaps (Polygon other) {
			return other.Overlaps(this);
		}

		public bool ContainsPoint (Vector2 point) {
			return point.x >= xMin && point.x <= xMax &&
			       point.y >= yMin && point.y <= yMax;
		}
	}
}
