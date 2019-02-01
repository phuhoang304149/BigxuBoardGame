using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

public class SetInfoHero : MonoBehaviour {
	static MessageSending SetDataInfo;
	public List<HeroInfomation> hero = new List<HeroInfomation>();
	public HeroInfomation hero1 = new HeroInfomation();
	public HeroInfomation hero2 = new HeroInfomation();
	public HeroInfomation hero3 = new HeroInfomation();
	public HeroInfomation hero4 = new HeroInfomation();
	public HeroInfomation hero5 = new HeroInfomation();
	public HeroInfomation hero6 = new HeroInfomation();
	public HeroInfomation hero7 = new HeroInfomation();
	public HeroInfomation hero8 = new HeroInfomation();
	public HeroInfomation hero9 = new HeroInfomation();
	public HeroInfomation hero10 = new HeroInfomation();
	public HeroInfomation hero11 = new HeroInfomation();
	public string[] title = { "hero", "dmg1", "dmg2", "mHP", "mMN", "mShld", "MNsk1", "MNsk2", "MNult", "dlSk1", "dlSk2", "dlUlt" };
	public string[] heroname;
	public void ConfigData() {
		//buttonScr.IP = PlayerPrefs.GetString(string.Format("room'{0}'IP", i));
		//PlayerPrefs.SetFloat(string.Format("room'{0}'bet", i), bet);

	}
	public void AddChild() {
		hero.Clear();
		hero.Add(hero1);
		hero.Add(hero2);
		hero.Add(hero3);
		hero.Add(hero4);
		hero.Add(hero5);
		hero.Add(hero6);
		hero.Add(hero7);
		hero.Add(hero8);
		hero.Add(hero9);
		hero.Add(hero10);
		hero.Add(hero11);
	}
	public void GetDataClient() {
		AddChild();

		if (hero != null) {
			for (int i = 0; i < hero.Count; i++) {
				hero[i].damgeAttack1 = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._damge1, i), 1);
				hero[i].damgeAttack2 = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._damge2, i), 2);
				hero[i].max_hp = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._max_hp, i), 3);
				hero[i].max_mana = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._max_mana, i), 4);
				hero[i].max_shield = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._max_shield, i), 5);
				hero[i].manaOfSkill1 = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._manaOfSkill1, i), 6);
				hero[i].manaOfSkill2 = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._manaOfSkill2, i), 7);
				hero[i].manaOfUltimate = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._manaOfUltimate, i), 8);
				hero[i].delayOfSkill1 = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._delayOfSkill1, i), 9);
				hero[i].delayOfSkill2 = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._delayOfSkill2, i), 10);
				hero[i].delayOfUltimate = (short)PlayerPrefs.GetInt(string.Format(HeroInfomation._delayOfUltimate, i), 11);
			}
		}
		print("Load successful " + hero.Count + " hero");
	}
	public void SaveDataClient() {
		if (hero != null) {
			for (int i = 0; i < hero.Count; i++) {
				PlayerPrefs.SetInt(string.Format(HeroInfomation._damge1, i), hero[i].damgeAttack1);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._damge2, i), hero[i].damgeAttack2);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._max_hp, i), hero[i].max_hp);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._max_mana, i), hero[i].max_mana);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._max_shield, i), hero[i].max_shield);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._manaOfSkill1, i), hero[i].manaOfSkill1);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._manaOfSkill2, i), hero[i].manaOfSkill2);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._manaOfUltimate, i), hero[i].manaOfUltimate);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._delayOfSkill1, i), hero[i].delayOfSkill1);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._delayOfSkill2, i), hero[i].delayOfSkill2);
				PlayerPrefs.SetInt(string.Format(HeroInfomation._delayOfUltimate, i), hero[i].delayOfUltimate);
			}
		}
		print("save successful " + hero.Count + " hero");
	}
	public void SetAndGetData() {
		AddChild();
		SendDATA(hero,
			 (_messageReceiving, _error) => {
				 if (_messageReceiving != null) {
					 Debug.Log("nhan du liêu " + hero.Count);
					 var sbytenhanve = _messageReceiving.readByte();
					 GetDataClient();
				 } else {
#if TEST
					 Debug.LogError("Error Code: " + _error);
#endif
				 }
			 }
		 );
	}
	public void SendDATA(List<HeroInfomation> listhero, System.Action<MessageReceiving, int> _onFinished) {
		if (CoreGameManager.instance.giaLapNgatKetNoi) {
			if (_onFinished != null) {
				_onFinished(null, 1);
			}
			return;
		}
		if (CoreGameManager.instance.giaLapMangChapChon) {
			if (Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang) {
				if (_onFinished != null) {
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (SetDataInfo == null) {
			SetDataInfo = new MessageSending(CMD_ONEHIT.TEST_SETUP_BOL);
		} else {
			SetDataInfo.ClearData();
		}
		Debug.Log("read " + listhero.Count);
		for (int i = 0; i < listhero.Count; i++) {
			SetDataInfo.writeshort(listhero[i].damgeAttack1);//damge1
			SetDataInfo.writeshort(listhero[i].damgeAttack2);//damge2
			SetDataInfo.writeshort(listhero[i].max_hp);//maxHp
			SetDataInfo.writeshort(listhero[i].max_mana);//maxMana
			SetDataInfo.writeshort(listhero[i].max_shield);//maxShield
			SetDataInfo.writeshort(listhero[i].manaOfSkill1);//manaofskill1
			SetDataInfo.writeshort(listhero[i].manaOfSkill2);//manaofskill2
			SetDataInfo.writeshort(listhero[i].manaOfUltimate);//manaofultimate
			SetDataInfo.writeshort(listhero[i].delayOfSkill1);//delayskill1
			SetDataInfo.writeshort(listhero[i].delayOfSkill2);//delayskill2
			SetDataInfo.writeshort(listhero[i].delayOfUltimate);//delayultimate
			Debug.Log(
				 listhero[i].damgeAttack1 + "|" +
				 listhero[i].damgeAttack2 + "|" +
				 listhero[i].max_hp + "|" +
				 listhero[i].max_mana + "|" +
				 listhero[i].max_shield + "|" +
				 listhero[i].manaOfSkill1 + "|" +
				 listhero[i].manaOfSkill2 + "|" +
				 listhero[i].manaOfUltimate + "|" +
				 listhero[i].delayOfSkill1 + "|" +
				 listhero[i].delayOfSkill2 + "|" +
				 listhero[i].delayOfUltimate
					);
		}
#if TEST
		Debug.Log(">>>CMD Register : " + SetDataInfo.getCMD());
		Debug.Log(Debugs.ColorString("Send Sucessfull", Color.red));
#endif
		NetworkGlobal.instance.StartOnehit(SetDataInfo, _onFinished);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SetInfoHero))]
public class Test_SetInfoHero : Editor {
	public float values = 40;
	public string Accept;
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		SetInfoHero myScript = (SetInfoHero)target;
		SetupTitle(myScript.title);
		SetupList(myScript.hero1, myScript.heroname[0]);
		SetupList(myScript.hero2, myScript.heroname[1]);
		SetupList(myScript.hero3, myScript.heroname[2]);
		SetupList(myScript.hero4, myScript.heroname[3]);
		SetupList(myScript.hero5, myScript.heroname[4]);
		SetupList(myScript.hero6, myScript.heroname[5]);
		SetupList(myScript.hero7, myScript.heroname[6]);
		SetupList(myScript.hero8, myScript.heroname[7]);
		SetupList(myScript.hero9, myScript.heroname[8]);
		SetupList(myScript.hero10, myScript.heroname[9]);
		SetupList(myScript.hero11, myScript.heroname[10]);
		GUILayout.Label(">>> For Test <<<");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(">>> input YES for Save<<<");
		Accept = EditorGUILayout.TextField(Accept, GUILayout.Width(values));
		EditorGUILayout.EndHorizontal();



		if (GUILayout.Button("Set and Get Data from server")) {
			myScript.SetAndGetData();
		}
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Get Data In Client")) {
			myScript.GetDataClient();
		}
		if (GUILayout.Button("Save Data In Client") && Accept == "YES") {
			myScript.SaveDataClient();
			Accept = null;
		}
		EditorGUILayout.EndHorizontal();

	}
	void SetupList(HeroInfomation matrix, string nameva) {
		EditorGUILayout.BeginHorizontal();
		matrix.heroname = EditorGUILayout.TextField(nameva, GUILayout.Width(50));
		matrix.damgeAttack1 = (short)EditorGUILayout.IntField(matrix.damgeAttack1, GUILayout.Width(values));
		matrix.damgeAttack2 = (short)EditorGUILayout.IntField(matrix.damgeAttack2, GUILayout.Width(values));
		matrix.max_hp = (short)EditorGUILayout.IntField(matrix.max_hp, GUILayout.Width(values));
		matrix.max_mana = (short)EditorGUILayout.IntField(matrix.max_mana, GUILayout.Width(values));
		matrix.max_shield = (short)EditorGUILayout.IntField(matrix.max_shield, GUILayout.Width(values));
		matrix.manaOfSkill1 = (short)EditorGUILayout.IntField(matrix.manaOfSkill1, GUILayout.Width(values));
		matrix.manaOfSkill2 = (short)EditorGUILayout.IntField(matrix.manaOfSkill2, GUILayout.Width(values));
		matrix.manaOfUltimate = (short)EditorGUILayout.IntField(matrix.manaOfUltimate, GUILayout.Width(values));
		matrix.delayOfSkill1 = (short)EditorGUILayout.IntField(matrix.delayOfSkill1, GUILayout.Width(values));
		matrix.delayOfSkill2 = (short)EditorGUILayout.IntField(matrix.delayOfSkill2, GUILayout.Width(values));
		matrix.delayOfUltimate = (short)EditorGUILayout.IntField(matrix.delayOfUltimate, GUILayout.Width(values));
		EditorGUILayout.EndHorizontal();
	}
	void SetupTitle(string[] titles) {
		EditorGUILayout.BeginHorizontal();
		for (int i = 0; i < titles.Length; i++) {
			if (i == 0) {
				titles[i] = EditorGUILayout.TextField(titles[i], GUILayout.Width(50));
			} else {
				titles[i] = EditorGUILayout.TextField(titles[i], GUILayout.Width(values));
			}

		}
		EditorGUILayout.EndHorizontal();
	}
}
#endif

public class HeroInfomation {
	public const string _damge1 = "hero'{0}'_damge1";
	public const string _damge2 = "hero'{0}'_damge2";
	public const string _max_hp = "hero'{0}'_max_hp";
	public const string _max_mana = "hero'{0}'_max_mana";
	public const string _max_shield = "hero'{0}'_max_shield";
	public const string _manaOfSkill1 = "hero'{0}'_manaOfSkill1";
	public const string _manaOfSkill2 = "hero'{0}'_manaOfSkill2";
	public const string _manaOfUltimate = "hero'{0}' _manaOfUltimate";
	public const string _delayOfSkill1 = "hero'{0}'_delayOfSkill1";
	public const string _delayOfSkill2 = "hero'{0}_delayOfSkill2";
	public const string _delayOfUltimate = "hero'{0}'delayOfUltimate";
	public string heroname;
	public short damgeAttack1;
	public short damgeAttack2;
	public short max_hp;
	public short max_mana;
	public short max_shield;
	public short manaOfSkill1;
	public short manaOfSkill2;
	public short manaOfUltimate;
	public short delayOfSkill1;
	public short delayOfSkill2;
	public short delayOfUltimate;
}
