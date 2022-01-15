using UnityEngine;
namespace Constants {
	public static class LayerMasks {
		public static readonly LayerMask anythingClickable = LayerMask.GetMask("Ground", "Road", "MouseDraggable", "Water");
		public static readonly LayerMask bareGround = LayerMask.GetMask("Ground");
		public static readonly LayerMask road = LayerMask.GetMask("Road");
		public static readonly LayerMask anyLand = LayerMask.GetMask("Ground", "Road");
		public static readonly LayerMask nonRoadSurface = LayerMask.GetMask("Ground", "Water");
		public static readonly LayerMask anySurface = LayerMask.GetMask("Ground", "Road", "Water");
		public static readonly LayerMask mouseDraggable = LayerMask.GetMask("MouseDraggable");
	}
}
