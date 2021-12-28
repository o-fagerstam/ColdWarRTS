using UnityEngine;
namespace Graphics {
	public struct GpuInstance {
		public Vector3 position { get; }
		public Vector3 scale { get; }
		public Quaternion rotation { get;}
		public bool enabled { get; }

		public GpuInstance (Vector3 position, Vector3 scale, Quaternion rotation, bool enabled = true) {
			this.position = position;
			this.scale = scale;
			this.rotation = rotation;
			this.enabled = enabled;
		}
		public Matrix4x4 ToMatrix () => Matrix4x4.TRS(position, rotation, scale);
	}
}
