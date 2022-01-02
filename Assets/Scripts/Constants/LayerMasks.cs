using UnityEngine;
namespace Constants {
	public static class LayerMasks {
		public static readonly LayerMask anythingClickable = LayerMask.GetMask("Ground", "MouseDraggable");
		public static readonly LayerMask ground = LayerMask.GetMask("Ground");
		public static readonly LayerMask mouseDraggable = LayerMask.GetMask("MouseDraggable");
	}
}
