using Controls;
using Controls.MapEditorTools;
using Sirenix.OdinInspector;
using UnityEngine;
namespace UI {
	public class MapEditorUi : MonoBehaviour {
		[Required][SceneObjectsOnly][SerializeField]
		private MapEditorController mapEditorController;
		[ReadOnly][ShowInInspector] private TooltipText tooltip;
		private void Start () {
			tooltip = GetComponentInChildren<TooltipText>();
			foreach (ToolSelectButton toolSelectButton in GetComponentsInChildren<ToolSelectButton>()) {
				toolSelectButton.OnToolSelected += HandleToolSelected;
			}
		}

		private void HandleToolSelected (AMapEditorTool tool) {
			tooltip.Subscribe(tool);
			tool.OnToolFinished += HandleToolFinished;
			mapEditorController.SelectTool(tool);
		}
		private void HandleToolFinished (AMapEditorTool tool) {
			tooltip.Unsubscribe(tool);
			tool.OnToolFinished -= HandleToolFinished;
		}
	}
}
