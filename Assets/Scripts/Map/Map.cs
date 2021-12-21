using System;
using System.Collections.Generic;
using UnityEngine;
namespace Map {
	public class Map : MonoBehaviour {
		private List<MapChunk> chunks = new List<MapChunk>();
		[SerializeField] private MapChunk mapChunkPrefab;

		private void Start () {
			GenerateFlatMap(10, 250, 100f);
		}

		public void GenerateFlatMap (int chunksPerSide, int chunkResolution, float mapSize) {
			ClearMap();

			float chunkSize = mapSize/chunksPerSide;
			Vector3 chunkBaseWorldOffset = transform.position + new Vector3(-(mapSize/2f), 0f, -(mapSize/2f));
			for (int y = 0; y < chunksPerSide; y++) {
				for (int x = 0; x < chunksPerSide; x++) {
					Vector3 newChunkPosition = new Vector3(x * chunkSize, 0f, y*chunkSize) + chunkBaseWorldOffset;
					MapChunk newChunk = Instantiate(mapChunkPrefab, newChunkPosition, Quaternion.identity);
					newChunk.GenerateFlatChunk(chunkResolution, chunkSize);
					chunks.Add(newChunk);
				}
			}
		}

		private void ClearMap () {
			foreach (MapChunk chunk in chunks) {
				Destroy(chunk);
			}
			chunks.Clear();
		}
	}
}
