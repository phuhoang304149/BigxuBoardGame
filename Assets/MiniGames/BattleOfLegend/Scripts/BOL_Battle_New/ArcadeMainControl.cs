using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMainControl : MonoBehaviour {

	public static ArcadeMainControl instance {
		get {
			return ins;
		}
	}
	static ArcadeMainControl ins;
	void Awake() {
		ins = this;
	}
	public void SeflDestruction() {
		ins = null;
	}
	public const float Rowfoot = 0.25f;
	public const float Colfoot = -0.25f;
	public GameObject currentPieces;
	public GameObject nextPieces;
	public List<GameObject> pieces_default;
	public int rowBg, colBg, rowFn, colFn, rowCr, colCr;
	public float speedMoveVertical = 0.1f;
	LTDescr movetweenVertical;
	private void Start() {
		InitData();
	}
	public void InitData() {
		PiecesControl.instance.SpawnNewObject();
		Delay(PiecesControl.instance.finishSpawnNewObject, () => {
			currentPieces.transform.localPosition = PointFinish(0, 3);
			colCr = 3;
			rowCr = 0;
			PiecesControl.instance.CreateValueCurrentPiece();
			SetValuePiece(currentPieces,
				PiecesControl.instance.nextPiece1,
				PiecesControl.instance.nextPiece2,
				PiecesControl.instance.nextPiece3
			);
			PiecesControl.instance.SpawnNewObject();
			Delay(PiecesControl.instance.finishSpawnNewObject, () => {
				SetValuePiece(nextPieces,
					PiecesControl.instance.nextPiece1,
					PiecesControl.instance.nextPiece2,
					PiecesControl.instance.nextPiece3
				);
				StarMove(0, GetRowFinishMove(0, 3), movetweenVertical);
			});
		});
	}




	IEnumerator _moveVertical;
	IEnumerator MoveVertical(int rbegin, int rfinish, LTDescr movetween) {
		yield return null;
		while (rbegin < rfinish) {
			rbegin++;
			rowCr = rbegin;
			movetween = LeanTween.moveLocalY(currentPieces,
			VectorRowFinish(rowCr), speedMoveVertical).setOnComplete(() => {
				movetween = null;
			});
			yield return Yielders.Get(speedMoveVertical);
		}
		if (PiecesControl.instance.piece1 != 0) {
			int piece = PiecesControl.instance.piece1;
			MatrixControl.instance.AddPieceInMatrix(pieces_default[piece], piece, rowCr, colCr);
		}
		if (PiecesControl.instance.piece2 != 0) {
			int piece = PiecesControl.instance.piece2;
			MatrixControl.instance.AddPieceInMatrix(pieces_default[piece], piece, rowCr - 1, colCr);

		}
		if (PiecesControl.instance.piece3 != 0) {
			int piece = PiecesControl.instance.piece3;
			MatrixControl.instance.AddPieceInMatrix(pieces_default[piece], piece, rowCr - 2, colCr);
		}
	}
	void StarMove(int rbegin, int rfinish, LTDescr movetween) {
		if (_moveVertical != null) {
			StopCoroutine(_moveVertical);
			_moveVertical = null;
		}
		_moveVertical = MoveVertical(rbegin, rfinish, movetween);
		StartCoroutine(_moveVertical);
	}
	public int GetRowFinishMove(int row, int column) {
		int i = row;
		while (i < 11) {
			i++;
			if (MatrixControl.instance.matrixShow[i, column] != 0) {
				return i - 1;
			}
		}
		return i;
	}
	public void ResetMain() {
		//resetpiece;
		for (int i = 0; i < 3; i++) {
			currentPieces.GetComponent<SpriteRenderer>().sprite = null;
			nextPieces.GetComponent<SpriteRenderer>().sprite = null;
		}
	}
	void Delay(float time, Action method) {
		StartCoroutine(_Delay(time, method));
	}
	void Delay(bool boolean, Action method) {
		StartCoroutine(_Delay(boolean, method));
	}
	IEnumerator _Delay(float time, Action method) {
		yield return Yielders.Get(time);
		method();
	}
	IEnumerator _Delay(bool boolean, Action method) {
		yield return new WaitUntil(() => boolean);
		boolean = false;
		method();
	}
	public Vector3 PointFinish(int row, int column) {
		return new Vector3(column * Rowfoot + Rowfoot / 2, row * Colfoot + Colfoot / 2);
	}
	float VectorRowFinish(int row) {
		return row * Colfoot + Colfoot / 2;
	}
	float VectorColumnFinish(int column) {
		return column * Rowfoot + Rowfoot / 2;
	}
	void SetValuePiece(GameObject piceceObject, int value1, int value2, int value3) {
		if (value1 != 0) {
			piceceObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite =
					pieces_default[value1].GetComponent<SpriteRenderer>().sprite;
		} else {
			piceceObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;

		}
		if (value2 != 0) {
			piceceObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite =
				   pieces_default[value2].GetComponent<SpriteRenderer>().sprite;
		} else {
			piceceObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
		}
		if (value3 != 0) {
			piceceObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite =
				   pieces_default[value3].GetComponent<SpriteRenderer>().sprite;
		} else {
			piceceObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
		}

	}
IEnumerator SpawnObject(){
		yield return new WaitUntil(() => MatrixControl.instance.finishAddPiece);
        
 }

}
