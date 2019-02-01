using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

[System.Serializable]
public class Circle
{
	public Vector2 point;
	public float radius;
}

[System.Serializable]
public class RandomValue
{
	public float min, max;

	public RandomValue ()
	{
		min = 0f;
		max = 0f;
	}

	public RandomValue (float _min, float _max)
	{
		min = _min;
		max = _max;
	}
}

[System.Serializable]
public class Bound
{
	public float xLeft;
	public float xRight;
	public float yTop;
	public float yBottom;
}

public static class MyConstant{
	
	#region Save Info
	public const string rootSaveName = "BigxuOnl_DataInfo.dat";
	public const string save_kSfxName = "BigxuOnl_Sfx";
	public const string save_kMusicName = "BigxuOnl_Music";
	public const string save_kVibrationName = "BigxuOnl_Vibration";
	public const string save_kVersionDataName = "BigxuOnl_VersionData";

	public const string save_kRatingApp = "BigxuOnl_RatingApp";
	public const string save_kCountTimeShowPopupRemindRating = "BigxuOnl_CountTimeShowRemindRating";
	public const string save_kCountTimePressOkOnPopupRating = "BigxuOnl_CountTimePressOkOnPopupRating";
	public const string save_kNextTimeCanShowPopupRemindRating = "BigxuOnl_NextTimeCanShowPopupRemindRating";
	#endregion

	#region Version Info
	public const long featureVersionCore = 20190109;
	#endregion
	
	#region Link
	public static string linkApp{
		get{
			#if UNITY_EDITOR
				return "https://play.google.com/store/apps/details?id=" + Application.identifier;
				// return "https://sites.google.com/view/bigxu-online/home";
			#elif UNITY_ANDROID
				return "market://details?id=" + Application.identifier;
               	// return "market://details?id=com.facebook.katana";
			#elif UNITY_IOS
				return("itms-apps://itunes.apple.com/app/id" + Application.identifier);
			#else
				return "https://sites.google.com/view/bigxu-online/home";
			#endif
		}
	}
	public const string linkDetectConnectInfo = "http://ip-api.com/json";
	public const string linkContactUs = "https://sites.google.com/view/bigxu-online/home/help-center";
	public const string linkPrivacyPolicy = "https://www.bigxuonline.com/home/help-center/privacy-policy";
	public const string linkTermOfService = "https://www.bigxuonline.com/home/help-center/term-of-service";
	#endregion

	// #region Product ID
	// public static string kProductID_Gold_Package1USD = "1_usd";
	// public static string kProductID_Gold_Package2USD = "2_usd";
	// public static string kProductID_Gold_Package5USD = "5_usd";
	// public static string kProductID_Gold_Package10USD = "10_usd";
	// public static string kProductID_Gold_Package20USD = "20_usd";
	// public static string kProductID_Gold_Package50USD = "50_usd";
	// public static string kProductID_Gold_Package100USD = "100_usd";
	// public static string kProductID_Gold_Package200USD = "200_usd";
	// public static string kProductID_Gold_Package400USD = "400_usd";
	// #endregion

	#region Scene Name
	public const string SCENE_HOME = "Home";
	public const string SCENE_SUBGAMEPLAY = "SubGamePlay";
	public const string SCENE_POKER_PLAY = "Poker_GamePlay";
	public const string SCENE_ANIMALRACING_PLAY = "AnimalRacing_GamePlay";
	public const string SCENE_BATTLE_PLAY = "Battle_Scene";
	public const string SCENE_UNO_PLAY = "Uno_GamePlay";
	#endregion

	#region Scene Name
	public const string SORTINGLAYERNAME_DEFAULT = "Default";
	public const string SORTINGLAYERNAME_CONSUMABLESCREEN = "ConsumableScreen";
	#endregion

	public static bool IsAvailableUserNameAndPass(this string _text){
		if (string.IsNullOrEmpty (_text)) {
			return false;
		}
		
		// string _textCensor = _text.ToLower();
		// for (int i = 0; i < CoreGameManager.instance.gameInfomation.userNameFilterInfo.listSpecialChars.Count; i++) {
		// 	_textCensor = _textCensor.Replace (CoreGameManager.instance.gameInfomation.userNameFilterInfo.listSpecialChars[i], "*");
		// }
		// if (_textCensor.Contains ("*")) {
		// 	return false;
		// }
		return true;
	}

