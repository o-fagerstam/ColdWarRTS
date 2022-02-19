using Cinemachine;
using Singleton;
using UnityEngine;
namespace Controls {
	public class ControllerFollowCamera : MonoBehaviour {
		private void OnEnable () {
			if (SingletonManager.TryRetrieveAnySubclass(out ARtsController playerController)) {
				GetComponent<CinemachineVirtualCamera>().Follow = playerController.transform;
			} else {
				SingletonManager.OnSingletonRegistered += HandleOnSingletonRegistered;
			}
		}
		private void HandleOnSingletonRegistered (object sender, SingletonManager.OnSingletonRegisteredArgs e) {
			if (e.RegisteredType.IsSubclassOf(typeof(ARtsController))) {
				SingletonManager.TryRetrieveAnySubclass(out ARtsController playerController);
				GetComponent<CinemachineVirtualCamera>().Follow = playerController.transform;
				SingletonManager.OnSingletonRegistered -= HandleOnSingletonRegistered;
			}
		}

		private void OnDisable () {
			SingletonManager.OnSingletonRegistered -= HandleOnSingletonRegistered;
		}
	}
}
