using System;
using Controls.MapEditorTools;
using TMPro;
using UnityEngine;
namespace UI {
	public class TooltipText : MonoBehaviour {
		private TMP_Text text;

		private void Start () {
			text = GetComponent<TMP_Text>();
		}

		public void Subscribe (MapEditorTool tool) {
			tool.OnTooltipUpdate += HandleTooltipUpdate;
		}

		public void Unsubscribe (MapEditorTool tool) {
			tool.OnTooltipUpdate -= HandleTooltipUpdate;
			text.text = "";
		}

		public void HandleTooltipUpdate (string newText) {
			text.text = newText;
		}
	}
}
