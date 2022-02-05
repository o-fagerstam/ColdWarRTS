using System;
using UnityEngine;
namespace Singleton {
	public class ASingletonMonoBehaviour : MonoBehaviour {
		protected virtual void OnEnable () {
			SingletonManager.Register(this);
		}
		protected virtual void OnDisable () {
			SingletonManager.Unregister(this);
		}
	}
}
