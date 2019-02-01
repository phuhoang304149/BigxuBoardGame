using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDataPlayGame : MonoBehaviour {
	public static ShowDataPlayGame instance {
		get {
			return ins;
		}
	}
	static ShowDataPlayGame ins;
	public long bets;
	public short mapID;
	public long timePlay;
	public ShowDataplayer Left;
	public ShowDataplayer Right;
	private void Awake() {
		ins = this;
	}
}
[System.Serializable]
public class ShowDataplayer {

	public short sessionid;
	public sbyte databaseid;
	public long userid;
	public sbyte avatarid;
	public long GOLD;
	public string nameShow;
	public int win;
	public int tie;
	public int draw;
    public long facebookid;
    public string stringicon;
    
	public sbyte charackterID;
	public short hp;
	public short mana;
	public short damgeAttack1;
	public short damgeAttack2;
	public short shield;
	public short spell;
	public short manaOfSkill1;
	public long delayOfSkill1;
	public long manaOfSkill2;
	public long delayOfSkill2;
	public long manaOfUtimate;
	public long delayOfUtimate;
	public short max_hp;
	public short max_mana;
	public short max_shield;
	public long timeDelaySpell;
	public long timeDelaySkill1;
	public long timeDelaySkill2;
	public long timeDelayUltimate;
	public long effect1_time_stun;
	public long effect2_time_doNotRecoverHP;
	public long effect3_time_upDamage;
	public long effect3_value_upDamage;
	public long effect4_time_slowMatrix;
	public long effect5_time_doNotMovePiece;
	public long effect6_time_doNotChangePiece;
	public long effect7_time_disableSpell;
	public long effect8_time_freeze;
	public long effect9_time_speedupMatrix;
	public long effect10_time_hypnosis_pieceMoveBackwards;
	public long effect11_time_backDamage;
	public long effect12_time_avoidDamage;
	public long effect13_time_downDamage;
	public long effect13_value_downDamage;
	public long effect14_time_virtual_hp;
	public long effect14_value_virtual_hp;
	public long effect15_time_bloodsucking;
	public long effect15_perthousand_bloodsucking;
	public long effect16_time_avoidEffect;
	public sbyte[] currentPieces;// show lúc bắt đầu game
	public sbyte[] nextPieces;
}
