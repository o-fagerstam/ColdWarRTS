using UnityEngine;
namespace Utils {
	public static class VectorUtil {
		public static Vector2 Flatten (this Vector3 v) => new Vector2(v.x, v.z);
		public static Vector3 AddY (this Vector2 v, float y = 0f) => new Vector3(v.x, y, v.y);
		public static Vector3 WorldPos2Local (this Vector3 worldPos, Vector3 localOrigin) => worldPos - localOrigin;
		public static Vector3 LocalPos2World (this Vector3 localPos, Vector3 localOrigin) => localPos + localOrigin;
		
		public static Vector2 Rotate(this Vector2 v, float degrees) {
			float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
			float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
			float tx = v.x;
			float ty = v.y;
			return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
		}
	}
}
