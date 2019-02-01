using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MyButtonController : MonoBehaviour, IPointerUpHandler, IPointerExitHandler, IPointerDownHandler
{
    public CanvasGroup myGlowCanvasGroup;
    public Transform myGlow;
    public UnityEvent onClick;
    protected bool isOnPointerExit, isPressed;
    IEnumerator actionOnClicked;

    void OnEnable()
    {
        isPressed = false;
        isOnPointerExit = false;

        myGlowCanvasGroup.alpha = 0;
        LeanTween.cancel(myGlow.gameObject);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
// #if TEST
//         Debug.Log("Pressed");
// #endif

        isPressed = true;
        //TODO: Play Animation

		myGlowCanvasGroup.alpha = 1f;
		myGlow.rotation = Quaternion.identity;
		LeanTween.rotateZ(myGlow.gameObject, -180f, 0.25f).setRepeat(-1);

        if (actionOnClicked != null)
        {
            StopCoroutine(actionOnClicked);
            actionOnClicked = null;
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!isPressed)
        {
            return;
        }
// #if TEST
//         Debug.Log("isOnPointerExit : " + isOnPointerExit);
// #endif
        isOnPointerExit = true;
    }

    //Do this when the mouse click on this selectable UI object is released.
    public virtual void OnPointerUp(PointerEventData eventData)
    {
		myGlowCanvasGroup.alpha = 0f;
		LeanTween.cancel(myGlow.gameObject);

        if (isOnPointerExit || !isPressed)
        {
// #if TEST
//             Debug.Log("OnPointerUp return");
// #endif
            isPressed = false;
            isOnPointerExit = false;
            return;
        }
// #if TEST
//         Debug.Log("The mouse click was released");
// #endif

        actionOnClicked = DoActionOnClicked();
        StartCoroutine(actionOnClicked);
    }

    IEnumerator DoActionOnClicked()
    {
        if (isOnPointerExit)
        {
            isPressed = false;
            isOnPointerExit = false;
            yield break;
        }
        isPressed = false;
        isOnPointerExit = false;
        float _countTime = 0;
        while (_countTime < 0.15f)
        {
            yield return null;
            _countTime += Time.unscaledDeltaTime;
        }
        if (onClick != null)
        {
            onClick.Invoke();
        }
        actionOnClicked = null;
    }
}