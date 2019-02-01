using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroInfo", menuName = "GameInfo/BOL/HeroInfo")]
public class BOL_HeroInfo : ScriptableObject {

	public enum Type {
		  hero1_yasuo,
        hero2_Ashe,
        hero3_Leesin,
       hero4_Jhin,
       hero5_Zed,
       hero6_Talon,
       hero7_Jinx,
       hero8_Tristana,
       hero9_Lux,
       hero10_Nami,
       hero11_Kindred
	}

	[Header("Basic Info")]
	public Type myType;
	[SerializeField] string myNameKeyLocalize;
	public string myName {
		get {
			return MyLocalize.GetString(myNameKeyLocalize);
		}
	}
	public GameObject heroPrefab;
	[DelayedAssetTypeAttribute(typeof(GameObject))]
	//public DelayedAsset prefabsss;
	public bool autoUnlockAtFirst;
	public bool canEnable;

	[Header("Basic Value")]
	public short baseHp;
	public short baseMana;
	public short baseShield;
	public short baseDamageAtk1;
	public short baseDamageAtk2;
	public short baseDamageSkill1;
	public short baseDamageSkill2;
	public short baseDamageSpell;
	public short baseDamagUtil;

	[Header("Exp Need To Unlock")]
	public long expUnlockAtk2;
	public long expUnlockSkill01;
	public long expUnlockSkill02;
	public long expUnlockUlti;

	[Header("Skill")]
	public BOL_SkillInfo skill01;
	public BOL_SkillInfo skill02;
	public BOL_SkillInfo spell;
	public BOL_SkillInfo ulti;
}
