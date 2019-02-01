using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardInfo", menuName="GameInfo/CardInfo")]
public class ICardInfo : ScriptableObject {
	public enum CardType{
		_A_1, _A_2, _A_3, _A_4, 
		_2_1, _2_2, _2_3, _2_4, 
		_3_1, _3_2, _3_3, _3_4,
		_4_1, _4_2, _4_3, _4_4,
		_5_1, _5_2, _5_3, _5_4,
		_6_1, _6_2, _6_3, _6_4,
		_7_1, _7_2, _7_3, _7_4,
		_8_1, _8_2, _8_3, _8_4,
		_9_1, _9_2, _9_3, _9_4,
		_10_1, _10_2, _10_3, _10_4,
		_J_1, _J_2, _J_3, _J_4,
		_Q_1, _Q_2, _Q_3, _Q_4,
		_K_1, _K_2, _K_3, _K_4,
	}
	public CardType cardType;
	public string strValue;
	public Color colorValue;
	public Sprite imgKind;

}

[System.Serializable] public class CardDetail {
	public ICardInfo cardInfo;
	public int cardId;
}