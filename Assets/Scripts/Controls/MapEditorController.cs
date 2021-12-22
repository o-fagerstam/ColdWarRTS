using System;
using Map;
using Math;
using UnityEngine;
namespace Controls {
	public class MapEditorController : RtsController {
		private ForestSection currentForestSection;
		[SerializeField] private ForestSection forestSectionPrefab;
		[SerializeField] private Map.Map map;

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
				OnExitForestDrawMode?.Invoke();
			}
			currentForestSection = null;
		}
	}
}
