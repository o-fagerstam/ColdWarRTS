using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Architecture.ScriptableObjectArchitecture {
	
	public class RuntimeList<T> : ScriptableObject, IList<T> where T : Object {
		private List<T> _list;
		public event Action<T> OnElementAdded;

		public IEnumerator<T> GetEnumerator () {
			return _list.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable)this).GetEnumerator();
		}
		public void Add (T item) {
			_list.Add(item);
			OnElementAdded.Invoke(item);
		}
		public void Clear () {
			_list.Clear();
		}
		public bool Contains (T item) {
			return _list.Contains(item);
		}
		public void CopyTo (T[] array, int arrayIndex) {
			_list.CopyTo(array, arrayIndex);
		}
		public bool Remove (T item) {
			return _list.Remove(item);
		}
		public int Count => _list.Count;
		public bool IsReadOnly => false;
		public int IndexOf (T item) {
			return _list.IndexOf(item);
		}
		public void Insert (int index, T item) {
			_list.Insert(index, item);
			OnElementAdded?.Invoke(item);
		}
		public void RemoveAt (int index) {
			_list.RemoveAt(index);
		}
		public T this [int index] {
			get => _list[index];
			set {
				if (_list[index] == value) {
					return;
				}
				_list[index] = value;
				OnElementAdded?.Invoke(value);
			} 
		}
	}
}
