using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameInformation", menuName="GameInfo/CoreGameInformation")]
public class GameInformation : ScriptableObject {
	public static GameInformation instance{
		get{
			return CoreGameManager.instance.gameInfomation;
		}
	}

	public UserNameFilterInfo userNameFilterInfo;
	public List<IMiniGameInfo> listMiniGames;
	public List<IAvatarInfo> listAvatars;
	public List<IItemInfo> listItems;
	public MyListLocalizeInfo myListLocalizeInfo;

	[Header("Global Audio Info")]
	public GlobalAudioInfo globalAudioInfo;

	[Header("BOL Info")]
	public List<BOL_HeroInfo> listHeroInfo;

	[Header("Game Bet To Win Info")]
	public IGameBetToWinInfo luckyWheelInfo;
	public IGameBetToWinInfo slotPokerInfo;

	[Header("Other Info")]
	public OthersInfo otherInfo;
	[DelayedAssetTypeAttribute (typeof(Sprite))]
	public DelayedAsset tipInviteFriend;

	public IMiniGameInfo GetMiniGameInfo(IMiniGameInfo.Type _gameType){
		if (listMiniGames == null || listMiniGames.Count == 0) {
			Debug.LogError ("listMiniGames is NULL"); 
			return null;
		}
		for(int i = 0; i < listMiniGames.Count; i++){
			if (listMiniGames [i].gameType == _gameType) {
				return listMiniGames [i];
			}
		}
		return null;
	}

	public IMiniGameInfo GetMiniGameInfo(short _gameId){
		if (listMiniGames == null || listMiniGames.Count == 0) {
			Debug.LogError ("listMiniGames is NULL"); 
			return null;
		}
		for(int i = 0; i < listMiniGames.Count; i++){
			if (listMiniGames [i].gameId == _gameId) {
				return listMiniGames [i];
			}
		}
		return null;
	}

	public IAvatarInfo GetAvatarInfo(sbyte _id){
		if (listAvatars == null || listAvatars.Count == 0) {
			Debug.LogError ("listAvatars is NULL"); 
			return null;
		}
		for(int i = 0; i < listAvatars.Count; i++){
			if (listAvatars [i].avatarid == _id) {
				return listAvatars [i];
			}
		}
		return null;
	}

	public IItemInfo GetItemInfo(IItemInfo.ItemType _itemType){
		if (listItems == null || listItems.Count == 0) {
			Debug.LogError ("listItems is NULL"); 
			return null;
		}
		for(int i = 0; i < listItems.Count; i++){
			if (listItems [i].itemType == _itemType) {
				return listItems [i];
			}
		}
		return null;
	}

	public BOL_HeroInfo GetHeroInfo(BOL_HeroInfo.Type _heroType){
		if (listHeroInfo == null || listHeroInfo.Count == 0) {
			Debug.LogError ("listHeroInfo is NULL"); 
			return null;
		}
		for(int i = 0; i < listHeroInfo.Count; i++){
			if (listHeroInfo [i].myType == _heroType) {
				return listHeroInfo [i];
			}
		}
		return null;
	}

	#if UNITY_EDITOR
	[ContextMenu("Sort List minigame info")]
	void SortListMiniGameInfo(){
		listMiniGames.Sort(delegate(IMiniGameInfo x, IMiniGameInfo y)
			{
				return x.gameId.CompareTo(y.gameId);
			});
		
		UnityEditor.AssetDatabase.SaveAssets ();
	}
	#endif
}

[System.Serializable] public class OthersInfo{
	public Texture2D avatarDefault;
	public Texture2D avatarIncognito;
	public Sprite iconAccFb;
	public Sprite iconAccDevice;
	public Sprite iconAccGoogle;
	public Sprite iconAccBigXu;

	public long timeToGetListServerByGameId = 3600000;
	public List<Sprite> listChooseTableIcon;
}

[System.Serializable] public class GlobalAudioInfo{

	[Header("Sfx")]
	public AudioClip sfx_Click;
	public AudioClip sfx_TogglePanel;
	public AudioClip sfx_Gold;
}