using System.Collections.Generic;
using Math;
using Singleton;
using UnityEngine;
using Utils;
namespace Map {
	public class GameMap : ASingleton {
		[SerializeField] private List<MapChunk> chunks = new List<MapChunk>();
		[SerializeField] private MapChunk mapChunkPrefab;

		protected override void Start () {
			base.Start();
			GenerateFlatMap(20, 20, 100f);
		}

		private void GenerateFlatMap (int chunksPerSide, int chunkResolution, float mapSize) {
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

		public void RegisterStaticMapElement (AStaticMapElement element) {
			foreach (MapChunk mapChunk in chunks) {
				if (element.Overlaps(mapChunk.worldRectangle)) {
					mapChunk.AddStaticMapElement(element);
				} else {
					mapChunk.TryRemoveStaticMapElement(element);
				}
			}
		}

		private void ClearMap () {
			foreach (MapChunk chunk in chunks) {
				SafeDestroyUtil.SafeDestroyGameObject(chunk);
			}
			chunks.Clear();
			foreach (AStaticMapElement staticMapElement in FindObjectsOfType<AStaticMapElement>(true)) {
				SafeDestroyUtil.SafeDestroyGameObject(staticMapElement);
			}
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
	}
}
