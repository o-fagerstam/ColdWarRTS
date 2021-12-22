using UnityEngine;
namespace Utils {
	public static class GetComponentUtil {
		public static bool TryGetComponentInParent<T> (Component fromComponent, out T targetComponent) where T : Component {
			Component c = fromComponent;
			bool res;
			// ReSharper disable once AssignmentInConditionalExpression
			while ( !(res = c.TryGetComponent(out targetComponent)) &&
			              (c = c.transform.parent) != null) {}
			return res;
		}

		public static bool TryGetComponentInParent<T> (GameObject fromGameObject, out T targetComponent) where T : Component {
			return TryGetComponentInParent(fromGameObject.transform, out targetComponent);
		}
	}
}
