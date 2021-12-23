using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	public class GameMap : MonoBehaviour {
		[SerializeField] private List<MapChunk> chunks = new List<MapChunk>();
		[SerializeField] private MapChunk mapChunkPrefab;

		[Button]
		public void GenerateFlatMap (int chunksPerSide, int chunkResolution, float mapSize) {
			ClearMap();

			float chunkSize = mapSize/chunksPerSide;
			Vector3 chunkBaseWorldOffset = transform.position + new Vector3(-(mapSize/2f), 0f, -(mapSize/2f));
			for (int y = 0; y < chunksPerSide; y++) {
				for (int x = 0; x < chunksPerSide; x++) {
					Vector3 newChunkPosition = new Vector3(x * chunkSize, 0f, y*chunkSize) + chunkBaseWorldOffset;
					MapChunk newChunk = Instantiate(mapChunkPrefab, newChunkPosition, Quaternion.identity, transform);
					newChunk.GenerateFlatChunk(chunkResolution, chunkSize);
					chunks.Add(newChunk);
				}
			}
		}

		private void ClearMap () {
			foreach (MapChunk chunk in chunks) {
				SafeDestroyUtil.SafeDestroyGameObject(chunk);
			}
			chunks.Clear();
		}
		public void EditElevation (Vector3 hitPoint, float radius, float magnitude) {
			Vector2 rectSize = new Vector2(radius * 2, radius * 2);
			Rect hitRect = new Rect(VectorUtil.Flatten(hitPoint) - rectSize/2, rectSize);

			foreach (MapChunk chunk in chunks) {
				if (chunk.worldRect.Overlaps(hitRect)) {
					chunk.EditElevation(hitPoint, radius, magnitude);
				}
			}
			
		}
	}
}
