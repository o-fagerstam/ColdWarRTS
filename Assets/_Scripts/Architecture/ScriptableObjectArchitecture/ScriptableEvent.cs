using System;
using UnityEngine;
namespace ScriptableObjectArchitecture {
	public abstract class ScriptableEvent<T> : ScriptableObject where T : EventArgs {
		public event EventHandler<T> Event;
		public void Invoke (object origin, T eventArgs) {
			Event?.Invoke(origin, eventArgs);
		}
	}
}
