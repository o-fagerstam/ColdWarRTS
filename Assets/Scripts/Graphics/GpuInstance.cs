using System;
using UnityEngine;
namespace Graphics {
	[Serializable]
	public struct GpuInstance {
		public Vector3 position;
		public Vector3 scale;
		public Quaternion rotation;
		public bool enabled;

		public GpuInstance (Vector3 position, Vector3 scale, Quaternion rotation, bool enabled = true) {
			this.position = position;
			this.scale = scale;
			this.rotation = rotation;
			this.enabled = enabled;
		}
		public Matrix4x4 ToMatrix () => Matrix4x4.TRS(position, rotation, scale);
	}
}
