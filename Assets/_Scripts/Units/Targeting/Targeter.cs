using Mirror;
using UnityEngine;
namespace Units.Targeting {
	public class Targeter : NetworkBehaviour {
		[SerializeField]
		private Targetable _target;
		public Targetable Target => _target;


		#region Server
		[Server]
		public void SetTarget (Targetable targetable) {
			_target = targetable;
		}

		[Server]
		public void ClearTarget () {
			_target = null;
		}
		#endregion
	}
}
