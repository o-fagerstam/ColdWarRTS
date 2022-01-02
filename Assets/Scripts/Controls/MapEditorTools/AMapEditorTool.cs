using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls.MapEditorTools {
	public abstract class AMapEditorTool : MonoBehaviour {

		public event Action<AMapEditorTool> OnToolFinished;
		public event Action<string> OnTooltipUpdate;

		public virtual void Deactivate () { OnToolFinished?.Invoke(this);}
		protected void UpdateTooltip(string message) { OnTooltipUpdate?.Invoke(message);}
		public abstract void Activate ();
		
		public abstract void UpdateKeyboard ();
		public abstract void UpdateMouse (Ray mouseRay);
	}
}
