using System;
using System.Collections.Generic;
using Math;
using Singleton;
using UnityEngine;
using Utils;
namespace Map {
	public abstract class AStaticMapElement : MonoBehaviour {
		private bool elevationUpdateNeeded;
		
		/// <summary>
		/// Triggers on component destruction. Args: This object
		/// </summary>
		public event Action<AStaticMapElement> OnDestruction;

		private void OnDestroy () {
			OnDestruction?.Invoke(this);
		}

		protected virtual void Update () {
			if (elevationUpdateNeeded) { ElevationUpdate(); }
		}
		protected abstract void ElevationUpdate ();
		public abstract bool Overlaps (Rectangle worldRectangle);
		public void NotifyUpdateNeeded () {
			elevationUpdateNeeded = true;
		}


	}
}
