using Sirenix.OdinInspector;
using UnityEngine;
namespace Controls.MapEditorTools {
	public class EditElevationTool : MapEditorTool {
		private const string TOOLTIP_BASE = "Left click: Raise, Right click: Lower, Scroll: Change brush size, Space: Exit";
		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
		}
	}
}
