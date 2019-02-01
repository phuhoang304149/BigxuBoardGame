using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class HeroMng : MonoBehaviour {
[CreateAssetMenu(fileName = "NewHero", menuName = "GameInfo/BOL/Hero")]
public class HeroMng : ScriptableObject {
	// Use this for initialization
    [DelayedAssetTypeAttribute (typeof(GameObject))]
	public DelayedAsset prefabsss;
}
