using System;
using System.Collections.Generic;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Map {
	public class ForestSection : MonoBehaviour {

		private readonly Polygon polygon = new Polygon();
		private readonly List<GameObject> trees = new List<GameObject>();
		[AssetsOnly][Required][SerializeField] private GameObject treePrefab;
		[SerializeField] private float treeRadius = 0.15f;
		public bool IsClosed => polygon.IsClosed;
		public IEnumerable<Vector3> Points {
			get {
				List<Vector3> points = new List<Vector3>();
				foreach (Vector2 vertex in polygon.Vertices) {
					points.Add(LocalVec2ToWorldVec3(vertex));
				}
				return points;
			}
		}

		public event Action OnPolygonChanged;

		public bool AddPoint (Vector3 point) {
			bool result = polygon.AddPoint(WorldVec3ToLocalVec2(point));
			if (result) {
				OnPolygonChanged?.Invoke();
			}
			return result;
		}

		public bool ValidateAddPoint (Vector3 point) {
			return polygon.ValidateNewPoint(WorldVec3ToLocalVec2(point));
		}

		public bool Close () {
			bool result = polygon.ClosePolygon();
			if (result) {
				GenerateTrees();
				OnPolygonChanged?.Invoke();
			}
			return result;
		}

		public bool PointInsideSection (Vector3 point) {
			return polygon.PointInPolygon(WorldVec3ToLocalVec2(point));
		}

		private Vector2 WorldVec3ToLocalVec2 (Vector3 point) {
			Vector3 position = transform.position;
			return new Vector2(point.x - position.x, point.z - position.z);
		}

		private Vector3 LocalVec2ToWorldVec3 (Vector2 point) {
			Vector3 position = transform.position;
			return new Vector3(point.x + position.x, position.y, point.y + position.z);
		}
		
		private void GenerateTrees () {
			List<Vector2> polygonPoints = PoissonDiscSampling.GeneratePointsFromPolygon(treeRadius, polygon);
			foreach (Vector2 point in polygonPoints) {
				Vector3 worldPoint = LocalVec2ToWorldVec3(point);
				Vector3 rotation = new Vector3(0f, Random.value*360, 0f);
				Quaternion quatRotation = Quaternion.Euler(rotation);
				GameObject newTree = Instantiate(treePrefab, worldPoint, quatRotation, transform);
				trees.Add(newTree);
			}
		}
	}
}
