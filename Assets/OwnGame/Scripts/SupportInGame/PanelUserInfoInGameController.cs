using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelUserInfoInGameController : MonoBehaviour {

    public enum State{
        Show, Hide
    }
    public State currentState{get;set;}

    [SerializeField] CanvasGroup myCanvasGroup;
    public RawImage userAvata;
    [SerializeField] Text txtUserInfo_Name;
    [SerializeField] int maxLengthOfUserName;
    [SerializeField] Text txtUserInfo_Gold;
    [SerializeField] Image imgDatabaseType;

    IEnumerator actionRefreshGoldInfo;

    long virtualGold, tmpDeltaGold, realGold;
    int minGoldCheck = 9999;

    private void Awake() {
        currentState = State.Show;
        if(myCanvasGroup != null){
            myCanvasGroup.alpha = 1f;
        }
    }

	public void InitData()
    {   
        if(txtUserInfo_Name != null){
            txtUserInfo_Name.text = MyConstant.ConvertString(DataManager.instance.userData.nameShowInGame, maxLengthOfUserName);
        }

        if(imgDatabaseType != null){
            Sprite _iconDatabaseID = DataManager.instance.userData.GetIconDatabaseID();
            if(_iconDatabaseID != null){
                imgDatabaseType.gameObject.SetActive(true);
                imgDatabaseType.sprite = _iconDatabaseID;
            }else{
                imgDatabaseType.gameObject.SetActive(false);
            }
        }

        RefreshGoldInfo(true);

        if(userAvata != null){
            DataManager.instance.userData.LoadAvatar(this, userAvata.rectTransform.rect.width, userAvata.rectTransform.rect.height,
            (_avatar) =>
            {
                try{
                    if(_avatar != null){
                        userAvata.texture = _avatar;
                    }
                }catch{}
            });
        }
    }

	public void Show() {
        currentState = State.Show;
        if(myCanvasGroup != null){
            myCanvasGroup.alpha = 1f;
        }
    }

    public void Hide() {
        currentState = State.Hide;
        if(myCanvasGroup != null){
            myCanvasGroup.alpha = 0f;
        }
    }

    public void RefreshGoldInfo(bool _updateNow = false){
        if(txtUserInfo_Gold == null){
            return;
        }

        realGold = DataManager.instance.userData.GetGoldView();

        if(_updateNow){
			if(actionRefreshGoldInfo != null){
                StopCoroutine(actionRefreshGoldInfo);
                actionRefreshGoldInfo = null;
            }
			virtualGold = realGold;
			txtUserInfo_Gold.text = MyConstant.GetMoneyString(virtualGold, minGoldCheck);
		}else{
            if(actionRefreshGoldInfo != null){
                StopCoroutine(actionRefreshGoldInfo);
                actionRefreshGoldInfo = null;
            }
            actionRefreshGoldInfo = MyConstant.TweenValue(virtualGold, realGold, 5, (_valueUpdate)=>{
                virtualGold = _valueUpdate;
                txtUserInfo_Gold.text = MyConstant.GetMoneyString(virtualGold, minGoldCheck);
            }, (_valueFinish)=>{
                virtualGold = _valueFinish;
                txtUserInfo_Gold.text = MyConstant.GetMoneyString(virtualGold, minGoldCheck);
                actionRefreshGoldInfo = null;
            });
            StartCoroutine(actionRefreshGoldInfo);
        }
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }
}
