using Constants;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils;
namespace Controls.MapEditorTools {
	public class PlaceForestSectionTool : AMapEditorTool {
		
		[AssetsOnly] [Required]
		[SerializeField] private ForestSection forestSectionPrefab;
		[ShowInInspector] [ReadOnly] private ForestSection _currentForestSection;
		private const string TOOLTIP_BASE = "Left click on the ground to start placing forest";
		private const string TOOLTIP_DURING_PLACEMENT = "Left click: Place vertex, Space: Close section, Right click: Stop editing";

		public override void Activate () {
			UpdateTooltip(TOOLTIP_BASE);
		}
		public override void UpdateKeyboard () {
			if (Keyboard.current.spaceKey.wasPressedThisFrame) { OnSpacePressed();}
		}
		public override void UpdateMouse (Ray mouseRay) {
			if (!Mouse.current.leftButton.isPressed && !Mouse.current.rightButton.isPressed) {return;}
			if (EventSystem.current.IsPointerOverGameObject()) { return; }
			if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, LayerMasks.bareGround)) { return; }

			if (Mouse.current.leftButton.wasPressedThisFrame) {
				OnLeftClickGround(hit);
			}
			if (Mouse.current.rightButton.wasPressedThisFrame) {
				OnRightClickGround(hit);
			}
		}
		public void OnLeftClickGround (RaycastHit hit) {
			Vector3 point = hit.point;
			if (_currentForestSection == null) {
				_currentForestSection = Instantiate(
					forestSectionPrefab, 
					point, 
					Quaternion.identity, 
					SingletonManager.Retrieve<GameMap>().transform);
				UpdateTooltip(TOOLTIP_DURING_PLACEMENT);
			}

			_currentForestSection.AddPoint(point);
		}

		public void OnRightClickGround (RaycastHit hit) {
			if (_currentForestSection != null) {
				SafeDestroyUtil.SafeDestroyGameObject(_currentForestSection);
				_currentForestSection = null;
				UpdateTooltip(TOOLTIP_BASE);
			}
		}

		public void OnSpacePressed () {
			if (_currentForestSection != null) {
				if (!_currentForestSection.Close()) {
					return;
				}
				UpdateTooltip(TOOLTIP_BASE);
				_currentForestSection = null;
			}
		}
	}
}
