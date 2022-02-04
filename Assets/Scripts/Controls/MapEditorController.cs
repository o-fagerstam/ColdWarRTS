using System;
using System.Collections.Generic;
using Constants;
using Controls.MapEditorTools;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls {
	public class MapEditorController : ARtsController {
		[ReadOnly][ShowInInspector] private AMapEditorTool currentTool;
		
		public void SelectTool (AMapEditorTool tool) {
			currentTool = tool;
			tool.Activate();
		}
		
		protected override void UpdateKeyboardControl () {
			base.UpdateKeyboardControl();
			if (currentTool != null) {
				if (Keyboard.current.escapeKey.wasPressedThisFrame) {
					ClearTool();
				} else {
					currentTool.UpdateKeyboard();
				}
			}
		}

		protected override void UpdateMouseControl () {
			Camera mainCamera = Camera.main;
			Ray mouseRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (currentTool != null) {
				currentTool.UpdateMouse(mouseRay);
			}
		}

		private void ClearTool () {
			currentTool.Deactivate();
			currentTool = null;
		}
	}
}
