using Controls.MapEditorTools;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls {
	public class MapEditorController : ARtsController {
		[SerializeField, AssetsOnly, Required] private MapEditorControllerRuntimeSet mapEditorControllerRuntimeSet;
		[SerializeField, Required, AssetsOnly] private GameMapEvent onMapReload;
		[ReadOnly][ShowInInspector] private AMapEditorTool _currentTool;
		

		protected override void OnEnable () {
			base.OnEnable();
			SelectTool(null);
			onMapReload.Event += HandleMapReload;
			mapEditorControllerRuntimeSet.Add(this);
		}

		protected override void OnDisable () {
			base.OnDisable();
			SelectTool(null);
			onMapReload.Event -= HandleMapReload;
			mapEditorControllerRuntimeSet.Remove(this);
		}


		public void SelectTool (AMapEditorTool tool) {
			ClearTool();
			_currentTool = tool;
			if (_currentTool != null) {
				_currentTool.Activate();
			}
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
			if (_currentTool != null) {
				_currentTool.Deactivate();
			}
			_currentTool = null;
		}
		
		private void HandleMapReload (object sender, GameMapEventArgs e) {
			ClearTool();
		}
	}
}
