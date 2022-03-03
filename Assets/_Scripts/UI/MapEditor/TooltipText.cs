using Controls.MapEditorTools;
using TMPro;
using UnityEngine;
namespace UI.MapEditor {
	public class TooltipText : MonoBehaviour {
		private TMP_Text _text;

		private void Start () {
			_text = GetComponent<TMP_Text>();
		}

		public void Subscribe (AMapEditorTool tool) {
			tool.OnTooltipUpdate += HandleTooltipUpdate;
		}

		public void Unsubscribe (AMapEditorTool tool) {
			tool.OnTooltipUpdate -= HandleTooltipUpdate;
			_text.text = "";
		}

		public void HandleTooltipUpdate (string newText) {
			_text.text = newText;
		}
	}
}
