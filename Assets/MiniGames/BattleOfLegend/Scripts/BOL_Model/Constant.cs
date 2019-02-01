using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.CodeDom;
using System.Runtime.CompilerServices;
using Lean.Pool;

public class Constant {
	//IP,PortTest
	public const string IP_CONECT = "bigxuonline.com";
	public const int PORT = 1168;
	public const int PORT_REALTIME = 3389;
	//
	public const string rootSaveName = "BigxuOFF_DataInfo.dat";
	public const byte CHAIR_LEFT = 0;
	public const byte CHAIR_RIGHT = 1;
	public const byte CHAIR_LEAVE = 2;
	public const byte CHAIR_VIEWER = 3;
	public const byte CHAIR_PLAYER = 4;
	public const int ROW = 12;
	public const int COL = 8;

	public const string idle = "Idle";
	public const string attack1 = "Attack1";
	public const string attack2 = "Attack2";
	public const string attackQ = "AttackQ";
	public const string attackW = "AttackW";
	public const string attackE = "AttackE";
	public const string dash1 = "Dash";
	public const string dash2 = "Dash2";

	public enum PieceIngame
	{
		none,
		attack_1,
		attack_2,
		health,
		mana,
		shield,
		special
	}
	
	public static void ActiveObject(GameObject gameObject, bool boolean, float alphaObject = 0) {
		gameObject.SetActive(boolean);
		if (gameObject.GetComponent<CanvasGroup>() != null) {
			CanvasGroup canvasGroupObject = gameObject.GetComponent<CanvasGroup>();
			canvasGroupObject.interactable = boolean;
			canvasGroupObject.blocksRaycasts = boolean;
			if (alphaObject != 0) {
				canvasGroupObject.alpha = alphaObject;
			} else {
				canvasGroupObject.alpha = Convert.ToInt32(boolean);
			}

		} else {
#if TEST
			Debug.Log("gameobject không có canvas");
#endif
		}
	}
	public static void ConverLongToHour(Text textShow, long timeShow) {
		TimeSpan t = TimeSpan.FromMilliseconds(timeShow);
		string dateTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
		textShow.text = dateTime;
	}
	public static void ConverLongToDateTime(Text textShow, long timeShow) {
		DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		DateTime date = start.AddMilliseconds(timeShow).ToLocalTime();
		textShow.text = date.ToString();
	}
    public static double ConvertMillisecondsToSeconds(double milliseconds)
    {
        return TimeSpan.FromMilliseconds(milliseconds).TotalSeconds;
    }
	public enum Databaseid {
		DATABASEID_DEVICE,
		DATABASEID_BIGXU,
		DATABASEID_FACEBOOK,
		DATABASEID_GOOGLE,
		DATABASEID_TWITTER,
		DATABASEID_ZALO,
		DATABASEID_CHINA
	}
	public enum Hero_Status {
		Idle,
		Attack1,
		Attack2,
		Skill1,
		Skill2,
		Special,
		Spell
	}
}
