using System;
using UnityEngine;
namespace Singleton {
	public class ASingletonMonoBehaviour : MonoBehaviour {
		private void OnEnable () {
			SingletonManager.Register(this);
		}
		private void OnDisable () {
			SingletonManager.Unregister(this);
		}
	}
}
