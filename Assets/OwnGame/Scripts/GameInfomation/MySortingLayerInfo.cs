using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class MySortingLayerInfo{
	public enum MyLayerName{
		Default, 
		CharactorInGame,
		ChatScreen,
		ConsumableScreen,
		LoadingCanvas,
		SceneLoader,
		Popup
	}
	public MyLayerName layerName;
	public int layerOrderId;
}