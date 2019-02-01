using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;
using Lean.Pool;
using System.Globalization;
using UnityEngine.Assertions.Must;
using UnityEngine.SocialPlatforms.Impl;

public class BOL_ChoiceHero : BOL_MySceneMain
{
    public static BOL_ChoiceHero instance
    {
        get
        {
            return ins;
        }
    }
    public static BOL_ChoiceHero ins;
    //void Awake() {
    //	ins = this;
    //}
    bool _IsMe;
    public bool IsMe
    {
        get
        {
            return _IsMe;
        }
        set
        {
            _IsMe = value;
            if (_IsMe)
            {
                _btnChoiceChairLeft.enabled = false;
                _btnChoiceChairRight.enabled = false;
                BOL_Main_Controller.instance.panelUserInfo.Hide();

            }
            else
            {
                _btnChoiceChairLeft.enabled = true;
                _btnChoiceChairRight.enabled = true;
                BOL_Main_Controller.instance.panelUserInfo.RefreshGoldInfo(true);
                BOL_Main_Controller.instance.panelUserInfo.Show();
            }
        }

    }
    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    [Header("FOR CONTROL CHOICE")]
    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    public Canvas canvasHeroChoice;
    public GameObject imageVS;
    public GameObject TableChoice;
    [SerializeField] RectTransform tableChoiceRect;
    [SerializeField] CanvasGroup tableChoiceCanvas;
    private Vector3 vector_tablechoice;
    public CanvasGroup PanelChoiceHero;
    public CanvasGroup PanelChoiceSpell;
    public InputField betInGame;
    public int tmpHero = 1;
    public int tmpSpell = 0;
    public List<GameObject> listLock;
    public List<BOL_ButtonController> listHeroChoose;
    public List<BOL_ButtonController> listSpellChoose;
    public bool isChoiceChair;
    public Button btnStartGame;
    LTDescr tweenLeft;
    LTDescr tweenRight;
    LTDescr tweenScale;
    LTDescr tweenAlpha;
    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    [Header("FOR CHOICE HERO AND SPELL")]
    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    public Button _btnChoiceChairLeft;
    public Button _btnChoiceChairRight;
    public BOL_ButtonController btnChoiceChairLeft;
    public BOL_ButtonController btnChoiceChairRight;
    public List<IEnumerator> listProcessAction;

    public GameObject panelShowSkillHero;
    public override UIScene mySceneType
    {
        get
        {
            return UIScene.ChoiceHero;
        }
    }

    private void Start()
    {
        InitData();
    }

