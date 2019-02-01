using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lean.Pool;
using UnityEngine.Serialization;
using Facebook.Unity;

public class BOL_PlaySkill_Controller : BOL_MySceneMain
{
    public static BOL_PlaySkill_Controller instance
    {
        get
        {
            return ins;
        }
    }
    public static BOL_PlaySkill_Controller ins;
    void Awake()
    {
        ins = this;
    }
    public enum indexSpell { attack1, attack2, health, mana, shield }
    public Canvas canvasShowHero;
    public bool isStartGame;
    public GameObject panelMove;
    public GameObject panelChange;
    [Header("-------------------------------")]
    [Header("BUTTON IN GAME")]
    [Header("-------------------------------")]
    public Button moveLeft;
    public Button moveRight;
    public Button moveDown;
    public Button changePiece;
    float leftTimestamp, rightTimestamp, downTimestamp, changeTimestamp;
    public float TimeBetweenShots = 0.1f;
    [Header("-------------------------------")]
    [Header("LIST ACTION")]
    [Header("-------------------------------")]
    public List<Action> _listAction;
    [Header("-------------------------------")]
    [Header("CONTROL SKILL IN GAME ")]
    [Header("-------------------------------")]
    public BOL_Hero_Controler _Hero_left_ControlSkill;// setup và bol skill
    public BOL_Hero_Controler _Hero_right_ControlSkill;
    [Header("-------------------------------")]
    [Header("INFO SPELL AND SKILL")]
    [Header("-------------------------------")]
    public List<Sprite> ListSpellSprite;
    string[] skillHero = { "hero0", "hero1", "hero2", "hero3", "hero4", "hero5" };
    public List<GameObject> listSpellinGame;
    public List<GameObject> listEffectInPiece;

    public int skillpostion;
    public int spellposition;
    public Vector3 leftPositionEffect;
    public Vector3 rightPositionEffect;
    [Header("-------------------------------")]
    [Header("CONTROL SKILL")]
    [Header("-------------------------------")]
    IEnumerator[] listSkillDelay;
    public Button _buttonQ;
    public Button _buttonW;
    public Button _buttonE;
    public Button _buttonSpell;
    public ButtonSkillController btnSkillQ;
    public ButtonSkillController btnSkillW;
    public ButtonSkillController btnSkillE;
    public ButtonSkillController btnSkillSpell;
    IEnumerator enumeratorQ;
    IEnumerator enumeratorW;
    IEnumerator enumeratorE;
    IEnumerator enumeratorSpell;
    [Header("-------------------------------")]
    [Header("CONTROL INFO")]
    [Header("-------------------------------")]
    // public BOL_ShowPlayer_Controller info1;
    // public BOL_ShowPlayer_Controller info2;
    /// <summary>
    /// The is finish.
    ///1. <see langword="true"/> : ket thuc
    ///2.<see langword="false"/>: chua het
    /// </summary>
    public bool isFinish;
    public override UIScene mySceneType
    {
        get
        {
            return UIScene.ShowHeroAndSkill;
        }
    }

    private void Start()
    {
        listSkillDelay = new IEnumerator[4];
    }
    public override void InitData()
    {
        if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_PLAYER || BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_VIEWER)
        {
            SetActivePanel(false);
        }
        else
        {
            SetActivePanel(true, skillpostion, spellposition);
        }

        canvasShowHero.worldCamera = BOL_Manager.instance.mainCamera.mainCamera;
        _listAction = new List<Action>();
        //OnPressInScene();
        //StartCoroutine(DelayPlaySkillFirstData());


