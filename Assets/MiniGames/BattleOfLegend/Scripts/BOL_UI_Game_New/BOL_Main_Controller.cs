using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;
using UnityEditor;

public class BOL_Main_Controller : MonoBehaviour
{
    public static BOL_Main_Controller instance
    {
        get
        {
            return ins;
        }
    }
    public static BOL_Main_Controller ins;
    public List<BOL_HeroInfo> listHero;
    public string[] TypeStringPath = {
       "hero1_yasuo",
       "hero2_Ashe",
       "hero3_Leesin",
       "hero4_Jhin",
       "hero5_Zed",
       "hero6_Talon",
       "hero7_Jinx",
       "hero8_Tristana",
       "hero9_Lux",
       "hero10_Nami",
       "hero11_Kindred"
    };
    public const int WIN = 1;
    public const int LOSE = 0;
    public float ratitoscaele;
    public bool isFinishAnimation;
    public enum StateScene
    {
        unknow = 0,
        waiting = 1,
        playing = 2,
        finish = 3,
        showheroandskill = 4,
        choicehero = 5,
        showplayer = 6,
        choiceherofinishgame = 7,
        win = 9,
        lose = 10,
        tie = 11,
        viewer = 12

    }
    public StateScene stateScene
    {
        get
        {
            return StateScene.waiting;
        }
    }

    public StateScene stateWaiting
    {
        get
        {
            return StateScene.showplayer;
        }
    }

    public StateScene stateFinish
    {
        get
        {
            return StateScene.unknow;
        }
    }

