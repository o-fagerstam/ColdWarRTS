using Controls.MapEditorTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls {
	public class MapEditorController : ARtsController {
		[ReadOnly][ShowInInspector] private AMapEditorTool _currentTool;
		
		public void SelectTool (AMapEditorTool tool) {
			_currentTool = tool;
			tool.Activate();
		}
		
		protected override void UpdateKeyboardControl () {
			base.UpdateKeyboardControl();
			if (_currentTool != null) {
				if (Keyboard.current.escapeKey.wasPressedThisFrame) {
					ClearTool();
				} else {
					_currentTool.UpdateKeyboard();
				}
			}
		}

		protected override void UpdateMouseControl () {
			Camera mainCamera = Camera.main;
			Ray mouseRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (_currentTool != null) {
				_currentTool.UpdateMouse(mouseRay);
			}
		}

		private void ClearTool () {
			_currentTool.Deactivate();
			_currentTool = null;
		}
	}
}
