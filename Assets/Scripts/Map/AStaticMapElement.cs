using System;
using System.Collections.Generic;
using Math;
using Singleton;
using UnityEngine;
namespace Map {
	public abstract class AStaticMapElement : MonoBehaviour {
		protected readonly Polygon polygon = new Polygon();
		public bool IsClosed => polygon.IsClosed;
		private bool elevationUpdateNeeded;
		public IEnumerable<Vector3> Points {
			get {
				List<Vector3> points = new List<Vector3>();
				foreach (Vector2 vertex in polygon.Vertices) {
					points.Add(LocalVec2ToWorldVec3(vertex));
				}
				return points;
			}
		}
		
		/// <summary>
		/// Triggers when the polygon changes (or is closed).
		/// Args: This component, isClosed
		/// </summary>
		public event Action<AStaticMapElement, bool> OnPolygonChanged;
		
		/// <summary>
		/// Triggers on component destruction. Args: This object
		/// </summary>
		public event Action<AStaticMapElement> OnDestruction;

		private void OnDestroy () {
			OnDestruction?.Invoke(this);
		}

		private void Update () {
			if (elevationUpdateNeeded) { ElevationUpdate(); }
		}
		protected abstract void ElevationUpdate ();

		public bool AddPoint (Vector3 point) {
			bool result = polygon.AddPoint(WorldVec3ToLocalVec2(point));
			if (result) {
				OnPolygonChanged?.Invoke(this, IsClosed);
			}
			return result;
		}

		public bool ValidateAddPoint (Vector3 point) {
			return polygon.ValidateNewPoint(WorldVec3ToLocalVec2(point));
		}

		public virtual bool Close () {
			bool result = polygon.ClosePolygon();
			if (result) {
				OnPolygonChanged?.Invoke(this, true);
				SingletonManager.Retrieve<GameMap>().RegisterStaticMapElement(this);
			}
			return result;
		}

		public bool PointInsideSection (Vector3 point) {
			return polygon.PointInPolygon(WorldVec3ToLocalVec2(point));
		}
		
		
		protected Vector2 WorldVec3ToLocalVec2 (Vector3 point) {
			Vector3 position = transform.position;
			return new Vector2(point.x - position.x, point.z - position.z);
		}

		protected Vector3 LocalVec2ToWorldVec3 (Vector2 point) {
			Vector3 position = transform.position;
			return new Vector3(point.x + position.x, position.y, point.y + position.z);
		}


		public void NotifyUpdateNeeded () {
			elevationUpdateNeeded = true;
		}

		public bool Overlaps (Rectangle rectangle) {
			return polygon.Overlaps(rectangle);
		}
	}
}
