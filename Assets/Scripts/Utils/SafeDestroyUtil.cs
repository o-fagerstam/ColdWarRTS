using UnityEngine;
namespace Utils {
	public static class SafeDestroyUtil {
		private static T SafeDestroy<T> (T obj) where T : Object {
			if (Application.isEditor) {
				Object.DestroyImmediate(obj);
			} else {
				Object.Destroy(obj);
			}
			return null;
		}
		public static T SafeDestroyGameObject<T> (T component) where T : Component {
			if (component != null) {
				SafeDestroy(component.gameObject);
			}
			return null;
		}
	}
}
