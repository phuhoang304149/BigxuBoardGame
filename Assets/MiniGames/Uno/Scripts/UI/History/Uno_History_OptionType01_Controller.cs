using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

/**
*	Uno_History_OptionType01_Controller : Option dành cho player show bài
**/
public class Uno_History_OptionType01_Controller : MySimplePoolObjectController {

	[SerializeField] Image imgStar;
	[SerializeField] Text txtGoldBonus;
	[SerializeField] RawImage imgAvatar;
	[SerializeField] Text txtName;
	[SerializeField] Text txtPoint;
	[SerializeField] Transform panelCardHolderContainer;
	[SerializeField] Transform panelCardContainer;
	[SerializeField] Transform panelShadow;

	[Header("Setting")]
	[SerializeField] int maxLengthOfUserName;

	[Header("Prefabs")]
	[SerializeField] GameObject cardPrefab;
	[SerializeField] GameObject cardHolderPrefab;

	MySimplePoolManager cardPoolManager;

	public UserDataInGame data{get;set;}

	public override void ResetData(){
		if(data != null){
			data = null;
		}
		if(cardPoolManager != null){
			cardPoolManager.ClearAllObjectsNow();
		}
	}

	public Coroutine InitData(UserDataInGame _userData, bool _isWin, long _goldBonus, int _point, List<sbyte> _cardValue){
		return StartCoroutine(DoActionInitData(_userData, _isWin, _goldBonus, _point, _cardValue));
	}

	IEnumerator DoActionInitData(UserDataInGame _userData, bool _isWin, long _goldBonus, int _point, List<sbyte> _cardValue){
		data = _userData;

		if(cardPoolManager == null){
			cardPoolManager = new MySimplePoolManager();
		}

		if(_isWin){
			imgStar.color = Color.white;
			panelShadow.gameObject.SetActive(false);
			txtGoldBonus.text = "+" + MyConstant.GetMoneyString(_goldBonus, 9999);
			txtGoldBonus.color = Color.yellow;
			txtName.color = Color.yellow;
			txtPoint.color = Color.yellow;
		}else{
			imgStar.color = Color.gray;
			panelShadow.gameObject.SetActive(true);
			txtGoldBonus.text = "-" + MyConstant.GetMoneyString(_goldBonus, 9999);
			txtGoldBonus.color = Color.red;
			txtName.color = Color.white;
			txtPoint.color = Color.white;
		}

		txtName.text = MyConstant.ConvertString(data.nameShowInGame, maxLengthOfUserName);
		txtPoint.text = "" + MyConstant.GetMoneyString(_point);

		imgAvatar.texture = CoreGameManager.instance.gameInfomation.otherInfo.avatarDefault;
		data.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height, (_avatar) => {
			try{
				if(_avatar != null){
					imgAvatar.texture = _avatar;
				}
			}catch{}
		});

		int _totalCards = _cardValue.Count;
		if(_totalCards > 20){
			_totalCards = 20;
		}
		List<CardHolderController> _tmpListCardHolder = new List<CardHolderController>();
		for(int i = 0; i < _totalCards; i++){
			CardHolderController _cardHolder = LeanPool.Spawn(cardHolderPrefab, Vector3.zero, Quaternion.identity, panelCardHolderContainer).GetComponent<CardHolderController>();
			_tmpListCardHolder.Add(_cardHolder);
		}
		yield return Yielders.EndOfFrame;
		
		for(int i = 0; i < _totalCards; i++){
			CardUnoInfo _cardInfo = null;
			if(Uno_GamePlay_Manager.instance.unoGamePlayData.IsWildCardColor(_cardValue[i])){
				_cardInfo = Uno_GamePlay_Manager.instance.GetCardInfo(CardUnoInfo.CardType._Special_WildColor);
			}else if(Uno_GamePlay_Manager.instance.unoGamePlayData.IsWildCardDraw(_cardValue[i])){
				_cardInfo = Uno_GamePlay_Manager.instance.GetCardInfo(CardUnoInfo.CardType._Special_Draw4Cards);
			}else{
				_cardInfo = Uno_GamePlay_Manager.instance.GetCardInfo(_cardValue[i]);
			}
			if(_cardInfo == null){
				#if TEST
				Debug.LogError(">>> Không tìm thấy cardInfo (0): " + _cardValue[i]);
				#endif
			}
			PanelCardUnoDetailController _card = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, panelCardContainer).GetComponent<PanelCardUnoDetailController>();
			cardPoolManager.AddObject(_card);
			_card.transform.position = _tmpListCardHolder[i].transform.position;
			_card.transform.rotation = _tmpListCardHolder[i].transform.rotation;
			_card.ShowNow(_cardInfo, (int) _cardValue[i]);
			_card.ResizeAgain(Uno_GamePlay_Manager.instance.UIManager.sizeCardDefault.x, Uno_GamePlay_Manager.instance.UIManager.sizeCardDefault.y);
			_card.transform.localScale = Vector3.one * _tmpListCardHolder[i].ratioScale;
		}

		for(int i = 0; i < _tmpListCardHolder.Count; i++){
			_tmpListCardHolder[i].SelfDestruction();
		}
		_tmpListCardHolder.Clear();
	}
}
