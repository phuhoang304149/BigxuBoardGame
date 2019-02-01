using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillInfo", menuName = "GameInfo/BOL/SkillInfo")]
public class BOL_SkillInfo : ScriptableObject {

	public enum Type {
		Hero1_Attack1,	Hero1_Attack2,Hero1_Attack_Ultimate,Hero1_Attack_Spell,
		Hero2_Attack1,Hero2_Attack2,Hero2_Attack_Ultimate,Hero2_Attack_Spell,
		Hero3_Attack1,Hero3_Attack2,Hero3_Attack_Ultimate,Hero3_Attack_Spell,
		Hero4_Attack1,Hero4_Attack2,Hero4_Attack_Ultimate,Hero4_Attack_Spell,
		Hero5_Attack1,Hero5_Attack2,Hero5_Attack_Ultimate,Hero5_Attack_Spell,
		Hero6_Attack1,Hero6_Attack2,Hero6_Attack_Ultimate,Hero6_Attack_Spell,
		Hero7_Attack1,Hero7_Attack2,Hero7_Attack_Ultimate,Hero7_Attack_Spell,
		Hero8_Attack1,Hero8_Attack2,Hero8_Attack_Ultimate,Hero8_Attack_Spell,
		Hero9_Attack1,Hero9_Attack2,Hero9_Attack_Ultimate,Hero9_Attack_Spell,
		Hero10_Attack1,Hero10_Attack2,Hero10_Attack_Ultimate,Hero10_Attack_Spell,
		Hero11_Attack1,Hero11_Attack2,Hero11_Attack_Ultimate,Hero11_Attack_Spell,
	}
	public Type myType;
	[SerializeField] string myNameKeyLocalize;
	public string myName {
		get {
			return MyLocalize.GetString(myNameKeyLocalize);
		}
	}
	[SerializeField] string myDescriptionKeyLocalize;
	public string myDescription {
		get {
			return MyLocalize.GetString(myDescriptionKeyLocalize);
		}
	}

	[Header("Value")]
	public short baseMana;
	public short baseDamage;
	public short baseDuring;
	public short baseRecover;
}