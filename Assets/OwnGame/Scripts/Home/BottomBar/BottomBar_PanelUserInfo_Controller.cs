using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomBar_PanelUserInfo_Controller : MySimplePanelController
{

    [SerializeField] CanvasGroup myCanvasGroup;
    [SerializeField] RawImage userAvata;
    [SerializeField] Text txtUserInfo_Name;
    [SerializeField] Text txtUserInfo_Gold;
    [SerializeField] Image imgDatabaseType;

    [Header("Setting")]
    [SerializeField] int maxLengthOfUserName = 15;

    Coroutine actionLoadAvatar;

    public override void InitData(System.Action _onFinished = null)
    {
        txtUserInfo_Name.text = MyConstant.ConvertString(DataManager.instance.userData.nameShowInGame, maxLengthOfUserName);
        
        Sprite _iconDatabaseID = DataManager.instance.userData.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			imgDatabaseType.gameObject.SetActive(true);
			imgDatabaseType.sprite = _iconDatabaseID;
		}else{
			imgDatabaseType.gameObject.SetActive(false);
		}

        RefreshPanelMyCashInfo();

        if(userAvata != null){
            if(actionLoadAvatar != null){
                StopCoroutine(actionLoadAvatar);
                actionLoadAvatar = null;
            }
            actionLoadAvatar = DataManager.instance.userData.LoadAvatar(this, userAvata.rectTransform.rect.width, userAvata.rectTransform.rect.height,
                (_avatar) =>
                {
                    try{
                        if(_avatar != null){
                            userAvata.texture = _avatar;
                        }
                    }catch{}
                    actionLoadAvatar = null;
                });
        }
        
    }

    public override Coroutine Show()
    {
        myCanvasGroup.alpha = 1f;
        myCanvasGroup.blocksRaycasts = true;
        return null;
    }

    public override Coroutine Hide()
    {
        myCanvasGroup.alpha = 0f;
        myCanvasGroup.blocksRaycasts = false;
        if(actionLoadAvatar != null){
            StopCoroutine(actionLoadAvatar);
            actionLoadAvatar = null;
        }
        return null;
    }

    public override void RefreshData()
    {
        if(userAvata != null){
            if(actionLoadAvatar != null){
                StopCoroutine(actionLoadAvatar);
                actionLoadAvatar = null;
            }
            actionLoadAvatar = DataManager.instance.userData.LoadAvatar(this, userAvata.rectTransform.rect.width, userAvata.rectTransform.rect.height,
                (_avatar) =>
                {
                    try{
                        if(_avatar != null){
                            userAvata.texture = _avatar;
                        }
                    }catch{}
                    actionLoadAvatar = null;
                });
        }
        RefreshPanelMyCashInfo();
    }

    public void RefreshPanelMyCashInfo()
    {
        if(txtUserInfo_Gold != null){
            txtUserInfo_Gold.text = MyConstant.GetMoneyString(DataManager.instance.userData.gold, 9999);
        }
    }

    #region On Button Clicked
    public void OnEnterUserDetail()
    {
        MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        HomeManager.instance.ChangeScreen(UIHomeScreenController.UIType.UserDetail);
    }
    #endregion
}
