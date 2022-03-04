using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Architecture.ScriptableObjectArchitecture {
	public abstract class RuntimeSet<T> : ScriptableObject, ICollection<T> {
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
		public void Clear () {
			_items.Clear();
		}
		public bool Contains (T item) {
			return _items.Contains(item);
		}
		public void CopyTo (T[] array, int arrayIndex) {
			_items.CopyTo(array, arrayIndex);
		}
		void ICollection<T>.Add (T item) {
			_items.Add(item);
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
		public int Count => _items.Count;
		public bool IsReadOnly => false;
	}
}
