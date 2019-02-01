using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Coffee.UIExtensions;

/**
** ChipDetailController: chip dạng Panel để click chọn chip mình cần
**     - index tham chiếu tới list chip info trong gameplay
**/
public class ChipDetailController : MySimplePoolObjectController, IPointerUpHandler, IPointerExitHandler, IPointerDownHandler {
	
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] RectTransform myRectTransform;
	[SerializeField] Image imgChip;
	[SerializeField] Image imgGlow;
	[SerializeField] CanvasGroup canvasGroupGlow;

	bool isOnPointerExit, isPressed;
	LTDescr tweenRotateGlow, tweenAlphaGlow;
	public ChipDetail chipDetail{get;set;}

	PanelListChipDetailController panelListChipDetail;

	private void Awake() {
		ResetData();
	}

	public override void ResetData(){
		StopAllCoroutines();
		if(tweenRotateGlow != null){
			LeanTween.cancel(tweenRotateGlow.uniqueId);
			tweenRotateGlow = null;
		}
		if(tweenAlphaGlow != null){
			LeanTween.cancel(tweenAlphaGlow.uniqueId);
			tweenAlphaGlow = null;
		}

		isPressed = false;
        isOnPointerExit = false;

		imgGlow.transform.rotation = Quaternion.identity;
		canvasGroupGlow.alpha = 0f;

		chipDetail = null;
		panelListChipDetail = null;
	}

	void ScrollCellIndex (int _index) 
    {	
		panelListChipDetail = transform.parent.parent.parent.parent.gameObject.GetComponent<PanelListChipDetailController>();
		if(panelListChipDetail == null){
			Debug.LogError("panelListChipDetail is null");
		}
		_index = Mathf.Clamp(_index, 0, panelListChipDetail.listChipDetail.Count - 1);
		InitData(panelListChipDetail.listChipDetail[_index]);
	}

	public void InitData(ChipDetail _chipDetail){
		chipDetail = _chipDetail;
		imgChip.sprite = chipDetail.chipInfo.mySprite;
		imgGlow.sprite = chipDetail.chipInfo.mySprite;

		RefreshUI();

		StartCoroutine(DoActionRefreshUI());
	}

	public void RefreshUI(){
		if(chipDetail == null){
			return;
		}
		if(chipDetail.isFocusing){
			SetFocus();
		} else{
			SetUnFocus();
		}
		if(chipDetail.isDisable){
			SetDisable();
		}else{
			SetEnable();
		}
	}

	IEnumerator DoActionRefreshUI(){
		if(chipDetail == null){
			yield break;
		}
		bool _isFocusing = chipDetail.isFocusing;
		bool _isDisable = chipDetail.isDisable;
		while(true){
			yield return Yielders.Get(0.1f);
			if(_isFocusing != chipDetail.isFocusing){
				if(chipDetail.isFocusing){
					SetFocus();
				} else{
					SetUnFocus();
				}
				_isFocusing = chipDetail.isFocusing;
			}
			if(_isDisable != chipDetail.isDisable){
				if(chipDetail.isDisable){
					SetDisable();
				}else{
					SetEnable();
				}
				_isDisable = chipDetail.isDisable;
			}
		}
	}

	[ContextMenu("Focus")]
	public void SetFocus(){
		if(chipDetail == null){
			return;
		}
		if(tweenRotateGlow != null){
			LeanTween.cancel(tweenRotateGlow.uniqueId);
			tweenRotateGlow = null;
		}
		if(tweenAlphaGlow != null){
			LeanTween.cancel(tweenAlphaGlow.uniqueId);
			tweenAlphaGlow = null;
		}

		tweenRotateGlow = LeanTween.rotateAround(imgGlow.gameObject, Vector3.forward, 360f, 2f).setLoopCount(-1);
		tweenAlphaGlow = LeanTween.alphaCanvas(canvasGroupGlow, 1f, 0.5f).setOnComplete(()=>{
			tweenAlphaGlow = LeanTween.alphaCanvas(canvasGroupGlow, 0.5f, 0.5f).setLoopPingPong(-1);
		});
		chipDetail.isFocusing = true;
	}

	[ContextMenu("UnFocus")]
	public void SetUnFocus(){
		if(chipDetail == null){
			return;
		}
		if(tweenRotateGlow != null){
			LeanTween.cancel(tweenRotateGlow.uniqueId);
			tweenRotateGlow = null;
		}
		if(tweenAlphaGlow != null){
			LeanTween.cancel(tweenAlphaGlow.uniqueId);
			tweenAlphaGlow = null;
		}
		
		tweenRotateGlow = LeanTween.rotate(imgGlow.gameObject, Vector3.zero, 0.5f).setOnComplete(()=>{
			tweenRotateGlow = null;
		});

		tweenAlphaGlow = LeanTween.alphaCanvas(canvasGroupGlow, 0f, 0.5f).setOnComplete(()=>{
			tweenAlphaGlow = null;
		});
		chipDetail.isFocusing = false;
	}

	[ContextMenu("Set Disable")]
	public void SetDisable(){
		myCanvasGroup.alpha = 0.3f;
		chipDetail.isDisable = true;
	}

	[ContextMenu("Set Enable")]
	public void SetEnable(){
		myCanvasGroup.alpha = 1f;
		chipDetail.isDisable = false;
	}

	public virtual void OnPointerDown(PointerEventData eventData)
    {
		if(chipDetail == null){
			return;
		}
// #if TEST
//         Debug.Log("Pressed");
// #endif
        isPressed = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
		if(chipDetail == null){
			return;
		}
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
		if(chipDetail == null){
			return;
		}
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
		isPressed = false;
        isOnPointerExit = false;

		if(chipDetail.isDisable){
			PopupManager.Instance.CreateToast("Not Enough Gold");
			return;
		}
        
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		
		panelListChipDetail.OnSelected(chipDetail);
    }
}