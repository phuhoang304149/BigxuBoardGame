using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class PopupChatManager : MonoBehaviour {

	public enum PopupChatPosType{
		Top, Bottom, Left, Right
	}

	[SerializeField] Canvas myCanvas;
	[SerializeField] Transform pool;
	[SerializeField] GameObject popupChatPrefab_Top;
	[SerializeField] GameObject popupChatPrefab_Bottom;
	[SerializeField] GameObject popupChatPrefab_Left;
	[SerializeField] GameObject popupChatPrefab_Right;

	MySimplePoolManager popupChatPoolManager;

	private void Awake() {
		popupChatPoolManager = new MySimplePoolManager();
	}

	public void SetSortingLayerInfoAgain(MySortingLayerInfo _sortingLayerInfo){
		myCanvas.sortingLayerName = _sortingLayerInfo.layerName.ToString();
		myCanvas.sortingOrder = _sortingLayerInfo.layerOrderId;
	}

	// private void Start() {
	// 	StartCoroutine(TEST());
	// }

	// IEnumerator TEST(){
	// 	while(true){
	// 		yield return Yielders.Get(2f);
	// 		PopupChatController _tmpPopup = LeanPool.Spawn(popupChatPrefab_Top, Vector3.zero, Quaternion.identity, pool).GetComponent<PopupChatController>();
	// 		_tmpPopup.InitData("haha haha haha haha haha haha haha haha haha haha haha", null);
	// 		_tmpPopup.Show();
	// 	}
	// }

	public void ForcedRemoveAll(){
		popupChatPoolManager.ClearAllObjectsNow();
	}

	public PopupChatController CreatePopupChat(PopupChatPosType _posType, string _mess, Vector3 _pos){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}
		GameObject _tmpPrefab = null;
		switch(_posType){
		case PopupChatPosType.Top:
			_tmpPrefab = popupChatPrefab_Top;
			break;
		case PopupChatPosType.Bottom:
			_tmpPrefab = popupChatPrefab_Bottom;
			break;
		case PopupChatPosType.Left:
			_tmpPrefab = popupChatPrefab_Left;
			break;
		case PopupChatPosType.Right:
			_tmpPrefab = popupChatPrefab_Right;
			break;
		default:
			Debug.LogError("Cần thêm vào PopupChatPosType: " + _posType.ToString());
			break;
		}
		if(_tmpPrefab == null){
			Debug.LogError("_tmpPrefab is null");
			return null;
		}
		PopupChatController _tmpPopup = LeanPool.Spawn(_tmpPrefab, _pos, Quaternion.identity, pool).GetComponent<PopupChatController>();
		_tmpPopup.transform.position = _pos;
		_tmpPopup.InitData(_mess);
		_tmpPopup.Show();
		_tmpPopup.transform.SetAsLastSibling();
		popupChatPoolManager.AddObject(_tmpPopup);
		return _tmpPopup;
	}
}
