﻿using System;
using System.Collections.Generic;
using Math;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Map {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class MapChunk : MonoBehaviour {
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;
		[SerializeField][ReadOnly] private List<AStaticMapElement> staticMapElements;
		private int squaresPerSide;
		private float visibleChunkSideDimension ;
		[ShowInInspector] [ReadOnly] public GameMap Map { get; private set; }
		public Rectangle worldRectangle {
			get {
				Vector2 size = new Vector2(visibleChunkSideDimension, visibleChunkSideDimension);
				Vector2 pos2d = VectorUtil.Flatten(transform.position);
				return new Rectangle(pos2d, size);
			}
		}

		private void Start () {
			if (GetComponentUtil.TryGetComponentInParent(this, out GameMap m)) {
				Map = m;
			} else {
				throw new MissingComponentException($"Failed to find {nameof(GameMap)} in parent. Did you parent this chunk to a {nameof(GameMap)} on instancing?");
			}
		}
		public void GenerateFlatChunk (int squaresPerSide, float chunkSize) {
			this.squaresPerSide = squaresPerSide;
			int visiblePointsPerSide = CalculateVisiblePointsPerSide(squaresPerSide);
			int realPointsPerSide = CalculateRealPointsPerSide(squaresPerSide);

			visibleChunkSideDimension = chunkSize;
			float realChunkSideDimension = chunkSize*((float)realPointsPerSide/visiblePointsPerSide);

			Vector3[] vertices = new Vector3[realPointsPerSide*realPointsPerSide];
			Vector2[] uvs = new Vector2[realPointsPerSide*realPointsPerSide];

			float startSideOffset = -(realChunkSideDimension/2);
			float dstBetweenPoints = visibleChunkSideDimension/squaresPerSide;

			for (int y = 0; y < realPointsPerSide; y++) {
				for (int x = 0; x < realPointsPerSide; x++) {
					int vertIndex = x + y*realPointsPerSide;
					vertices[vertIndex] = new Vector3(x*dstBetweenPoints + startSideOffset, 0f, y*dstBetweenPoints + startSideOffset);
					uvs[vertIndex] = new Vector2(
						Mathf.Clamp01((x -.5f)/visiblePointsPerSide), 
						Mathf.Clamp01((y-.5f)/visiblePointsPerSide));
				}
			}
			RecalculateMesh(vertices, uvs);
		}

		private int CalculateVisiblePointsPerSide (int squaresPerSide) => squaresPerSide + 1;
		private int CalculateRealPointsPerSide (int squaresPerSide) => squaresPerSide + 3;

		private void RecalculateMesh (Vector3[] vertices, Vector2[] uvs) {
			int visiblePointsPerSide = CalculateVisiblePointsPerSide(squaresPerSide);
			int realPointsPerSide = CalculateRealPointsPerSide(squaresPerSide);
			int[] triangles = new int[(realPointsPerSide-1) * (realPointsPerSide-1) * 6];
			for (int y = 0, triangleIndex = 0; y < realPointsPerSide-1; y++) {
				for (int x = 0; x < realPointsPerSide-1; x++) {
					int a = x + y*realPointsPerSide;
					int b = x + 1 + y*realPointsPerSide;
					int c = x + (y + 1)*realPointsPerSide;
					int d = x + 1 + (y + 1)*realPointsPerSide;

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
			mesh.uv = uvs;
			
			meshFilter.sharedMesh = mesh;
			meshFilter.sharedMesh.RecalculateBounds();
			meshFilter.sharedMesh.RecalculateNormals();
			meshFilter.sharedMesh.RecalculateTangents();

			int[] visibleTriangles = new int[(visiblePointsPerSide - 1)*(visiblePointsPerSide - 1)*6];
			for (int y = 0, triangleIndex = 0; y < visiblePointsPerSide-1; y++) {
				for (int x = 0; x < visiblePointsPerSide-1; x++) {
					int a = x+1 + (y+1)*realPointsPerSide;
					int b = x + 2 + (y+1)*realPointsPerSide;
					int c = x + 1 + (y + 2)*realPointsPerSide;
					int d = x + 2 + (y + 2)*realPointsPerSide;

					visibleTriangles[triangleIndex++] = a;
					visibleTriangles[triangleIndex++] = c;
					visibleTriangles[triangleIndex++] = d;
					
					visibleTriangles[triangleIndex++] = a;
					visibleTriangles[triangleIndex++] = d;
					visibleTriangles[triangleIndex++] = b;
				}
			}

			Mesh newMesh = meshFilter.sharedMesh;
			newMesh.triangles = visibleTriangles;
			meshFilter.mesh = newMesh;
			meshCollider.sharedMesh = newMesh;
		}

		public void EditElevation (Vector3 hitPointWorld, float radius, float magnitude) {
			Vector3 hitPointLocal = VectorUtil.WorldPos2Local(hitPointWorld, transform.position);
			Vector3[] vertices = (Vector3[]) meshFilter.mesh.vertices.Clone();
			float sqrRadius = radius*radius;
			for (int i = 0; i < vertices.Length; i++ ) {
				float sqrDistance = (VectorUtil.Flatten(hitPointLocal) - VectorUtil.Flatten(vertices[i])).sqrMagnitude;
				if (sqrDistance > sqrRadius) {
					continue;
				}
				float distance = Mathf.Sqrt(sqrDistance);
				float centerPointCloseness = 1f - distance/radius;
				AnimationCurve curve = AnimationCurve.EaseInOut(0f,0f,1f,1f);
				vertices[i].y += magnitude*curve.Evaluate(centerPointCloseness);
			}

			RecalculateMesh(
				vertices,
				meshFilter.sharedMesh.uv
			);

			foreach (AStaticMapElement staticMapElement in staticMapElements) {
				staticMapElement.NotifyUpdateNeeded();
			}
		}

		public void AddStaticMapElement (AStaticMapElement element) {
			staticMapElements.Add(element);
			element.OnDestruction += HandleStaticMapElementDestroyed;
		}

		public void HandleStaticMapElementDestroyed (AStaticMapElement element) {
			element.OnDestruction -= HandleStaticMapElementDestroyed;
			staticMapElements.Remove(element);
		}
	}
}
