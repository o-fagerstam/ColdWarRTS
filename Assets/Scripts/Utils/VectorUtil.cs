using UnityEngine;
namespace Utils {
	public static class VectorUtil {
		public static Vector2 Flatten (Vector3 v) => new Vector2(v.x, v.z);
		public static Vector3 AddY (Vector2 v, float y = 0f) => new Vector3(v.x, y, v.y);
		public static Vector3 WorldPos2Local (Vector3 worldPos, Vector3 localOrigin) => worldPos - localOrigin;
		public static Vector3 LocalPos2World (Vector3 localPos, Vector3 localOrigin) => localPos + localOrigin;
	}
}
