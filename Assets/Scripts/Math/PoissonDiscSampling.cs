using System.Collections.Generic;
using UnityEngine;
namespace Math {
	public static class PoissonDiscSampling {

		public static List<Vector2> GeneratePointsFromPolygon (float radius, Polygon polygon) {
			// Get square poisson sample
			List<Vector2> pointsInBoundingBox = GeneratePoints(radius, polygon.BoundingBoxSize);
			// Remove points outside polygon
			List<Vector2> pointsInPolygon = new List<Vector2>();
			Vector2 polyOriginRelativeToBoundingBoxOrigin = polygon.PolyOriginRelativeToBoundingBoxOrigin;
			foreach (Vector2 boundingBoxPoint in pointsInBoundingBox) {
				Vector2 polygonPoint = boundingBoxPoint + polyOriginRelativeToBoundingBoxOrigin;
				if (polygon.PointInPolygon(polygonPoint)) { pointsInPolygon.Add(polygonPoint); }
			}
			return pointsInPolygon;
		}
		
		public static List<Vector2> GeneratePoints (float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30) {
			float cellSize = radius/Mathf.Sqrt(2);

			// Array with index of the point that lies in the corresponding cell
			// 0 = no point, 1 = point with an index of 0, 2 = index 1, and so on...
			int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x/cellSize), Mathf.CeilToInt(sampleRegionSize.y/cellSize)];
			List<Vector2> points = new List<Vector2>();
			List<Vector2> spawnPoints = new List<Vector2>();

			spawnPoints.Add(sampleRegionSize/2);
			while (spawnPoints.Count > 0) {
				int spawnIndex = Random.Range(0, spawnPoints.Count);
				Vector2 spawnCenter = spawnPoints[spawnIndex];

				bool candidateAccepted = false;
				for (int i = 0; i < numSamplesBeforeRejection; i++) {
					float angle = Random.value*Mathf.PI * 2;
					Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					Vector2 candidate = spawnCenter + dir*Random.Range(radius, radius*2);
					if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) {
						points.Add(candidate);
						spawnPoints.Add(candidate);
						grid[(int)(candidate.x/cellSize), (int)(candidate.y/cellSize)] = points.Count;
						candidateAccepted = true;
						break;
					}
				}
				if (!candidateAccepted) {
					spawnPoints.RemoveAt(spawnIndex);
				}
			}
			
			return points;
		}
		private static bool IsValid (Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
			if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y) {
				int cellX = (int)(candidate.x/cellSize);
				int cellY = (int)(candidate.y/cellSize);
				int searchStartX = Mathf.Max(0, cellX - 2);
				int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);	
				int searchStartY = Mathf.Max(0, cellY - 2);
				int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

				for (int x = searchStartX; x <= searchEndX; x++) {
					for (int y = searchStartY; y <= searchEndY; y++) {
						int pointIndex = grid[x, y] - 1;
						if (pointIndex != -1) {
							float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
							if (sqrDst < radius * radius) {
								return false;
							}
						}
					}
				}
				return true;
			}
			return false;
		}
	}
}
