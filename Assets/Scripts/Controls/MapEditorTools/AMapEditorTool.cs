using System;
using UnityEngine;
namespace Controls.MapEditorTools {
	public abstract class AMapEditorTool : MonoBehaviour {

		public event Action<AMapEditorTool> OnToolFinished;
		public event Action<string> OnTooltipUpdate;

		protected void ToolFinished () { OnToolFinished?.Invoke(this);}
		protected void UpdateTooltip(string message) { OnTooltipUpdate?.Invoke(message);}
		public abstract void Activate ();
		public virtual void OnLeftClickGround (RaycastHit hit) {}
		public virtual void OnRightClickGround (RaycastHit hit) {}
		public virtual void OnSpacePressed() {}
	}
}
