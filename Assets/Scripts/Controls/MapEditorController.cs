using System;
using System.Collections.Generic;
using Controls.MapEditorTools;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
namespace Controls {
	public class MapEditorController : RtsController {
		[Title("Map Editor Controller")]
		[SceneObjectsOnly] 
		[SerializeField] private Map.Map map;
		[Title("Debug")]
		[ReadOnly] [ShowInInspector] private MapEditorTool currentTool;
		
		public void SelectTool (MapEditorTool tool) {
			currentTool = tool;
			tool.OnToolFinished += HandleToolFinished;
			tool.Activate();
		}
		
		private void HandleToolFinished (MapEditorTool tool) {
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
