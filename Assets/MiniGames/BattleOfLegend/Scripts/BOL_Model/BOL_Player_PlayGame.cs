using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOL_Player_PlayGame  {
	public static BOL_Player_PlayGame instance {
        get {
            if (ins == null) {
				ins = new BOL_Player_PlayGame();
            }
            return ins;
        }
    }

	static BOL_Player_PlayGame ins;
	public BOL_Player_PlayGame() { }

    public static void SelfDestruction() {
        ins = null;
    }
	#region variable
    public static long bet;
    public static short mapid;
    public static long TIME_PLAY;
    public static sbyte status;

    public static short sessionid0;
    public static sbyte characterid0;
    public static short sessionid1;
    public static sbyte characterid1;
    // player 1 chair2: left;
    public static short sessionid;
    public static sbyte leftdatabaseid;
    public static long leftuserid;
    public static sbyte leftavatarids;
    public static long leftGOLD;
    public static string leftnameShow;
    public static int leftwin;
    public static int lefttie;
    public static int leftdraw;
    public static sbyte leftcharackterID;
    public static short lefthp;
    public static short leftmana;
    public static short leftdamgeAttack1;
    public static short leftdamgeAttack2;
    public static short leftshield;
    public static sbyte leftspell;
    public static short leftmanaOfSkill1;
    public static long leftdelayOfSkill1;
    public static short leftmanaOfSkill2;
    public static long leftdelayOfSkill2;
    public static short leftmanaOfUtimate;
    public static long leftdelayOfUtimate;
    public static short leftmax_hp;
    public static short leftmax_mana;
    public static short leftmax_shield;
    public static long lefttimeDelaySpell;
    public static long lefttimeDelaySkill1;
    public static long lefttimeDelaySkill2;
    public static long lefttimeDelayUltimate;
    public static long lefteffect1_time_stun;
    public static long lefteffect2_time_doNotRecoverHP;
    public static long lefteffect3_time_upDamage;
    public static short lefteffect3_value_upDamage;
    public static long lefteffect4_time_slowMatrix;
    public static long lefteffect5_time_doNotMovePiece;
    public static long lefteffect6_time_doNotChangePiece;
    public static long lefteffect7_time_disableSpell;
    public static long lefteffect8_time_freeze;
    public static long lefteffect9_time_speedupMatrix;
    public static long lefteffect10_time_hypnosis_pieceMoveBackwards;
    public static long lefteffect11_time_backDamage;
    public static long lefteffect12_time_avoidDamage;
    public static long lefteffect13_time_downDamage;
    public static short lefteffect13_value_downDamage;
    public static long lefteffect14_time_virtual_hp;
    public static short lefteffect14_value_virtual_hp;
    public static long lefteffect15_time_bloodsucking;
    public static short lefteffect15_perthousand_bloodsucking;
    public static long lefteffect16_time_avoidEffect;
    public static sbyte[] leftcurrentPiece;
    public static sbyte[] leftnextPiece;
    public static sbyte[,] leftmatrix = new sbyte[Constant.ROW, Constant.COL];
    // player 2 chair4: right;
    public static short sessionid1s;
    public static sbyte rightdatabaseid;
    public static long rightuserid;
    public static sbyte rightavatarids;
    public static long rightGOLD;
    public static string rightnameShow;
    public static int rightwin;
    public static int righttie;
    public static int rightdraw;
    public static sbyte rightcharackterID;
    public static short righthp;
    public static short rightmana;
    public static short rightdamgeAttack1;
    public static short rightdamgeAttack2;
    public static short rightshield;
    public static sbyte rightspell;
    public static short rightmanaOfSkill1;
    public static long rightdelayOfSkill1;
    public static short rightmanaOfSkill2;
    public static long rightdelayOfSkill2;
    public static short rightmanaOfUtimate;
    public static long rightdelayOfUtimate;
    public static short rightmax_hp;
    public static short rightmax_mana;
    public static short rightmax_shield;
    public static long righttimeDelaySpell;
    public static long righttimeDelaySkill1;
    public static long righttimeDelaySkill2;
    public static long righttimeDelayUltimate;
    public static long righteffect1_time_stun;
    public static long righteffect2_time_doNotRecoverHP;
    public static long righteffect3_time_upDamage;
    public static short righteffect3_value_upDamage;
    public static long righteffect4_time_slowMatrix;
    public static long righteffect5_time_doNotMovePiece;
    public static long righteffect6_time_doNotChangePiece;
    public static long righteffect7_time_disableSpell;
    public static long righteffect8_time_freeze;
    public static long righteffect9_time_speedupMatrix;
    public static long righteffect10_time_hypnosis_pieceMoveBackwards;
    public static long righteffect11_time_backDamage;
    public static long righteffect12_time_avoidDamage;
    public static long righteffect13_time_downDamage;
    public static short righteffect13_value_downDamage;
    public static long righteffect14_time_virtual_hp;
    public static short righteffect14_value_virtual_hp;
    public static long righteffect15_time_bloodsucking;
    public static short righteffect15_perthousand_bloodsucking;
    public static long righteffect16_time_avoidEffect;
    public static sbyte[] rightcurrentPiece;
    public static sbyte[] rightnextPiece;
    public static sbyte[,] rightmatrix = new sbyte[Constant.ROW, Constant.COL];
    #endregion

}
