using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class GetGoldScreen_PanelBuyGold_PanelHistory_Controller : MySimplePanelController {

	public enum State{
		Hide,
		Show
	}
	public State currentState{ get; set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Transform mainContainer;
	[SerializeField] Transform panelOptionContainer;
	[SerializeField] Text txtEmpty;

	[Header("Prefab")]
	[SerializeField] GameObject prefabPurchaseHistoryOption;

	[Header("Setting")]
	[SerializeField] float timeShowScreen;
	[SerializeField] float timeHideScreen;

	MySimplePoolManager optionInfoPoolManager;
	LTDescr tweenCanvasGroup, tweenMainContainer;

	public override void ResetData(){
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}

		if(optionInfoPoolManager != null){
			optionInfoPoolManager.ClearAllObjectsNow();
		}
	}

	public override void InitData (System.Action _onFinished = null){
		optionInfoPoolManager = new MySimplePoolManager();
		if(DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count == 0){
			txtEmpty.gameObject.SetActive(true);
		}else{
			txtEmpty.gameObject.SetActive(false);
			int _count = DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count;
			for(int i = 0; i < _count; i ++){
				GetGoldScreen_PanelBuyGold_PanelHistory_OptionInfo_Controller _tmpPanel = LeanPool.Spawn(prefabPurchaseHistoryOption, Vector3.zero, Quaternion.identity, panelOptionContainer.transform).GetComponent<GetGoldScreen_PanelBuyGold_PanelHistory_OptionInfo_Controller>();
				_tmpPanel.InitData(DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail[i]);
				optionInfoPoolManager.AddObject(_tmpPanel);
			}
		}
	}

	public override Coroutine Show (){
		if(currentState == State.Show){
			return null;
		}

		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = true;

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);
		
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		Vector3 _pos = Vector3.zero;
		_pos.x = 250f;
		mainContainer.localPosition = _pos;
		tweenMainContainer = LeanTween.moveLocalX(mainContainer.gameObject, 0f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
		return null;
	}

	public override Coroutine Hide (){
		if(currentState == State.Hide){
			return null;
		}

		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeHideScreen).setOnComplete(()=>{
			tweenCanvasGroup = null;
			ResetData();
		}).setEase(LeanTweenType.easeInBack);
		
		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		tweenMainContainer = LeanTween.moveLocalX(mainContainer.gameObject, -250f, timeHideScreen).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
		return null;
	}

	public void OnBtnCloseClicked(){
		Hide();
		GetGoldScreenController.instance.currentPanel.RefreshData();
	}
}
