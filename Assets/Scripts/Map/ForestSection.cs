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
				worldPoint = RaycastElevation(worldPoint);
				Vector3 rotation = new Vector3(0f, Random.value*360, 0f);
				Quaternion quatRotation = Quaternion.Euler(rotation);
				GpuInstance newTree = new GpuInstance(worldPoint, Vector3.one, quatRotation);
				treeInstances.Add(newTree);
			}
			gpuInstancer.SetInstances(treeInstances);
		}

		protected override void ElevationUpdate () {
			for (int i = 0; i < treeInstances.Count; i++) {
				GpuInstance newInstance = new GpuInstance(
					RaycastElevation(treeInstances[i].position),
					treeInstances[i].scale,
					treeInstances[i].rotation
				);
				treeInstances[i] = newInstance;
			}
			gpuInstancer.SetInstances(treeInstances);
		}

		private static Vector3 RaycastElevation (Vector3 point) {
			if (!RaycastUtil.ElevationRaycast(point, out RaycastHit hit)) {
				throw new Exception("Failed to raycast new tree position: No ground below?");
			}
			point.y = hit.point.y;
			return point;
		}
	}
}
