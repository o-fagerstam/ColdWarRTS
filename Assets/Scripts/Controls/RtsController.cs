using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Controls {
	public class RtsController : MonoBehaviour {
		[SerializeField] private LayerMask groundLayerMask;
		
		private float scrollSpeed = 5f;
		private Transform cameraFollowTransform;
		private void Start () {
			cameraFollowTransform = transform;
		}
		private void Update()
		{
			UpdateKeyboardMovement();
			UpdateMouseControl();
		}
		private void UpdateMouseControl () {
			Camera mainCamera = Camera.main;

			if (!Mouse.current.leftButton.wasPressedThisFrame &&
			    !Mouse.current.rightButton.wasPressedThisFrame) 
			{
				return;
			}
			
			Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (!Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, groundLayerMask)) {
				return;
			}
			
			Debug.Log($"Clicked on point: {raycastHit.point}");
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
	}
}
