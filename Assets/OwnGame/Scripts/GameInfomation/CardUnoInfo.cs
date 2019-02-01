using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardUnoInfo", menuName="GameInfo/CardUnoInfo")]
public class CardUnoInfo : ScriptableObject {

	public enum CardType{
		_Red_0, _Red_1, _Red_2, _Red_3, _Red_4, _Red_5, _Red_6, _Red_7, _Red_8, _Red_9, _Red_Draw2Cards, _Red_Skip, _Red_Reverse,
		_Green_0, _Green_1, _Green_2, _Green_3, _Green_4, _Green_5, _Green_6, _Green_7, _Green_8, _Green_9, _Green_Draw2Cards, _Green_Skip, _Green_Reverse,
		_Blue_0, _Blue_1, _Blue_2, _Blue_3, _Blue_4, _Blue_5, _Blue_6, _Blue_7, _Blue_8, _Blue_9, _Blue_Draw2Cards, _Blue_Skip, _Blue_Reverse,
		_Yellow_0, _Yellow_1, _Yellow_2, _Yellow_3, _Yellow_4, _Yellow_5, _Yellow_6, _Yellow_7, _Yellow_8, _Yellow_9, _Yellow_Draw2Cards, _Yellow_Skip, _Yellow_Reverse,
		_Special_Draw4Cards, _Special_WildColor,
		_CardCover
	}
	public CardType cardType;
	public Sprite imgMainIcon;
	public string strValue; // nếu = -1 thì sử dụng mainIcon hiện lên miniIcon
	public Color colorValue;
	public Color colorImgOval;

}
[System.Serializable] public class CardUnoDetail {
	public CardUnoInfo cardInfo;
	public int cardId;
}