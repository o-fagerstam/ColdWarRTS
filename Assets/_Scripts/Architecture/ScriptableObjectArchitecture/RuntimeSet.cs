using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScriptableObjectArchitecture {
	public abstract class RuntimeSet<T> : ScriptableObject, IEnumerable<T> {
		private readonly HashSet<T> _items = new HashSet<T>();
		public event Action<T> OnElementAdded;
		public bool LogEvents;

		public IEnumerator<T> GetEnumerator () => _items.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
		
		public bool Add (T item) {
			if (!_items.Add(item)) {
				return false;
			}
			if (LogEvents) {
				Debug.Log($"{this}: Added {item.GetType()} " + item);
			}
			OnElementAdded?.Invoke(item);
			return true;
		}
		public bool Remove (T item) {
			if (!_items.Remove(item)) {
				return false;
			}
			if (LogEvents) {
				Debug.Log($"{this}: Removed {item.GetType()} " + item);
			}
			return true;
		}
	}
}
