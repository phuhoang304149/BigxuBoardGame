using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="GameInfo/UserNameFilterInfo")]
public class UserNameFilterInfo : ScriptableObject {
	public List<string> listSpecialChars;

//	[ContextMenu("hahaha")]
//	public void Test(){
//		listSpecialChars = new List<string> ();
//		string _tmp =  "\"\'!@#$^&%*()+=-[]\\/{}|:<>?,.~`";
//		char[] _tmp2 = _tmp.ToCharArray ();
//		for (int i = 0; i < _tmp.Length; i++) {
//			listSpecialChars.Add(_tmp.Substring(i, 1));
//		}
//		UnityEditor.AssetDatabase.SaveAssets();
//	}
}
