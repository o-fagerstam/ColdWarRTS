using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Units.Targeting {
	public class Targetable : NetworkBehaviour {
		[SerializeField] [Required] [ChildGameObjectsOnly] private Transform aimAtPoint;
		public Transform AimAtPoint => aimAtPoint;
	}
}
