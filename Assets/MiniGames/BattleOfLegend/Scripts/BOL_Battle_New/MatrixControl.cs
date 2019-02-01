using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class MatrixControl : MonoBehaviour {
	public static MatrixControl instance {
		get {
			return ins;
		}
	}
	static MatrixControl ins;
	void Awake() {
		ins = this;
	}
	public void SeflDestruction() {
		ins = null;
	}
	public int[,] matrixShow = new int[Constant.ROW, Constant.COL];
	public int[,] matrixBreak = new int[Constant.ROW, Constant.COL];
	public int[,] matrixTween = new int[Constant.ROW, Constant.COL];
	public GameObject[,] ObjectsMatrix = new GameObject[Constant.ROW, Constant.COL];
	public GameObject matrixParent;
	public bool finishAddPiece;
	public bool finishBreak;
	public void AddPieceInMatrix(GameObject piece,int valuePiece, int row, int col) {
		ObjectsMatrix[row, col] = SpawnObjectPools(
			   piece,
				ArcadeMainControl.instance.PointFinish(row, col),
				matrixParent.transform
				);
		matrixShow[row, col] = valuePiece;     
		finishAddPiece = true;
	}
	public void AddPieceTween() {
		if (finishBreak) {
		}
	}
	public void AddPieceBreak() {

	}
	public GameObject SpawnObjectPools(GameObject prefab, Vector3 position, Transform parent = null) {
		if (parent != null) {
			GameObject piece = LeanPool.Spawn(prefab, position, Quaternion.identity, parent);
			return piece;
		} else {
			GameObject piece = LeanPool.Spawn(prefab, position, Quaternion.identity);
			return piece;
		}
	}
}