	public static bool IsAvailableEmail(this string _text){
		if (!_text.Contains ("@")) {
			return false;
		}
		return true;
	}

	/// <summary>
	/// Xáo trộn vị trí trong mảng có độ dài length
	/// </summary>
	public static List<int> RandomViTri (int _length)
	{
		if (_length == 0) {
			return null;
		}
		List<int> A = new List<int> ();
		for (int i = 0; i < _length; i++) {
			A.Add (i);
		}

		List<int> B = new List<int> ();
		while (A.Count > 0) {
			int vitri = Random.Range (0, A.Count);
			B.Add (A [vitri]);
			A.RemoveAt (vitri);
		}

		return B;
	}

	public static List<int> LayViTriRandomTrongMang (int _sovitri, int _lengthList)
	{
		if (_lengthList == 0) {
			return null;
		}
		List<int> A = new List<int> ();
		for (int i = 0; i < _lengthList; i++) {
			A.Add (i);
		}

		List<int> B = new List<int> ();
		int _tmp = 0;
		while (_tmp < _sovitri
			&& _tmp < _lengthList) {
			int vitri = Random.Range (0, A.Count);
			B.Add (A [vitri]);
			A.RemoveAt (vitri);
			_tmp++;
		}

		return B;
	}

	public static Vector3 ColisionPoint (Vector3 sphere1, Vector3 sphere2, float sphere1radius)
	{
		Vector3 Direction = (sphere2 - sphere1).normalized;
		Vector3 ColisionPoint = sphere1 + (Direction * sphere1radius);
		return ColisionPoint;
	}

//	public static string convertMoneyToLocalLocale (double money)
//	{
//		//		CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
//		CultureInfo elGR = CultureInfo.CurrentUICulture;
//		string moneyStr = string.Format (elGR, "{0:N0}", money);
//		return moneyStr;
//	}

	public static bool IsOverLap (Vector2 posA, Vector2 posB, Rect b)
	{
		float xoA = posA.x;
		float yoA = posA.y;
		float xoB = posB.x + b.x;
		float yoB = posB.y + b.y;

		return ((xoB > xoA ? xoB - xoA : xoA - xoB) <= ((b.width + 0.1f) / 2) && (yoB > yoA ? yoB
			- yoA
			: yoA - yoB) <= ((b.height + 0.1f) / 2));
	}

	public static bool IsOverLap (Vector3 posA, Rect a, Vector3 posB, Rect b)
	{
		float xoA = posA.x + a.x;
		float yoA = posA.y + a.y;
		float xoB = posB.x + b.x;
		float yoB = posB.y + b.y;

		return ((xoB > xoA ? xoB - xoA : xoA - xoB) <= ((b.width + a.width) / 2) && (yoB > yoA ? yoB
			- yoA
			: yoA - yoB) <= ((b.height + a.height) / 2));
	}

	public static bool IsOverLap (Circle a, Circle b)
	{
		var dx = a.point.x - b.point.x;
		var dy = a.point.y - b.point.y;

		return Mathf.Sqrt (dx * dx + dy * dy) <= a.radius + b.radius;
	}

	public static bool IsOverLap (Circle a, Rect b)
	{
		var px = a.point.x;
		var py = a.point.y;

		if (px < b.xMin)
			px = b.xMin;
		else if (px > b.xMax)
			px = b.xMax;

		if (py < b.yMin)
			py = b.yMin;
		else if (py > b.yMax)
			py = b.yMax;

		var dx = a.point.x - px;
		var dy = a.point.y - py;

		return (dx * dx + dy * dy) <= a.radius * a.radius;
	}

	public static bool isVector3Valid (Vector3 v)
	{
		if (float.IsNaN (v.x)
			|| float.IsNaN (v.y)
			|| float.IsNaN (v.z)) {
			return false;
		}
		return true;
	}

	public static T GetAssest<T> (string _path) where T : Object
	{
		if (_path.Equals ("")) {
			return null;
		}
		return Resources.Load<T> (_path);
	}

	public static bool IsOutOfCamera (MyCameraController _camera, Vector3 _point, Vector2 _size)
	{
		Vector3 _posCam = _camera.transform.position;
		Vector2 _sizeCam = _camera.sizeOfCamera;
		if (_point.x + _size.x / 2 < _posCam.x - _sizeCam.x / 2
			|| _point.x - _size.x / 2 > _posCam.x + _sizeCam.x / 2
			|| _point.y + _size.y / 2 < _posCam.y - _sizeCam.y / 2
			|| _point.y - _size.y / 2 > _posCam.y + _sizeCam.y / 2) {
			return true;
		}
		return false;
	}

