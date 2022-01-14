﻿using System;
using System.Collections.Generic;
using Math;
using Singleton;
using UnityEngine;
using Utils;
namespace Map {
	public abstract class AStaticMapElement : MonoBehaviour {
		private bool updateNeeded;
		
		/// <summary>
		/// Triggers on component destruction. Args: This object
		/// </summary>
		public event Action<AStaticMapElement> OnDestruction;
		public event Action<AStaticMapElement> OnShapeChanged;

		private void OnDestroy () {
			OnDestruction?.Invoke(this);
		}

		protected virtual void Update () {
			if (updateNeeded) {
				UpdateElementVisuals();
				updateNeeded = false;
			}
		}
		protected abstract void UpdateElementVisuals ();
		public abstract bool Overlaps (Rectangle worldRectangle);
		public void NotifyVisualUpdateNeeded () {
			updateNeeded = true;
		}
		protected void InvokeShapeChanged () {
			OnShapeChanged?.Invoke(this);
		}
	}
}
