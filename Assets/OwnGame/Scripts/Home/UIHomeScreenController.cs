using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHomeScreenController : MySimplePoolObjectController {

	public enum UIType{
		Unknown,
		SplashScreen, LoginScreen, RegisterScreen, ChooseGame, ChooseTable, GetGold,
		SettingScreen, UserDetail, LuckyWheel, Leaderboard, SubGame
	}

	public virtual UIType myType{
		get{ 
			return UIType.Unknown;
		}
	}

	public enum State{
		Hide,
		Show
	}
	public State currentState{ get; set;}

	public virtual bool isSubScreen{
		get{ 
			return false;
		}
	}
	public UIType myLastType{ get; set;} // dùng cho subScreen

	public System.Action onPressBack; // xử lý tình huống bấm nút back

	public virtual void InitData (){}

	///<summary>
	/// LateInitData: sử dụng cho các hàm load dữ liệu thật từ data về
	///</summary>
	public virtual void LateInitData (){} // sử dụng cho các hàm load dữ liệu thật từ data về
	public virtual void RefreshData (){}

	#region Show And Hide
	public virtual void Show ()
	{
		currentState = State.Show;
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
	}

	public virtual void Hide()
	{
		StopAllCoroutines();
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		myLastType = UIType.Unknown;
		onPressBack = null;
	}
	#endregion

	public CanvasGroup myCanvasGroup;
}
