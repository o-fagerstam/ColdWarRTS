using Map;
using UnityEngine;
namespace Controls.MapEditorTools {
	public class EditElevationTool : AMapEditorTool {
		private const string TOOLTIP_BASE = "Left click: Raise, Right click: Lower, Scroll: Change brush size, Space: Exit";
		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
		}

		public override void OnLeftClickGround (RaycastHit hit) {
			base.OnLeftClickGround(hit);
			EditMapElevation(hit, 2f);
		}

		public override void OnRightClickGround (RaycastHit hit) {
			base.OnRightClickGround(hit);
			EditMapElevation(hit, -2f);
		}

		public void EditMapElevation (RaycastHit hit, float magnitude) {
			MapChunk chunk = hit.transform.GetComponent<MapChunk>();
			GameMap map = chunk.Map;
			map.EditElevation(hit.point, 5f, magnitude);
		}
	}
}
