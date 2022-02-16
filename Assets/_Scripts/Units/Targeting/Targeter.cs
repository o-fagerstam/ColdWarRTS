using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Units.Targeting {
	public class Targeter : NetworkBehaviour {
		[SerializeField]
		private Targetable _target;
		public Targetable Target => _target;


		#region Server
		[Command]
		public void CmdSetTarget (GameObject targetGameObject) {
			if (!targetGameObject.TryGetComponent(out Targetable targetable)) {
				UnityEngine.Debug.LogWarning($"{targetGameObject} lacks {nameof(Targetable)} component.", this);
				return;
			}
			_target = targetable;
		}

		[Server]
		public void ClearTarget () {
			_target = null;
		}
		#endregion
	}
}