    public override void InitData()
    {
#if TEST
        Debug.Log(Debugs.ColorString("init data choice hero", Color.red));
#endif
        ins = this;
        //btnStandupLeft.gameObject.SetActive(false);
        //btnStandupRight.gameObject.SetActive(false);
        btnChoiceChairLeft.isChoiceChair = false;
        btnChoiceChairRight.isChoiceChair = false;
        btnChoiceChairLeft.btnStandUp.gameObject.SetActive(false);
        btnChoiceChairRight.btnStandUp.gameObject.SetActive(false);

        canvasHeroChoice.worldCamera = BOL_Manager.instance.mainCamera.mainCamera;
        listProcessAction = new List<IEnumerator>();
        StartCoroutine(StartActionStandDown());
#if UNITY_ANDROID
        tableChoiceRect.localScale = Vector3.one * ((0.8f / 2.81203f) * BOL_Manager.instance.mainCamera.mainCamera.orthographicSize);
        //TableChoice.GetComponent<RectTransform>().localScale = Vector3.one * ((0.8f / 2.81203f) * BOL_Manager.instance.MainCam.orthographicSize);
#endif
#if UNITY_IOS
		tableChoiceRect.localScale = Vector3.one * 0.8f;
		//TableChoice.GetComponent<RectTransform>().localScale = Vector3.one * 0.8f;
#endif
        vector_tablechoice = TableChoice.transform.localScale;
        TableChoice.SetActive(false);
        betInGame.text = BOL_Manager.instance.bol_Table_Info.bet.ToString();
        ChoiceChairPosition();
        betInGame.onEndEdit.AddListener(SetBetInGame);
        ReceiveReadyGame(false);
        btnStartGame.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            if (btnStartGame.transform.GetChild(0).gameObject.activeSelf)
            {
                SetReadyGame(true);
            }
            else
            {
                SetReadyGame(false);
            }
        });
    }
    void ChoiceChairPosition()
    {
        _btnChoiceChairLeft.onClick.AddListener(() =>
        {
            _btnChoiceChairRight.enabled = false;
            if (btnChoiceChairLeft.isChoiceChair)
            {
                Debug.Log("choice chair left true");
            }
            else
            {
                BOL_Main_Controller.instance.ChairPosition = Constant.CHAIR_LEFT;
                //btnChoiceChairLeft.isChoiceChair = true;
                btnChoiceChairLeft.positon = Constant.CHAIR_LEFT;
            }
        });
        _btnChoiceChairRight.onClick.AddListener(() =>
        {
            _btnChoiceChairLeft.enabled = false;
            if (btnChoiceChairRight.isChoiceChair)
            {
                Debug.Log("choice chair right true");
            }
            else
            {
                BOL_Main_Controller.instance.ChairPosition = Constant.CHAIR_RIGHT;
                //btnChoiceChairRight.isChoiceChair = true;
                btnChoiceChairRight.positon = Constant.CHAIR_RIGHT;
            }
        });
    }
    IEnumerator StartActionStandDown()
    {
        yield return null;
        while (true)
        {
            yield return new WaitUntil(() => listProcessAction.Count > 0);
            yield return StartCoroutine(listProcessAction[0]);
            listProcessAction.RemoveAt(0);
        }
    }
    public void ChoiceHero(int position)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        if (!listLock[position].activeSelf)
        {
            //ListHeroChoice[tmpHero].alpha = 0;
            //ListHeroChoice[position].alpha = 1;
            listHeroChoose[tmpHero]._isChoiceHero = false;
            listHeroChoose[position]._isChoiceHero = true;
            tmpHero = position;
            SetChoiceHeroAnhSpell(tmpHero, tmpSpell);
#if TEST
            Debug.Log("choice hero " + tmpHero + "  choice spell " + tmpSpell);
#endif
        }
        else
        {
            PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("Global/CommingSoon")));
#if TEST
            Debug.Log("comming soon");
