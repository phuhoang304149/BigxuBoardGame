using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestForGetLocalizeInfo : MonoBehaviour {

	public MyListLocalizeInfo myListLocalizeInfo;

	public string googleSheetAppID;
	public string spreadSheetID;
	public string sheetID;
	[Header("Localize Setting")]
	public string fromCell;
	public string toCell;
	
	[Header("Debug")]
	public List<string> listDetail;

	#if UNITY_EDITOR
	[ContextMenu("Get Info")]
	public void GetInfo(){
		StartCoroutine(DoActionGetInfo());
	}

	IEnumerator DoActionGetInfo(){
		GoogleSheetReader _googleSheetReader = new GoogleSheetReader(googleSheetAppID, spreadSheetID);
		
		Debug.Log("Load Key Localize From Google!!!");
		yield return _googleSheetReader.Load(fromCell, toCell, sheetID);
		var _value = _googleSheetReader.GetValues();
		Debug.Log("Start Load Key: " + _googleSheetReader.rawResult);
		listDetail = new List<string>();
		for(int i = 0; i < _value.Count; i ++){
			for(int j = 0; j < _value[i].Count; j ++){
				string _tmp = _value[i][j].Value;
				if(string.IsNullOrEmpty(_tmp)){
					Debug.LogError("Bug : i = "  + i + " + j = " + j);
					yield break;
				}
				if(_tmp.Equals("NULL")){
					listDetail.Add(string.Empty);
					continue;
				}
				// _tmp = _tmp.ToLower();
				_tmp = _tmp.TrimStart();
				_tmp = _tmp.TrimEnd();
				// _tmp = _tmp.Replace("%", string.Empty); 
				listDetail.Add(_tmp);
			}
		}

		if(listDetail == null || listDetail.Count == 0){
			Debug.Log("listDetail is null!!!");
			yield break;
		}

		myListLocalizeInfo.localize_En.store = StringStringDictionary.New<StringStringDictionary>();

		for(int i = 0; i < listDetail.Count; i++){
			string[] _fields = listDetail[i].Split('#');
			if (_fields == null || _fields.Length == 0) {
				continue;
			}
			string _nameSheet = _fields[0];
			for(int j = 1; j < _fields.Length; j ++){
				if(string.IsNullOrEmpty(_fields[j])){
					Debug.LogWarning("Skip At Index : " + i + " with detail : " + j);
					break;
				}
				string[] _fields_01 = _fields[j].Split(';');
				if (_fields_01 == null || _fields_01.Length == 0) {
					continue;
				}
				string _key = _fields_01[0];
				string _valueEN = _fields_01[1];
				myListLocalizeInfo.localize_En.store.dictionary.Add(_nameSheet+"/"+_key, _valueEN);
			}
		}

		UnityEditor.EditorUtility.SetDirty(myListLocalizeInfo.localize_En);

		Debug.Log("Completed!!!");
	}
	
	#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestForGetLocalizeInfo))]
public class TestForGetLocalizeInfoEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		TestForGetLocalizeInfo myScript = (TestForGetLocalizeInfo)target;

		if (GUILayout.Button ("Get Info")) {
			myScript.GetInfo();
		}
	}
}
#endif