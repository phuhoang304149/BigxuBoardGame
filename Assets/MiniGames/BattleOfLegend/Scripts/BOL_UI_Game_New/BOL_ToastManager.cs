using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
public class BOL_ToastManager : MonoBehaviour {
	[SerializeField] Canvas myCanvas;
	//[SerializeField] GraphicRaycaster gpRaycaster;
	[SerializeField] GameObject pool;
	[SerializeField] ToastController toastPrefab;
	List<ToastController> toasts;
	List<IPopupController> popupsActive;
	static BOL_ToastManager ins;
	public static BOL_ToastManager Instance {
		get {
			return ins;
		}
	}
	void Awake() {
		if (ins != null && ins != this) {
			Destroy(this.gameObject);
			return;
		}
		ins = this;
		DontDestroyOnLoad(this.gameObject);

		//gpRaycaster.enabled = false;
		if (popupsActive == null) {
			popupsActive = new List<IPopupController>();
		}
		if (toasts == null) {
			toasts = new List<ToastController>();
		}
	}
	public void CreateToast(string _textInfo, Vector3 _pos, Color _colorText, float _maxSizeW) {
		if (myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null) {
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		} else {
			myCanvas.worldCamera = Camera.main;
		}
		//gpRaycaster.enabled = true;
		ToastController _tmpPopup = LeanPool.Spawn(toastPrefab, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<ToastController>();
		_tmpPopup.transform.position = _pos;
		_tmpPopup.Init(_textInfo, _colorText, _maxSizeW, () => {
			if (toasts != null && toasts.Count > 0) {
				toasts.Remove(_tmpPopup);
			}
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		if (toasts == null) {
			toasts = new List<ToastController>();
		}
		for (int i = 0; i < toasts.Count; i++) {
			toasts[i].SetSpeedUp();
		}
		toasts.Add(_tmpPopup);
		AddPopupActive(_tmpPopup);
	}
	public void RemovePopupActive(IPopupController _popup) {
		if (popupsActive.Count == 0) {
			return;
		}
		popupsActive.Remove(_popup);
		_popup.SelfDestruction();
		if (popupsActive.Count == 0) {
			//gpRaycaster.enabled = false;
		}
	}
	void AddPopupActive(IPopupController _popup) {
		_popup.transform.SetAsLastSibling();
		popupsActive.Add(_popup);
	}

}
