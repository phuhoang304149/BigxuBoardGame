using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
#if UNITY_EDITOR
[CustomEditor(typeof(MatrixControl))]
public class ShowMatrixInspector : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		MatrixControl someClass = (MatrixControl)target;
		string name1 = "nameof(someClass.matrixBreak)";
		string name2 = "nameof(someClass.matrixTween)";
		string name3 = "nameof(someClass.matrixShow)";
		string name4 = "nameof(someClass.ObjectsMatrix)";
		EditorGUILayout.BeginHorizontal();
		SetupArray(someClass.matrixBreak, name1);
		GUILayout.Width(5);
		SetupArray(someClass.matrixTween, name2);
		EditorGUILayout.EndHorizontal();
		SetupArray(someClass.matrixShow, name3);
		SetupArray(someClass.ObjectsMatrix, name4);
	}
	void SetupArray(int[,] matrix, string nameva) {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.PrefixLabel(nameva);
		if (matrix != null && matrix.Length > 0) {
			for (int i = 0; i < 12; i++) {
				EditorGUILayout.BeginHorizontal();
				for (int j = 0; j < 8; j++) {
					matrix[i, j] = EditorGUILayout.IntField(matrix[i, j], GUILayout.Width(15));
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		EditorGUILayout.EndVertical();
	}

	void SetupArray(GameObject[,] matrix, string nameva) {
		EditorGUILayout.BeginVertical();
		EditorGUILayout.PrefixLabel(nameva);
		if (matrix != null && matrix.Length > 0) {
			for (int i = 0; i < 12; i++) {
				EditorGUILayout.BeginHorizontal();
				for (int j = 0; j < 8; j++) {
					//matrix[i, j] = EditorGUILayout.ObjectField(matrix[i, j], GUILayout.Width(15));
					matrix[i, j] = (UnityEngine.GameObject)EditorGUILayout.ObjectField(matrix[i, j], typeof(GameObject), true);
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		EditorGUILayout.EndVertical();
	}
}
#endif
