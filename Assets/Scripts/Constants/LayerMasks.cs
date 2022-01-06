using UnityEngine;
namespace Constants {
	public static class LayerMasks {
		public static readonly LayerMask anythingClickable = LayerMask.GetMask("Ground", "Road", "MouseDraggable");
		public static readonly LayerMask bareGround = LayerMask.GetMask("Ground");
		public static readonly LayerMask road = LayerMask.GetMask("Road");
		public static readonly LayerMask anyLand = LayerMask.GetMask("Ground", "Road");
		public static readonly LayerMask mouseDraggable = LayerMask.GetMask("MouseDraggable");
	}
}
