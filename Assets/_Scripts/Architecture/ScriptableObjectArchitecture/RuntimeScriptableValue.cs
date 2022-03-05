using System;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Architecture.ScriptableObjectArchitecture {
	public class RuntimeScriptableValue<T> : ScriptableObject where T : class {
		public event Action<T> OnValueUpdated;
		[ShowInInspector, ReadOnly] private T _value;
		public T Value {
			get => _value;
			set {
				if (_value == value) { return; }
				_value = value;
				Debug.Log($"Value set to {value}");
				OnValueUpdated?.Invoke(Value);
			} 
		}
	}
}
