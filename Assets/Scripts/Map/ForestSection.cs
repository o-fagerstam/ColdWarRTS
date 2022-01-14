﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Constants;
using Graphics;
using Math;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;
namespace Map {
	public class ForestSection : AStaticMapElement {

		[ShowInInspector] private List<GpuInstance> treeInstances = new List<GpuInstance>();
		[AssetsOnly][Required][SerializeField] private GameObject treePrefab;
		[Range(2f, 10f)]
		[ShowInInspector] private static readonly float TreeRadiusMeters = 10f;
		
		private Polygon localSpacePolygon = new Polygon();
		private GpuInstancer gpuInstancer;
		private int treeGenSeed;

		private void Awake() {
			gpuInstancer = new GpuInstancer(treePrefab);
		}
		
		protected override void Update () {
			base.Update();
			gpuInstancer?.RenderBatches();
		}

		public IEnumerable<Vector3> Points {
			get {
				List<Vector3> points = new List<Vector3>();
				foreach (Vector2 vertex in localSpacePolygon.Vertices) {
					points.Add(LocalVec2ToWorldVec3(vertex));
				}
				return points;
			}
		}

		public void CreateFromSaveData (ForestSectionData data) {
			localSpacePolygon = new Polygon(data.polygon);
			GenerateTrees(data.seed);
		}

		public bool IsClosed => localSpacePolygon.IsClosed;

		public Polygon WorldPolygon => localSpacePolygon.ToWorldPolygon(transform.position.Flatten());

		public bool AddPoint (Vector3 point) {
			bool couldAddToPolygon = localSpacePolygon.AddVertex(WorldVec3ToLocalVec2(point));
			bool overlapsOtherForest = false;
			if (couldAddToPolygon) {
				foreach (ForestSection forestSection in SingletonManager.Retrieve<GameMap>().AllForests) {
					if (forestSection == this) {
						continue;
					}
					if (Overlaps(forestSection)) {
						overlapsOtherForest = true;
						localSpacePolygon.RemoveLastVertex();
						break;
					}
				}
			}

			bool result = couldAddToPolygon && !overlapsOtherForest;
			if (result) {
				InvokeShapeChanged();
			}
			return result;
		}

		public bool Close () {
			bool result = localSpacePolygon.ClosePolygon();
			if (!result) {
				return false;
			}
			InvokeShapeChanged();
			SingletonManager.Retrieve<GameMap>().RegisterStaticMapElement(this);
			GenerateTrees(Random.Range(int.MinValue, int.MaxValue));
			return true;
		}


		
		private void GenerateTrees (int seed) {
			treeGenSeed = seed;
			List<Vector2> polygonPoints = PoissonDiscSampling.GeneratePointsFromPolygon(ScaleUtil.GameToUnity(TreeRadiusMeters), localSpacePolygon, seed);
			treeInstances.Clear();
			MersenneTwister twister = new MersenneTwister(seed);
			foreach (Vector2 point in polygonPoints) {
				Vector3 worldPoint = LocalVec2ToWorldVec3(point);
				Vector3 rotation = new Vector3(0f, twister.NextFloatPositive()*360f, 0f);
				Quaternion quatRotation = Quaternion.Euler(rotation);
				GpuInstance newTree = new GpuInstance(worldPoint, Vector3.one * Mathf.Lerp(0.8f, 1.2f, twister.NextFloatPositive()), quatRotation, false);
				newTree = RaycastTree(newTree);
				treeInstances.Add(newTree);
			}
			gpuInstancer.SetInstances(treeInstances);
		}

		protected override void UpdateElementVisuals () {
			for (int i = 0; i < treeInstances.Count; i++) {
				treeInstances[i] = RaycastTree(treeInstances[i]);
			}
			gpuInstancer.SetInstances(treeInstances);
		}

		private static GpuInstance RaycastTree (GpuInstance instance) {
			if (!RaycastUtil.GroundLayerOnlyElevationRaycast(instance.position, out RaycastHit hit)) {
				throw new Exception("Failed to raycast new tree position: No ground below?");
			}
			bool treeOnRoad = RaycastUtil.RoadLayerOnlyElevationRaycast(instance.position, out RaycastHit roadHit);
			Vector3 newPosition = new Vector3(instance.position.x, hit.point.y, instance.position.z);
			float angle = Vector3.Angle(Vector3.up, hit.normal);
			bool enabled = angle < GeographyConstants.TREE_SLOPE_ANGLE_MAX &&
			               newPosition.y > GeographyConstants.TREE_ELEVATION_OVER_WATER_MIN &&
			               !treeOnRoad;
			return new GpuInstance(
				newPosition,
				instance.scale,
				instance.rotation,
				enabled
			);
		}

		public bool Overlaps (ForestSection other) {
			return WorldPolygon.Overlaps(other.WorldPolygon);
		}
		
		public override bool Overlaps (Rectangle worldRectangle) {
			return WorldPolygon.Overlaps(worldRectangle);
		}

		private Vector2 WorldVec3ToLocalVec2 (Vector3 point) {
			Vector3 position = transform.position;
			return new Vector2(point.x - position.x, point.z - position.z);
		}

		private Vector3 LocalVec2ToWorldVec3 (Vector2 point) {
			Vector3 position = transform.position;
			return new Vector3(point.x + position.x, position.y, point.y + position.z);
		}

		public ForestSectionData CreateSaveData () {
			return new ForestSectionData(transform.position, localSpacePolygon.CreateSaveData(), treeGenSeed);
		}

		[Serializable]
		public class ForestSectionData {
			public Vector3 worldPosition;
			public Polygon.PolygonSaveData polygon;
			public int seed;

			public ForestSectionData (Vector3 worldPosition, Polygon.PolygonSaveData polygon, int seed) {
				this.worldPosition = worldPosition;
				this.polygon = polygon;
				this.seed = seed;
			}
		}
	}
}
