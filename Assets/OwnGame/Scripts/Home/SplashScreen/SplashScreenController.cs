using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook;
using Facebook.Unity;
using UnityEngine.Networking;

public class SplashScreenController : UIHomeScreenController
{
    public override UIType myType{
		get{ 
			return UIType.SplashScreen;
		}
	}

    [SerializeField] Image imgLogo;

    public override void Show()
    {
        base.Show();
        StartCoroutine(DoActionRun());
    }

    public override void Hide()
	{
        base.Hide();
        Destroy(gameObject);
	}

    IEnumerator DoActionRun()
    {   
        long _tmpTime = MyConstant.currentTimeMilliseconds;
        float _deltaTime = 0f;
        yield return StartCoroutine(HomeManager.instance.DoActionGetFirstDataOnAndroid());

        if (DataManager.instance.userData.userId != 0){
            bool _isLoadDataGoldGemFromServerFinished = false;
            HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished = ()=>{
                _isLoadDataGoldGemFromServerFinished = true;
            };
            HomeManager.instance.LoadDataGoldGemFromServer();
            while(!_isLoadDataGoldGemFromServerFinished){
                yield return null;
                _deltaTime = (MyConstant.currentTimeMilliseconds - _tmpTime) / 1000f;
                if(_deltaTime > 5f){
                    break;
                }
            }
            HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished = null;
        }

        _deltaTime = (MyConstant.currentTimeMilliseconds - _tmpTime) / 1000f;
        if(_deltaTime < 1f){
            yield return Yielders.Get(1f - _deltaTime);
        }

        if(!DataManager.instance.getFirstDataSuccessfull){
            HomeManager.instance.TryToGetFirstData();
        }

        if (DataManager.instance.userData.userId != 0)
        { // đã đăng nhập thành công trước đó
            if (DataManager.instance.userData.databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK)
            {
                bool _loginFbSuccessfully = false;
                yield return StartCoroutine(FacebookAPI.DoActionLoginFb(()=>{
                    _loginFbSuccessfully = true;
                }));
                if(_loginFbSuccessfully){
                    Debug.Log("Login FB successful!");
                    // AccessToken class will have session details
                    var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                    Debug.Log("UserID: " + aToken.UserId + " - " + aToken.ToJson() + " - " + aToken.ToString());
                }
            }

            bool _isFinished = false;
            LeanTween.alpha(imgLogo.rectTransform, 0f, 0.2f).setOnComplete(()=>{
                _isFinished = true;
            });
            yield return new WaitUntil(()=>_isFinished);
            yield return Yielders.Get(0.1f);

            HomeManager.showAnnouncement = true;
            HomeManager.getEmailInfoAgain = true;

            HomeManager.instance.ChangeScreen(UIType.ChooseGame);
        }
        else
        {
            if(FacebookAPI.IsLoggedIn()){
                FacebookAPI.LogOut();
            }
            // bool _isFinished = false;
            // LeanTween.alpha(imgLogo.rectTransform, 0f, 0.2f).setOnComplete(()=>{
            //     _isFinished = true;
            // });
            // yield return new WaitUntil(()=>_isFinished);
            yield return Yielders.Get(0.1f);
            HomeManager.instance.ChangeScreen(UIType.LoginScreen);
        }
    }
}
