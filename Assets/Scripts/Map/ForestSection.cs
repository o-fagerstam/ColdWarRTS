using System;
using System.Collections.Generic;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;
namespace Map {
	public class ForestSection : AStaticMapElement {
		[ShowInInspector] private readonly List<GameObject> trees = new List<GameObject>();
		[AssetsOnly][Required][SerializeField] private GameObject treePrefab;
		[ShowInInspector] private static readonly float TreeRadius = 0.4f;

		public override bool Close () {
			bool result = base.Close();
			if (result) {
				GenerateTrees();
				return true;
			}
			return false;
		}
		
		private void GenerateTrees () {
			List<Vector2> polygonPoints = PoissonDiscSampling.GeneratePointsFromPolygon(TreeRadius, polygon);
			foreach (Vector2 point in polygonPoints) {
				Vector3 worldPoint = LocalVec2ToWorldVec3(point);
				Vector3 rotation = new Vector3(0f, Random.value*360, 0f);
				Quaternion quatRotation = Quaternion.Euler(rotation);
				GameObject newTree = Instantiate(treePrefab, worldPoint, quatRotation, transform);
				SetTreeElevation(newTree);
				trees.Add(newTree);
			}
		}

		protected override void ElevationUpdate () {
			foreach (GameObject tree in trees) {
				SetTreeElevation(tree);
			}
		}

		private void SetTreeElevation (GameObject tree) {
			Vector3 treePosition = tree.transform.position;
			if (!RaycastUtil.ElevationRaycast(treePosition, out RaycastHit hit)) {
				throw new Exception("Failed to raycast new tree position: No ground below?");
			}
			treePosition.y = hit.point.y;
			tree.transform.position = treePosition;
		}
	}
}
