using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseAvatarScreenController : UIHomeScreenController {

	// [SerializeField] Transform mainContainer;
	// [SerializeField] List<RawImage> listAvatar;
	// [SerializeField] Text txtTitleChooseAvatar;
	// [SerializeField] Transform panelTick;

	// [Header("Setting")]
	// [SerializeField] float timeShowScreen;
	// [SerializeField] float timeHideScreen;

	// LTDescr tweenCanvasGroup, tweenMainContainer;

	// public override UIType myType {
	// 	get {
	// 		return UIType.ChooseAvatar;
	// 	}
	// }

	// public override bool isSubScreen {
	// 	get {
	// 		return true;
	// 	}
	// }

	// private void Awake() {
	// 	base.Hide ();
	// }

	// public override void InitData ()
	// {
	// 	if (DataManager.instance.userData.databaseId == UserData.DATABASEID_FACEBOOK) {
	// 		listAvatar[0].texture = GameInformation.instance.otherInfo.avatarDefault;
	// 		StartCoroutine(CoreGameManager.instance.DoActionLoadAvatarFB (DataManager.instance.userData.facebookId.ToString(), listAvatar[0].rectTransform.rect.width, listAvatar[0].rectTransform.rect.height, (_avatar) => {
	// 			listAvatar[0].texture = _avatar;
	// 		})); 
	// 	}
	// 	txtTitleChooseAvatar.text = MyLocalize.GetString("Global/ChooseAvatar");
	// 	onPressBack = () => {
	// 		HomeManager.instance.ChangeScreen (myLastType);
	// 	};
	// 	CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
	// }

	// #region Show And Hide
	// public override void Show ()
	// {
	// 	if(currentState == State.Show){
	// 		return;
	// 	}
	// 	currentState = State.Show;
	// 	myCanvasGroup.alpha = 0f;
	// 	myCanvasGroup.blocksRaycasts = true;

	// 	MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

	// 	if(tweenCanvasGroup != null){
	// 		LeanTween.cancel(tweenCanvasGroup.uniqueId);
	// 		tweenCanvasGroup = null;
	// 	}
	// 	tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
	// 		tweenCanvasGroup = null;
	// 	});

	// 	if(tweenMainContainer != null){
	// 		LeanTween.cancel(tweenMainContainer.uniqueId);
	// 		tweenMainContainer = null;
	// 	}
	// 	Vector3 _pos = Vector3.zero;
	// 	_pos.x = 250f;
	// 	mainContainer.localPosition = _pos;
	// 	tweenMainContainer = LeanTween.moveLocalX(mainContainer.gameObject, 0f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
	// 		tweenMainContainer = null;
	// 	});

	// 	StartCoroutine(DoActionShow());
	// }

	// IEnumerator DoActionShow(){
	// 	yield return Yielders.EndOfFrame;
	// 	sbyte _tmpAvatarid = (sbyte)(DataManager.instance.userData.avatarid % UserData.maxLengthListAvatarID);
	// 	panelTick.position = listAvatar[_tmpAvatarid].transform.position;
	// }

	// public override void Hide ()
	// {
	// 	if(currentState == State.Hide){
	// 		return;
	// 	}
	// 	if(tweenCanvasGroup != null){
	// 		LeanTween.cancel(tweenCanvasGroup.uniqueId);
	// 		tweenCanvasGroup = null;
	// 	}
	// 	if(tweenMainContainer != null){
	// 		LeanTween.cancel(tweenMainContainer.uniqueId);
	// 		tweenMainContainer = null;
	// 	}

	// 	if (onPressBack != null) {
	// 		CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
	// 		onPressBack = null;
	// 	}
	// 	MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

	// 	base.Hide();
	// }
	// #endregion

	// #region On Button Clicked
	// public void OnButtonBackClicked(){
	// 	MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
	// 	if (onPressBack != null) {
	// 		CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
	// 		onPressBack ();
	// 		onPressBack = null;
	// 	}
	// }

	// public void OnButtonChooseAvatarClicked(int _avatarid){
	// 	MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

	// 	DataManager.instance.userData.avatarid = (sbyte)_avatarid;
	// 	HomeManager.instance.ChangeScreen (myLastType);

	// 	OneHitAPI.ChooseAvatar (DataManager.instance.userData.databaseId, DataManager.instance.userData.userId, (sbyte)_avatarid,
	// 		(_messageReceiving, _error) => {
	// 			if (_messageReceiving != null) {
	// 				bool _isSuccess = _messageReceiving.readBoolean();
	// 				if(_isSuccess){
	// 					DataManager.instance.userData.avatarid = (sbyte)_avatarid;
	// 				}else{
	// 					#if TEST
	// 					Debug.LogError("Can not change new avatar!");
	// 					#endif
	// 				}
	// 			}else{
	// 				#if TEST
	// 				Debug.LogError("Error Code: " + _error);
	// 				#endif
	// 			}
	// 		}
	// 	);
	// }
	// #endregion
}
