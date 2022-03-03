using UnityEngine;
using Utils;
namespace Singleton {
	public abstract class ASingletonMonoBehaviour<T> : MonoBehaviour, ISingleton where T : MonoBehaviour {
		private static T _instance;
		protected virtual void OnEnable () {
			if (_instance == null) {
				_instance = this as T;
				SingletonManager.Register(this);
			} else {
				SafeDestroyUtil.SafeDestroyGameObject(this);
			}

		}
		protected virtual void OnDisable () {
			if (_instance == this) {
				_instance = null;
				SingletonManager.Unregister(this);
			}
		}
	}
}
