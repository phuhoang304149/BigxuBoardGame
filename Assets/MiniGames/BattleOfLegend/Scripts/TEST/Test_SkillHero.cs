using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lean.Pool;
using UnityEditor;
using UnityEngine;
using System;

public class Test_SkillHero : MonoBehaviour {
	public static Test_SkillHero instance {
		get {
			return ins;
		}
	}
	public static Test_SkillHero ins;
	private void Awake() {
		ins = this;
		BOL_Main_Controller.instance.tmpCharacterLeft = 12;
		BOL_Main_Controller.instance.tmpCharacterRight = 12;
		listActionInTest = new List<IEnumerator>();
		StartCoroutine(DelayAction());
	}
	public int heroleft;
	public int heroright;
	List<IEnumerator> listActionInTest;
	IEnumerator Convert2Coroutine(Action action) {
		yield return null;
		action();
	}
	public void AddAction2Coroutine(Action action) {
		listActionInTest.Add(Convert2Coroutine(action));
	}
	IEnumerator DelayAction() {
		while (true) {
			yield return new WaitUntil(() => listActionInTest.Count > 0);
			yield return StartCoroutine(listActionInTest[0]);
			listActionInTest.RemoveAt(0);
			yield return new WaitUntil(() => BOL_PlaySkill_Controller.instance.isFinish);
			BOL_PlaySkill_Controller.instance.isFinish = false;
		}
	}
	public void SpawnHeroForTest(int poshero) {
		switch (poshero) {
			case Constant.CHAIR_LEFT:
				BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_LEFT, heroleft, 1);
				BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill.InitData();
				break;
			case Constant.CHAIR_RIGHT:
				BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_RIGHT, heroright, 1);
				BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill.InitData();
				break;
		}
	}
	public void CallAttack1(int poshero) {
		AddAction2Coroutine(() => {
			switch (poshero) {
				case Constant.CHAIR_LEFT:
					BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill.Attack1();
					break;
				case Constant.CHAIR_RIGHT:
					BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill.Attack1();
					break;
			}
		});
	}
	public void CallAttack2(int poshero) {
		AddAction2Coroutine(() => {
			switch (poshero) {
				case Constant.CHAIR_LEFT:
					BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill.Attack2();

					break;
				case Constant.CHAIR_RIGHT:
					BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill.Attack2();
					break;
			}
		});
	}
	public void CallAttackQ(int poshero) {
		AddAction2Coroutine(() => {
			switch (poshero) {
				case Constant.CHAIR_LEFT:
					BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill.Attack_Q();
					break;
				case Constant.CHAIR_RIGHT:
					BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill.Attack_Q();
					break;
			}
		});
	}
	public void CallAttackW(int poshero) {
		AddAction2Coroutine(() => {
			switch (poshero) {
				case Constant.CHAIR_LEFT:
					BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill.Attack_W();
					break;
				case Constant.CHAIR_RIGHT:
					BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill.Attack_W();
					break;
			}
		});
	}
	public void CallAttackE(int poshero) {
		AddAction2Coroutine(() => {
			switch (poshero) {
				case Constant.CHAIR_LEFT:
					BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill.Attack_E();
					break;
				case Constant.CHAIR_RIGHT:
					BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill.Attack_E();
					break;
			}
		});
	}
	public void CallSpell(int poshero, int spell) {
		BOL_PlaySkill_Controller.instance.AttackSpell(spell, poshero);
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(Test_SkillHero))]
public class TestSkillHero : Editor {
	public float values = 40;
	public string Accept;
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		Test_SkillHero myScript = (Test_SkillHero)target;
		GUILayout.Label(">>> For Test <<<");
		if (GUILayout.Button("INIT DATA")) {
		}
		if (GUILayout.Button("Clear data")) {

		}
		if (GUILayout.Button("Terminal")) {
			System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
			proc.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
			proc.Arguments = "ls";
			System.Diagnostics.Process.Start(proc);
		}
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Spawn Hero left")) {
			myScript.SpawnHeroForTest(Constant.CHAIR_LEFT);
		}
		if (GUILayout.Button("Spawn Hero right")) {
			myScript.SpawnHeroForTest(Constant.CHAIR_RIGHT);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("left Attack_1")) {
			myScript.CallAttack1(Constant.CHAIR_LEFT);
		}
		if (GUILayout.Button("right Attack_1")) {
			myScript.CallAttack1(Constant.CHAIR_RIGHT);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("left Attack_2")) {
			myScript.CallAttack2(Constant.CHAIR_LEFT);
		}
		if (GUILayout.Button("right Attack_2")) {
			myScript.CallAttack2(Constant.CHAIR_RIGHT);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("left Attack_Q")) {
			myScript.CallAttackQ(Constant.CHAIR_LEFT);
		}
		if (GUILayout.Button("right Attack_Q")) {
			myScript.CallAttackQ(Constant.CHAIR_RIGHT);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("left Attack_W")) {
			myScript.CallAttackW(Constant.CHAIR_LEFT);
		}
		if (GUILayout.Button("right Attack_W")) {
			myScript.CallAttackW(Constant.CHAIR_RIGHT);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("left Attack_E")) {
			myScript.CallAttackE(Constant.CHAIR_LEFT);
		}
		if (GUILayout.Button("right Attack_E")) {
			myScript.CallAttackE(Constant.CHAIR_RIGHT);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical();
		if (GUILayout.Button("LEFT SPELL 1")) {
			myScript.CallSpell(Constant.CHAIR_LEFT, 0);
		}
		if (GUILayout.Button("LEFT SPELL 2")) {
			myScript.CallSpell(Constant.CHAIR_LEFT, 1);
		}
		if (GUILayout.Button("LEFT SPELL 3")) {
			myScript.CallSpell(Constant.CHAIR_LEFT, 2);
		}
		if (GUILayout.Button("LEFT SPELL 4")) {
			myScript.CallSpell(Constant.CHAIR_LEFT, 3);
		}
		if (GUILayout.Button("LEFT SPELL 5")) {
			myScript.CallSpell(Constant.CHAIR_LEFT, 4);
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical();
		if (GUILayout.Button("RIGHT SPELL 1")) {
			myScript.CallSpell(Constant.CHAIR_RIGHT, 0);
		}
		if (GUILayout.Button("RIGHT SPELL 2")) {
			myScript.CallSpell(Constant.CHAIR_RIGHT, 1);
		}
		if (GUILayout.Button("RIGHT SPELL 3")) {
			myScript.CallSpell(Constant.CHAIR_RIGHT, 2);
		}
		if (GUILayout.Button("RIGHT SPELL 4")) {
			myScript.CallSpell(Constant.CHAIR_RIGHT, 3);
		}
		if (GUILayout.Button("RIGHT SPELL 5")) {
			myScript.CallSpell(Constant.CHAIR_RIGHT, 4);
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}

}
#endif