	public static bool IsOutOfRange (Vector3 _point, Vector2 _range, Vector2 _size)
	{
		Vector3 _posCam = Vector3.zero;
		if (_point.x + _size.x / 2 < _posCam.x - _range.x / 2
			|| _point.x - _size.x / 2 > _posCam.x + _range.x / 2
			|| _point.y + _size.y / 2 < _posCam.y - _range.y / 2
			|| _point.y - _size.y / 2 > _posCam.y + _range.y / 2) {
			return true;
		}
		return false;
	}

	public static string ToFixed (this float number, uint decimals)
	{
		return number.ToString ("N" + decimals);
	}

	public static void TweenLookAt (this Transform _myTransform, Vector2 _des, float _rotSpeed)
	{
		Vector2 _dir = _des - (Vector2)_myTransform.position;
		float _zAngle = Mathf.Atan2 (_dir.y, _dir.x) * Mathf.Rad2Deg - 90;
		Quaternion _desiredRot = Quaternion.Euler (0, 0, _zAngle);
		_myTransform.rotation = Quaternion.RotateTowards (_myTransform.rotation, _desiredRot, _rotSpeed * Time.fixedDeltaTime);
	}

	public static void TweenMoveAtObjBaseOnCompass (this Transform _myTransform, Transform _compass, float _speedMove)
	{
		Vector3 _pos = _myTransform.position;
		Vector3 _velocity = new Vector3 (0, _speedMove * Time.fixedDeltaTime, 0f); // di chuyển theo y
		_pos += (_compass.rotation * _velocity);
		_myTransform.position = _pos;
	}

	#region Compare 2 Vector

	public static bool V2Equal (Vector2 a, Vector2 b)
	{
		return Vector2.SqrMagnitude (a - b) < 0.0001f;
	}

	#endregion

	public static long currentTimeMilliseconds { get { return (long)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds; } }

	public static string GetMoneyString(long _gold, long _minGoldCheck = 0){
		if(_gold < 10){
			return _gold.ToString();
		}
		if(_gold <= _minGoldCheck){
			return string.Format("{0:0,0}", _gold);
		}
		// Debug.Log(">>>> " + _gold);
        string _result = "";
        if(_gold<1000){
            _result = _gold+"";
        }else if (_gold < 1000000){
            _result = (_gold / 1000) + "";
            _gold = (_gold % 1000)/10;
            if (_gold > 0)
                if (_gold > 9)
					if (_gold % 10==0)
						_result = _result + "." + (_gold/10) + "K";
					else
                    	_result = _result + "." + _gold + "K";
                else
                    _result = _result + "." + "0" + _gold + "K";
            else
                _result = _result + "K";
        }else if (_gold < 1000000000){
            _result = (_gold / 1000000) + "";
            _gold = (_gold % 1000000)/10000;
            if (_gold > 0)
                if (_gold > 9)
					if (_gold % 10==0)
						_result = _result + "." + (_gold/10) + "M";
					else
                    	_result = _result + "." + _gold + "M";
                else
                    _result = _result + "." + "0" + _gold + "M";
            else
                _result = _result + "M";
        }else if (_gold < 1000000000000){
            _result = (_gold / 1000000000) + "";
            _gold = (_gold % 1000000000)/10000000;
            if (_gold > 0)
                if (_gold > 9)
                    if (_gold % 10==0)
						_result = _result + "." + (_gold/10) + "B";
					else
                    	_result = _result + "." + _gold + "B";
                else
                    _result = _result + "." + "0" + _gold + "B";
            else
                _result = _result + "B";
		}else if (_gold < 1000000000000000){
            _result = (_gold / 1000000000000) + "";
            _gold = (_gold % 1000000000000)/10000000000;
            if (_gold > 0)
                if (_gold > 9)
                    if (_gold % 10==0)
						_result = _result + "." + (_gold/10) + "T";
					else
                    	_result = _result + "." + _gold + "T";
                else
                    _result = _result + "." + "0" + _gold + "T";
            else
                _result = _result + "T";
		}else{
            _result = (_gold / 1000000000000000) + "";
            _gold = (_gold % 1000000000000000)/10000000000000;
            if (_gold > 0)
                if (_gold > 9)
                    if (_gold % 10==0)
						_result = _result + "." + (_gold/10) + "Q";
					else
                    	_result = _result + "." + _gold + "Q";
                else
                    _result = _result + "." + "0" + _gold + "Q";
            else
                _result = _result + "Q";
		}
        return _result;
    }

