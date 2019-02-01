public class CMD_REALTIME {
	public const short S_DATA_INVALIDATE = 1;
	public const short C_TEST = 2;
	public const short S_ALERT_UPDATE_SERVER = 99;
	public const short S_SERVER_DEBUG_LOG = 999;
	public const short C_ADMIN_PLAYNOW_TO_TABLE = 1000;
	public const short S_ADMIN_PLAYNOW_TO_TABLE = 1001;
	public const short C_ADMIN_JOIN_TO_TABLE = 1002;
	public const short S_ADMIN_JOIN_TO_TABLE = 1003;
	public const short Game_Get_Bigxu_Version = 2000;
	public const short C_GAMEPLAY_SET_PARENT = 2500;
	public const short S_GAMEPLAY_SET_PARENT = 2501;
	public const short S_ON_PLAYER_UPDATE_GOLD = 2600;
	public const short S_onPlayerAddGold = 2601;
	public const short C_TABLE_PLAYNOW_TO_TABLE = 2620;
	public const short S_TABLE_PLAYNOW_TO_TABLE = 2621;
	public const short C_TABLE_JOIN_TO_TABLE = 2622;
	public const short S_TABLE_JOIN_TO_TABLE = 2623;
	public const short C_TABLE_CREATE_PASSWORD_TABLE = 2625;
	public const short S_TABLE_CREATE_PASSWORD_TABLE = 2626;
	public const short C_TABLE_JOIN_TO_MINIGAME_STATE = 2650;
	public const short S_TABLE_JOIN_TO_MINIGAME_STATE = 2651;
	public const short S_GAMEPLAY_PLAYER_JOIN_GAME = 2689;
	public const short S_GAMEPLAY_TABLE_DATA = 2690;
	public const short C_GAMEPLAY_GET_TABLE_INFO = 2692;
	public const short S_GAMEPLAY_GET_TABLE_INFO = 2693;
	public const short C_GAMEPLAY_GET_TABLE_STATUS = 2695;
	public const short S_GAMEPLAY_GET_TABLE_STATUS = 2696;
	public const short S_GAMEPLAY_SERVER_UPDATE_LIST_PLAYING = 2697;
	public const short S_GAMEPLAY_SERVER_UPDATE_LIST_CHAIRS = 2698;
	public const short S_GAMEPLAY_SERVER_UPDATE_LIST_VIEWER = 2699;
	public const short C_GAMEPLAY_LEFT_TABLE = 2700;
	public const short S_GAMEPLAY_PLAYER_LEFT_GAME = 2701;
	public const short C_GAMEPLAY_PLAYER_SET_TABLE_PASSWORD = 2710;
	public const short S_GAMEPLAY_PLAYER_SET_TABLE_PASSWORD = 2711;
	public const short C_GAMEPLAY_SITDOWN = 2800;
	public const short S_GAMEPLAY_SITDOWN = 2801;
	public const short S_GAMEPLAY_PLAYER_SITDOWN = 2802;
	public const short C_GAMEPLAY_STANDUP = 2803;
	public const short S_GAMEPLAY_STANDUP = 2804;
	public const short S_GAMEPLAY_PLAYER_STANDUP = 2805;
	public const short C_GAMEPLAY_INVITE_ROBOT = 2809;
	public const short C_BetToWin_NoAchievement = 2820;
	public const short S_BetToWin_NoAchievement = 2821;
	public const short C_GAMEPLAY_GET_CARD = 2830;
	public const short S_GAMEPLAY_GET_CARD = 2831;
	public const short S_GAMEPLAY_PLAYER_GET_CARD = 2832;
	public const short C_GAMEPLAY_PUT_CARD = 2833;
	public const short S_GAMEPLAY_PUT_CARD = 2834;
	public const short S_GAMEPLAY_PLAYER_PUT_CARD = 2835;
	public const short C_GAMEPLAY_END_TURN = 2836;
	public const short S_GAMEPLAY_END_TURN = 2837;
	public const short C_GAMEPLAY_CALL_WIN = 2838;
	public const short S_GAMEPLAY_CALL_WIN = 2839;
	public const short S_GAMEPLAY_PLAYER_CALL_WIN = 2840;
	public const short C_GAMEPLAY_ATTACK_WIN = 2841;
	public const short S_GAMEPLAY_ATTACK_WIN = 2842;
	public const short S_GAMEPLAY_PLAYER_ATTACK_WIN = 2843;
	public const short S_GAMEPLAY_RECEIVE_ATTACK_WIN = 2844;
	public const short C_GAMEPLAY_ADMIN_CHEAT = 2846;
	public const short S_GAMEPLAY_ADMIN_CHEAT = 2847;
	public const short C_GAMEPLAY_ADMIN_WATCH_RESULT = 2848;
	public const short S_GAMEPLAY_ADMIN_WATCH_RESULT = 2849;
	public const short C_GAMEPLAY_SETBET = 2850;
	public const short S_GAMEPLAY_SETBET = 2851;
	public const short S_GAMEPLAY_PLAYER_SETBET = 2852;
	public const short S_GAMEPLAY_TABLE_CURRENT_BET = 2853;
	public const short C_GAMEPLAY_ADDBET = 2854;
	public const short S_GAMEPLAY_ADDBET = 2855;
	public const short S_GAMEPLAY_PLAYER_ADDBET = 2856;
	public const short S_GAMEPLAY_UPDATE_TABLE_BET = 2857;
	public const short C_GAMEPLAY_KICK_PLAYER = 2858;
	public const short S_GAMEPLAY_KICK_PLAYER = 2859;
	public const short C_GAMEPLAY_CHAT_IN_TABLE = 2860;
	public const short S_GAMEPLAY_CHAT_IN_TABLE = 2861;
	public const short C_GAMEPLAY_ATTACK_WITH_EMOTION = 2862;
	public const short S_GAMEPLAY_ATTACK_WITH_EMOTION = 2863;
	public const short S_GAMEPLAY_PLAYER_RECONNECT = 2865;
	public const short C_GAMEPLAY_GET_USERINFO_IN_GAME = 2868;
	public const short S_GAMEPLAY_GET_USERINFO_IN_GAME = 2869;
	public const short C_GAMEPLAY_GET_USERAchievement_IN_GAME = 2870;
	public const short S_GAMEPLAY_GET_USERAchievement_IN_GAME = 2871;
	public const short C_GAMEPLAY_CHAT_TO_VIERWER = 2880;
	public const short S_GAMEPLAY_CHAT_TO_VIERWER = 2881;
	public const short C_GAMEPLAY_READY = 2882;
	public const short S_GAMEPLAY_READY = 2883;
	public const short S_GAMEPLAY_PLAYER_READY = 2884;
	public const short C_GAMEPLAY_START = 2885;
	public const short S_GAMEPLAY_START = 2886;
	public const short S_GAMEPLAY_START_GAME = 2887;
	public const short C_GAMEPLAY_CHANGE_POSITION_CHAIR = 2888;
	public const short S_GAMEPLAY_CHANGE_POSITION_CHAIR = 2889;
	public const short S_GAMEPLAY_SERVER_CHANGE_TURN = 2896;
	public const short S_GAMEPLAY_RESULT_GAME = 2899;
	public const short C_GAMEPLAY_GET_CHAIR_STATUS = 2900;
	public const short S_GAMEPLAY_GET_CHAIR_STATUS = 2901;
	public const short C_GAMEPLAY_GET_LOSE = 2902;
	public const short S_GAMEPLAY_GET_LOSE = 2903;
	public const short S_GAMEPLAY_PLAYER_GET_LOSE = 2904;
	public const short C_GAMEPLAY_REQUEST_DRAW = 2905;
	public const short S_GAMEPLAY_REQUEST_DRAW = 2906;
	public const short S_GAMEPLAY_PLAYER_REQUEST_DRAW = 2907;
	public const short C_GAMEPLAY_ACCEPT_REQUEST_DRAW = 2908;
	public const short S_GAMEPLAY_ACCEPT_REQUEST_DRAW = 2909;
	public const short S_GAMEPLAY_RESULT_DRAW = 2910;
	public const short S_GAMEPLAY_FINISH_GAME = 2911;
	public const short C_MINIGAME_TAIXIU_JOIN_GAME = 3100;
	public const short S_MINIGAME_TAIXIU_JOIN_GAME = 3101;
	public const short C_MINIGAME_TAIXIU_GET_GAMEINFO = 3102;
	public const short S_MINIGAME_TAIXIU_GET_GAMEINFO = 3103;
	public const short C_MINIGAME_TAIXIU_LEFT_GAME = 3104;
	public const short C_MINIGAME_TAIXIU_ADDBET = 3105;
	public const short S_MINIGAME_TAIXIU_ADDBET = 3106;
	public const short S_MINIGAME_TAIXIU_TABLE_BET = 3108;
	public const short S_MINIGAME_TAIXIU_PROCESS_RESULT = 3109;
	public const short C_MINIGAME_BAUCUA_JOIN_GAME = 3200;
	public const short S_MINIGAME_BAUCUA_JOIN_GAME = 3201;
	public const short C_MINIGAME_BAUCUA_GET_GAMEINFO = 3202;
	public const short S_MINIGAME_BAUCUA_GET_GAMEINFO = 3203;
	public const short C_MINIGAME_BAUCUA_LEFT_GAME = 3204;
	public const short C_MINIGAME_BAUCUA_ADDBET = 3205;
	public const short S_MINIGAME_BAUCUA_ADDBET = 3206;
	public const short S_MINIGAME_BAUCUA_TABLE_BET = 3208;
	public const short S_MINIGAME_BAUCUA_PROCESS_RESULT = 3209;
	public const short C_MINIGAME_BAUCUA_CHAT_ALL = 3220;
	public const short S_MINIGAME_BAUCUA_CHAT_ALL = 3221;
	public const short C_MINIGAME_DUANGUA_JOIN_GAME = 3300;
	public const short S_MINIGAME_DUANGUA_JOIN_GAME = 3301;
	public const short C_MINIGAME_DUANGUA_GET_GAMEINFO = 3302;
	public const short S_MINIGAME_DUANGUA_GET_GAMEINFO = 3303;
	public const short C_MINIGAME_DUANGUA_LEFT_GAME = 3304;
	public const short C_MINIGAME_DUANGUA_ADDBET = 3305;
	public const short S_MINIGAME_DUANGUA_ADDBET = 3306;
	public const short S_MINIGAME_DUANGUA_TABLE_BET = 3308;
	public const short S_MINIGAME_DUANGUA_PROCESS_RESULT = 3309;
	public const short C_MINIGAME_LONGHO_JOIN_GAME = 3500;
	public const short S_MINIGAME_LONGHO_JOIN_GAME = 3501;
	public const short C_MINIGAME_LONGHO_GET_GAMEINFO = 3502;
	public const short S_MINIGAME_LONGHO_GET_GAMEINFO = 3503;
	public const short C_MINIGAME_LONGHO_LEFT_GAME = 3504;
	public const short C_MINIGAME_LONGHO_ADDBET = 3505;
	public const short S_MINIGAME_LONGHO_ADDBET = 3506;
	public const short S_MINIGAME_LONGHO_TABLE_BET = 3508;
	public const short S_MINIGAME_LONGHO_PROCESS_RESULT = 3509;
	public const short C_MINIGAME_LONGHO_CHAT_ALL = 3520;
	public const short S_MINIGAME_LONGHO_CHAT_ALL = 3521;
	public const short C_XHCD_CHANGE_BACKGROUND = 6000;
	public const short S_XHCD_CHANGE_BACKGROUND = 6001;
	public const short C_XHCD_CHANGE_CHARACTER = 6002;
	public const short S_XHCD_CHANGE_CHARACTER = 6003;
	public const short S_XHCD_PLAYER_CHANGE_CHARACTER = 6005;
	public const short C_XHCD_MOVE_LEFT = 6006;
	public const short S_XHCD_MOVE_LEFT = 6008;
	public const short S_XHCD_PLAYER_MOVE_LEFT = 6009;
	public const short C_XHCD_MOVE_RIGHT = 6010;
	public const short S_XHCD_MOVE_RIGHT = 6011;
	public const short S_XHCD_PLAYER_MOVE_RIGHT = 6012;
	public const short C_XHCD_MOVE_DOWN = 6013;
	public const short S_XHCD_MOVE_DOWN = 6014;
	public const short S_XHCD_PLAYER_MOVE_DOWN = 6015;
	public const short C_XHCD_CHANGE_PIECE_STATE = 6016;
	public const short S_XHCD_CHANGE_PIECE_STATE = 6017;
	public const short S_XHCD_PLAYER_CHANGE_PIECE_STATE = 6018;
	public const short S_XHCD_CURRENT_PIECE = 6019;
	public const short S_XHCD_ADD_PIECE_TO_MATRIX_AND_NEXT_PIECE = 6020;
	public const short S_XHCD_PIECE_FALLING = 6021;
	public const short S_XHCD_MAXTRIX_DATA = 6022;
	public const short S_XHCD_MORE_ATTACK = 6030;
	public const short S_XHCD_PieceBreak_ATTACK_1 = 6031;
	public const short S_XHCD_PieceBreak_ATTACK_2 = 6032;
	public const short S_XHCD_PieceBreak_HP = 6033;
	public const short S_XHCD_PieceBreak_MANA = 6035;
	public const short S_XHCD_PieceBreak_SHIELD = 6036;
	public const short S_XHCD_PieceBreak_SPECIAL = 6038;
	public const short C_XHCD_USE_SPELL = 6050;
	public const short S_XHCD_USE_SPELL = 6051;
	public const short S_XHCD_PLAYER_USE_SPELL = 6052;
	public const short C_XHCD_CALLSKILL_1 = 6060;
	public const short S_XHCD_CALLSKILL_1 = 6061;
	public const short S_XHCD_PLAYER_CALLSKILL_1 = 6062;
	public const short C_XHCD_CALLSKILL_2 = 6063;
	public const short S_XHCD_CALLSKILL_2 = 6064;
	public const short S_XHCD_PLAYER_CALLSKILL_2 = 6065;
	public const short C_XHCD_CALLSKILL_ULTIMATE = 6066;
	public const short S_XHCD_CALLSKILL_ULTIMATE = 6068;
	public const short S_XHCD_PLAYER_CALLSKILL_ULTIMATE = 6069;
	public static string getCMD (short cmd) {
		switch (cmd) {
			case 1:
				return "S_DATA_INVALIDATE(1)";
			case 2:
				return "C_TEST(2)";
			case 99:
				return "S_ALERT_UPDATE_SERVER(99)";
			case 999:
				return "S_SERVER_DEBUG_LOG(999)";
			case 1000:
				return "C_ADMIN_PLAYNOW_TO_TABLE(1000)";
			case 1001:
				return "S_ADMIN_PLAYNOW_TO_TABLE(1001)";
			case 1002:
				return "C_ADMIN_JOIN_TO_TABLE(1002)";
			case 1003:
				return "S_ADMIN_JOIN_TO_TABLE(1003)";
			case 2000:
				return "Game_Get_Bigxu_Version(2000)";
			case 2500:
				return "C_GAMEPLAY_SET_PARENT(2500)";
			case 2501:
				return "S_GAMEPLAY_SET_PARENT(2501)";
			case 2600:
				return "S_ON_PLAYER_UPDATE_GOLD(2600)";
			case 2601:
				return "S_onPlayerAddGold(2601)";
			case 2620:
				return "C_TABLE_PLAYNOW_TO_TABLE(2620)";
			case 2621:
				return "S_TABLE_PLAYNOW_TO_TABLE(2621)";
			case 2622:
				return "C_TABLE_JOIN_TO_TABLE(2622)";
			case 2623:
				return "S_TABLE_JOIN_TO_TABLE(2623)";
			case 2625:
				return "C_TABLE_CREATE_PASSWORD_TABLE(2625)";
			case 2626:
				return "S_TABLE_CREATE_PASSWORD_TABLE(2626)";
			case 2650:
				return "C_TABLE_JOIN_TO_MINIGAME_STATE(2650)";
			case 2651:
				return "S_TABLE_JOIN_TO_MINIGAME_STATE(2651)";
			case 2689:
				return "S_GAMEPLAY_PLAYER_JOIN_GAME(2689)";
			case 2690:
				return "S_GAMEPLAY_TABLE_DATA(2690)";
			case 2692:
				return "C_GAMEPLAY_GET_TABLE_INFO(2692)";
			case 2693:
				return "S_GAMEPLAY_GET_TABLE_INFO(2693)";
			case 2695:
				return "C_GAMEPLAY_GET_TABLE_STATUS(2695)";
			case 2696:
				return "S_GAMEPLAY_GET_TABLE_STATUS(2696)";
			case 2697:
				return "S_GAMEPLAY_SERVER_UPDATE_LIST_PLAYING(2697)";
			case 2698:
				return "S_GAMEPLAY_SERVER_UPDATE_LIST_CHAIRS(2698)";
			case 2699:
				return "S_GAMEPLAY_SERVER_UPDATE_LIST_VIEWER(2699)";
			case 2700:
				return "C_GAMEPLAY_LEFT_TABLE(2700)";
			case 2701:
				return "S_GAMEPLAY_PLAYER_LEFT_GAME(2701)";
			case 2710:
				return "C_GAMEPLAY_PLAYER_SET_TABLE_PASSWORD(2710)";
			case 2711:
				return "S_GAMEPLAY_PLAYER_SET_TABLE_PASSWORD(2711)";
			case 2800:
				return "C_GAMEPLAY_SITDOWN(2800)";
			case 2801:
				return "S_GAMEPLAY_SITDOWN(2801)";
			case 2802:
				return "S_GAMEPLAY_PLAYER_SITDOWN(2802)";
			case 2803:
				return "C_GAMEPLAY_STANDUP(2803)";
			case 2804:
				return "S_GAMEPLAY_STANDUP(2804)";
			case 2805:
				return "S_GAMEPLAY_PLAYER_STANDUP(2805)";
			case 2809:
				return "C_GAMEPLAY_INVITE_ROBOT(2809)";
			case 2820:
				return "C_BetToWin_NoAchievement(2820)";
			case 2821:
				return "S_BetToWin_NoAchievement(2821)";
			case 2830:
				return "C_GAMEPLAY_GET_CARD(2830)";
			case 2831:
				return "S_GAMEPLAY_GET_CARD(2831)";
			case 2832:
				return "S_GAMEPLAY_PLAYER_GET_CARD(2832)";
			case 2833:
				return "C_GAMEPLAY_PUT_CARD(2833)";
			case 2834:
				return "S_GAMEPLAY_PUT_CARD(2834)";
			case 2835:
				return "S_GAMEPLAY_PLAYER_PUT_CARD(2835)";
			case 2836:
				return "C_GAMEPLAY_END_TURN(2836)";
			case 2837:
				return "S_GAMEPLAY_END_TURN(2837)";
			case 2838:
				return "C_GAMEPLAY_CALL_WIN(2838)";
			case 2839:
				return "S_GAMEPLAY_CALL_WIN(2839)";
			case 2840:
				return "S_GAMEPLAY_PLAYER_CALL_WIN(2840)";
			case 2841:
				return "C_GAMEPLAY_ATTACK_WIN(2841)";
			case 2842:
				return "S_GAMEPLAY_ATTACK_WIN(2842)";
			case 2843:
				return "S_GAMEPLAY_PLAYER_ATTACK_WIN(2843)";
			case 2844:
				return "S_GAMEPLAY_RECEIVE_ATTACK_WIN(2844)";
			case 2846:
				return "C_GAMEPLAY_ADMIN_CHEAT(2846)";
			case 2847:
				return "S_GAMEPLAY_ADMIN_CHEAT(2847)";
			case 2848:
				return "C_GAMEPLAY_ADMIN_WATCH_RESULT(2848)";
			case 2849:
				return "S_GAMEPLAY_ADMIN_WATCH_RESULT(2849)";
			case 2850:
				return "C_GAMEPLAY_SETBET(2850)";
			case 2851:
				return "S_GAMEPLAY_SETBET(2851)";
			case 2852:
				return "S_GAMEPLAY_PLAYER_SETBET(2852)";
			case 2853:
				return "S_GAMEPLAY_TABLE_CURRENT_BET(2853)";
			case 2854:
				return "C_GAMEPLAY_ADDBET(2854)";
			case 2855:
				return "S_GAMEPLAY_ADDBET(2855)";
			case 2856:
				return "S_GAMEPLAY_PLAYER_ADDBET(2856)";
			case 2857:
				return "S_GAMEPLAY_UPDATE_TABLE_BET(2857)";
			case 2858:
				return "C_GAMEPLAY_KICK_PLAYER(2858)";
			case 2859:
				return "S_GAMEPLAY_KICK_PLAYER(2859)";
			case 2860:
				return "C_GAMEPLAY_CHAT_IN_TABLE(2860)";
			case 2861:
				return "S_GAMEPLAY_CHAT_IN_TABLE(2861)";
			case 2862:
				return "C_GAMEPLAY_ATTACK_WITH_EMOTION(2862)";
			case 2863:
				return "S_GAMEPLAY_ATTACK_WITH_EMOTION(2863)";
			case 2865:
				return "S_GAMEPLAY_PLAYER_RECONNECT(2865)";
			case 2868:
				return "C_GAMEPLAY_GET_USERINFO_IN_GAME(2868)";
			case 2869:
				return "S_GAMEPLAY_GET_USERINFO_IN_GAME(2869)";
			case 2870:
				return "C_GAMEPLAY_GET_USERAchievement_IN_GAME(2870)";
			case 2871:
				return "S_GAMEPLAY_GET_USERAchievement_IN_GAME(2871)";
			case 2880:
				return "C_GAMEPLAY_CHAT_TO_VIERWER(2880)";
			case 2881:
				return "S_GAMEPLAY_CHAT_TO_VIERWER(2881)";
			case 2882:
				return "C_GAMEPLAY_READY(2882)";
			case 2883:
				return "S_GAMEPLAY_READY(2883)";
			case 2884:
				return "S_GAMEPLAY_PLAYER_READY(2884)";
			case 2885:
				return "C_GAMEPLAY_START(2885)";
			case 2886:
				return "S_GAMEPLAY_START(2886)";
			case 2887:
				return "S_GAMEPLAY_START_GAME(2887)";
			case 2888:
				return "C_GAMEPLAY_CHANGE_POSITION_CHAIR(2888)";
			case 2889:
				return "S_GAMEPLAY_CHANGE_POSITION_CHAIR(2889)";
			case 2896:
				return "S_GAMEPLAY_SERVER_CHANGE_TURN(2896)";
			case 2899:
				return "S_GAMEPLAY_RESULT_GAME(2899)";
			case 2900:
				return "C_GAMEPLAY_GET_CHAIR_STATUS(2900)";
			case 2901:
				return "S_GAMEPLAY_GET_CHAIR_STATUS(2901)";
			case 2902:
				return "C_GAMEPLAY_GET_LOSE(2902)";
			case 2903:
				return "S_GAMEPLAY_GET_LOSE(2903)";
			case 2904:
				return "S_GAMEPLAY_PLAYER_GET_LOSE(2904)";
			case 2905:
				return "C_GAMEPLAY_REQUEST_DRAW(2905)";
			case 2906:
				return "S_GAMEPLAY_REQUEST_DRAW(2906)";
			case 2907:
				return "S_GAMEPLAY_PLAYER_REQUEST_DRAW(2907)";
			case 2908:
				return "C_GAMEPLAY_ACCEPT_REQUEST_DRAW(2908)";
			case 2909:
				return "S_GAMEPLAY_ACCEPT_REQUEST_DRAW(2909)";
			case 2910:
				return "S_GAMEPLAY_RESULT_DRAW(2910)";
			case 2911:
				return "S_GAMEPLAY_FINISH_GAME(2911)";
			case 3100:
				return "C_MINIGAME_TAIXIU_JOIN_GAME(3100)";
			case 3101:
				return "S_MINIGAME_TAIXIU_JOIN_GAME(3101)";
			case 3102:
				return "C_MINIGAME_TAIXIU_GET_GAMEINFO(3102)";
			case 3103:
				return "S_MINIGAME_TAIXIU_GET_GAMEINFO(3103)";
			case 3104:
				return "C_MINIGAME_TAIXIU_LEFT_GAME(3104)";
			case 3105:
				return "C_MINIGAME_TAIXIU_ADDBET(3105)";
			case 3106:
				return "S_MINIGAME_TAIXIU_ADDBET(3106)";
			case 3108:
				return "S_MINIGAME_TAIXIU_TABLE_BET(3108)";
			case 3109:
				return "S_MINIGAME_TAIXIU_PROCESS_RESULT(3109)";
			case 3200:
				return "C_MINIGAME_BAUCUA_JOIN_GAME(3200)";
			case 3201:
				return "S_MINIGAME_BAUCUA_JOIN_GAME(3201)";
			case 3202:
				return "C_MINIGAME_BAUCUA_GET_GAMEINFO(3202)";
			case 3203:
				return "S_MINIGAME_BAUCUA_GET_GAMEINFO(3203)";
			case 3204:
				return "C_MINIGAME_BAUCUA_LEFT_GAME(3204)";
			case 3205:
				return "C_MINIGAME_BAUCUA_ADDBET(3205)";
			case 3206:
				return "S_MINIGAME_BAUCUA_ADDBET(3206)";
			case 3208:
				return "S_MINIGAME_BAUCUA_TABLE_BET(3208)";
			case 3209:
				return "S_MINIGAME_BAUCUA_PROCESS_RESULT(3209)";
			case 3220:
				return "C_MINIGAME_BAUCUA_CHAT_ALL(3220)";
			case 3221:
				return "S_MINIGAME_BAUCUA_CHAT_ALL(3221)";
			case 3300:
				return "C_MINIGAME_DUANGUA_JOIN_GAME(3300)";
			case 3301:
				return "S_MINIGAME_DUANGUA_JOIN_GAME(3301)";
			case 3302:
				return "C_MINIGAME_DUANGUA_GET_GAMEINFO(3302)";
			case 3303:
				return "S_MINIGAME_DUANGUA_GET_GAMEINFO(3303)";
			case 3304:
				return "C_MINIGAME_DUANGUA_LEFT_GAME(3304)";
			case 3305:
				return "C_MINIGAME_DUANGUA_ADDBET(3305)";
			case 3306:
				return "S_MINIGAME_DUANGUA_ADDBET(3306)";
			case 3308:
				return "S_MINIGAME_DUANGUA_TABLE_BET(3308)";
			case 3309:
				return "S_MINIGAME_DUANGUA_PROCESS_RESULT(3309)";
			case 3500:
				return "C_MINIGAME_LONGHO_JOIN_GAME(3500)";
			case 3501:
				return "S_MINIGAME_LONGHO_JOIN_GAME(3501)";
			case 3502:
				return "C_MINIGAME_LONGHO_GET_GAMEINFO(3502)";
			case 3503:
				return "S_MINIGAME_LONGHO_GET_GAMEINFO(3503)";
			case 3504:
				return "C_MINIGAME_LONGHO_LEFT_GAME(3504)";
			case 3505:
				return "C_MINIGAME_LONGHO_ADDBET(3505)";
			case 3506:
				return "S_MINIGAME_LONGHO_ADDBET(3506)";
			case 3508:
				return "S_MINIGAME_LONGHO_TABLE_BET(3508)";
			case 3509:
				return "S_MINIGAME_LONGHO_PROCESS_RESULT(3509)";
			case 3520:
				return "C_MINIGAME_LONGHO_CHAT_ALL(3520)";
			case 3521:
				return "S_MINIGAME_LONGHO_CHAT_ALL(3521)";
			case 6000:
				return "C_XHCD_CHANGE_BACKGROUND(6000)";
			case 6001:
				return "S_XHCD_CHANGE_BACKGROUND(6001)";
			case 6002:
				return "C_XHCD_CHANGE_CHARACTER(6002)";
			case 6003:
				return "S_XHCD_CHANGE_CHARACTER(6003)";
			case 6005:
				return "S_XHCD_PLAYER_CHANGE_CHARACTER(6005)";
			case 6006:
				return "C_XHCD_MOVE_LEFT(6006)";
			case 6008:
				return "S_XHCD_MOVE_LEFT(6008)";
			case 6009:
				return "S_XHCD_PLAYER_MOVE_LEFT(6009)";
			case 6010:
				return "C_XHCD_MOVE_RIGHT(6010)";
			case 6011:
				return "S_XHCD_MOVE_RIGHT(6011)";
			case 6012:
				return "S_XHCD_PLAYER_MOVE_RIGHT(6012)";
			case 6013:
				return "C_XHCD_MOVE_DOWN(6013)";
			case 6014:
				return "S_XHCD_MOVE_DOWN(6014)";
			case 6015:
				return "S_XHCD_PLAYER_MOVE_DOWN(6015)";
			case 6016:
				return "C_XHCD_CHANGE_PIECE_STATE(6016)";
			case 6017:
				return "S_XHCD_CHANGE_PIECE_STATE(6017)";
			case 6018:
				return "S_XHCD_PLAYER_CHANGE_PIECE_STATE(6018)";
			case 6019:
				return "S_XHCD_CURRENT_PIECE(6019)";
			case 6020:
				return "S_XHCD_ADD_PIECE_TO_MATRIX_AND_NEXT_PIECE(6020)";
			case 6021:
				return "S_XHCD_PIECE_FALLING(6021)";
			case 6022:
				return "S_XHCD_MAXTRIX_DATA(6022)";
			case 6030:
				return "S_XHCD_MORE_ATTACK(6030)";
			case 6031:
				return "S_XHCD_PieceBreak_ATTACK_1(6031)";
			case 6032:
				return "S_XHCD_PieceBreak_ATTACK_2(6032)";
			case 6033:
				return "S_XHCD_PieceBreak_HP(6033)";
			case 6035:
				return "S_XHCD_PieceBreak_MANA(6035)";
			case 6036:
				return "S_XHCD_PieceBreak_SHIELD(6036)";
			case 6038:
				return "S_XHCD_PieceBreak_SPECIAL(6038)";
			case 6050:
				return "C_XHCD_USE_SPELL(6050)";
			case 6051:
				return "S_XHCD_USE_SPELL(6051)";
			case 6052:
				return "S_XHCD_PLAYER_USE_SPELL(6052)";
			case 6060:
				return "C_XHCD_CALLSKILL_1(6060)";
			case 6061:
				return "S_XHCD_CALLSKILL_1(6061)";
			case 6062:
				return "S_XHCD_PLAYER_CALLSKILL_1(6062)";
			case 6063:
				return "C_XHCD_CALLSKILL_2(6063)";
			case 6064:
				return "S_XHCD_CALLSKILL_2(6064)";
			case 6065:
				return "S_XHCD_PLAYER_CALLSKILL_2(6065)";
			case 6066:
				return "C_XHCD_CALLSKILL_ULTIMATE(6066)";
			case 6068:
				return "S_XHCD_CALLSKILL_ULTIMATE(6068)";
			case 6069:
				return "S_XHCD_PLAYER_CALLSKILL_ULTIMATE(6069)";
			default:
				return cmd + "";
		}
	}
}