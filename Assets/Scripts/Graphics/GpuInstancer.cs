using System.Collections.Generic;
using UnityEngine;
namespace Graphics {
	public class GpuInstancer {
		private int numInstances;
		private readonly Mesh mesh;
		private readonly Material[] materials;
		private List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

		public GpuInstancer (GameObject prefab) {
			mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
			materials = new []{prefab.GetComponent<MeshRenderer>().sharedMaterial};
		}

		public void SetInstances (IEnumerable<GpuInstance> instances) {
			int batchCount = 0;
			numInstances = 0;
			batches.Clear();
			batches.Add(new List<Matrix4x4>());
			foreach (GpuInstance instance in instances) {
				if (batchCount == 1000) {
					batches.Add(new List<Matrix4x4>());
					batchCount = 0;
				}
				batches[batches.Count-1].Add(instance.ToMatrix());
				batchCount++;
				numInstances++;
			}
		}
		
		public void RenderBatches () {
			foreach (List<Matrix4x4> batch in batches) {
				for (int i = 0; i < mesh.subMeshCount; i++) {
					UnityEngine.Graphics.DrawMeshInstanced(mesh, i, materials[i], batch);
				}
			}
		}
	}
}
