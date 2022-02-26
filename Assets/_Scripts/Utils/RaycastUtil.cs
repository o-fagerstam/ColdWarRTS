using Constants;
using UnityEngine;
namespace Utils {
	public static class RaycastUtil {
		public static bool GroundLayerOnlyElevationRaycast (Vector3 position, out RaycastHit hit) {
			Ray ray = new Ray(position + Vector3.up*10000f, Vector3.down);
			return Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMasks.bareGround);
		}
		
		public static bool RoadLayerOnlyElevationRaycast (Vector3 position, out RaycastHit hit) {
			Ray ray = new Ray(position + Vector3.up*10000f, Vector3.down);
			return Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMasks.road);
		}
		
		public static bool NonRoadSurfaceElevationRaycast (Vector3 position, out RaycastHit hit) {
			Ray ray = new Ray(position + Vector3.up*10000f, Vector3.down);
			return Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMasks.nonRoadSurface);
		}
		
	}
}
