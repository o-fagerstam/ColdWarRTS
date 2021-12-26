﻿using System.Collections.Generic;
using UnityEngine;
namespace Math {
	public readonly struct Rectangle {
		private readonly Vector2 center; //Position of center
		private readonly Vector2 dimension; //Length of sides

		public Vector2 Center => center;
		public Vector2 Dimension => dimension;
		public IEnumerable<Vector2> GetCorners () {
			yield return new Vector2(xMin, yMin);
			yield return new Vector2(xMin, yMax);
			yield return new Vector2(xMax, yMax);
			yield return new Vector2(xMax, yMin);
		}
		public float xMin => center.x - dimension.x/2f;
		public float xMax => center.x + dimension.x/2f;
		public float yMin => center.y - dimension.y/2f;
		public float yMax => center.y + dimension.y/2f;
		
		public Rectangle (Vector2 center, Vector2 dimension) {
			this.center = center;
			this.dimension = dimension;
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
	}
}