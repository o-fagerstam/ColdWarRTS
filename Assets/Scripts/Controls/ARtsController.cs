using Constants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls {
	public abstract class ARtsController : MonoBehaviour {

		private float scrollSpeed = 5f;
		protected virtual void Update()
		{
			UpdateKeyboardControl();
			UpdateMouseControl();
		}
		private void UpdateMouseControl () {
			Camera mainCamera = Camera.main;

			if (!Mouse.current.leftButton.wasPressedThisFrame &&
			    !Mouse.current.rightButton.wasPressedThisFrame) 
			{
				return;
			}

			if (EventSystem.current.IsPointerOverGameObject()) {
				return;
			}
			
			Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (!Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, LayerMasks.groundMask)) {
				return;
			}

			if (Mouse.current.leftButton.wasPressedThisFrame) {
				OnLeftClickGround(raycastHit);
			}
			if (Mouse.current.rightButton.wasPressedThisFrame) {
				OnRightClickGround(raycastHit);
			}
		}
		
		protected virtual void OnLeftClickGround (RaycastHit hit) {}
		protected virtual void OnRightClickGround (RaycastHit hit) {}

		private void UpdateKeyboardControl () {
			UpdateKeyboardMovement();
			if (Keyboard.current.spaceKey.wasPressedThisFrame) {
				OnSpacePressed();
			}
		}
		protected virtual void OnSpacePressed () {}

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
	}
}
