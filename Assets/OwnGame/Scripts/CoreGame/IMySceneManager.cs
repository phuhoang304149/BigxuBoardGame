using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMySceneManager : MonoBehaviour {
	public enum Type{
		Home = 0,
		AnimalRacingGameplay,
		BolGamePlay,
		PokerGamePlay,
		Uno,
		SubGamePlayScene = 999
	}
	public virtual Type mySceneType{
		get{
			return Type.Home;
		}
	}
	public bool canShowScene;
	public MyCameraController mainCamera;
	public MyCameraController cameraForConsumableScreen;

	public virtual void RefreshAgainWhenCloseSubGamePlay(){}
}