#endif
        }
    }
    public void ChoiceSpell(int position)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        if (isChoiceChair)
        {
            if (tmpSpell == position)
            {
                CallPopupInfo(tmpSpell);
            }
            listSpellChoose[tmpSpell]._ischoiceSpell = false;
            listSpellChoose[position]._ischoiceSpell = true;
            tmpSpell = position;
            SetChoiceHeroAnhSpell(tmpHero, tmpSpell);
#if TEST
            Debug.Log("choice spell " + position);
#endif
        }
        else
        {
#if TEST
            Debug.Log("chưa chọn vị trí ngồi");
#endif
        }
    }
    public void SetChoiceHeroAnhSpell(int hero, int spell)
    {
        MessageSending changecharacter = new MessageSending(CMD_REALTIME.C_XHCD_CHANGE_CHARACTER);
        changecharacter.writeByte((byte)hero);
        changecharacter.writeByte((byte)spell);
        NetworkGlobal.instance.SendMessageRealTime(changecharacter);
    }
    public void SetBetInGame(string arg0)
    {
        MessageSending mgs = new MessageSending(CMD_REALTIME.C_GAMEPLAY_SETBET);
        mgs.writeLong(Convert.ToInt64(arg0));
        NetworkGlobal.instance.SendMessageRealTime(mgs);
    }
    public void SetReadyGame(bool boolean)
    {
        MessageSending readygame = new MessageSending(CMD_REALTIME.C_GAMEPLAY_READY);
        readygame.writeBoolean(boolean);
        NetworkGlobal.instance.SendMessageRealTime(readygame);
    }
    public void ShowPanelSkill(bool boolean)
    {
        panelShowSkillHero.SetActive(boolean);


    }
    public void ReceiveReadyGame(bool boolean)
    {
        if (boolean)
        {
#if TEST
            Debug.Log("ready game");
#endif

            btnStartGame.transform.GetChild(0).gameObject.SetActive(false);
            btnStartGame.transform.GetChild(1).gameObject.SetActive(true);
            BOL_ControllShowSkill.instance.InitData(tmpHero, true);
        }
        else
        {
            btnStartGame.transform.GetChild(0).gameObject.SetActive(true);
            btnStartGame.transform.GetChild(1).gameObject.SetActive(false);
            BOL_ControllShowSkill.instance.InitData(tmpHero, false);
        }
    }
    public void SetActiveChoiceHero(bool boolean)
    {
#if TEST
        Debug.Log(boolean);
#endif

        PanelChoiceHero.interactable = boolean;
        PanelChoiceSpell.interactable = boolean;
        if (boolean)
        {
#if TEST
            Debug.Log(boolean);
#endif
            TableChoice.SetActive(true);

            if (tweenScale != null)
            {
                LeanTween.cancel(TableChoice, tweenScale.uniqueId);
                tweenScale = null;
            }
            tweenScale = LeanTween.scale(TableChoice, vector_tablechoice, 0.2f).setOnComplete(() =>
            {
                tweenScale = null;
                tweenScale = LeanTween.scale(imageVS, Vector3.zero, 0.2f).setOnComplete(() =>
                {
                    tweenScale = null;
                });
            }).setEase(LeanTweenType.easeOutBack).setOnCompleteOnStart(true);
            if (tweenAlpha != null)
            {
                LeanTween.cancel(TableChoice, tweenAlpha.uniqueId);
                tweenAlpha = null;
            }
            tweenAlpha = LeanTween.alphaCanvas(tableChoiceCanvas, 1, 0.2f).setOnComplete(() =>
            {
                //tweenAlpha = LeanTween.alphaCanvas(TableChoice.GetComponent<CanvasGroup>(), 1, 0.2f).setOnComplete(() => {
                tweenAlpha = null;
            });
        }
        else
        {
#if TEST
            Debug.Log(boolean);
#endif
            TableChoice.SetActive(true);
            if (tweenScale != null)
            {
                LeanTween.cancel(TableChoice, tweenScale.uniqueId);
                tweenScale = null;
            }
            tweenScale = LeanTween.scale(TableChoice, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                tweenScale = null;
                tableChoiceCanvas.alpha = 0;
                tweenScale = LeanTween.scale(imageVS, Vector3.one, 0.2f).setEase(LeanTweenType.easeInOutBack).setOnComplete(() =>
                {
                    tweenScale = null;
                });
            }).setEase(LeanTweenType.easeInBack).setOnCompleteOnStart(true);
        }
    }
    public override void ResetData()
    {
        isChoiceChair = false;
        btnStartGame.transform.GetChild(0).gameObject.SetActive(true);
        BOL_ControllShowSkill.instance.InitData(tmpHero, false);
    }
    public override void Hide()
    {
        base.Hide();
        btnStartGame.transform.GetChild(0).gameObject.SetActive(true);
        btnStartGame.transform.GetChild(1).gameObject.SetActive(false);
        BOL_ControllShowSkill.instance.InitData(tmpHero, false);
    }
    public override void Show()
    {
        base.Show();
        SetActiveChoiceHero(true);
    }
    public override void SelfDestruction()
    {
        base.SelfDestruction();
        ins = null;
    }
    public void CallPopupInfo(int postSpell)
    {
        switch (postSpell)
        {
            case 1:
                PopupManager.Instance.CreatePopupInfo(string.Format(MyLocalize.GetString("BOL/Message_Player_Spell_2")));
                break;
            case 2:
                PopupManager.Instance.CreatePopupInfo(string.Format(MyLocalize.GetString("BOL/Message_Player_Spell_3")));
                break;
            case 3:
                PopupManager.Instance.CreatePopupInfo(string.Format(MyLocalize.GetString("BOL/Message_Player_Spell_4")));
                break;
            case 4:
                PopupManager.Instance.CreatePopupInfo(string.Format(MyLocalize.GetString("BOL/Message_Player_Spell_5")));
                break;
            default:
                PopupManager.Instance.CreatePopupInfo(string.Format(MyLocalize.GetString("BOL/Message_Player_Spell_1")));
                break;

        }

    }



    private void OnDestroy()
    {
        ins = null;
    }
}