	public static long GetGoldPrefer(long _gold){
		long _result = 0;
		if(_gold<=0){
			return _result;
		}
		_result = 1;
		while(_result<=500000000000){
			if(_result*2>_gold){
				return _result;
			}
			if(_result*5>_gold){
				return _result*2;
			}
			if(_result*10>_gold){
				return _result*5;
			}
			_result=_result*10;
		}
		_result = 500000000000;
		return _result;
	}

	///<summary>
	/// Focus tới 1 phần tử trong scroll rect nằm dọc
	///</summary>
	public static void ScrollRectVerticalFocusCenterItem(ScrollRect _scrollRect, GameObject _item){
		if(_item == null){
			_scrollRect.verticalNormalizedPosition = 0f;
			return;
		}
        float halfViewportHeight = _scrollRect.viewport.rect.height / 2;
        float contentHeight = _scrollRect.content.rect.height;

        float localStartPoint = -_scrollRect.content.pivot.y * contentHeight + halfViewportHeight;
        float localEndPoint = (1 - _scrollRect.content.pivot.y) * contentHeight - halfViewportHeight;
        Vector3 itemlocalPosition = _scrollRect.content.transform.InverseTransformPoint(_item.transform.position);

        float normalizedPosition = Mathf.InverseLerp(localStartPoint, localEndPoint, itemlocalPosition.y);

        _scrollRect.verticalNormalizedPosition = normalizedPosition;
    }

	///<summary>
	/// Focus tới 1 phần tử trong scroll rect nằm ngang
	///</summary>
	public static void ScrollRectHorizontalFocusCenterItem(ScrollRect _scrollRect, GameObject _item){
		if(_item == null){
			_scrollRect.horizontalNormalizedPosition = 0f;
			return;
		}
        float halfViewportWidth = _scrollRect.viewport.rect.width / 2;
        float contentWidth = _scrollRect.content.rect.width;

        float localStartPoint = -_scrollRect.content.pivot.x * contentWidth + halfViewportWidth;
        float localEndPoint = (1 - _scrollRect.content.pivot.x) * contentWidth - halfViewportWidth;
        Vector3 itemlocalPosition = _scrollRect.content.transform.InverseTransformPoint(_item.transform.position);

        float normalizedPosition = Mathf.InverseLerp(localStartPoint, localEndPoint, itemlocalPosition.x);

        _scrollRect.horizontalNormalizedPosition = normalizedPosition;
    }

	/// <summary>
	/// Returns a normalized direction vector pointing at target from origin
	/// </summary>
	public static Vector3 DirectionVector(Vector3 _origin, Vector3 _target) {
		return (_target - _origin).normalized;
	}

	public static string ConvertString(string _text, int _maxLength){
		string _result = _text;
		if(_result.Length > _maxLength){
			_result = _result.Substring(0, _maxLength - 3) + "...";
		}
		return _result;
	}

	/// <summary>
	/// GetListChipInfo: Hàm trả về list chip tương ứng với số tiền truyền vào
	/// </summary>
	public static List<IChipInfo> GetListChipInfo(List<IChipInfo> _listChipInfo, long _gold){
		if(_listChipInfo == null || _listChipInfo.Count == 0 || _gold == 0){
			return null;
		}
		long _tmpGold = _gold;
		List<IChipInfo> _tmpListChipInfo = new List<IChipInfo>();
		for(int i = 0; i < _listChipInfo.Count; i ++){
			if(_listChipInfo[i].value <= _tmpGold){
				_tmpListChipInfo.Add(_listChipInfo[i]);
			}
		}

		List<IChipInfo> _result = new List<IChipInfo>();
		for(int i = _tmpListChipInfo.Count - 1; i >= 0 && i < _tmpListChipInfo.Count; i --){
			if(_tmpListChipInfo[i].value <= _tmpGold){
				_tmpGold -= _tmpListChipInfo[i].value;
				_result.Add(_tmpListChipInfo[i]);
				if(_tmpGold <= 0) {
					break;
				}

				i++;
				continue;
			}else{
				_tmpListChipInfo.RemoveAt(i);
				i++;
				continue;
			}
		}
		
		return _result;
	}

