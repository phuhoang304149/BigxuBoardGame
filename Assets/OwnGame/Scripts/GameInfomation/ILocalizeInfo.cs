using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLocalizeInfo", menuName = "GameInfo/ILocalizeInfo")]
public class ILocalizeInfo : ScriptableObject
{
    public enum Language
    {
		EN = 0
    }
	public Language myLanguage;

    public StringStringDictionary store;
}


[System.Serializable] public class MyListLocalizeInfo{
    public ILocalizeInfo localize_En;
}