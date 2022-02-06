using System;
using System.Collections.Generic;
using Math;
using Singleton;
using UnityEngine;
using Utils;
namespace Map {
	public abstract class AStaticMapElement : MonoBehaviour {
		private bool _updateNeeded;
		
		/// <summary>
		/// Triggers on component destruction. Args: This object
		/// </summary>
		public event Action<AStaticMapElement> OnDestruction;
		public event Action<AStaticMapElement> OnShapeChanged;

		private void OnDestroy () {
			OnDestruction?.Invoke(this);
		}

		protected virtual void Update () {
			if (_updateNeeded) {
				UpdateElementVisuals();
				_updateNeeded = false;
			}
		}
		protected abstract void UpdateElementVisuals ();
		public abstract bool Overlaps (Rectangle worldRectangle);
		public void NotifyVisualUpdateNeeded () {
			_updateNeeded = true;
		}
		protected void InvokeShapeChanged () {
			OnShapeChanged?.Invoke(this);
		}
	}
}
