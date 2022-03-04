using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls {
	public abstract class ARtsController : MonoBehaviour {
		[SerializeField, AssetsOnly, Required] private RtsControllerRuntimeSet rtsControllerRuntimeSet;
		[SerializeField] private float scrollSpeed = 5f;

		protected virtual void OnEnable () {
			rtsControllerRuntimeSet.Add(this);
		}

		protected virtual void OnDisable () {
			rtsControllerRuntimeSet.Remove(this);
		}

		protected virtual void Update () {
			UpdateKeyboardControl();
			UpdateMouseControl();
		}
		
		protected virtual void UpdateKeyboardControl () {
			UpdateKeyboardMovement();
		}
		private void UpdateKeyboardMovement () {
			if (Keyboard.current.wKey.isPressed) {
				transform.position += Vector3.forward*scrollSpeed*Time.deltaTime;
			}
			if (Keyboard.current.sKey.isPressed) {
				transform.position += Vector3.back*scrollSpeed*Time.deltaTime;
			}
			if (Keyboard.current.aKey.isPressed) {
				transform.position += Vector3.left*scrollSpeed*Time.deltaTime;
			}
			if (Keyboard.current.dKey.isPressed) {
				transform.position += Vector3.right*scrollSpeed*Time.deltaTime;
			}
		}

		protected abstract void UpdateMouseControl ();
	}
}
