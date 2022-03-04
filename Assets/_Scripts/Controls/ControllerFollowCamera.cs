using System.Linq;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Controls {
	public class ControllerFollowCamera : MonoBehaviour {
		[SerializeField, AssetsOnly, Required] private RtsControllerRuntimeSet rtsControllerRuntimeSet;
		private void OnEnable () {
			if (rtsControllerRuntimeSet.Count == 0) {
				rtsControllerRuntimeSet.OnElementAdded += HandleControllerAdded;
			} else {
				GetComponent<CinemachineVirtualCamera>().Follow = rtsControllerRuntimeSet.First().transform;
			}
		}
		private void HandleControllerAdded (ARtsController controller) {
			GetComponent<CinemachineVirtualCamera>().Follow = controller.transform;
			rtsControllerRuntimeSet.OnElementAdded -= HandleControllerAdded;
		}

		private void OnDisable () {
			rtsControllerRuntimeSet.OnElementAdded -= HandleControllerAdded;
		}
	}
}
