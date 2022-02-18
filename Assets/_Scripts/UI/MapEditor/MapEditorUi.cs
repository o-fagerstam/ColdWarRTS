using Controls;
using Controls.MapEditorTools;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
namespace UI {
	public class MapEditorUi : MonoBehaviour {
		[ReadOnly][ShowInInspector] private TooltipText _tooltip;
		private void Start () {
			_tooltip = GetComponentInChildren<TooltipText>();
			foreach (ToolSelectButton toolSelectButton in GetComponentsInChildren<ToolSelectButton>()) {
				toolSelectButton.OnToolSelected += HandleToolSelected;
			}
		}

		private void HandleToolSelected (AMapEditorTool tool) {
			_tooltip.Subscribe(tool);
			tool.OnToolFinished += HandleToolFinished;
			SingletonManager.Retrieve<MapEditorController>().SelectTool(tool);
		}
		private void HandleToolFinished (AMapEditorTool tool) {
			_tooltip.Unsubscribe(tool);
			tool.OnToolFinished -= HandleToolFinished;
		}
	}
}
