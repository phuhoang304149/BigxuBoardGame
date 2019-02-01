using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISimpleAnimFrameByFrameController : MonoBehaviour {

    public Image image;
    public Sprite[] sprites;
    public float animationSpeed;
    public bool autoPlayOnEnable;
    IEnumerator actionPlayAnim;

    private void OnEnable()
    {
        if (autoPlayOnEnable) {
            PlayAnim();
        }
    }

    public Coroutine PlayAnim() {
        if (actionPlayAnim != null) {
            return null;
        }
        actionPlayAnim = DoActionPlayAnim();
        return StartCoroutine(actionPlayAnim);
    }

    IEnumerator DoActionPlayAnim()
    {
        while (true) {
            for (int i = 0; i < sprites.Length; i++){
                image.sprite = sprites[i];
                yield return new WaitForSeconds(animationSpeed);
            }
        }
        actionPlayAnim = null;
    }
}
