using System;
using Controls.MapEditorTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
namespace UI {
	public class ToolSelectButton : MonoBehaviour {
		[SerializeField] [Required] private AMapEditorTool tool;
		[ShowInInspector] [ReadOnly] private Button _button;
		public event Action<AMapEditorTool> OnToolSelected;
		protected void Start () {
			_button = GetComponent<Button>();
			_button.onClick.AddListener(InvokeToolSelected);
		}

		protected void OnDestroy () {
			_button.onClick.RemoveListener(InvokeToolSelected);
		}

		private void InvokeToolSelected () {
			OnToolSelected?.Invoke(tool);
		}
		
	}
}
