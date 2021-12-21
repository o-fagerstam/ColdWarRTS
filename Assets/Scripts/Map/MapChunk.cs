using System;
using UnityEngine;
namespace Map {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class MapChunk : MonoBehaviour {
		private MeshFilter meshFilter;
		private int pointsPerSide = 251;
		private float chunkSideDimension = 10f;
		private Vector3[] vertices;
		private int[] triangles;
		private Vector2[] uvs;
		private void Start () {
			meshFilter = GetComponent<MeshFilter>();
		}

		public void GenerateFlatChunk (int squaresPerSide, float chunkSize) {
			pointsPerSide = squaresPerSide + 1;
			chunkSideDimension = chunkSize;
			
			vertices = new Vector3[pointsPerSide*pointsPerSide];
			triangles = new int[(pointsPerSide-1) * (pointsPerSide-1) * 6];
			uvs = new Vector2[pointsPerSide*pointsPerSide];

			float startSideOffset = -(chunkSideDimension/2);
			float dstBetweenPoints = chunkSideDimension/(pointsPerSide - 1);

			for (int y = 0; y < pointsPerSide; y++) {
				for (int x = 0; x < pointsPerSide; x++) {
					int vertIndex = x + y*pointsPerSide;
					vertices[vertIndex] = new Vector3(x*dstBetweenPoints + startSideOffset, 0f, y*dstBetweenPoints + startSideOffset);
					uvs[vertIndex] = new Vector2((float)y/pointsPerSide, (float)y/pointsPerSide);
				}
			}

			for (int y = 0, triangleIndex = 0; y < pointsPerSide-1; y++) {
				for (int x = 0; x < pointsPerSide-1; x++) {
					int a = x + y*pointsPerSide;
					int b = x + 1 + y*pointsPerSide;
					int c = x + (y + 1)*pointsPerSide;
					int d = x + 1 + (y + 1)*pointsPerSide;

					triangles[triangleIndex++] = a;
					triangles[triangleIndex++] = c;
					triangles[triangleIndex++] = d;
					
					triangles[triangleIndex++] = a;
					triangles[triangleIndex++] = d;
					triangles[triangleIndex++] = b;
				}
			}

			Mesh mesh = new Mesh {
				name = "ChunkMesh"
			};

			mesh.vertices = vertices;
			mesh.triangles = triangles;

			meshFilter.mesh = mesh;

		}
	}
}
