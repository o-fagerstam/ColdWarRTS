using System;
using System.Collections.Generic;
using Constants;
using Graphics;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;
namespace Map {
	public class ForestSection : AStaticMapElement {
		[ShowInInspector] private readonly List<GpuInstance> treeInstances = new List<GpuInstance>();
		[AssetsOnly][Required][SerializeField] private GameObject treePrefab;
		[Range(2f, 10f)]
		[ShowInInspector] private static readonly float TreeRadiusMeters = 10f;
		private GpuInstancer gpuInstancer;

		public override bool Close () {
			bool result = base.Close();
			if (result) {
				GenerateTrees();
				return true;
			}
			return false;
		}

		protected override void Update () {
			base.Update();
			gpuInstancer?.RenderBatches();
		}

		private void GenerateTrees () {
			List<Vector2> polygonPoints = PoissonDiscSampling.GeneratePointsFromPolygon(ScaleUtil.GameToUnity(TreeRadiusMeters), localSpacePolygon);
			gpuInstancer = new GpuInstancer(treePrefab);
			treeInstances.Clear();
			foreach (Vector2 point in polygonPoints) {
				Vector3 worldPoint = LocalVec2ToWorldVec3(point);
				Vector3 rotation = new Vector3(0f, Random.value*360, 0f);
				Quaternion quatRotation = Quaternion.Euler(rotation);
				GpuInstance newTree = new GpuInstance(worldPoint, Vector3.one, quatRotation, false);
				newTree = RaycastTree(newTree);
				treeInstances.Add(newTree);
			}
			gpuInstancer.SetInstances(treeInstances);
		}

		protected override void ElevationUpdate () {
			for (int i = 0; i < treeInstances.Count; i++) {
				treeInstances[i] = RaycastTree(treeInstances[i]);
			}
			gpuInstancer.SetInstances(treeInstances);
		}

		private static GpuInstance RaycastTree (GpuInstance instance) {
			if (!RaycastUtil.ElevationRaycast(instance.position, out RaycastHit hit)) {
				throw new Exception("Failed to raycast new tree position: No ground below?");
			}
			Vector3 newPosition = new Vector3(instance.position.x, hit.point.y, instance.position.z);
			float angle = Vector3.Angle(Vector3.up, hit.normal);
			bool enabled = angle < GeographyConstants.TREE_SLOPE_ANGLE_MAX &&
			               newPosition.y > GeographyConstants.TREE_ELEVATION_OVER_WATER_MIN;
			return new GpuInstance(
				newPosition,
				instance.scale,
				instance.rotation,
				enabled
			);
		}
	}
}
