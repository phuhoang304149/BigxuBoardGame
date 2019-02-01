using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook;
using Facebook.Unity;
using System;

public static class FacebookAPI {

	// static List<string> perms = new List<string>(){"public_profile", "email", "user_friends"};
	static List<string> perms = new List<string>(){"public_profile"};
	static List<string> permsForPublic = new List<string> (){ "publish_actions" };

	public static void Init(InitDelegate _onInitComplete = null, HideUnityDelegate _onHideUnity = null){
		if (!FB.IsInitialized) {
			Debug.Log("INIT");
			// Initialize the Facebook SDK
			FB.Init(_onInitComplete, _onHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}

	public static void LogIn(FacebookDelegate<ILoginResult> callback){
		if (!FB.IsInitialized) {
			return;
		}
		FB.LogInWithReadPermissions(perms, callback);
		//xin quyền xem thông tin email, quyền public - callBackLogin là callback gọi sau khi login(có thể là thành công hoặc không thành công để xử lý)
	}

	public static void LogInForPublish (FacebookDelegate<ILoginResult> callback){
		// It is generally good behavior to split asking for read and publish
		// permissions rather than ask for them all at once.
		//
		// In your own game, consider postponing this call until the moment
		// you actually need it.
		if (!FB.IsInitialized) {
			return;
		}
		FB.LogInWithPublishPermissions(permsForPublic, callback);
		//xin quyền xem thông tin email, quyền public - callBackLogin là callback gọi sau khi login(có thể là thành công hoặc không thành công để xử lý)
	}

	public static bool IsLoggedIn(){
		if (!FB.IsInitialized) {
			return false;
		}
		return FB.IsLoggedIn;
	}

	public static void LogOut(){
		if(!FB.IsLoggedIn){
			return;
		}
		FB.LogOut();
	}

	public static void GetApplink(){
		FB.GetAppLink(CallbackGetApplink);
	}

	static void CallbackGetApplink(IAppLinkResult result){
		if (result.Error != null){
			Debug.Log("Error Response:\n" + result.Error);
		}else if (!FB.IsLoggedIn){
			Debug.Log("Login First Pls!");
		}else{
			Debug.Log("Get App link: "+result.RawResult);
//			applinkUrl = new Uri(result.RawResult);
		}
	}

	public static void GetName (string _userID, FacebookDelegate<IGraphResult> _callback){
		if(!FB.IsLoggedIn){
			return;
		}
		string _uri = "/" + _userID + "?fields=name&access_token=" + AccessToken.CurrentAccessToken.TokenString;
		FB.API (_uri, HttpMethod.GET, _callback);
	}

	public static void GetAvatar (string _userID, float _w, float _h, FacebookDelegate<IGraphResult> _callback){
		if(!IsLoggedIn()){
			return;
		}
		//string uri = "https://graph.facebook.com/" + id + "/picture?width=" + _w + "&height=" + _h;
		string _uri = "/" + _userID +"/picture?width=" + _w + "&height=" + _h + "&access_token=" + AccessToken.CurrentAccessToken.TokenString;
		FB.API (_uri, HttpMethod.GET, _callback);
	}

	public static void AskForHelp(string _message, string _objectId, IEnumerable<string> _excludeIds, FacebookDelegate<IAppRequestResult> _callback){
		if(!FB.IsLoggedIn){
			return;
		}
		List<object> _filters = new List<object>(){"app_users"};
		FB.AppRequest(
			_message,
			OGActionType.ASKFOR,
			_objectId,
			_filters,
			_excludeIds,
			null,
			"",
			"",
			_callback);
	}

	public static void SendObject(string _message, IEnumerable<string> _to, string _objectId, FacebookDelegate<IAppRequestResult> _callback){
		if(!FB.IsLoggedIn){
			return;
		}
		FB.AppRequest(
			_message,
			OGActionType.SEND,
			_objectId,
			_to,
			"",
			"",
			_callback);
	}

	public static void InvitePlayer(Uri _appLinkUrl, Uri _previewImageUrl = null, FacebookDelegate<IAppInviteResult> _callback = null){
		if(!FB.IsLoggedIn){
			return;
		}
		//		List<object> _filters = new List<object>(){"app_non_users"};
		//		FB.AppRequest(_message, null, _filters, null, null, "", "", _callback);
		FB.Mobile.AppInvite(_appLinkUrl , _previewImageUrl, _callback);
	}


	public static void ReadingAllRequest(FacebookDelegate<IGraphResult> _callback){
		if(!FB.IsLoggedIn){
			return;
		}
		string _uri = "/me/apprequests?fields==id,to,from,data,message,action_type,object,created_time&access_token=" + AccessToken.CurrentAccessToken.TokenString;
		FB.API (_uri, HttpMethod.GET, _callback);
	}

	public static void DeleteAppRequest(string _requestID, FacebookDelegate<IGraphResult> _callback){
		if(!FB.IsLoggedIn){
			return;
		}
		string _uri = "/" + _requestID + "?access_token=" + AccessToken.CurrentAccessToken.TokenString;
		FB.API (_uri, HttpMethod.DELETE, _callback);
	}

	public static void GetFriendList(string _idUser, FacebookDelegate<IGraphResult> _callback){
		if(!FB.IsLoggedIn){
			return;
		}
		string _uri = "/" + _idUser + "/friends?access_token=" + AccessToken.CurrentAccessToken.TokenString;
		FB.API (_uri, HttpMethod.GET, _callback);
	}

	public static void Share(string _contentURL = "", string _contentTitle = "", string _contentDescription = "", string _photoURL = "", FacebookDelegate<IShareResult> _callback = null){
		if(!FB.IsLoggedIn){
			return;
		}
		FB.ShareLink(
			new Uri(_contentURL)
			, _contentTitle
			, _contentDescription
			, new Uri(_photoURL)
			,_callback
		);
	}

	public static IEnumerator DoActionLoginFb(System.Action _onLoginFbFinished = null, System.Action _onStopCourotine = null){
		bool _isFinished = false;
		bool _isError = false;
		float _tmpTime = 0f;
		float _timeOut = 60f;
		// LoadingCanvasController.instance.Show ();
		if(!FB.IsInitialized){
			Init(()=>{
				_isFinished = true;
			});
		}else{
			_isFinished = true;
		}
		_tmpTime = 0f;
		while(!_isFinished){
			yield return Yielders.FixedUpdate;
			_tmpTime += Time.fixedDeltaTime;
			if(_tmpTime >= _timeOut){
				LoadingCanvasController.instance.Hide ();
				if(_onStopCourotine != null){
					_onStopCourotine();
				}
				yield break;
			}
		}

		if(!IsLoggedIn()){
			_isFinished = false;
			LogIn((_result) => {
				_isFinished = true;
				if (_result.Cancelled) {
					#if TEST
					Debug.Log("Cancel!");
					#endif
					_isError = true;
				}else if (_result.Error != null) {
					#if TEST
					Debug.LogError("LogIn FB error: " + _result.Error);
					#endif
					// PopupManager.Instance.CreatePopupMessage(MyConstant.kError, "Error Code: " + _result.Error, MyConstant.kOk);
					_isError = true;
				}else{
					// notthing todo
				}
			});

			_tmpTime = 0f;
			while(!_isFinished){
				yield return Yielders.FixedUpdate;
				_tmpTime += Time.fixedDeltaTime;
				if(_tmpTime >= _timeOut){
					// LoadingCanvasController.instance.Hide ();
					if(_onStopCourotine != null){
						_onStopCourotine();
					}
					yield break;
				}
			}
		}

		// LoadingCanvasController.instance.Hide ();
		if(!_isError){
			if(_onLoginFbFinished != null){
				_onLoginFbFinished();
			}
		}
		if(_onStopCourotine != null){
			_onStopCourotine();
		}
	}
}
