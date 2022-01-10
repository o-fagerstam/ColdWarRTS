using System;
using System.Collections.Generic;
using System.Linq;
using Math;
using Persistence;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	public class GameMap : ASingleton {
		[SerializeField] private int chunksPerSide;
		[SerializeField] private int chunkResolution;
		[SerializeField] private float mapSize;

		[SerializeField] private List<MapChunk> chunks = new List<MapChunk>();
		[SerializeField] private MapChunk mapChunkPrefab;
		[ShowInInspector][ReadOnly] private List<ForestSection> allForests = new List<ForestSection>();
		[ShowInInspector][ReadOnly] private List<AStaticMapElement> allStaticMapElements = new List<AStaticMapElement>();
		public IEnumerable<ForestSection> AllForests => allForests;
		protected override void Start () {
			base.Start();
			GenerateFlatMap();
		}

		private void GenerateFlatMap () {
			ClearMap();

			float chunkSize = mapSize/chunksPerSide;
			Vector3 chunkBaseWorldOffset = transform.position + new Vector3(-(mapSize/2f), 0f, -(mapSize/2f));
			for (int y = 0; y < chunksPerSide; y++) {
				for (int x = 0; x < chunksPerSide; x++) {
					Vector3 newChunkPosition = new Vector3(x*chunkSize, 0f, y*chunkSize) + chunkBaseWorldOffset;
					MapChunk newChunk = Instantiate(mapChunkPrefab, newChunkPosition, Quaternion.identity, transform);
					newChunk.GenerateFlatChunk(chunkResolution, chunkSize);
					chunks.Add(newChunk);
				}
			}
		}

		public void SaveMap () {
			MapSaveSystem.SaveMap(this);
		}

		public void LoadMap () {
			MapSaveSystem.LoadMap(this);
		}

		public void GenerateFromMapData (GameMapData data) {
			ClearMap();

			chunksPerSide = data.chunksPerSide;
			chunkResolution = data.chunkResolution;
			mapSize = data.mapSize;

			float chunkSize = mapSize/chunksPerSide;
			Vector3 chunkBaseWorldOffset = transform.position + new Vector3(-(mapSize/2f), 0f, -(mapSize/2f));
			int chunkIndex = 0;
			for (int y = 0; y < chunksPerSide; y++) {
				for (int x = 0; x < chunksPerSide; x++) {
					Vector3 newChunkPosition = new Vector3(x*chunkSize, 0f, y*chunkSize) + chunkBaseWorldOffset;
					MapChunk newChunk = Instantiate(mapChunkPrefab, newChunkPosition, Quaternion.identity, transform);
					newChunk.GenerateFromChunkData(data.chunkData[chunkIndex++]);
					chunks.Add(newChunk);
				}
			}
		}

		private void ClearMap () {
			foreach (MapChunk chunk in chunks) {
				SafeDestroyUtil.SafeDestroyGameObject(chunk);
			}
			chunks.Clear();
			foreach (AStaticMapElement staticMapElement in allStaticMapElements) {
				staticMapElement.OnDestruction -= HandleStaticElementDestroyed;
				staticMapElement.OnShapeChanged -= HandleStaticElementShapeChanged;
				SafeDestroyUtil.SafeDestroyGameObject(staticMapElement);
			}
			allStaticMapElements.Clear();
			allForests.Clear();
		}
		public void EditElevation (Vector3 hitPoint, float radius, float magnitude) {
			Vector2 rectangleSize = new Vector2(radius*2, radius*2);
			Rectangle hitRectangle = new Rectangle(VectorUtil.Flatten(hitPoint), rectangleSize);

			foreach (MapChunk chunk in GetOverlappingChunks(hitRectangle)) {
				chunk.EditElevation(hitPoint, radius, magnitude);
			}
		}

		public IEnumerable<MapChunk> GetOverlappingChunks (Rectangle worldSpaceRectangle) {
			foreach (MapChunk chunk in chunks) {
				if (chunk.worldRectangle.Overlaps(worldSpaceRectangle)) {
					yield return chunk;
				}
			}
		}
		public IEnumerable<MapChunk> GetOverlappingChunks (Polygon worldSpacePolygon) {
			foreach (MapChunk chunk in chunks) {
				if (chunk.worldRectangle.Overlaps(worldSpacePolygon)) {
					yield return chunk;
				}
			}
		}

		public void RegisterStaticMapElement (AStaticMapElement element) {
			element.OnShapeChanged += HandleStaticElementShapeChanged;
			element.OnDestruction += HandleStaticElementDestroyed;
			allStaticMapElements.Add(element);
			if (element is ForestSection forestSection) {
				allForests.Add(forestSection);
			}
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
			allStaticMapElements.Remove(element);
			if (element is ForestSection forestSection) {
				allForests.Remove(forestSection);
			}

		}

		public GameMapData CreateMapData () {
			List<ForestSection> forestSections = new List<ForestSection>();
			foreach (AStaticMapElement staticMapElement in allStaticMapElements) {
				if (staticMapElement is ForestSection f) {
					forestSections.Add(f);
				}
			}
			return new GameMapData(
				chunksPerSide,
				chunkResolution,
				mapSize,
				chunks,
				forestSections
			);
		}

		[Serializable]
		public class GameMapData {
			public int chunksPerSide;
			public int chunkResolution;
			public float mapSize;
			public List<MapChunk.MapChunkData> chunkData;
			public List<ForestSection> forests;

			public GameMapData (
				int chunksPerSide,
				int chunkResolution,
				float mapSize,
				List<MapChunk> chunks,
				List<ForestSection> forests
			) {
				this.chunksPerSide = chunksPerSide;
				this.chunkResolution = chunkResolution;
				this.mapSize = mapSize;
				chunkData = chunks.Select(c => c.CreateChunkData()).ToList();
				this.forests = forests;
			}
		}
	}
}
