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
	public class GameMap : MonoBehaviour, ISingleton {
		[SerializeField] private int chunksPerSide;
		[SerializeField] private int chunkResolution;
		[SerializeField] private float mapSize;

		[SerializeField] private List<MapChunk> chunks = new List<MapChunk>();
		[SerializeField] private MapChunk mapChunkPrefab;
		[SerializeField] private ForestSection forestSectionPrefab;
		[ShowInInspector][ReadOnly] private List<AStaticMapElement> allStaticMapElements = new List<AStaticMapElement>();
		public IEnumerable<ForestSection> AllForests => allStaticMapElements.OfType<ForestSection>();

		public void GenerateFlatMap () {
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

		public void GenerateFromMapData (GameMapSaveData saveData) {
			ClearMap();

			chunksPerSide = saveData.chunksPerSide;
			chunkResolution = saveData.chunkResolution;
			mapSize = saveData.mapSize;

			float chunkSize = mapSize/chunksPerSide;
			Vector3 chunkBaseWorldOffset = transform.position + new Vector3(-(mapSize/2f), 0f, -(mapSize/2f));
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
				RegisterStaticMapElement(newForestSection);
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
		}

		public GameMapSaveData CreateSaveData () {
			List<ForestSection> forestSections = new List<ForestSection>();
			foreach (AStaticMapElement staticMapElement in allStaticMapElements) {
				if (staticMapElement is ForestSection f) {
					forestSections.Add(f);
				}
			}
			return new GameMapSaveData(
				chunksPerSide,
				chunkResolution,
				mapSize,
				chunks.Select(c => c.CreateChunkData()).ToList(),
				forestSections.Select(f => f.CreateSaveData()).ToList()
			);
		}

		[Serializable]
		public class GameMapSaveData {
			public int chunksPerSide;
			public int chunkResolution;
			public float mapSize;
			public List<MapChunk.MapChunkSaveData> chunkData;
			public List<ForestSection.ForestSectionData> forestSectionData;

			public GameMapSaveData (
				int chunksPerSide,
				int chunkResolution,
				float mapSize,
				List<MapChunk.MapChunkSaveData> chunkData,
				List<ForestSection.ForestSectionData> forestSectionData
			) {
				this.chunksPerSide = chunksPerSide;
				this.chunkResolution = chunkResolution;
				this.mapSize = mapSize;
				this.chunkData = chunkData;
				this.forestSectionData = forestSectionData;
			}
		}
	}
}
