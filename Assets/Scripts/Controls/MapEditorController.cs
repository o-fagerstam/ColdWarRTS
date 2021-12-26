using System;
using System.Collections.Generic;
using Controls.MapEditorTools;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
namespace Controls {
	public class MapEditorController : ARtsController {
		[Title("Map Editor Controller")]
		[SceneObjectsOnly] 
		[SerializeField] private Map.GameMap map;
		[Title("Debug")]
		[ReadOnly] [ShowInInspector] private AMapEditorTool currentTool;
		
		public void SelectTool (AMapEditorTool tool) {
			currentTool = tool;
			tool.OnToolFinished += HandleToolFinished;
			tool.Activate();
		}
		
		private void HandleToolFinished (AMapEditorTool tool) {
			ClearTool();
			tool.OnToolFinished -= HandleToolFinished;
		}
		
		public void ClearTool () {
			currentTool = null;
		}

		protected override void OnLeftClickGround (RaycastHit hit) {
			if (currentTool != null) { currentTool.OnLeftClickGround(hit);  }
		}

		protected override void OnRightClickGround (RaycastHit hit) {
			if (currentTool != null) { currentTool.OnRightClickGround(hit);}
		}

		protected override void OnSpacePressed () {
			if (currentTool != null) { currentTool.OnSpacePressed(); }
		}
	}
}
