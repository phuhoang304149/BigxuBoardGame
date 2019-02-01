using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameBetToWinInfo", menuName="GameInfo/GameBetToWinInfo")]
public class IGameBetToWinInfo : ScriptableObject  {
	public long betDefault;
	public List<long> bet;
	public List<BetToWinValueDetail> listDetail;
}

[System.Serializable] public class BetToWinValueDetail{
	public int id;
	public int weight; // trọng số (biến cũ là numplay)
	public float ratioWin;
}