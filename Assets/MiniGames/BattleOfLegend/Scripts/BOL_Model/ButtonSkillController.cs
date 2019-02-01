using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSkillController : MonoBehaviour
{

    public long timeDefault;
    public long timeDelay;
    public long manaValue;
    public long manaCharacterValue;
    public GameObject objectImageButton;
    public Text txtTimeDelay;
    public CanvasGroup canvasTxtTimeDelay;
    public Image imageDelay;
    public CanvasGroup canvasImageDelay;
    public Image imageSkill;
    public Button btnSkill;

    LTDescr tweenScale;
    LTDescr tweenAlpha;
    public IEnumerator DelaySkill()
    {
#if TEST
        Debug.Log("start delay skill");
#endif
        double timedelaygame = timeDelay;
        canvasTxtTimeDelay.alpha = 1;
        btnSkill.interactable = false;
        imageDelay.fillAmount = 1;
        canvasImageDelay.alpha = 1;
        timedelaygame = Constant.ConvertMillisecondsToSeconds(timedelaygame);
        float timedelaysecond = (float)Constant.ConvertMillisecondsToSeconds(timeDelay);
        txtTimeDelay.text = string.Format("{0:00}", (long)timedelaygame);
        while (timedelaygame > 0)
        {
            yield return null;
            timedelaygame -= Time.unscaledDeltaTime;
            txtTimeDelay.text = string.Format("{0:00}", (long)timedelaygame);
            imageDelay.fillAmount -= Time.unscaledDeltaTime / timedelaysecond;

        }
        canvasTxtTimeDelay.alpha = 0;
        btnSkill.interactable = true;
        imageDelay.fillAmount = 0;
        canvasImageDelay.alpha = 0;
    }

    public void CheckGlowSkill()
    {		
        switch (BOL_Main_Controller.instance.ChairPosition)
        {
            case Constant.CHAIR_LEFT:
                manaCharacterValue = BOL_Main_Controller.instance._BOL_PlayBattle_left._mpvalue;
                break;
            case Constant.CHAIR_RIGHT:
                manaCharacterValue = BOL_Main_Controller.instance._BOL_PlayBattle_right._mpvalue;
                break;
        }

        if (manaCharacterValue > manaValue)
        {	
			if(tweenAlpha!=null&& tweenScale!=null){

				return;
			}
            TweenImageButton(true);
        }
        else
        {
            TweenImageButton(false);
        }


    }
    public void TweenImageButton(bool boolean)
    {
        if (tweenAlpha != null)
        {
            tweenAlpha = null;
        }
        if (tweenScale != null)
        {
            tweenScale = null;
        }
        if (boolean)
        {
            tweenScale = LeanTween.scale(objectImageButton, Vector3.one *1.5f, 0.5f).setLoopPingPong();
            tweenAlpha = LeanTween.alpha(objectImageButton, 0.3f, 0.5f).setLoopPingPong();
        }
        else
        {
#if TEST
            Debug.Log("mana hien tai thap hon mana tween");
#endif
        }
    }
}
