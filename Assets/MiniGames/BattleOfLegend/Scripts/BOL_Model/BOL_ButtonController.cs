using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;
using Lean.Pool;

public class BOL_ButtonController : MonoBehaviour
{
    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    [Header("FOR CHOICE HERO AND SPELL")]
    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    public UIEffect ImageColor;
    public UIEffect ImageHalo;
    public UIShiny ImageAvatar;
    public GameObject imageHalo;
    public CanvasGroup canvasHalo;
    
    public int characterID;
    public int spellID;
    public bool ischoiceSpell;
    public bool _ischoiceSpell
    {
        get
        {
            return ischoiceSpell;
        }
        set
        {
            ischoiceSpell = value;
            if (ischoiceSpell)
            {
                ImageColor.effectFactor = 0;
                imageHalo.SetActive(true);
                if (ImageAvatar != null)
                {
                    ImageAvatar.Play();
                }
            }
            else
            {
                ImageColor.effectFactor = 1;
                imageHalo.SetActive(false);
                if (ImageAvatar != null)
                {
                    ImageAvatar.Stop();
                    ImageAvatar.effectFactor = 0;
                }
            }
        }
    }
    private bool isChoiceHero;
    public bool _isChoiceHero
    {
        get
        {
            return isChoiceHero;
        }
        set
        {
            isChoiceHero = value;
            if (isChoiceHero)
            {
                ImageHalo.effectFactor = 0;
                ImageHalo.colorFactor = 1;
                canvasHalo.alpha = 1;
                if (ImageAvatar != null)
                {
                    ImageAvatar.Play();
                }
            }
            else
            {
                ImageHalo.effectFactor = 1;
                ImageHalo.colorFactor = 0;
                canvasHalo.alpha = 0;
                if (ImageAvatar != null)
                {
                    ImageAvatar.Stop();
                    ImageAvatar.effectFactor = 0;
                }
            }
        }
    }
    public GameObject ChairChoiceSpawn;
    public int tmpCharacterid;

    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    [Header("FOR CHOICE CHAIR")]
    [Header(">>>>>>>>>>>>>><<<<<<<<<<<<")]
    public int sessionIdUserInChair;
    public Text txtName;
    public Text txtNameShowMiniAvatar;
    public Text txtGold;
     public Text txtGoldShowMiniAvatar;
    public GameObject ArrowChoice;
    public MyArrowFocusController _arrowchoice;
    public GameObject PlusChoice;
    public GameObject imgReady;
    public Image _imgReady;
    public Button btnStandUp;
    public Button btnSitdown;

    public PanelPlayerInfoInGameController infoPlayer;
    [SerializeField]
    float timeDelay;
    [SerializeField]
    float timeBetween = 0.5f;

    bool _isChoiceChair;
    public bool isChoiceChair
    {
        get
        {
            return _isChoiceChair;
        }
        set
        {
            _isChoiceChair = value;
            if (_isChoiceChair)
            {
            }
            else
            {
            }
        }
    }
    int _positon;
    public int positon
    {
        get
        {
            return _positon;
        }
        set
        {
            _positon = value;
            if (_positon == Constant.CHAIR_VIEWER)
            {

            }
            else
            {
                Sitdown((byte)_positon);
            }

        }
    }
    public void ShowInfoPlayer(){
        infoPlayer.RefreshGoldInfo();
        infoPlayer.Show();
    }
    