    public bool isInitSuccess;
    //public SpriteRenderer imageVS;
    LTDescr tweenScale;
    public Transform mainScreeenHolder;
    public BOL_ListStateScene listStateScene;
    public BOL_MySceneMain _BOL_ShowHeroAndSkill;
    public BOL_MySceneMain _BOL_ChoiceHero;
    public BOL_MySceneMain _BOL_ShowFinishWin;
    public BOL_MySceneMain _BOL_ShowFinishLose;
    public PanelUserInfoInGameController panelUserInfo;
    [Header("-------------------------")]
    [Header("POSITION HERO")]
    [Header("-------------------------")]
    public GameObject chairLeft;
    public GameObject _chairLeftSpawn;
    [SerializeField]
    Animator leftAnimator;
    public BOL_Hero_Controler _hero_left;
    public GameObject chairRight;
    public GameObject _chairRightSpawn;
    [SerializeField]
    Animator rightAnimator;
    public BOL_Hero_Controler _hero_right;
    [Header("Choice Hero")]
    public int ChairPosition;
    public BOL_Battle_PlayerInGame _BOL_PlayBattle_left;
    public BOL_MySceneMain _BOL_PlayBattleleft;
    public BOL_Battle_PlayerInGame _BOL_PlayBattle_right;
    public BOL_MySceneMain _BOL_PlayBattleright;
    List<IEnumerator> listprocess;
    public int tmpCharacterLeft;
    public int tmpCharacterRight;
    void Awake()
    {
        ins = this;
        ChairPosition = Constant.CHAIR_VIEWER;
    }
    void Start()
    {
        ratitoscaele = BOL_Manager.instance.mainCamera.mainCamera.orthographicSize / 2.81203f;
        StartInitData(InitDataWaiting, () => { ActionWaiting(StateScene.showplayer); });
        listprocess = new List<IEnumerator>();
        panelUserInfo.InitData();
        panelUserInfo.Show();

        Starprocess();
        tmpCharacterLeft = 12;
        tmpCharacterRight = 12;

    }
    private void OnEnable()
    {
        StartCoroutine(LoadHeroAfterLoadScene());
    }
    IEnumerator LoadHeroAfterLoadScene()
    {
        for (int i = 0; i < listHero.Count; i++)
        {
            //var request = Resources.LoadAsync("HeroPrefab/" + TypeStringPath[i], typeof(GameObject));
#if TEST
            Debug.Log("load hero positon " + i);
#endif
            yield return StartCoroutine(LoadHero(i));
        }
    }
    IEnumerator LoadHero(int position)
    {
        listHero[position].heroPrefab = Resources.Load<GameObject>("HeroPrefab/" + TypeStringPath[position]);
        yield return null;
    }
    public void StartInitData(Action action1, Action action2)
    {
        StartCoroutine(DelayActionInit(action1, action2));
    }
    IEnumerator DelayActionInit(Action initData, Action actionData)
    {

        initData();
        yield return new WaitUntil(() => isInitSuccess);

        actionData();
    }
    public void InitDataWaiting()
    {
        if (_BOL_ChoiceHero == null)
        {
            _BOL_ChoiceHero = listStateScene.BOL_ChoiceHero;
            _BOL_ChoiceHero.InitData();
        }
        //if (_BOL_ShowPlayer == null) {
        //	_BOL_ShowPlayer = listStateScene.BOL_ShowPlayer;
        //	_BOL_ShowPlayer.InitData();
        //}
        isInitSuccess = true;
        if (_BOL_PlayBattleleft != null)
        {
            _BOL_PlayBattleleft.ResetData();
            _BOL_PlayBattleleft.SelfDestruction();
            _BOL_PlayBattle_left = null;
            _BOL_PlayBattleleft = null;
        }
        if (_BOL_PlayBattleright != null)
        {
            _BOL_PlayBattleright.ResetData();
            _BOL_PlayBattleright.SelfDestruction();
            _BOL_PlayBattle_right = null;
            _BOL_PlayBattleright = null;
        }
        panelUserInfo.RefreshGoldInfo();



    }
    public void InitDataPlaying()
    {
        if (_BOL_PlayBattleleft == null)
        {
            _BOL_PlayBattleleft = listStateScene.BOL_PlayBattle_left;
        }
        if (_BOL_PlayBattleright == null)
        {
            _BOL_PlayBattleright = listStateScene.BOL_PlayBattle_right;
        }
        isInitSuccess = true;
        _BOL_ChoiceHero.ResetData();
        if (tweenScale != null)
        {
            LeanTween.cancel(BOL_ChoiceHero.instance.TableChoice, tweenScale.uniqueId);
            tweenScale = null;
        }
        tweenScale = LeanTween.scale(BOL_ChoiceHero.instance.TableChoice, Vector3.zero, 0.2f).setOnComplete(() =>
        {
            tweenScale = null;
            tweenScale = LeanTween.scale(BOL_ChoiceHero.instance.imageVS, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                tweenScale = null;
                Debug.Log(" null tweanscale");
            });
        }).setEase(LeanTweenType.easeOutBack).setOnCompleteOnStart(true);
        //_BOL_ShowPlayer = null;
        panelUserInfo.Hide();
    }
    public void InitDataFinish()
    {
        //_BOL_PlayBattleleft.ResetData();
        //_BOL_PlayBattleleft.SelfDestruction();
        //_BOL_PlayBattleright.ResetData();
        //_BOL_PlayBattleright.SelfDestruction();
        //_BOL_PlayBattle_left = null;
        //_BOL_PlayBattle_right = null;
        //_BOL_PlayBattleleft = null;
        //_BOL_PlayBattleright = null;
        //ResetAnimation(_chairLeftSpawn.transform.GetChild(0).GetComponent<Animator>());
        //ResetAnimation(_chairRightSpawn.transform.GetChild(0).GetComponent<Animator>());
        ResetAnimation(leftAnimator);
        ResetAnimation(rightAnimator);
        LeanTween.delayedCall(1, () =>
        {
            isInitSuccess = true;
        });
        //Delay(1, () => {
        //	isInitSuccess = true;
        //});
    }
    public void ActionWaiting(StateScene stateStatus)
    {
        isInitSuccess = false;
        _BOL_ShowHeroAndSkill.Hide();
        switch (stateStatus)
        {
            case StateScene.showplayer:
                if (_BOL_ChoiceHero != null)
                {
                    _BOL_ChoiceHero.Hide();
                    //_BOL_ShowPlayer.Show();
                    //imageVS.gameObject.SetActive(true);
                    //if (tweenScale != null) {
                    //	LeanTween.cancel(imageVS.gameObject, tweenScale.uniqueId);
                    //	tweenScale = null;
                    //}
                    //tweenScale = LeanTween.scale(imageVS.gameObject, Vector3.one, 0.5f).setOnComplete(() => { tweenScale = null; }).setEase(LeanTweenType.easeOutBack);
                }
                break;
            case StateScene.choicehero:
                Debug.Log("choice hero   " + ChairPosition);
                _BOL_ShowHeroAndSkill.Hide();
                _BOL_ChoiceHero.Hide();
                if (ChairPosition == Constant.CHAIR_LEFT || ChairPosition == Constant.CHAIR_RIGHT)
                {
                    _BOL_ChoiceHero.Show();
                }
                break;
            case StateScene.choiceherofinishgame:
                _BOL_ShowHeroAndSkill.Hide();
                break;
        }
    }
    public void ActionPlaying()
    {
        isInitSuccess = false;
        if (ChairPosition == Constant.CHAIR_LEFT || ChairPosition == Constant.CHAIR_RIGHT)
        {
            _BOL_ShowHeroAndSkill.Show();
        }
        else
        {
            //if (tweenScale != null) {
            //	LeanTween.cancel(imageVS.gameObject, tweenScale.uniqueId);
            //	tweenScale = null;
            //}
            //tweenScale = LeanTween.scale(imageVS.gameObject, Vector3.zero, 0.2f).setOnComplete(() => {
            //	tweenScale = null;
            //}).setEase(LeanTweenType.easeInBack);
        }
        _BOL_ChoiceHero.Hide();
        _BOL_PlayBattle_left.InitData();
        _BOL_PlayBattle_right.InitData();
    }
    public void ActionFinish(StateScene state, Action action = null)
    {
        isInitSuccess = false;
        _BOL_ShowHeroAndSkill.Hide();
        panelUserInfo.RefreshGoldInfo(true);
        panelUserInfo.Show();
#if TEST
        Debug.Log("ChairPosition " + ChairPosition);
#endif
        if (ChairPosition == Constant.CHAIR_LEFT || ChairPosition == Constant.CHAIR_RIGHT)
        {
            switch (state)
            {
                case StateScene.win:
                    if (_BOL_ShowFinishWin == null)
                    {
#if TEST
                        Debug.Log("show win");
#endif
                        _BOL_ShowFinishWin = listStateScene.BOL_ShowFinishWin;
                        Delay(5, () =>
                        {
                            _BOL_ShowFinishWin.ResetData();
                            _BOL_ShowFinishWin.SelfDestruction();
                            _BOL_ShowFinishWin = null;
                            if (ChairPosition != Constant.CHAIR_PLAYER && ChairPosition != Constant.CHAIR_VIEWER)
                            {
                                BOL_PlaySkill_Controller.instance.isStartGame = false;
                            }
                        });
                        StartInitData(InitDataWaiting, () =>
                        {
                            ActionWaiting(StateScene.choiceherofinishgame);
                            int mySessionid = DataManager.instance.userData.sessionId;
                            if (mySessionid == BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].sessionId)
                            {
                                //BOL_ChoiceHero.instance.SetActiveImageChoice(Constant.CHAIR_LEFT, false, true);
                                BOL_ChoiceHero.instance.btnChoiceChairLeft.isChoiceChair = true;
                                BOL_ChoiceHero.instance.Show();
                            }
                            else if (mySessionid == BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].sessionId)
                            {
                                BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = true;
                                //BOL_ChoiceHero.instance.SetActiveImageChoice(Constant.CHAIR_RIGHT, false, true);
                                BOL_ChoiceHero.instance.Show();
                            }
                        });
                    }
                    break;
                case StateScene.lose:
                    if (_BOL_ShowFinishLose == null)
                    {
                        _BOL_ShowFinishLose = listStateScene.BOL_ShowFinishLose;
                        Delay(5, () =>
                        {
                            _BOL_ShowFinishLose.ResetData();
                            _BOL_ShowFinishLose.SelfDestruction();
                            _BOL_ShowFinishLose = null;
                            if (ChairPosition != Constant.CHAIR_PLAYER && ChairPosition != Constant.CHAIR_VIEWER)
                            {
                                BOL_PlaySkill_Controller.instance.isStartGame = false;
                            }
                        });
                        StartInitData(InitDataWaiting, () =>
                        {
                            ActionWaiting(StateScene.choiceherofinishgame);
                            int mySessionid = DataManager.instance.userData.sessionId;
                            if (mySessionid == BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].sessionId)
                            {
                                //BOL_ChoiceHero.instance.SetActiveImageChoice(Constant.CHAIR_LEFT, false, true);
                                BOL_ChoiceHero.instance.btnChoiceChairLeft.isChoiceChair = true;
                                BOL_ChoiceHero.instance.Show();
                            }
                            else if (mySessionid == BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].sessionId)
                            {
                                //BOL_ChoiceHero.instance.SetActiveImageChoice(Constant.CHAIR_RIGHT, false, true);
                                BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = true;
                                BOL_ChoiceHero.instance.Show();
                            }
                        });
                    }
                    break;
                case StateScene.tie:
                    break;
                case StateScene.viewer:
                    break;
                default:
