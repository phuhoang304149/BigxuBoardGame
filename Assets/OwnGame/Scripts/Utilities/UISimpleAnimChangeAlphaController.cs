using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISimpleAnimChangeAlphaController : MonoBehaviour {

    public Image image;
    public float animationSpeed;
    public bool autoPlayOnEnable;
    LTDescr tweenChange;

    private void OnEnable()
    {
        if (autoPlayOnEnable)
        {
            PlayAnim();
        }
    }

    public void PlayAnim()
    {
        if (tweenChange != null)
        {
            return;
        }
        tweenChange = LeanTween.alpha(image.rectTransform, 0.4f, animationSpeed).setLoopPingPong(-1);
    }
}
