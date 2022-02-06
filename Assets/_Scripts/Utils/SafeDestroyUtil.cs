using UnityEngine;
namespace Utils {
	public static class SafeDestroyUtil {
		public static void SafeDestroy<T> (T obj) where T : Object {
			if (Application.isEditor) {
				Object.DestroyImmediate(obj);
			} else {
				Object.Destroy(obj);
			}
		}
		public static void SafeDestroyGameObject<T> (T component) where T : Component {
			if (component != null) {
				SafeDestroy(component.gameObject);
			}
		}
	}
}