	public static IEnumerator DownloadIcon(string url, System.Action<Texture2D> _onFinished) {
		using (WWW www = new WWW(url))
        {
            // Wait for download to complete
            yield return www;
			if(_onFinished != null){
				_onFinished(www.texture);
			}
			www.Dispose();
        }
		// // Start a download of the given URL
		// var www = new WWW(url);
		// // wait until the download is done
		// yield return www;
		// // Create a texture in DXT1 format
		// Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);

		// // assign the downloaded image to sprite
		// www.LoadImageIntoTexture(texture);
		
		// Rect rec = new Rect(0, 0, texture.width, texture.height);
		// Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
		// imageToDisplay.sprite = spriteToUse;
		// www.Dispose();
		// www = null;
	}

	public static bool IsAppInstalled(string bundleID){
		#if UNITY_EDITOR
		return true;
		#endif
		#if UNITY_ANDROID
		AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
		Debug.Log(" ********LaunchOtherApp ");
		AndroidJavaObject launchIntent = null;
		//if the app is installed, no errors. Else, doesn't get past next line
		try{
			launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage",bundleID);
			//        
			//        ca.Call("startActivity",launchIntent);
		}catch(System.Exception ex){
			Debug.Log("exception"+ex.Message);
		}
		if(launchIntent == null)
			return false;
		return true;
		#else
		return false;
		#endif
	}

	public static IEnumerator TweenValue(long _fromValue, long _toValue, int _tick, System.Action<long> _onUpdate = null, System.Action<long> _onFinished = null) {
		double _virtualValue = _fromValue;
		long _realValue = _toValue;
		double _deltaValue = (_realValue - _virtualValue) / _tick;
		if(_deltaValue < 2&&_deltaValue>=0){
			if(_onFinished != null){
				_onFinished(_realValue);
			}
			yield break;
		}
			
		while(true){
			yield return Yielders.Get(0.05f);
            _virtualValue += _deltaValue;
			if(_onUpdate != null){
				_onUpdate((long) _virtualValue);
			}
            if(_deltaValue < 0){
                if(_virtualValue < _realValue){
                    _virtualValue = _realValue;
                    break;
                }
            }else if(_deltaValue > 0){
                if(_virtualValue > _realValue){
                    _virtualValue = _realValue;
                    break;
                }
            }else{
                _virtualValue = _realValue;
                break;
            }
		}
		if(_onFinished != null){
			_onFinished(_realValue);
		}
	}

	public static IEnumerator DoActionShowEffectGoldFly(GameObject _prefab, MySimplePoolManager _poolManager, MySortingLayerInfo _sortingLayerInfo
			, Vector2 _startPoint, Vector2 _endPoint, int _numGold, float _ratioScale, float _timeMovePerGold, System.Action _onPerGoldObjFinished = null){
		Vector2 _newStartPoint = Vector2.zero;
		for(int i = 0; i < _numGold; i++){
			_newStartPoint.x = Random.Range(_startPoint.x - 0.2f, _startPoint.x + 0.2f);
			_newStartPoint.y = Random.Range(_startPoint.y - 0.2f, _startPoint.y + 0.2f);

			GoldObjectController _gold = LeanPool.Spawn(_prefab, _newStartPoint, Quaternion.identity).GetComponent<GoldObjectController>();
			_poolManager.AddObject(_gold);
			_gold.InitData(_sortingLayerInfo, _ratioScale);
			_gold.MoveAndSelfDestruction(_endPoint, _timeMovePerGold, LeanTweenType.easeInBack, _onPerGoldObjFinished);
			if(_numGold > 1){
				yield return Yielders.Get(0.06f);
			}
		}
	}

	public static IEnumerator DoActionShowPopupWinGold(GameObject _prefab, MySimplePoolManager _poolManager, Transform _parent, Vector2 _pos, float _timeDelay, long _goldAdd, System.Action _onFinished = null){
		yield return Yielders.Get(_timeDelay);
		PanelBonusGoldInGameController _tmpPanelGoldBonus = LeanPool.Spawn(_prefab, _pos, Quaternion.identity, _parent).GetComponent<PanelBonusGoldInGameController>();
		_poolManager.AddObject(_tmpPanelGoldBonus);
		_tmpPanelGoldBonus.transform.position = _pos;
		_tmpPanelGoldBonus.Show(_goldAdd);

		if(_onFinished != null){
			_onFinished();
		}
	}
}