        //DelaySkillWhenPress(timedelayAttack_Q, textAttackQ, _buttonQ, imgDelayQ);
        //DelaySkillWhenPress(timedelayAttack_W, textAttackW, _buttonW, imgDelayW);
        //DelaySkillWhenPress(timedelayAttack_E, textAttackE, _buttonE, imgDelayE);
        //DelaySkillWhenPress(timedelayAttack_Spell, textAttackSpell, _buttonSpell, imgDelaySpell);
        BOL_Manager.instance.StartControlMatrix();
        BOL_Manager.instance.StartCurrentMatrix();
#if TEST
        Debug.Log(Debugs.ColorString("start delay spell", Color.red));
#endif
        if (enumeratorQ != null)
        {
            StopCoroutine(enumeratorQ);
            enumeratorQ = null;
        }
        if (enumeratorW != null)
        {
            StopCoroutine(enumeratorW);
            enumeratorW = null;
        }
        if (enumeratorE != null)
        {
            StopCoroutine(enumeratorE);
            enumeratorE = null;
        }
        if (enumeratorSpell != null)
        {
            StopCoroutine(enumeratorSpell);
            enumeratorSpell = null;
        }
        enumeratorE = btnSkillE.DelaySkill();
        enumeratorW = btnSkillW.DelaySkill();
        enumeratorQ = btnSkillQ.DelaySkill();
        enumeratorSpell = btnSkillSpell.DelaySkill();
        StartCoroutine(enumeratorQ);
        StartCoroutine(enumeratorW);
        StartCoroutine(enumeratorE);
        StartCoroutine(enumeratorSpell);
        OnclickButton();
        Hide();
        StartDelaySkill();
        leftPositionEffect = _Hero_left_ControlSkill.ParentBody.transform.position;
        rightPositionEffect = _Hero_right_ControlSkill.ParentBody.transform.position;
    }
    public void ClearAllActionWhenFinish()
    {
        StopDelaySkill();
        _listAction.Clear();
        _listAction = new List<Action>();
    }
    void OnclickButton()
    {

        moveLeft.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            if (Time.time >= leftTimestamp)
            {
                EventMove(CMD_REALTIME.C_XHCD_MOVE_LEFT);
                leftTimestamp = Time.time + TimeBetweenShots;
            }
        });
        moveRight.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            if (Time.time >= rightTimestamp)
            {
                EventMove(CMD_REALTIME.C_XHCD_MOVE_RIGHT);
                rightTimestamp = Time.time + TimeBetweenShots;
            }
        });
        moveDown.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            if (Time.time >= downTimestamp)
            {
                EventMove(CMD_REALTIME.C_XHCD_MOVE_DOWN);
                downTimestamp = Time.time + TimeBetweenShots;
            }
        });
        changePiece.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            if (Time.time >= changeTimestamp)
            {
                EventMove(CMD_REALTIME.C_XHCD_CHANGE_PIECE_STATE);
                changeTimestamp = Time.time + TimeBetweenShots;
            }
        });
        _buttonQ.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            CallSkill(CMD_REALTIME.C_XHCD_CALLSKILL_1);
        });
        _buttonW.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            CallSkill(CMD_REALTIME.C_XHCD_CALLSKILL_2);
        });
        _buttonE.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            CallSkill(CMD_REALTIME.C_XHCD_CALLSKILL_ULTIMATE);
        });
        _buttonSpell.onClick.AddListener(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
            }
            CallSpell(CMD_REALTIME.C_XHCD_USE_SPELL);
        });
    }
    public void SetActivePanel(bool boolean, int postSkill = 0, int postSpell = 0)
    {
        panelMove.SetActive(boolean);
        panelChange.SetActive(boolean);
        if (boolean)
        {
            GetImageForButton(postSkill, postSpell);
        }
    }
    public void GetImageForButton(int skill, int spell)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("PieceSkill/" + skillHero[skill]);
        btnSkillQ.imageSkill.sprite = sprites[0];
        btnSkillW.imageSkill.sprite = sprites[1];
        btnSkillE.imageSkill.sprite = sprites[2];
        btnSkillSpell.imageSkill.sprite = ListSpellSprite[spell];
    }
    public void Left_Attack1()
    {
        if (_Hero_left_ControlSkill != null)
        {
            _Hero_left_ControlSkill.Attack1();
        }
        else
        {
#if TEST
            Debug.Log("_heroleft control null");
#endif
        }

    }
    public void Left_Attack2()
    {

        if (_Hero_left_ControlSkill != null)
        {
            _Hero_left_ControlSkill.Attack2();
        }
        else
        {
#if TEST
            Debug.Log("_heroleft control null");
#endif
        }
    }
    public void Left_AttackQ()
    {
        if (_Hero_left_ControlSkill != null)
        {
            _Hero_left_ControlSkill.Attack_Q();
        }
        else
        {
#if TEST
            Debug.Log("_heroleft control null");
#endif
            return;
        }

        if (enumeratorQ != null)
        {
            StopCoroutine(enumeratorQ);
            enumeratorQ = null;
        }
        enumeratorQ = btnSkillQ.DelaySkill();
        StartCoroutine(enumeratorQ);
        //DelaySkillWhenPress(timedelayAttack_Q, textAttackQ, _buttonQ, imgDelayQ);
    }
    public void Left_AttackW()
    {
        if (_Hero_left_ControlSkill != null)
        {
            _Hero_left_ControlSkill.Attack_W();
        }
        else
        {
#if TEST
            Debug.Log("_heroleft control null");
#endif
            return;
        }

        if (enumeratorW != null)
        {
            StopCoroutine(enumeratorW);
            enumeratorW = null;
        }
        enumeratorW = btnSkillW.DelaySkill();
        StartCoroutine(enumeratorW);
        //DelaySkillWhenPress(timedelayAttack_W, textAttackW, _buttonW, imgDelayW);
    }
    public void Left_AttackE()
    {
        if (_Hero_left_ControlSkill != null)
        {
            _Hero_left_ControlSkill.Attack_E();
        }
        else
        {
#if TEST
            Debug.Log("_heroleft control null");
#endif
            return;
        }

        if (enumeratorE != null)
        {
            StopCoroutine(enumeratorE);
            enumeratorE = null;
        }
        enumeratorE = btnSkillE.DelaySkill();
        StartCoroutine(enumeratorE);
        //DelaySkillWhenPress(timedelayAttack_E, textAttackE, _buttonE, imgDelayE);
    }
    public void Left_AttackSpell(sbyte spellplay)
    {
        if (_Hero_left_ControlSkill != null)
        {

        }
        else
        {
#if TEST
            Debug.Log("_heroleft control null");
#endif
            return;
        }
        // _Hero_left_ControlSkill.Attack_Spell();
        AttackSpell(spellplay, Constant.CHAIR_LEFT);
        if (enumeratorSpell != null)
        {
            StopCoroutine(enumeratorSpell);
            enumeratorSpell = null;
        }
        enumeratorSpell = btnSkillSpell.DelaySkill();
        StartCoroutine(enumeratorSpell);
        //DelaySkillWhenPress(timedelayAttack_Spell, textAttackSpell, _buttonSpell, imgDelaySpell);
    }
    public void Right_Attack1()
    {
        if (_Hero_right_ControlSkill != null)
        {
            _Hero_right_ControlSkill.Attack1();
        }
        else
        {
#if TEST
            Debug.Log("hero right control null");
#endif
        }

    }
    public void Right_Attack2()
    {

        if (_Hero_right_ControlSkill != null)
        {
            _Hero_right_ControlSkill.Attack2();
        }
        else
        {
#if TEST
            Debug.Log("heroright control null");
#endif
        }
    }
    public void Right_AttackQ()
    {
        if (_Hero_right_ControlSkill != null)
        {
            _Hero_right_ControlSkill.Attack_Q();
        }
        else
        {
#if TEST
            Debug.Log("_hero right control null");
#endif
            return;
        }

        if (enumeratorQ != null)
        {
            StopCoroutine(enumeratorQ);
            enumeratorQ = null;
        }
        enumeratorQ = btnSkillQ.DelaySkill();
        StartCoroutine(enumeratorQ);
        //DelaySkillWhenPress(timedelayAttack_Q, textAttackQ, _buttonQ, imgDelayQ);
    }
    public void Right_AttackW()
    {

        if (_Hero_right_ControlSkill != null)
        {
            _Hero_right_ControlSkill.Attack_W();
        }
        else
        {
#if TEST
            Debug.Log("_hero right control null");
#endif
            return;
        }
        if (enumeratorW != null)
        {
            StopCoroutine(enumeratorW);
            enumeratorW = null;
        }
        enumeratorW = btnSkillW.DelaySkill();
        StartCoroutine(enumeratorW);
        //DelaySkillWhenPress(timedelayAttack_W, textAttackW, _buttonW, imgDelayW);
    }
    public void Right_AttackE()
    {

        if (_Hero_right_ControlSkill != null)
        {
            _Hero_right_ControlSkill.Attack_E();
        }
        else
        {
#if TEST
            Debug.Log("_hero right control null");
#endif
            return;
        }
        if (enumeratorE != null)
        {
            StopCoroutine(enumeratorE);
            enumeratorE = null;
        }
        enumeratorE = btnSkillE.DelaySkill();
        StartCoroutine(enumeratorE);
        //DelaySkillWhenPress(timedelayAttack_E, textAttackE, _buttonE, imgDelayE);
    }
    public void Right_AttackSpell(sbyte spellplay)
    {
        // _Hero_right_ControlSkill.Attack_Spell();
        if (_Hero_right_ControlSkill != null)
        {

        }
        else
        {
#if TEST
            Debug.Log("_hero right control null");
#endif
            return;
        }
        AttackSpell(spellplay, Constant.CHAIR_RIGHT);
        if (enumeratorSpell != null)
        {
            StopCoroutine(enumeratorSpell);
            enumeratorSpell = null;
        }
        enumeratorSpell = btnSkillSpell.DelaySkill();
        StartCoroutine(enumeratorSpell);
        //DelaySkillWhenPress(timedelayAttack_Spell, textAttackSpell, _buttonSpell, imgDelaySpell);
    }
    public void CallSkill(int skill)
    {
#if TEST
        Debug.Log("skill" + skill);
#endif
        MessageSending mgs = new MessageSending((short)skill);
        NetworkGlobal.instance.SendMessageRealTime(mgs);
    }
    public void CallSpell(int spell)
    {
#if TEST
        Debug.Log("spell" + spell);
#endif
        MessageSending mgs = new MessageSending((short)spell);
        NetworkGlobal.instance.SendMessageRealTime(mgs);
    }

    public void AttackSpell(int spell, int chair)
    {

        Vector3 position = Vector3.zero;
        GameObject spellSpawn = null;
        switch (spell)
        {
            case (int)indexSpell.attack1:
                switch (chair)
                {
                    case Constant.CHAIR_LEFT:
                        position = _Hero_right_ControlSkill.ParentBody.transform.position;
                        break;
                    case Constant.CHAIR_RIGHT:
                        position = _Hero_left_ControlSkill.ParentBody.transform.position;
                        break;
                }
                spellSpawn = LeanPool.Spawn(listSpellinGame[spell], position, Quaternion.identity);
                LeanTween.delayedCall(3, () =>
                {
                    LeanPool.Despawn(spellSpawn);
                });
                break;
            case (int)indexSpell.attack2:
                switch (chair)
                {
                    case Constant.CHAIR_LEFT:
                        position = _Hero_right_ControlSkill.myBody.transform.position;
                        break;
                    case Constant.CHAIR_RIGHT:
                        position = _Hero_left_ControlSkill.myBody.transform.position;
                        break;
                }
                spellSpawn = LeanPool.Spawn(listSpellinGame[spell], position, Quaternion.identity);
                LeanTween.delayedCall(3, () =>
                {
                    LeanPool.Despawn(spellSpawn);
                });
                break;
            case (int)indexSpell.health:
            case (int)indexSpell.mana:
            case (int)indexSpell.shield:
                switch (chair)
                {
                    case Constant.CHAIR_LEFT:
                        position = _Hero_left_ControlSkill.myBody.transform.position;
                        break;
                    case Constant.CHAIR_RIGHT:
                        position = _Hero_right_ControlSkill.myBody.transform.position;
                        break;
                }
                spellSpawn = LeanPool.Spawn(listSpellinGame[spell], position, Quaternion.identity);
                LeanTween.delayedCall(3, () =>
                {
                    LeanPool.Despawn(spellSpawn);
                });
                break;
        }

        //GameObject spellChar = LeanPool.Spawn(listSpellinGame[spell], position, Quaternion.identity);
        //LeanTween.delayedCall(2, () => {
        //	LeanPool.Despawn(spellChar);
        //});
        //StartCoroutine(DelayDeSpawn(spellChar, 2));
    }
    IEnumerator DelayDeSpawn(GameObject objectSpell, float timeDelay)
    {
        yield return Yielders.Get(timeDelay);
        LeanPool.Despawn(objectSpell);
    }
    public void EventMove(int eventMove)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        short cmd = Convert.ToInt16(eventMove);
        MessageSending mgs = new MessageSending(cmd);
        NetworkGlobal.instance.SendMessageRealTime(mgs);
    }
    public void StarDelaySpell(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }
    void OnDestroy()
    {
        StopAllCoroutines();
        ins = null;
    }
    public override void Show()
    {
        panelMove.SetActive(true);
        panelChange.SetActive(true);
    }
    public override void Hide()
    {
        panelMove.SetActive(false);
        panelChange.SetActive(false);
    }
    public override void ResetData()
    {
        StopAllCoroutines();
        LeanTween.cancelAll();
    }
    public override void SelfDestruction()
    {
        ins = null;
        base.SelfDestruction();
    }
    public void ResetAnimation(Animator animator)
    {
        animator.speed = 1f;
        animator.SetTrigger(Constant.idle);
        animator.Update(0.1f);
        animator.Play(Constant.idle, -1, 0f);
    }

    public void StartDelaySkill()
    {
        if (_DelayPlaySkill != null)
        {
            StopCoroutine(_DelayPlaySkill);
            _DelayPlaySkill = null;
        }
        _DelayPlaySkill = DelayPlaySkill();
        StartCoroutine(_DelayPlaySkill);
    }
    public void StopDelaySkill()
    {
        if (_DelayPlaySkill != null)
        {
            StopCoroutine(_DelayPlaySkill);
            _DelayPlaySkill = null;
        }

    }
    IEnumerator _DelayPlaySkill;
    IEnumerator DelayPlaySkill()
    {
        while (true)
        {
            yield return new WaitUntil(() => _listAction.Count > 0);
            Debug.Log("_listAction =" + _listAction.Count);
            yield return StartCoroutine(AddActiontoCoroutine(_listAction[0]));
            _listAction.RemoveAt(0);
            yield return new WaitUntil(() => isFinish);
            isFinish = false;
        }
    }
    IEnumerator AddActiontoCoroutine(Action action)
    {
        yield return null;
        action();

    }

    public void CallEffectBreakPiece(int postPiece, int postChair)
    {

        switch (postPiece)
        {
            case (int)Constant.PieceIngame.attack_1:
                break;
            case (int)Constant.PieceIngame.attack_2:
                break;
            case (int)Constant.PieceIngame.health:
                switch (postChair)
                {
                    case Constant.CHAIR_LEFT:
                        {
                            GameObject objectTmp = LeanPool.Spawn(listEffectInPiece[0], leftPositionEffect, Quaternion.identity);
                            break;
                        }
                    case Constant.CHAIR_RIGHT:
                        {
                            GameObject objectTmp = LeanPool.Spawn(listEffectInPiece[0], rightPositionEffect, Quaternion.identity);
                            break;
                        }

                }
                break;
            case (int)Constant.PieceIngame.mana:
                switch (postChair)
                {
                    case Constant.CHAIR_LEFT:
                        {
                            GameObject objectTmp = LeanPool.Spawn(listEffectInPiece[1], leftPositionEffect, Quaternion.identity);
                            break;
                        }
                    case Constant.CHAIR_RIGHT:
                        {
                            GameObject objectTmp = LeanPool.Spawn(listEffectInPiece[1], rightPositionEffect, Quaternion.identity);
                            break;
                        }

                }
                break;
            case (int)Constant.PieceIngame.shield:
                switch (postChair)
                {
                    case Constant.CHAIR_LEFT:
                        {
                            GameObject objectTmp = LeanPool.Spawn(listEffectInPiece[2], leftPositionEffect, Quaternion.identity);
                            break;
                        }
                    case Constant.CHAIR_RIGHT:
                        {
                            GameObject objectTmp = LeanPool.Spawn(listEffectInPiece[2], rightPositionEffect, Quaternion.identity);
                            break;
                        }

                }
                break;
            case (int)Constant.PieceIngame.special:
                break;

        }
    }
}
