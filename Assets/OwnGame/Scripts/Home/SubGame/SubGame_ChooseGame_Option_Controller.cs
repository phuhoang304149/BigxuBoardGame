using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubGame_ChooseGame_Option_Controller : MonoBehaviour {

	[SerializeField] IMiniGameInfo.Type gameType;
	[SerializeField] Text txtGameName;
	Vector3 localPosSaved;
	IMiniGameInfo miniGameInfo;

	public System.Action<IMiniGameInfo> onSelected;
	
	private void Awake() {
		localPosSaved = transform.localPosition;
	}

	public void InitData(System.Action<IMiniGameInfo> _onSelected){
		miniGameInfo = GameInformation.instance.GetMiniGameInfo(gameType);
		onSelected = _onSelected;
	}

	public void Show(float _timeTween){
		Color _c = txtGameName.color;
		_c.a = 0f;
		txtGameName.color = _c;

		transform.localPosition = Vector3.zero;
		LeanTween.alphaText(txtGameName.rectTransform, 1f, 0.1f).setDelay(_timeTween - 0.1f);
		LeanTween.moveLocal(gameObject, localPosSaved, _timeTween).setEase(LeanTweenType.easeOutBack);
	}

	public void Hide(float _timeTween, System.Action _onFinished = null){
		LeanTween.alphaText(txtGameName.rectTransform, 0f, 0.1f);
		LeanTween.moveLocal(gameObject, Vector3.zero, _timeTween).setEase(LeanTweenType.easeInBack);
	}

	public void OnSelected(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		if(onSelected != null){
			onSelected(miniGameInfo);
		}
	}
}