#if TEST
                    Debug.Log(Debugs.ColorString("không show cái gì cả", Color.red));
#endif
                    break;
            }



        }
        else
        {

            //if (tweenScale != null) {
            //	LeanTween.cancel(imageVS.gameObject, tweenScale.uniqueId);
            //	tweenScale = null;
            //}
            //tweenScale = LeanTween.scale(imageVS.gameObject, Vector3.one, 0.5f).setOnComplete(() => {
            //	tweenScale = null;
            //}).setEase(LeanTweenType.easeOutBack);
        }

    }
    public BOL_MySceneMain GetScreen(BOL_MySceneMain.UIScene _typeScreen)
    {
        switch (_typeScreen)
        {
            case BOL_MySceneMain.UIScene.ShowHeroAndSkill:
                return listStateScene.BOL_ShowHeroAndSkill;
        }
        Debug.LogError("NULL Screen: " + _typeScreen.ToString());
        return null;
    }
    public void SetupFinishGame(GameObject objectfinish)
    {
#if TEST
        Debug.Log("show finish game");
#endif
        LeanTween.rotateZ(objectfinish.transform.GetChild(0).gameObject, 720, 5);
        LeanTween.rotateZ(objectfinish.transform.GetChild(1).gameObject, 720, 5);
        LeanTween.scale(objectfinish.transform.GetChild(2).gameObject, Vector3.one, 1).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(objectfinish.transform.GetChild(3).gameObject, Vector3.one * 0.8f, 1).setEase(LeanTweenType.easeOutBack).setDelay(0.3f).setOnComplete(() =>
        {
            LeanTween.scale(objectfinish.transform.GetChild(3).gameObject, Vector3.one, 1).setOnComplete(() =>
            {
                StartCoroutine(_Delay(4, () =>
                {
                    objectfinish.transform.GetChild(0).transform.localPosition = Vector3.zero;
                    objectfinish.transform.GetChild(1).transform.localPosition = Vector3.zero;
                    objectfinish.transform.GetChild(2).transform.localScale = Vector3.zero;
                    objectfinish.transform.GetChild(3).transform.localScale = Vector3.zero; ;
                    LeanPool.Despawn(objectfinish);
                }));
            });
        });
    }
    public void DespawnGameObject(GameObject objecDespawn)
    {
        if (objecDespawn != null)
        {
            // LeanPool.Despawn(objecDespawn);
            Destroy(objecDespawn);
        }
    }
    public void SpawnHeroWhenChoice(int chairID, int heroID, int sessionId = -1)
    {
#if TEST
        Debug.Log("spawn hero |chairID->" + chairID + "  |hero ID->" + heroID);
#endif
        switch (chairID)
        {
            case Constant.CHAIR_LEFT:
                if (tmpCharacterLeft == heroID)
                {
#if TEST
                    Debug.Log("trung characterid left=> không spawn lại hero");
                    Debug.Log(tmpCharacterLeft + "    " + heroID);
#endif
                }
                else
                {
                    tmpCharacterLeft = heroID;
                    if (_chairLeftSpawn != null)
                    {
                        DespawnGameObject(_chairLeftSpawn);
                        _chairLeftSpawn = null;
                    }
                    if (heroID >= 0)
                    {
                        if (listHero[heroID].heroPrefab == null)
                        {
                            GameObject heroPref = Resources.Load("HeroPrefab/" + TypeStringPath[heroID]) as GameObject;
                            listHero[heroID].heroPrefab = heroPref;
                        }
                        _chairLeftSpawn = LeanPool.Spawn(listHero[heroID].heroPrefab, chairLeft.transform.position, Quaternion.identity);
                        _chairLeftSpawn.transform.localScale = new Vector3(0.6f, 0.6f);
                        leftAnimator = null;
                        leftAnimator = _chairLeftSpawn.transform.GetChild(0).GetComponent<Animator>();
                        _hero_left = _chairLeftSpawn.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
                        _hero_left.chairPositions = Constant.CHAIR_LEFT;
                        BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill = _hero_left;
                    }
                    else
                    {
#if TEST
                        Debug.Log("characterid âm, player đứng dậy, đã xóa hero" + heroID);
#endif
                    }
                }
                if (sessionId >= 0)
                {
                    for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
                    {
                        if (BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId == sessionId)
                        {
                            // BOL_ChoiceHero.instance.btnChoiceChairLeft.txtName.text = BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame;
                            BOL_ChoiceHero.instance.btnChoiceChairLeft.txtGold.text = MyConstant.GetMoneyString(BOL_ShowPlayer_Manager.instance.listUserIngame[i].gold);
                            BOL_ChoiceHero.instance.btnChoiceChairLeft.sessionIdUserInChair = sessionId;
                            break;
                        }
                    }
                }
                else
                {
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.txtName.text = string.Empty;
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.txtGold.text = string.Empty;
                }
                break;
            case Constant.CHAIR_RIGHT:
                if (tmpCharacterRight == heroID)
                {
#if TEST
                    Debug.Log("trung characterid right=> không spawn lại hero");
                    Debug.Log(tmpCharacterRight + "    " + heroID);
#endif
                }
                else
                {
                    tmpCharacterRight = heroID;
                    if (_chairRightSpawn != null)
                    {
                        DespawnGameObject(_chairRightSpawn);
                        _chairRightSpawn = null;
                    }
                    if (heroID >= 0)
                    {
                        if (listHero[heroID].heroPrefab == null)
                        {
                            GameObject abc = Resources.Load("HeroPrefab/" + TypeStringPath[heroID]) as GameObject;
                            listHero[heroID].heroPrefab = abc;
                        }
                        _chairRightSpawn = LeanPool.Spawn(listHero[heroID].heroPrefab, chairRight.transform.position, Quaternion.Euler(new Vector3(0, 180)));
                        _chairRightSpawn.transform.localScale = new Vector3(0.6f, 0.6f);
                        rightAnimator = null;
                        rightAnimator = _chairRightSpawn.transform.GetChild(0).GetComponent<Animator>();
                        _hero_right = _chairRightSpawn.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
                        _hero_right.chairPositions = Constant.CHAIR_RIGHT;
                        BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill = _hero_right;
                    }
                    else
                    {
#if TEST
                        Debug.Log("characterid âm , player đứng dậy, đã xóa hero" + heroID);
#endif
                    }
                }
                if (sessionId >= 0)
                {
#if TEST
                    Debug.Log("session id right ok =" + sessionId);
#endif
                    for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
                    {
                        if (BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId == sessionId)
                        {
                            // BOL_ChoiceHero.instance.btnChoiceChairRight.txtName.text = BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame;
                            BOL_ChoiceHero.instance.btnChoiceChairRight.txtGold.text = MyConstant.GetMoneyString(BOL_ShowPlayer_Manager.instance.listUserIngame[i].gold);
                            BOL_ChoiceHero.instance.btnChoiceChairRight.sessionIdUserInChair = sessionId;
                            break;
                        }
                    }
                }
                else
                {
#if TEST
                    Debug.Log("player đứng dây, session id =" + sessionId);
#endif
                    BOL_ChoiceHero.instance.btnChoiceChairRight.txtName.text = string.Empty;
                    BOL_ChoiceHero.instance.btnChoiceChairRight.txtGold.text = string.Empty;
                }
                break;
        }
    }
    public void OnDestroy()
    {
        ins = null;
    }
    public void onDestruction()
    {
        ins = null;
    }
    public void Delay(float time, Action method)
    {
        StartCoroutine(_Delay(time, method));
    }
    IEnumerator _Delay(float time, Action method)
    {
        yield return Yielders.Get(time);
        method();
    }
    public void ResetAnimation(Animator myAnimator)
    {
        if (myAnimator != null)
        {
            myAnimator.speed = 1f;
            myAnimator.SetTrigger(Constant.idle);
            myAnimator.Update(1f);
            myAnimator.Play(Constant.idle, -1, 0f);
        }

    }
    public IEnumerator _tweenValue1;
    public IEnumerator _tweenValue2;
    public void TweenValue(int result, long golduserleft, long golduserright)
    {
        if (_tweenValue1 != null)
        {
            StopCoroutine(_tweenValue1);
            _tweenValue1 = null;
        }
        if (_tweenValue2 != null)
        {
            StopCoroutine(_tweenValue2);
            _tweenValue2 = null;
        }
        _tweenValue1 = MyConstant.TweenValue(
                BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].gold,
                golduserleft, 20,
                (updateGold) =>
                {
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.txtGold.text = MyConstant.GetMoneyString(updateGold, 99999);
                },
                (finishGold) =>
                {
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.txtGold.text = MyConstant.GetMoneyString(finishGold, 99999);
                    _tweenValue1 = null;
                    BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].gold = golduserleft;
                });
        _tweenValue2 = MyConstant.TweenValue(
                BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].gold,
                golduserright, 20,
                (updateGold) =>
                {
                    BOL_ChoiceHero.instance.btnChoiceChairRight.txtGold.text = MyConstant.GetMoneyString(updateGold, 99999);
                },
                (finishGold) =>
                {
                    BOL_ChoiceHero.instance.btnChoiceChairRight.txtGold.text = MyConstant.GetMoneyString(finishGold, 99999);
                    _tweenValue2 = null;
                    BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].gold = golduserright;
                });
        CoroutineChain.Start.Parallel(_tweenValue1, _tweenValue2);
    }
    IEnumerator _actionProcess;
    IEnumerator _ActionProcess()
    {
        while (true)
        {
            yield return new WaitUntil(() => listprocess.Count > 0);
            yield return StartCoroutine(listprocess[0]);
            listprocess.RemoveAt(0);
        }
    }
    public void AddAction(IEnumerator action)
    {
        listprocess.Add(action);
    }
    public void AddAction(Action action, float timedelay = 0.2f)
    {
        listprocess.Add(proAction(action, timedelay));
    }
    IEnumerator proAction(Action action, float timedelay = 0.2f)
    {
        yield return Yielders.Get(timedelay);
        action();
    }
    public void Starprocess()
    {
        if (_actionProcess != null)
        {
            StopCoroutine(_actionProcess);
            _actionProcess = null;
        }
        _actionProcess = _ActionProcess();
        StartCoroutine(_actionProcess);
    }
}
[Serializable]
public class BOL_ListStateScene
{
    [Header("function")]
    public BOL_MySceneMain _BOL_ShowHeroAndSkill;
    public BOL_MySceneMain _BOL_ChoiceHero;
    public BOL_MySceneMain _BOL_PlayBattle_left;
    public BOL_MySceneMain _BOL_PlayBattle_right;
    public BOL_MySceneMain _BOL_ShowFinishWin;
    public BOL_MySceneMain _BOL_ShowFinishLose;


