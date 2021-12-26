using System;
using UnityEngine;
namespace Singleton {
	public abstract class ASingleton : MonoBehaviour {
		protected virtual void Start () {
			SingletonManager.Register(this);
		}
		protected virtual void OnDestroy () {
			SingletonManager.Unregister(this);
		}
	}
}
