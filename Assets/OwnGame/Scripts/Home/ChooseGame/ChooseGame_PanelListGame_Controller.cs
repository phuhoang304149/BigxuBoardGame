using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseGame_PanelListGame_Controller : MonoBehaviour {

    [SerializeField] ScrollRect myScrollRect;
    [SerializeField] ChooseGame_PanelGameOption_Controller animalRacing;
    [SerializeField] ChooseGame_PanelGameOption_Controller horseRacing;
    [SerializeField] ChooseGame_PanelGameOption_Controller battleOfLegend;
    [SerializeField] ChooseGame_PanelGameOption_Controller battleOfRobot;
    [SerializeField] ChooseGame_PanelGameOption_Controller baccarat;
    [SerializeField] ChooseGame_PanelGameOption_Controller blackjack;
    [SerializeField] ChooseGame_PanelGameOption_Controller poker;
    [SerializeField] ChooseGame_PanelGameOption_Controller chineseChess;
    [SerializeField] ChooseGame_PanelGameOption_Controller kingChess;
    [SerializeField] ChooseGame_PanelGameOption_Controller uno;

    bool isInstalled;

	public void InitData(){
		if (!isInstalled) {

            IMiniGameInfo _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.AnimalRacing);
            animalRacing.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.HourseRacing);
            horseRacing.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.BattleOfLegend);
            battleOfLegend.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.BattleOfRobots);
            battleOfRobot.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.Baccarat);
            baccarat.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.Blackjack);
            blackjack.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.Poker);
            poker.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.ChineseChess);
            chineseChess.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.KingChess);
            kingChess.InitData(_miniGameInfo);

            _miniGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo(IMiniGameInfo.Type.Uno);
            uno.InitData(_miniGameInfo);

            myScrollRect.horizontalNormalizedPosition = 0f;

            isInstalled = true;
		}
	}
}