    public BOL_MySceneMain BOL_ShowHeroAndSkill
    {
        get
        {
            string path = "BattleSceneOnline/CanvasShowHeroAndSkill";
            _BOL_ShowHeroAndSkill
            //= LeanPool.Spawn((GameObject)BOL_ShowHeroAndSkill_prefab.Load(),
            = LeanPool.Spawn(Resources.Load<GameObject>(path),
            Vector3.zero, Quaternion.identity,
            BOL_Main_Controller.instance.mainScreeenHolder).
            GetComponent<BOL_MySceneMain>();
            return _BOL_ShowHeroAndSkill;
        }
    }
    public BOL_MySceneMain BOL_ChoiceHero
    {
        get
        {
            string path = "BattleSceneOnline/CanvasChoiceHero";
            _BOL_ChoiceHero
            //= LeanPool.Spawn((GameObject)BOL_ChoiceHero_prefab.Load(),
            = LeanPool.Spawn(Resources.Load<GameObject>(path),
            Vector3.zero, Quaternion.identity,
            BOL_Main_Controller.instance.mainScreeenHolder).
            GetComponent<BOL_MySceneMain>();
            return _BOL_ChoiceHero;
        }
    }

    public BOL_MySceneMain BOL_PlayBattle_left
    {
        get
        {
            string path = "BattleSceneOnline/CanvasPlayer_Battle";
            GameObject game
            = LeanPool.Spawn(Resources.Load<GameObject>(path),
                //= LeanPool.Spawn((GameObject)BOL_PlayBattle_prefab.Load(),
                Vector3.zero, Quaternion.identity,
                BOL_Main_Controller.instance.mainScreeenHolder);
#if UNITY_ANDROID
            game.transform.localScale = new Vector3(1.3f, 1.3f);
            game.transform.localScale = game.transform.localScale * BOL_Main_Controller.instance.ratitoscaele;
#endif
#if UNITY_IOS
			game.transform.localScale = new Vector3(1.3f, 1.3f);
#endif

            if (game.transform.localScale.x < 0)
            {
                game.transform.localScale = new Vector3(game.transform.localScale.x * (-1), game.transform.localScale.y);
            }
            _BOL_PlayBattle_left = game.GetComponent<BOL_MySceneMain>();
            BOL_Main_Controller.instance._BOL_PlayBattle_left = game.GetComponent<BOL_Battle_PlayerInGame>();
#if TEST
            Debug.Log("khởi tạo xong matrix trái");
#endif
            return _BOL_PlayBattle_left;
        }
    }
    public BOL_MySceneMain BOL_PlayBattle_right
    {
        get
        {
            string path = "BattleSceneOnline/CanvasPlayer_Battle";
            GameObject game
            = LeanPool.Spawn(Resources.Load<GameObject>(path),
                    //= LeanPool.Spawn((GameObject)BOL_PlayBattle_prefab.Load(),
                    Vector3.zero, Quaternion.identity,
                    BOL_Main_Controller.instance.mainScreeenHolder);
#if UNITY_ANDROID
            game.transform.localScale = new Vector3(1.3f, 1.3f);
            game.transform.localScale = game.transform.localScale * BOL_Main_Controller.instance.ratitoscaele;
#endif
#if UNITY_IOS
			game.transform.localScale = new Vector3(1.3f, 1.3f);
#endif
            if (game.transform.localScale.x > 0)
            {
                game.transform.localScale = new Vector3(game.transform.localScale.x * (-1), game.transform.localScale.y);
            }
            _BOL_PlayBattle_right = game.GetComponent<BOL_MySceneMain>();
            BOL_Main_Controller.instance._BOL_PlayBattle_right = game.GetComponent<BOL_Battle_PlayerInGame>();
#if TEST
            Debug.Log("khởi tạo xong matrix phải");
#endif
            return _BOL_PlayBattle_right;
        }
    }
    public BOL_MySceneMain BOL_ShowFinishWin
    {
        get
        {
            string path = "BattleSceneOnline/prefabVictory";
            GameObject _BOL_Object_ShowFinishWin
            = LeanPool.Spawn(Resources.Load<GameObject>(path),
            //= LeanPool.Spawn((GameObject)BOL_ShowFinishWin_prefab.Load(),
            Vector3.zero, Quaternion.identity,
            BOL_Main_Controller.instance.mainScreeenHolder);
            _BOL_Object_ShowFinishWin.transform.localScale = _BOL_Object_ShowFinishWin.transform.localScale * BOL_Main_Controller.instance.ratitoscaele;
            BOL_Main_Controller.instance.SetupFinishGame(_BOL_Object_ShowFinishWin);
            _BOL_ShowFinishWin = _BOL_Object_ShowFinishWin.GetComponent<BOL_MySceneMain>();
            return _BOL_ShowFinishWin;
        }
    }
    public BOL_MySceneMain BOL_ShowFinishLose
    {
        get
        {
            string path = "BattleSceneOnline/prefabDefeat";
            GameObject _BOL_Object_ShowFinishhLose
             //= LeanPool.Spawn((GameObject)BOL_ShowFinishLose_prefab.Load(),
             = LeanPool.Spawn(Resources.Load<GameObject>(path),
            Vector3.zero, Quaternion.identity,
            BOL_Main_Controller.instance.mainScreeenHolder);
            _BOL_Object_ShowFinishhLose.transform.localScale = _BOL_Object_ShowFinishhLose.transform.localScale * BOL_Main_Controller.instance.ratitoscaele;
            BOL_Main_Controller.instance.SetupFinishGame(_BOL_Object_ShowFinishhLose);
            _BOL_ShowFinishLose = _BOL_Object_ShowFinishhLose.GetComponent<BOL_MySceneMain>();
            return _BOL_ShowFinishLose;
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(BOL_Main_Controller))]
public class BOL_Main_Controller_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BOL_Main_Controller myScript = (BOL_Main_Controller)target;

        GUILayout.Space(30);
        GUILayout.Label(">>> For Test <<<");
        if (GUILayout.Button(" PlayerPrefs.DeleteAll()"))
        {
            Debugs.LogRed("successfull");
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif