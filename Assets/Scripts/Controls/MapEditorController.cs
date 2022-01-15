using System;
using System.Collections.Generic;
using Constants;
using Controls.MapEditorTools;
using Map;
using Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Controls {
	public class MapEditorController : ARtsController {
		[Title("Map Editor Controller")]
		[SerializeField] private float scrollSpeed = 5f;
		[Title("Debug")]
		[ReadOnly][ShowInInspector] private AMapEditorTool currentTool;


		public void SelectTool (AMapEditorTool tool) {
			currentTool = tool;
			tool.Activate();
		}

		private void OnEnable () {
			SingletonManager.Register(this);
		}

		private void OnDisable () {
			SingletonManager.Unregister(this);
		}

		protected virtual void Update () {
			UpdateKeyboardControl();
			UpdateMouseControl();
		}

		private void UpdateKeyboardControl () {
			UpdateKeyboardMovement();
			if (currentTool != null) {
				if (Keyboard.current.escapeKey.wasPressedThisFrame) {
					ClearTool();
				} else {
					currentTool.UpdateKeyboard();
				}
			}
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

		private void UpdateMouseControl () {
			Camera mainCamera = Camera.main;
			Ray mouseRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (currentTool != null) {
				currentTool.UpdateMouse(mouseRay);
				return;
			}
		}

		public void ClearTool () {
			currentTool.Deactivate();
			currentTool = null;
		}
	}
}
