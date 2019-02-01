using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewMiniGameInfo", menuName="GameInfo/MiniGameInfo")]
public class IMiniGameInfo : ScriptableObject {

	public enum Type{
		BattleOfLegend,
		BattleOfRobots,
		AnimalRacing,
		HourseRacing,
		DragonTigerCasino,
		Poker,
		RouletteWheel,
		Blackjack,
		Baccarat,
		Sicbo,
		Koprok,
		KingChess,
		BillionaireChess,
		ChineseChess,
		SeaBattle,
		Dominos,
		DiskShocking,
		Uno, 

		VideoPoker
	}

	public Type gameType;
	public string myName;
	public short gameId;
	[DelayedAssetTypeAttribute (typeof(Sprite))]
	public DelayedAsset gameAvatar;
	public Sprite gameBanner;
	public long versionFeature;
	public bool isSubGame;
	public bool isBetToWinGame;

	[DelayedAssetTypeAttribute (typeof(GameObject))]
	public DelayedAsset gameManagerPrefab;
	public bool canEnable = true;

	public override int GetHashCode()
	{
		return gameId;
	}

	// Default comparer 
	public int CompareTo(IMiniGameInfo _compareMiniGameInfo)
	{
		// A null value means that this object is greater.
		if (_compareMiniGameInfo == null) {
			return 1;
		} else {
			return this.gameId.CompareTo (_compareMiniGameInfo.gameId);
		}
	}
}

#if UNITY_EDITOR
	[CustomEditor(typeof(IMiniGameInfo))]
	public class IMiniGameInfoEditor : Editor
	{	
		SerializedProperty propGameType;
		SerializedProperty propMyName;
		SerializedProperty propGameId;
		SerializedProperty propGameAvatar;
		SerializedProperty propGameBanner;
		SerializedProperty propVersionFeature;
		SerializedProperty propIsSubGame;
		SerializedProperty propIsBetToWinGame;
		SerializedProperty propGameManagerPrefab;
		SerializedProperty propCanEnable;
		
		void OnEnable()
		{
			propGameType = serializedObject.FindProperty("gameType");
			propMyName = serializedObject.FindProperty("myName");
			propGameId = serializedObject.FindProperty("gameId");
			propGameAvatar = serializedObject.FindProperty("gameAvatar");
			propGameBanner = serializedObject.FindProperty("gameBanner");
			propVersionFeature = serializedObject.FindProperty("versionFeature");
			propIsSubGame = serializedObject.FindProperty("isSubGame");
			propIsBetToWinGame = serializedObject.FindProperty("isBetToWinGame");
			propGameManagerPrefab = serializedObject.FindProperty("gameManagerPrefab");
			propCanEnable = serializedObject.FindProperty("canEnable");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(propGameType);
			EditorGUILayout.PropertyField(propMyName);
			EditorGUILayout.PropertyField(propGameId);
			EditorGUILayout.PropertyField(propGameAvatar);
			EditorGUILayout.PropertyField(propIsSubGame);
			EditorGUILayout.PropertyField(propIsBetToWinGame);
			if (propIsSubGame.boolValue){
				EditorGUILayout.PropertyField(propGameManagerPrefab);
			}else{
				EditorGUILayout.PropertyField(propGameBanner);
				EditorGUILayout.PropertyField(propVersionFeature);
			}
			
			EditorGUILayout.PropertyField(propCanEnable);
            serializedObject.ApplyModifiedProperties();
		}
	}
#endif