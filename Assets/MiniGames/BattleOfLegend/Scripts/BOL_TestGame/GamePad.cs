using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR;
using UnityEngine.UI;
using InControl;
using System;

public class GamePad : MonoBehaviour {
	bool isPaused;
	float Timestamp;
	[Range(0, 1)]
	public float TimeBetweenShots = 0.1f;
	public Text textPress;
	private void OnEnable() {
		InputManager.OnDeviceAttached += inputDevice => Debug.Log("Attached: " + inputDevice.Name);
		InputManager.OnDeviceDetached += inputDevice => Debug.Log("Detached: " + inputDevice.Name);
		InputManager.OnActiveDeviceChanged += inputDevice => Debug.Log("Active device changed to: " + inputDevice.Name);
		InputManager.OnUpdate += HandleInputUpdate;
	}
	void Start() {
#if UNITY_IOS
		ICadeDeviceManager.Active = true;
#endif
	}
	void HandleInputUpdate(ulong updateTick, float deltaTime) {
		CheckForPauseButton();
		var devicesCount = InputManager.Devices.Count;
		for (var i = 0; i < devicesCount; i++) {
			var inputDevice = InputManager.Devices[i];
			inputDevice.Vibrate(inputDevice.LeftTrigger, inputDevice.RightTrigger);
		}
	}
	void CheckForPauseButton() {
		if (Input.GetKeyDown(KeyCode.P) || InputManager.CommandWasPressed) {
			Time.timeScale = isPaused ? 1.0f : 0.0f;
			isPaused = !isPaused;
		}
	}
	private void Update() {
		if (Time.time >= Timestamp) {
			foreach (var inputDevice in InputManager.Devices) {
				var deviceIsActive = InputManager.ActiveDevice == inputDevice;
				var color = deviceIsActive ? Color.yellow : Color.white;
				if (inputDevice.IsUnknown) {
				} else {
					//Debug.Log("name device"+inputDevice.Name);
				}
				if (inputDevice.IsUnknown) {
					//Debug.Log("meta "+inputDevice.Meta);
				}
				//Debug.Log("deviece sstyle"+inputDevice.DeviceStyle);
				//Debug.Log("GUID"+inputDevice.GUID);
				//Debug.Log("sort order"+inputDevice.SortOrder);
				//Debug.Log("lastchange tick"+inputDevice.LastChangeTick);

				var nativeDevice = inputDevice as NativeInputDevice;
				if (nativeDevice != null) {
					var nativeDeviceInfo = String.Format("VID = 0x{0:x}, PID = 0x{1:x}, VER = 0x{2:x}", nativeDevice.Info.vendorID, nativeDevice.Info.productID, nativeDevice.Info.versionNumber);
					//Debug.Log("native device info"+nativeDeviceInfo);

				}
				foreach (var control in inputDevice.Controls) {
					if (control != null && !Utility.TargetIsAlias(control.Target)) {
						string controlName;

						if (inputDevice.IsKnown) {
							controlName = string.Format("{0} ({1})", control.Target, control.Handle);
						} else {
							controlName = control.Handle;
						}

						var label = string.Format("{0} {1}", controlName, control.State ? "= " + control.Value : "");
						//if(control.Value==1){
						//Debug.Log(label);
						//}

					}
				}
				color = deviceIsActive ? new Color(1.0f, 0.7f, 0.2f) : Color.white;
				if (inputDevice.IsKnown) {
					var control = inputDevice.Command;
					var label = string.Format("{0} {1}", "Command", control.State ? "= " + control.Value : "");
					Debug.Log(label);
					control = inputDevice.LeftStickX;
					label = string.Format("{0} {1}", "Left Stick X", control.State ? "= " + control.Value : "");
					Debug.Log(label); 
					control = inputDevice.LeftStickY;
					label = string.Format("{0} {1}", "Left Stick Y", control.State ? "= " + control.Value : "");
					Debug.Log(label);
					label = string.Format("{0} {1}", "Left Stick A", inputDevice.LeftStick.State ? "= " + inputDevice.LeftStick.Angle : "");
					Debug.Log(label);
					control = inputDevice.RightStickX;
					label = string.Format("{0} {1}", "Right Stick X", control.State ? "= " + control.Value : "");
					Debug.Log(label);
					control = inputDevice.RightStickY;
					label = string.Format("{0} {1}", "Right Stick Y", control.State ? "= " + control.Value : "");
					Debug.Log(label);
					label = string.Format("{0} {1}", "Right Stick A", inputDevice.RightStick.State ? "= " + inputDevice.RightStick.Angle : "");
					Debug.Log(label);
					control = inputDevice.DPadX;
					label = string.Format("{0} {1}", "DPad X", control.State ? "= " + control.Value : "");
					Debug.Log(label);
					control = inputDevice.DPadY;
					label = string.Format("{0} {1}", "DPad Y", control.State ? "= " + control.Value : "");
					Debug.Log(label);
				}
				var anyButton = inputDevice.AnyButton;
				if (anyButton) {
					Debug.Log("abc" + anyButton.Handle);
					//if (anyButton.Handle == "Button X") {
					//	Debug.Log(anyButton.Handle);
					//} else if (anyButton.Handle == "Button A") {
					//	Debug.Log(anyButton.Handle);
					//} else if (anyButton.Handle == "Button B") {
					//	Debug.Log(anyButton.Handle);
					//} else if (anyButton.Handle == "Button Y") {
					//	Debug.Log(anyButton.Handle);
					//}
				}
			}
			Timestamp = Time.time + TimeBetweenShots;
		}
	}

	void MoveInScene(short CMD) {
		BOL_PlaySkill_Controller.instance.EventMove(CMD);
	}
	void CallSkill(short skill) {
		BOL_PlaySkill_Controller.instance.CallSkill(skill);
	}
	void CallSpell(short spell, Vector3 Pos) {
		//BOL_PlaySkill_Controller.instance.AttackSpell(spell, Pos);
	}
}

