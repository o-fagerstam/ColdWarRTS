using System;
using Controls.MapEditorTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
namespace UI {
	public class ToolSelectButton : MonoBehaviour {
		[SerializeField] [Required] private MapEditorTool tool;
		[ShowInInspector] [ReadOnly] private Button button;
		public event Action<MapEditorTool> OnToolSelected;
		protected void Start () {
			button = GetComponent<Button>();
			button.onClick.AddListener(InvokeToolSelected);
		}

		protected void OnDestroy () {
			button.onClick.RemoveListener(InvokeToolSelected);
		}

		private void InvokeToolSelected () {
			OnToolSelected?.Invoke(tool);
		}
		
	}
}
