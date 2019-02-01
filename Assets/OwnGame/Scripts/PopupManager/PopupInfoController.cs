using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupInfoController : IPopupController {
	[SerializeField] Text textInfo;

	public void Init(string _textInfo, System.Action _onClose = null){
		textInfo.text = _textInfo;
		onClose = _onClose;
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (Close);

		Show();
	}

	public override void Close (){
		if(onClose != null){
			onClose.Invoke();
			onClose = null;
		}
		CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (Close);		
		SelfDestruction();
	}
}
