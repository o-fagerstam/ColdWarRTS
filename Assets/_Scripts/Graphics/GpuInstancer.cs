using System.Collections.Generic;
using UnityEngine;
namespace Graphics {
	public class GpuInstancer {
		private int _numInstances;
		private readonly Mesh _mesh;
		private readonly Material[] _materials;
		private List<List<Matrix4x4>> _batches = new List<List<Matrix4x4>>();

		public GpuInstancer (GameObject prefab) {
			_mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
			_materials = new []{prefab.GetComponent<MeshRenderer>().sharedMaterial};
		}

		public void SetInstances (IEnumerable<GpuInstance> instances) {
			int batchCount = 0;
			_numInstances = 0;
			_batches.Clear();
			_batches.Add(new List<Matrix4x4>());
			foreach (GpuInstance instance in instances) {
				if (!instance.enabled) { continue; }
				if (batchCount == 1000) {
					_batches.Add(new List<Matrix4x4>());
					batchCount = 0;
				}
				_batches[_batches.Count-1].Add(instance.ToMatrix());
				batchCount++;
				_numInstances++;
			}
		}
		
		public void RenderBatches () {
			foreach (List<Matrix4x4> batch in _batches) {
				for (int i = 0; i < _mesh.subMeshCount; i++) {
					UnityEngine.Graphics.DrawMeshInstanced(_mesh, i, _materials[i], batch);
				}
			}
		}
	}
}
