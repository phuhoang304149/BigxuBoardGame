using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Koprok_History_OptionInfo_Controller : MySimplePoolObjectController {

    [SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Image imgSlot_00;
    [SerializeField] Image imgSlot_01;
    [SerializeField] Image imgSlot_02;

    public override void ResetData(){
        myCanvasGroup.alpha = 0f;
        imgSlot_00.sprite = null;
        imgSlot_01.sprite = null;
        imgSlot_02.sprite = null;
    }

    public void InitData(Sprite _imgSlot00, Sprite _imgSlot01, Sprite _imgSlot02, float _alpha){
        imgSlot_00.sprite = _imgSlot00;
        imgSlot_01.sprite = _imgSlot01;
        imgSlot_02.sprite = _imgSlot02;
        myCanvasGroup.alpha = _alpha;
    }
}
