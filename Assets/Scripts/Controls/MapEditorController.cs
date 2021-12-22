using System;
using Map;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
namespace Controls {
	public class MapEditorController : RtsController {
		[Title("Map Editor Controller")]
		[SceneObjectsOnly] 
		[SerializeField] private Map.Map map;
		[AssetsOnly] 
		[SerializeField] private ForestSection forestSectionPrefab;
		[Title("Debug")]
		[ReadOnly] 
		[ShowInInspector] private ForestSection currentForestSection;

		public event Action OnEnterForestDrawMode;
		public event Action OnExitForestDrawMode;
		
		protected override void OnLeftButtonPressed (Vector3 point) {
			if (currentForestSection == null) {
				OnEnterForestDrawMode?.Invoke();
				currentForestSection = Instantiate(forestSectionPrefab, point, Quaternion.identity);
			}

			currentForestSection.AddPoint(point);
		}

		protected override void OnSpacePressed () {
			if (currentForestSection != null) {
				Debug.Log("Attempting close");
				if (!currentForestSection.Close()) {
					return;
				}
				Debug.Log("Closed");
				OnExitForestDrawMode?.Invoke();
				currentForestSection = null;
			}
		}
	}
}
