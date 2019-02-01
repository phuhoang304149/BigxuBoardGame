using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BOL_ControllShowSkill : MonoBehaviour
{
    public static BOL_ControllShowSkill instance
    {
        get
        {
            return ins;
        }
    }
    public static BOL_ControllShowSkill ins;
    public CanvasGroup panelChoiceHero;
    public CanvasGroup panelChoiceSpell;
    public CanvasGroup panelShowSkill;

    public Text textShowTime;

    public Image imageSkill1; public Text txtDesSkill1;
    public Image imageSkill2; public Text txtDesSkill2;
    public Image imageSkill3; public Text txtDesSkill3;
    [SerializeField]
    public List<DetailSkillHero> listDetailHero;
    void Awake()
    {
        ins = this;
        panelShowSkill.alpha = 0;
        panelShowSkill.interactable = false;
        panelShowSkill.blocksRaycasts = false;

    }
    public void InitData(int postHero, bool booleanShow)
    {
        listDetailHero[postHero].txtDes1 = MyLocalize.GetString(string.Format("BOL/HeroDetaihero_{0}_skill_1", postHero));
        listDetailHero[postHero].txtDes2 = MyLocalize.GetString(string.Format("BOL/HeroDetaihero_{0}_skill_2", postHero));
        listDetailHero[postHero].txtDes3 = MyLocalize.GetString(string.Format("BOL/HeroDetaihero_{0}_skill_3", postHero));
#if TEST
        Debug.Log("chon hero" + postHero + " show info " + booleanShow);
#endif
        panelShowSkill.interactable = booleanShow;
        panelShowSkill.blocksRaycasts = booleanShow;
        panelChoiceHero.interactable = !booleanShow;
        panelChoiceHero.blocksRaycasts = !booleanShow;
        panelChoiceSpell.interactable = !booleanShow;
        panelChoiceSpell.blocksRaycasts = !booleanShow;
        if (booleanShow)
        {
            panelShowSkill.alpha = 1;
            panelChoiceHero.alpha = 0;
            panelChoiceSpell.alpha = 0;

            imageSkill1.sprite = listDetailHero[postHero].image1;
            imageSkill2.sprite = listDetailHero[postHero].image2;
            imageSkill3.sprite = listDetailHero[postHero].image3;


            txtDesSkill1.text = listDetailHero[postHero].txtDes1;
            txtDesSkill2.text = listDetailHero[postHero].txtDes2;
            txtDesSkill3.text = listDetailHero[postHero].txtDes3;

        }
        else
        {
            panelShowSkill.alpha = 0;
            panelChoiceHero.alpha = 1;
            panelChoiceSpell.alpha = 1;
        }
    }
    public void StartCountdown()
    {
        if (_countdowntime != null)
        {
            StopCoroutine(_countdowntime);
            _countdowntime = null;
        }
        _countdowntime = CountDowntime();
        StartCoroutine(_countdowntime);

    }
    IEnumerator _countdowntime;
    IEnumerator CountDowntime()
    {
        int i = 3;
        textShowTime.text = string.Empty;
        while (i >= 0)
        {
            i=i-1;
            textShowTime.text = i.ToString();
            
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.StopMusic();
                MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Start);
            }
            yield return Yielders.Get(0.5f);

        }

        textShowTime.text = string.Empty;
    }

}
[System.Serializable]
public class DetailSkillHero
{
    public Sprite image1; public string txtDes1;
    public Sprite image2; public string txtDes2;
    public Sprite image3; public string txtDes3;
}