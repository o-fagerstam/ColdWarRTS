using System;
using System.Collections.Generic;
using System.Linq;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	public class GameMap : MonoBehaviour {
		[SerializeField, AssetsOnly, Required] private GameMapScriptableValue gameMapSingletonValue;
		
		[SerializeField] private int chunksPerSide;
		[SerializeField] private int chunkResolution;
		[SerializeField] private float mapSize;

		[SerializeField, Required, AssetsOnly] private ForestSectionRuntimeSet forestSectionRuntimeSet;
		[SerializeField, Required, AssetsOnly] private RoadSegmentRuntimeSet roadSegmentRuntimeSet;
		[SerializeField, Required, AssetsOnly] private CapturePointRuntimeSet capturePointRuntimeSet;

		[SerializeField, Required, AssetsOnly] private GameMapEvent onMapReload;

		[SerializeField] private List<MapChunk> chunks = new List<MapChunk>();
		[SerializeField, Required, AssetsOnly] private MapChunk mapChunkPrefab;
		[SerializeField, Required, AssetsOnly] private ForestSection forestSectionPrefab;
		[SerializeField, Required, AssetsOnly] private RoadSegment roadSegmentPrefab;
		[SerializeField, Required, AssetsOnly] private CapturePoint capturePointPrefab;
		public IEnumerable<ForestSection> AllForests => forestSectionRuntimeSet;
		public IEnumerable<RoadSegment> AllRoadSegments => roadSegmentRuntimeSet;
		public IEnumerable<CapturePoint> AllCapturePoints => capturePointRuntimeSet;
		public IEnumerable<AStaticMapElement> AllStaticMapElements => new IEnumerable<AStaticMapElement>[] {AllForests, AllCapturePoints, AllRoadSegments}.SelectMany(x => x);

		private void OnEnable () {
			forestSectionRuntimeSet.OnElementAdded += RegisterStaticMapElement;
			roadSegmentRuntimeSet.OnElementAdded += RegisterStaticMapElement;
			capturePointRuntimeSet.OnElementAdded += RegisterStaticMapElement;
			gameMapSingletonValue.Value = this;
		}
		
		private void OnDisable () {
			forestSectionRuntimeSet.OnElementAdded -= RegisterStaticMapElement;
			roadSegmentRuntimeSet.OnElementAdded -= RegisterStaticMapElement;
			capturePointRuntimeSet.OnElementAdded -= RegisterStaticMapElement;
			gameMapSingletonValue.Value = null;
		}
		
		public void GenerateFlatMap () {
			ClearMap();

			float chunkSize = mapSize/chunksPerSide;
			Vector3 chunkBaseWorldOffset = transform.position + new Vector3(-(mapSize/2f) + chunkSize/2f, 0f, (chunkSize - mapSize)/2f);
			for (int y = 0; y < chunksPerSide; y++) {
				for (int x = 0; x < chunksPerSide; x++) {
					Vector3 newChunkPosition = new Vector3(x*chunkSize, 0f, y*chunkSize) + chunkBaseWorldOffset;
					MapChunk newChunk = Instantiate(mapChunkPrefab, newChunkPosition, Quaternion.identity, transform);
					newChunk.GenerateChunk(chunkResolution, chunkSize);
					chunks.Add(newChunk);
				}
			}
			PostMapGeneration();
		}

		public void GenerateFromMapData (GameMapSaveData saveData) {
			ClearMap();

			chunksPerSide = saveData.chunksPerSide;
			chunkResolution = saveData.chunkResolution;
			mapSize = saveData.mapSize;

			float chunkSize = mapSize/chunksPerSide;
			Vector3 chunkBaseWorldOffset = transform.position + new Vector3(-(mapSize/2f) + chunkSize/2f, 0f, (chunkSize - mapSize)/2f);
			int chunkIndex = 0;
			for (int y = 0; y < chunksPerSide; y++) {
				for (int x = 0; x < chunksPerSide; x++) {
					Vector3 newChunkPosition = new Vector3(x*chunkSize, 0f, y*chunkSize) + chunkBaseWorldOffset;
					MapChunk newChunk = Instantiate(mapChunkPrefab, newChunkPosition, Quaternion.identity, transform);
					newChunk.GenerateFromChunkData(saveData.chunkData[chunkIndex++]);
					chunks.Add(newChunk);
				}
			}

			foreach (ForestSection.ForestSectionData forestSectionData in saveData.forestSectionData) {
				ForestSection newForestSection = Instantiate(forestSectionPrefab, forestSectionData.worldPosition, Quaternion.identity, transform);
				newForestSection.CreateFromSaveData(forestSectionData);
			}
			foreach (RoadSegment.RoadSegmentSaveData roadSegmentSaveData in saveData.roadSegmentData) {
				RoadSegment newRoadSegment = Instantiate(roadSegmentPrefab, roadSegmentSaveData.position, Quaternion.identity, transform);
				newRoadSegment.CreateFromSaveData(roadSegmentSaveData);
			}
			foreach (CapturePoint.CapturePointData capturePointData in saveData.capturePointData) {
				CapturePoint newCapturePoint = Instantiate(capturePointPrefab, capturePointData.position, Quaternion.identity, transform);
				newCapturePoint.RestoreFromSaveData(capturePointData);
			}
			PostMapGeneration();
		}

		private void PostMapGeneration () {
			InitializePathing();
			onMapReload.Invoke(this, new GameMapEventArgs(){Map = this});
		}

		private void InitializePathing () {
			float nodeSize = AstarPath.active.data.gridGraph.nodeSize;
			AstarPath.active.data.gridGraph.SetDimensions(Mathf.RoundToInt(mapSize/nodeSize), Mathf.RoundToInt(mapSize/nodeSize), nodeSize);
			RecalculateAstarGraph();
		}

		private void ClearMap () {
			foreach (MapChunk chunk in chunks) {
				SafeDestroyUtil.SafeDestroyGameObject(chunk);
			}
			chunks.Clear();
			List<AStaticMapElement> allElements = AllStaticMapElements.ToList();
			for (var i = allElements.Count - 1; i >= 0; i--) {
				AStaticMapElement staticMapElement = allElements[i];
				staticMapElement.OnDestruction -= HandleStaticElementDestroyed;
				staticMapElement.OnShapeChanged -= HandleStaticElementShapeChanged;
				SafeDestroyUtil.SafeDestroyGameObject(staticMapElement);
			}
		}

		private void RecalculateAstarGraph () {
			AstarPath.active.Scan();
		}

		private void RecalculateAstarGraph (Bounds bounds) {
			AstarPath.active.UpdateGraphs(bounds);
		}

		public void EditElevation (Vector3 hitPoint, float radius, float magnitude) {
			Vector2 rectangleSize = new Vector2(radius*2, radius*2);
			Rectangle hitRectangle = new Rectangle(VectorUtil.Flatten(hitPoint), rectangleSize);

			foreach (MapChunk chunk in GetOverlappingChunks(hitRectangle)) {
				chunk.EditElevation(hitPoint, radius, magnitude);
			}
			Bounds astarUpdateBounds = new Bounds(hitPoint, new Vector3(radius*2 + 2f, 30f, radius*2 + 2f));
			RecalculateAstarGraph(astarUpdateBounds);
		}

		public IEnumerable<MapChunk> GetOverlappingChunks (Rectangle worldSpaceRectangle) {
			foreach (MapChunk chunk in chunks) {
				if (chunk.worldRectangle.Overlaps(worldSpaceRectangle)) {
					yield return chunk;
				}
			}
		}

		public void RegisterStaticMapElement (AStaticMapElement element) {
			element.OnShapeChanged += HandleStaticElementShapeChanged;
			element.OnDestruction += HandleStaticElementDestroyed;
			ReevaluateStaticElementChunks(element);
		}

		private void ReevaluateStaticElementChunks (AStaticMapElement element) {
			foreach (MapChunk mapChunk in chunks) {
				if (element.Overlaps(mapChunk.worldRectangle)) {
					mapChunk.TryAddStaticMapElement(element);
				} else {
					mapChunk.TryRemoveStaticMapElement(element);
				}
			}
		}

		private void HandleStaticElementShapeChanged (AStaticMapElement element) {
			ReevaluateStaticElementChunks(element);
		}
		private void HandleStaticElementDestroyed (AStaticMapElement element) {
			element.OnDestruction -= HandleStaticElementDestroyed;
			element.OnShapeChanged -= HandleStaticElementShapeChanged;
		}

		public GameMapSaveData CreateSaveData () {
			return new GameMapSaveData(
				chunksPerSide,
				chunkResolution,
				mapSize,
				chunks.Select(c => c.CreateChunkData()).ToList(),
				(from fs in AllForests select fs.CreateSaveData()).ToList(),
				(from rs in AllRoadSegments select rs.CreateSaveData()).ToList(),
				(from cp in AllCapturePoints select cp.CreateSaveData()).ToList()
			);
		}

		[Serializable]
		public class GameMapSaveData {
			public int chunksPerSide;
			public int chunkResolution;
			public float mapSize;
			public List<MapChunk.MapChunkSaveData> chunkData;
			public List<ForestSection.ForestSectionData> forestSectionData;
			public List<RoadSegment.RoadSegmentSaveData> roadSegmentData;
			public List<CapturePoint.CapturePointData> capturePointData;

			public GameMapSaveData (
				int chunksPerSide,
				int chunkResolution,
				float mapSize,
				List<MapChunk.MapChunkSaveData> chunkData,
				List<ForestSection.ForestSectionData> forestSectionData,
				List<RoadSegment.RoadSegmentSaveData> roadSegmentData,
				List<CapturePoint.CapturePointData> capturePointData
			) {
				this.chunksPerSide = chunksPerSide;
				this.chunkResolution = chunkResolution;
				this.mapSize = mapSize;
				this.chunkData = chunkData;
				this.forestSectionData = forestSectionData;
				this.roadSegmentData = roadSegmentData;
				this.capturePointData = capturePointData;
			}
		}
	}
}
