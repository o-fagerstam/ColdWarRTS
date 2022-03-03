using Controls;
using Controls.MapEditorTools;
using Sirenix.OdinInspector;
using UnityEngine;
namespace UI.MapEditor {
	public class MapEditorUi : MonoBehaviour {
		[SerializeField, AssetsOnly, Required] private MapEditorControllerRuntimeSet mapEditorControllerRuntimeSet;
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
			foreach (MapEditorController mapEditorController in mapEditorControllerRuntimeSet) {
				mapEditorController.SelectTool(tool);
			}
		}
		private void HandleToolFinished (AMapEditorTool tool) {
			_tooltip.Unsubscribe(tool);
			tool.OnToolFinished -= HandleToolFinished;
		}

		private void OnDestroy () {
			foreach (ToolSelectButton toolSelectButton in GetComponentsInChildren<ToolSelectButton>()) {
				toolSelectButton.OnToolSelected -= HandleToolSelected;
			}
		}
	}
}
