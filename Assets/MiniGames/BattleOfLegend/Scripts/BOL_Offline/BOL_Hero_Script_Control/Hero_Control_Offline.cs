using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Control_Offline : MonoBehaviour {
	public static Hero_Control_Offline instance {
		get {
			return ins;
		}
	}
	static Hero_Control_Offline ins;
	public List<BOL_HeroInfo> listHeroInfo;
	public List<GameObject> listHeroPrefab;
	BOL_HeroInfo _HeroInfoPlayer { get; set; }
	BOL_HeroInfo _HeroInfoComp { get; set; }
	BOL_SkillInfo _SkillInfoPlayer { get; set; }
	BOL_SkillInfo _SkillInfoComp { get; set; }
	void Awake() {
		ins = this;
	}

}
