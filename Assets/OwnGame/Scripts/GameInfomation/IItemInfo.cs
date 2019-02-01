using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemInfo", menuName="GameInfo/IItemInfo")]
public class IItemInfo : ScriptableObject {
	public enum ItemType{
		Gold
	}
	public ItemType itemType;
	public string myName;
	public Sprite icon;
}
