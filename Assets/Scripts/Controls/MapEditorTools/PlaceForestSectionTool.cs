using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
namespace Controls.MapEditorTools {
	public class PlaceForestSectionTool : AMapEditorTool {
		
		[AssetsOnly] [Required]
		[SerializeField] private ForestSection forestSectionPrefab;
		[ShowInInspector] [ReadOnly] private ForestSection currentForestSection;
		private const string TOOLTIP_BASE = "Left click on the ground to start placing forest";
		private const string TOOLTIP_DURING_PLACEMENT = "Left click: Place vertex, Space: Close section, Right click: Stop editing";

		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
		}
		public override void OnLeftClickGround (RaycastHit hit) {
			base.OnLeftClickGround(hit);
			Vector3 point = hit.point;
			if (currentForestSection == null) {
				currentForestSection = Instantiate(
					forestSectionPrefab, 
					point, 
					Quaternion.identity, 
					SingletonManager.Retrieve<GameMap>().transform);
				UpdateTooltip(TOOLTIP_DURING_PLACEMENT);
			}

			currentForestSection.AddPoint(point);
		}

		public override void OnRightClickGround (RaycastHit hit) {
			base.OnRightClickGround(hit);
			if (currentForestSection != null) {
				SafeDestroyUtil.SafeDestroyGameObject(currentForestSection);
				ToolFinished();
			}
		}

		public override void OnSpacePressed () {
			base.OnSpacePressed();
			if (currentForestSection != null) {
				if (!currentForestSection.Close()) {
					return;
				}
				UpdateTooltip(TOOLTIP_BASE);
				currentForestSection = null;
			}
		}
	}
}