    public void ShowArrow()
    {
        ArrowChoice.SetActive(true);
        _arrowchoice.Show();
    }
    public void HideArrow()
    {
        ArrowChoice.SetActive(false);
        _arrowchoice.Hide();
    }
    public void SetSitdownFail()
    {
        ShowArrow();
        PlusChoice.SetActive(true);
        imgReady.SetActive(false);
        btnStandUp.gameObject.SetActive(false);
        btnStandUp.enabled = false;
        isChoiceChair = false;
        positon = Constant.CHAIR_VIEWER;
        sessionIdUserInChair = -1;
    }
    public void SetSitdownSuccess()
    {
#if TEST
        Debug.Log("player sitdown");
#endif
        HideArrow();
        PlusChoice.SetActive(false);
        imgReady.SetActive(false);
        btnStandUp.gameObject.SetActive(true);
        btnStandUp.enabled = true;
    }
    public void SetStandupSuccess()
    {
#if TEST
        Debug.Log("player standup");
#endif
        ShowArrow();
        PlusChoice.SetActive(true);
        imgReady.SetActive(false);
        btnStandUp.gameObject.SetActive(false); btnStandUp.enabled = false;
        sessionIdUserInChair = -1;
    }
    public void SetStartgame()
    {
        HideArrow();
        PlusChoice.SetActive(false);
        imgReady.SetActive(false);
        btnStandUp.gameObject.SetActive(false); btnStandUp.enabled = false;
        
    }
    public void SetFinishGame(bool boolean)
    {
        if (boolean)
        {
            HideArrow();
            PlusChoice.SetActive(false);
            imgReady.SetActive(false);
            btnStandUp.gameObject.SetActive(true); btnStandUp.enabled = true;
            
        }
        else
        {
            ShowArrow();
            PlusChoice.SetActive(true);
            imgReady.SetActive(false);
            btnStandUp.gameObject.SetActive(false); btnStandUp.enabled = false;
        }
        HideImageReady();
    }
    public void SetFinishforViewer(bool boolean)
    {
        if (boolean)
        {
            HideArrow();
            PlusChoice.SetActive(false);
            imgReady.SetActive(false);
            btnStandUp.gameObject.SetActive(false); btnStandUp.enabled = false;
        }
        else
        {
            ShowArrow();
            PlusChoice.SetActive(true);
            imgReady.SetActive(false);
            btnStandUp.gameObject.SetActive(false); btnStandUp.enabled = false;
        }
        HideImageReady();
    }
    public void ShowImageReady()
    {
        // txtName.color = Color.green;
        // txtGold.color = Color.green;
        // infoPlayer.Show();
        //imgReady.SetActive(true);
        //if (imageReadyGame != null) {
        //	StopCoroutine(imageReadyGame);
        //	imageReadyGame = null;
        //}
        //imageReadyGame = SetShowImageReady(_imgReady);
        //StartCoroutine(imageReadyGame);

    }
    public void HideImageReady()
    {
        // txtName.color = Color.red;
        // txtGold.color = Color.red;
        // infoPlayer.Hide();
        //imgReady.SetActive(true);
        //if (imageReadyGame != null) {
        //	StopCoroutine(imageReadyGame);
        //	imageReadyGame = null;
        //}
        //imageReadyGame = SetHideImageReady(_imgReady);
        //StartCoroutine(imageReadyGame);
    }
    IEnumerator imageReadyGame;
    IEnumerator SetShowImageReady(Image imagePref)
    {
        float ratio = imagePref.fillAmount;
        while (ratio < 1)
        {
            ratio += 0.1f;
            imagePref.fillAmount = ratio;
            yield return Yielders.Get(0.01f);
        }
    }
    IEnumerator SetHideImageReady(Image imagePref)
    {
        float ratio = imagePref.fillAmount;
        while (ratio > 0)
        {
            ratio -= 0.2f;
            imagePref.fillAmount = ratio;
            yield return Yielders.Get(0.01f);
        }
    }
    void SendChoiceChair()
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
    }
    public void SpawnHerowhenchoice(int characterid, int ssid)
    {
        if (ssid > 0)
        {
            if (tmpCharacterid == characterid)
            {

            }
            else
            {
                tmpCharacterid = characterid;
                if (ChairChoiceSpawn != null)
                {
                    LeanPool.Despawn(ChairChoiceSpawn);
                }
                if (BOL_Main_Controller.instance.listHero[characterid].heroPrefab == null)
                {
                    GameObject heroPref = Resources.Load("HeroPrefab/" + BOL_Main_Controller.instance.TypeStringPath[characterid]) as GameObject;
                    BOL_Main_Controller.instance.listHero[characterid].heroPrefab = heroPref;
                }
                BOL_Main_Controller.instance._chairLeftSpawn =
                 LeanPool.Spawn(BOL_Main_Controller.instance.listHero[characterid].heroPrefab,
                     ChairChoiceSpawn.transform.localPosition,
                      Quaternion.identity);
            }
        }
        else
        {
#if TEST
            Debug.Log("ssid <0");
#endif
            if (BOL_Main_Controller.instance._chairLeftSpawn != null)
            {
                LeanPool.Despawn(BOL_Main_Controller.instance._chairLeftSpawn);
            }
        }
    }
    public void Sitdown(byte positionChair)
    {
        if (Time.time >= timeDelay)
        {
            timeDelay = Time.time + timeBetween;
            MessageSending mgs = new MessageSending(CMD_REALTIME.C_GAMEPLAY_SITDOWN);
            mgs.writeByte(positionChair);
            NetworkGlobal.instance.SendMessageRealTime(mgs);
        }
        else
        {
#if TEST
            Debug.Log("chưa den lúc nhấn" + Time.time);
#endif
        }

    }
    public void Standup()
    {
        if (Time.time >= timeDelay)
        {
            timeDelay = Time.time + timeBetween;
            MessageSending mgs = new MessageSending(CMD_REALTIME.C_GAMEPLAY_STANDUP);
            NetworkGlobal.instance.SendMessageRealTime(mgs);
        }
        else
        {
#if TEST
            Debug.Log("chưa den lúc nhấn" + Time.time);
#endif
        }
    }
}
