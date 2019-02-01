using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using System;
using UnityEditor;
using System.Security.Cryptography;

public class MainControlArcade : MonoBehaviour {
	public static MainControlArcade instance {
		get {
			return ins;
		}
	}
	static MainControlArcade ins;
	public MainControlArcade() { }
	public const float Rowfoot = 0.25f;
	public const float Colfoot = -0.25f;
	public const int BeginRow = 0;
	public const int BeginCol = 3;
	public const float speedVertical = 0.4f;
	public const float speedHorizontal = 0.1f;
	public const float TimeBetweenShots = 0.2f;  // Allow 3 shots per second
	public float speedMove;
	float leftTimestamp, rightTimestamp, downTimestamp;
	LTDescr moveTween;
	LTDescr moveTweenHorizontal;
	LTDescr moveTweenVertical;
	public GameObject MatrixParent;
	public List<GameObject> pieces_default;
	/// <summary>
	/// The objectbreak.
	/// </summary>
	public GameObject objectbreak;
	/// <summary>
	///  piece hiện tại
	/// </summary>
	public GameObject currentPiece;
	/// <summary>
	/// piece tiếp theo
	/// </summary>
	public GameObject nextPiece;
	/// <summary>
	/// toa độ ô gạch
	/// </summary>
	public int[,] matrix = new int[12, 8];
	/// <summary>
	/// gameobject trong matrix
	/// </summary>
	public GameObject[,] matrixGameOject = new GameObject[12, 8];
	/// <summary>
	/// list gameobject break
	/// </summary>
	public GameObject[,] listObjectBreak = new GameObject[12, 8];
	/// <summary>
	/// list object dùng để tween  sau khi nổ
	/// </summary>
	public GameObject[,] listObjectTweenAfterBreak = new GameObject[12, 8];
	/// <summary>
	/// list object dùng để check sau khi tween và nổ
	/// </summary>
	public int[,] listTweenCheckAfterbreak = new int[12, 8];
	public Text matrixDebugs;
	//public Text matrixBreakDebugs;
	public Text matrixTweenDebugs;
	public Text textFinishGame;
	public Text textCountDown;
	int countPiece;
	int countNextPiece;
	int countEmpty;
	int value_1, value_1_nextpiece;
	int value_2, value_2_nextpiece;
	int value_3, value_3_nextpiece;
	int row_value1;
	int col_value1;
	int row_value2;
	int col_value2;
	int row_value3;
	int col_value3;
	float rowCurrent;
	float colCurrent;
	int rowBegin;
	int columnBegin;
	int rowFinish;
	int columnFinish;
	int countup, countdown, countleft, countright, countUpRight, countDownLeft, countUpLeft, countDownRight;
	int countObject = 0;
	bool finishTween;
	public int countObjectBreak;
	public int countPiece1Break, countPiece2Break, countPiece3break, countPiece4break, countPiece5break, countPiece6break;
	public Button btnLeft, btnRight, btnDown, btnChange, btnStart;
	public bool moveObject;
	private void Awake() {
		ins = this;
	}
	void OnDestroy() {
		ins = null;
	}
	public void SelfDestruction() {
		StopAllCoroutines();
		ResetMatrix();
		ShowMatrix();
		columnFinish = 3;
		speedMove = speedVertical;
		countNextPiece = 0;
	}
	void Start() {
		ResetMatrix();
		ShowMatrix();
		columnFinish = 3;
		speedMove = speedVertical;
		btnLeft.onClick.AddListener(() => {
			if (Time.time >= leftTimestamp) {
				if (colCurrent > 0) {
					StartMoveHorizontal(rowCurrent, colCurrent - 1, moveTween);
				}
				leftTimestamp = Time.time + TimeBetweenShots;
			}
		});

		btnRight.onClick.AddListener(() => {
			if (Time.time >= rightTimestamp) {
				if (colCurrent < 7) {
					StartMoveHorizontal(rowCurrent, colCurrent + 1, moveTween);
				}
				rightTimestamp = Time.time + TimeBetweenShots;
			}
		});
		btnDown.onClick.AddListener(() => {
			speedMove = 0.01f;
			downTimestamp = Time.time + TimeBetweenShots;
		});
		btnChange.onClick.AddListener(() => {
			ChangePiece();
		});
		btnStart.onClick.AddListener(() => {
			SpawnObject();
		});
	}
	void Update() {
		if (Time.time >= leftTimestamp && Input.GetKeyDown(KeyCode.LeftArrow)&&moveObject) {
			if (colCurrent > 0) {
				StartMoveHorizontal(rowCurrent, colCurrent - 1, moveTween);
			}
			leftTimestamp = Time.time + TimeBetweenShots;
		} else if (Time.time >= rightTimestamp && Input.GetKeyDown(KeyCode.RightArrow)&&moveObject) {
			if (colCurrent < 7) {
				StartMoveHorizontal(rowCurrent, colCurrent + 1, moveTween);
			}
			rightTimestamp = Time.time + TimeBetweenShots;
		} else if (Input.GetKeyDown(KeyCode.UpArrow)&&moveObject) {
			ChangePiece();
		} else if (Time.time >= downTimestamp && Input.GetKeyDown(KeyCode.DownArrow)&&moveObject) {
			speedMove = 0.01f;
			downTimestamp = Time.time + TimeBetweenShots;
		} else if (Input.GetKeyDown(KeyCode.Space)) {
			TestSpawn();
		}
	}
	public void TestSpawn() {
		SpawnObject();
	}
	public void ResetMatrix() {
		for (int i = 0; i < 12; i++) {
			for (int j = 0; j < 8; j++) {
				matrix[i, j] = 0;
				DespawnObjectPool(matrixGameOject[i, j]);
				matrixGameOject[i, j] = null;
			}
		}
		for (int i = 0; i < 3; i++) {
			currentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
			nextPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
		}
	}
	public void ShowMatrix() {
		matrixDebugs.text = "matrix" + "\n";
		for (int i = 0; i < 12; i++) {
			for (int j = 0; j < 8; j++) {
				matrixDebugs.text += matrix[i, j] + "   ";
			}
			matrixDebugs.text += " \n ";
		}
		matrixTweenDebugs.text = "matrix tween" + "\n";
		for (int i = 0; i < 12; i++) {
			for (int j = 0; j < 8; j++) {
				if (listObjectTweenAfterBreak[i, j] != null) {
				} else matrixTweenDebugs.text += 0 + "   ";

			}
			matrixTweenDebugs.text += " \n ";
		}
	}
	public void SpawnNextPiece() {
		for (int i = 0; i < 3; i++) {
			nextPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
		}
		value_1_nextpiece = UnityEngine.Random.Range(1, 7);
		countNextPiece = 1;
		nextPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = pieces_default[value_1_nextpiece].GetComponent<SpriteRenderer>().sprite;
		value_2_nextpiece = UnityEngine.Random.Range(0, 7);
		if (value_2_nextpiece != 0) {
			nextPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = pieces_default[value_2_nextpiece].GetComponent<SpriteRenderer>().sprite;
			countNextPiece = 2;
			value_3_nextpiece = UnityEngine.Random.Range(0, 7);
			if (value_3_nextpiece != 0) {
				countNextPiece = 3;
				nextPiece.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = pieces_default[value_3_nextpiece].GetComponent<SpriteRenderer>().sprite;
			}
		}
	}
	public void SpawnObject() {
		if (countNextPiece != 0) {
			countPiece = countNextPiece;
			for (int i = 0; i < 3; i++) {
				currentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
			}
			currentPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = nextPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
			currentPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = nextPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
			currentPiece.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = nextPiece.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite;
			value_1 = value_1_nextpiece;
			value_2 = value_2_nextpiece;
			value_3 = value_3_nextpiece;
			switch (countPiece) {
				case 1:
					currentPiece.transform.localPosition = new Vector3(BeginCol * Rowfoot + Rowfoot / 2, BeginRow * Colfoot + Colfoot / 2);
					rowBegin = 0;
					rowFinish = GetRowFinishMove(0, 3);
					break;
				case 2:
					currentPiece.transform.localPosition = new Vector3(BeginCol * Rowfoot + Rowfoot / 2, (BeginRow + 1) * Colfoot + Colfoot / 2);
					rowBegin = 1;
					rowFinish = GetRowFinishMove(1, 3);
					break;
				case 3:
					currentPiece.transform.localPosition = new Vector3(BeginCol * Rowfoot + Rowfoot / 2, (BeginRow + 2) * Colfoot + Colfoot / 2);
					rowBegin = 2;
					rowFinish = GetRowFinishMove(2, 3);
					break;
			}
		} else {
			Debugs.LogRed("vào chỗ này" + countNextPiece);
			for (int i = 0; i < 3; i++) {
				currentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
			}
			value_1 = UnityEngine.Random.Range(1, 7);
			countPiece = 1;
			currentPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = pieces_default[value_1].GetComponent<SpriteRenderer>().sprite;
			currentPiece.transform.localPosition = new Vector3(BeginCol * Rowfoot + Rowfoot / 2, BeginRow * Colfoot + Colfoot / 2);
			//matrix[0, 3] = value_1;
			rowBegin = 0;
			rowFinish = GetRowFinishMove(0, 3);
			value_2 = UnityEngine.Random.Range(0, 7);
			if (value_2 != 0) {
				currentPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = pieces_default[value_2].GetComponent<SpriteRenderer>().sprite;
				currentPiece.transform.localPosition = new Vector3(BeginCol * Rowfoot + Rowfoot / 2, (BeginRow + 1) * Colfoot + Colfoot / 2);
				//matrix[0, 3] = value_2;
				//matrix[1, 3] = value_1;
				countPiece = 2;
				rowBegin = 1;
				rowFinish = GetRowFinishMove(1, 3);
				value_3 = UnityEngine.Random.Range(0, 7);
				if (value_3 != 0) {
					currentPiece.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = pieces_default[value_3].GetComponent<SpriteRenderer>().sprite;
					currentPiece.transform.localPosition = new Vector3(BeginCol * Rowfoot + Rowfoot / 2, (BeginRow + 2) * Colfoot + Colfoot / 2);
					//matrix[0, 3] = value_3;
					//matrix[1, 3] = value_2;
					//matrix[2, 3] = value_1;
					rowBegin = 2;
					rowFinish = GetRowFinishMove(2, 3);
					countPiece = 3;
				}
			}
		}
		if (rowFinish <= rowBegin) {
			ShowFinish();
		} else {
			SpawnNextPiece();
			StartMoveVertical(rowBegin, rowFinish, columnFinish, moveTween);
		}
	}
	public void ChangePiece() {
		switch (countPiece) {
			case 1:
				// Do no thing
				break;
			case 2:
				Sprite sptitetmp = currentPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
				int valuetmp = value_1;
				value_1 = value_2;
				value_2 = valuetmp;
				currentPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = currentPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
				currentPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sptitetmp;
				break;
			case 3:
				Sprite sptitetmps = currentPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
				int valuetmps = value_1;
				value_1 = value_2;
				value_2 = value_3;
				value_3 = valuetmps;
				currentPiece.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = currentPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
				currentPiece.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = currentPiece.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite;
				currentPiece.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = sptitetmps;
				break;
		}
	}
	IEnumerator MoveVertical(float rbegin, float rfinish, float cfinish, LTDescr movetween) {
		ShowMatrix();
		yield return Yielders.Get(0.1f);
		while (rbegin < rfinish) {
			rbegin++;
			rowCurrent = rbegin;
			colCurrent = cfinish;
			movetween = LeanTween.moveLocalY(currentPiece, VectorRowFinish((int)rowCurrent), speedMove).setOnComplete(() => {
				movetween = null;
			});
			yield return Yielders.Get(speedMove);
		}
		yield return new WaitUntil(() => movetween == null);
		for (int i = 0; i < 3; i++) {
			currentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
		}
		AddPieceAndValueInMatrix(value_1, value_2, value_3, 0, 3, (int)rowCurrent, (int)colCurrent);
		yield return Yielders.Get(0.1f);
		BreakObject();
		speedMove = speedVertical;
		ShowMatrix();
		TweenAfterBreak();
		yield return new WaitUntil(() => finishTween);
		StartScanMatrix();
		if (countObject != 0) {
			yield return Yielders.Get(0.1f);
			while (countObject != 0) {
				yield return Yielders.Get(0.1f);
				StartScanMatrix();
			}
		}
		yield return new WaitUntil(() => countObject == 0);
		SpawnObject();
		moveObject = true;
	}
	IEnumerator _moveVertical;
	public void StartMoveVertical(float rbegin, float rfinish, float cfinish, LTDescr movetween) {
		if (_moveVertical == null) {
			_moveVertical = MoveVertical(rbegin, rfinish, cfinish, movetween);
			StartCoroutine(_moveVertical);
		} else {
			StopMoveVertical();
			_moveVertical = MoveVertical(rbegin, rfinish, cfinish, movetween);
			StartCoroutine(_moveVertical);
		}
	}
	public void StopMoveVertical() {
		if (_moveVertical != null) {
			StopCoroutine(_moveVertical);
			_moveVertical = null;
		}
	}
	IEnumerator MoveHorizontal(float rfinish, float cfinish, LTDescr movetween) {
		StopMoveVertical();
		yield return null;
		movetween = LeanTween.moveLocalX(currentPiece, VectorColumnFinish((int)cfinish), speedHorizontal).setOnComplete(() => {
			movetween = null;
		});
		yield return Yielders.Get(speedHorizontal);
		rowFinish = GetRowFinishMove((int)rfinish, (int)cfinish);
		StopMoveHorizontal();
		StartMoveVertical(rfinish, rowFinish, cfinish, movetween);
	}
	IEnumerator _moveHorizontal;
	public void StartMoveHorizontal(float rfinish, float cfinish, LTDescr movetween) {
		if (cfinish > 0 || cfinish < 8) {
			if (matrix[(int)rowCurrent, (int)cfinish] == 0) {
				colCurrent = cfinish;
				if (_moveHorizontal == null) {
					_moveHorizontal = MoveHorizontal(rfinish, cfinish, movetween);
					StartCoroutine(_moveHorizontal);
				} else {
					StopMoveHorizontal();
					_moveHorizontal = MoveHorizontal(rfinish, cfinish, movetween);
					StartCoroutine(_moveHorizontal);
				}
			}
		} else {
			StopMoveHorizontal();
		}
	}
	public void StopMoveHorizontal() {
		if (_moveHorizontal != null) {
			StopCoroutine(_moveHorizontal);
			_moveHorizontal = null;
		}
	}
	/// <summary>
	/// Adds the piece, value in matrix and add scan + add piece break to list break;
	/// </summary>
	public void AddPieceAndValueInMatrix(int value1, int value2, int value3, int rowBegin, int columnBegin, int rowfinish, int colfinish) {
		moveObject = false;
		switch (countPiece) {
			case 1:
				matrixGameOject[rowfinish, colfinish] = SpawnObjectPools(pieces_default[value1], PointFinish(rowfinish, colfinish), MatrixParent.transform);
				//matrix[rowBegin, columnBegin] = 0;
				matrix[rowfinish, colfinish] = value1;
				row_value1 = rowfinish;
				col_value1 = colfinish;
				ScanVertical(row_value1, col_value1);
				if ((countup + countdown) >= 2) {
					for (int i = 0; i <= countdown; i++) {
						AddObjecToBreak(row_value1 + i, col_value1);
					}
					for (int i = 0; i <= countup; i++) {
						AddObjecToBreak(row_value1 - i, col_value1);
					}
				}
				ScanHorizontal(row_value1, col_value1);
				if (countleft + countright >= 2) {
					for (int i = 0; i <= countleft; i++) {
						AddObjecToBreak(row_value1, col_value1 - i);
					}
					for (int i = 0; i <= countright; i++) {
						AddObjecToBreak(row_value1, col_value1 + i);
					}
				}
				ScanCrossUp(row_value1, col_value1);
				if (countUpRight + countDownLeft >= 2) {
					for (int i = 0; i <= countUpRight; i++) {
						AddObjecToBreak(row_value1 - i, col_value1 + i);
					}
					for (int i = 0; i <= countDownLeft; i++) {
						AddObjecToBreak(row_value1 + i, col_value1 - i);
					}
				}
				ScanCrossDown(row_value1, col_value1);
				if (countUpLeft + countDownRight >= 2) {
					for (int i = 0; i <= countUpLeft; i++) {
						AddObjecToBreak(row_value1 - i, col_value1 - i);
					}
					for (int i = 0; i <= countDownRight; i++) {
						AddObjecToBreak(row_value1 + i, col_value1 + i);
					}
				}
				break;
			case 2:
				matrixGameOject[rowfinish, colfinish] = SpawnObjectPools(pieces_default[value1], PointFinish(rowfinish, colfinish), MatrixParent.transform);
				matrixGameOject[rowfinish - 1, colfinish] = SpawnObjectPools(pieces_default[value2], PointFinish(rowfinish - 1, colfinish), MatrixParent.transform);
				//matrix[rowBegin, columnBegin] = 0;
				//matrix[rowBegin + 1, columnBegin] = 0;
				matrix[rowfinish, colfinish] = value1;
				matrix[rowfinish - 1, colfinish] = value2;
				row_value1 = rowfinish;
				col_value1 = colfinish;
				row_value2 = rowfinish - 1;
				col_value2 = colfinish;
				ScanVertical(row_value1, col_value1);
				if ((countup + countdown) >= 2) {
					for (int i = 0; i <= countdown; i++) {
						AddObjecToBreak(row_value1 + i, col_value1);
					}
					for (int i = 0; i <= countup; i++) {
						AddObjecToBreak(row_value1 - i, col_value1);
					}
				}
				ScanHorizontal(row_value1, col_value1);
				if (countleft + countright >= 2) {
					for (int i = 0; i <= countleft; i++) {
						AddObjecToBreak(row_value1, col_value1 - i);
					}
					for (int i = 0; i <= countright; i++) {
						AddObjecToBreak(row_value1, col_value1 + i);
					}
				}
				ScanHorizontal(row_value2, col_value2);
				if (countleft + countright >= 2) {
					for (int i = 0; i <= countleft; i++) {
						AddObjecToBreak(row_value2, col_value2 - i);
					}
					for (int i = 0; i <= countright; i++) {
						AddObjecToBreak(row_value2, col_value2 + i);
					}
				}
				ScanCrossUp(row_value1, col_value1);
				if (countUpRight + countDownLeft >= 2) {
					for (int i = 0; i <= countUpRight; i++) {
						AddObjecToBreak(row_value1 - i, col_value1 + i);
					}
					for (int i = 0; i <= countDownLeft; i++) {
						AddObjecToBreak(row_value1 + i, col_value1 - i);
					}
				}
				ScanCrossUp(row_value2, col_value2);
				if (countUpRight + countDownLeft >= 2) {
					for (int i = 0; i <= countUpRight; i++) {
						AddObjecToBreak(row_value2 - i, col_value2 + i);
					}
					for (int i = 0; i <= countDownLeft; i++) {
						AddObjecToBreak(row_value2 + i, col_value2 - i);
					}
				}
				ScanCrossDown(row_value1, col_value1);
				if (countUpLeft + countDownRight >= 2) {
					for (int i = 0; i <= countUpLeft; i++) {
						AddObjecToBreak(row_value1 - i, col_value1 - i);
					}
					for (int i = 0; i <= countDownRight; i++) {
						AddObjecToBreak(row_value1 + i, col_value1 + i);
					}
				}
				ScanCrossDown(row_value2, col_value2);
				if (countUpLeft + countDownRight >= 2) {
					for (int i = 0; i <= countUpLeft; i++) {
						AddObjecToBreak(row_value2 - i, col_value2 - i);
					}
					for (int i = 0; i <= countDownRight; i++) {
						AddObjecToBreak(row_value2 + i, col_value2 + i);
					}
				}

				break;
			case 3:
				matrixGameOject[rowfinish, colfinish] = SpawnObjectPools(pieces_default[value1], PointFinish(rowfinish, colfinish), MatrixParent.transform);
				matrixGameOject[rowfinish - 1, colfinish] = SpawnObjectPools(pieces_default[value2], PointFinish(rowfinish - 1, colfinish), MatrixParent.transform);
				matrixGameOject[rowfinish - 2, colfinish] = SpawnObjectPools(pieces_default[value3], PointFinish(rowfinish - 2, colfinish), MatrixParent.transform);
				//matrix[rowBegin, columnBegin] = 0;
				//matrix[rowBegin + 1, columnBegin] = 0;
				//matrix[rowBegin + 2, columnBegin] = 0;
				matrix[rowfinish, colfinish] = value1;
				matrix[rowfinish - 1, colfinish] = value2;
				matrix[rowfinish - 2, colfinish] = value3;
				row_value1 = rowfinish;
				col_value1 = colfinish;
				row_value2 = rowfinish - 1;
				col_value2 = colfinish;
				row_value3 = rowfinish - 2;
				col_value3 = colfinish;
				ScanVertical(row_value1, col_value1);
				if ((countup + countdown) >= 2) {
					for (int i = 0; i <= countdown; i++) {
						AddObjecToBreak(row_value1 + i, col_value1);
					}
					for (int i = 0; i <= countup; i++) {
						AddObjecToBreak(row_value1 - i, col_value1);
					}
				}
				ScanHorizontal(row_value1, col_value1);
				if (countleft + countright >= 2) {
					for (int i = 0; i <= countleft; i++) {
						AddObjecToBreak(row_value1, col_value1 - i);
					}
					for (int i = 0; i <= countright; i++) {
						AddObjecToBreak(row_value1, col_value1 + i);
					}
				}
				ScanHorizontal(row_value2, col_value2);
				if (countleft + countright >= 2) {
					for (int i = 0; i <= countleft; i++) {
						AddObjecToBreak(row_value2, col_value2 - i);
					}
					for (int i = 0; i <= countright; i++) {
						AddObjecToBreak(row_value2, col_value2 + i);
					}
				}
				ScanHorizontal(row_value3, col_value3);
				if (countleft + countright >= 2) {
					for (int i = 0; i <= countleft; i++) {
						AddObjecToBreak(row_value3, col_value3 - i);
					}
					for (int i = 0; i <= countright; i++) {
						AddObjecToBreak(row_value3, col_value3 + i);
					}
				}
				ScanCrossUp(row_value1, col_value1);
				if (countUpRight + countDownLeft >= 2) {
					for (int i = 0; i <= countUpRight; i++) {
						AddObjecToBreak(row_value1 - i, col_value1 + i);
					}
					for (int i = 0; i <= countDownLeft; i++) {
						AddObjecToBreak(row_value1 + i, col_value1 - i);
					}
				}
				ScanCrossUp(row_value2, col_value2);
				if (countUpRight + countDownLeft >= 2) {
					for (int i = 0; i <= countUpRight; i++) {
						AddObjecToBreak(row_value2 - i, col_value2 + i);
					}
					for (int i = 0; i <= countDownLeft; i++) {
						AddObjecToBreak(row_value2 + i, col_value2 - i);
					}
				}
				ScanCrossUp(row_value3, col_value3);
				if (countUpRight + countDownLeft >= 2) {
					for (int i = 0; i <= countUpRight; i++) {
						AddObjecToBreak(row_value3 - i, col_value3 + i);
					}
					for (int i = 0; i <= countDownLeft; i++) {
						AddObjecToBreak(row_value3 + i, col_value3 - i);
					}
				}
				ScanCrossDown(row_value1, col_value1);
				if (countUpLeft + countDownRight >= 2) {
					for (int i = 0; i <= countUpLeft; i++) {
						AddObjecToBreak(row_value1 - i, col_value1 - i);
					}
					for (int i = 0; i <= countDownRight; i++) {
						AddObjecToBreak(row_value1 + i, col_value1 + i);
					}
				}
				ScanCrossDown(row_value2, col_value2);
				if (countUpLeft + countDownRight >= 2) {
					for (int i = 0; i <= countUpLeft; i++) {
						AddObjecToBreak(row_value2 - i, col_value2 - i);
					}
					for (int i = 0; i <= countDownRight; i++) {
						AddObjecToBreak(row_value2 + i, col_value2 + i);
					}
				}
				ScanCrossDown(row_value3, col_value3);
				if (countUpLeft + countDownRight >= 2) {
					for (int i = 0; i <= countUpLeft; i++) {
						AddObjecToBreak(row_value3 - i, col_value3 - i);
					}
					for (int i = 0; i <= countDownRight; i++) {
						AddObjecToBreak(row_value3 + i, col_value3 + i);
					}
				}
				break;
			default:
				break;
		}
		ShowMatrix();
	}
	void ScanVertical(int row, int col) {
		countup = 0;
		countdown = 0;
		if (row < 11 && matrix[row, col] != 0) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck_down = true;
			while (contCheck_down) {
				if (rowcheck < 11 && (matrix[rowcheck, colcheck] == matrix[rowcheck + 1, colcheck])) {
					rowcheck++;
					contCheck_down = true;
					countdown++;
				} else {
					contCheck_down = false;
				}
			}
		}
		if (row > 0 && matrix[row, col] != 0) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck_up = true;
			while (contCheck_up) {
				if (rowcheck > 0 && (matrix[rowcheck, colcheck] == matrix[rowcheck - 1, colcheck])) {
					rowcheck--;
					contCheck_up = true;
					countup++;
				} else {
					contCheck_up = false;
				}
			}
		}
	}
	void ScanHorizontal(int row, int col) {
		countleft = 0;
		countright = 0;
		if (col < 7 && matrix[row, col] != 0) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck_left = true;
			while (contCheck_left) {
				if (colcheck < 7 && (matrix[rowcheck, colcheck] == matrix[rowcheck, colcheck + 1])) {
					colcheck++;
					contCheck_left = true;
					countright++;
				} else {
					contCheck_left = false;
				}
			}
		}
		if (row > 0 && matrix[row, col] != 0) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck_right = true;
			while (contCheck_right) {
				if (colcheck > 0 && (matrix[rowcheck, colcheck] == matrix[rowcheck, colcheck - 1])) {
					colcheck--;
					contCheck_right = true;
					countleft++;
				} else {
					contCheck_right = false;
				}
			}
		}
	}
	void ScanCrossUp(int row, int col) {
		countDownLeft = 0;
		countUpRight = 0;
		if (row > 0 && col < 7) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck = true;
			while (contCheck) {
				if (rowcheck > 0 && colcheck < 7 && (matrix[rowcheck, colcheck] == matrix[rowcheck - 1, colcheck + 1])) {
					rowcheck--; colcheck++;
					contCheck = true;
					countUpRight++;
				} else {
					contCheck = false;
				}
			}
		}
		if (row < 11 && col > 0) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck = true;
			while (contCheck) {
				if (rowcheck < 11 && colcheck > 0 && (matrix[rowcheck, colcheck] == matrix[rowcheck + 1, colcheck - 1])) {
					rowcheck++;
					colcheck--;
					contCheck = true;
					countDownLeft++;
				} else {
					contCheck = false;
				}
			}
		}
	}
	void ScanCrossDown(int row, int col) {
		countUpLeft = 0;
		countDownRight = 0;
		if (row < 11 && col < 7) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck = true;
			while (contCheck) {
				if (rowcheck < 11 && colcheck < 7 && (matrix[rowcheck, colcheck] == matrix[rowcheck + 1, colcheck + 1])) {
					rowcheck++; colcheck++;
					contCheck = true;
					countDownRight++;
				} else {
					contCheck = false;
				}
			}
		}
		if (row > 0 && col > 0) {
			int rowcheck = row;
			int colcheck = col;
			bool contCheck = true;
			while (contCheck) {
				if (rowcheck > 0 && colcheck > 0 && (matrix[rowcheck, colcheck] == matrix[rowcheck - 1, colcheck - 1])) {
					rowcheck--;
					colcheck--;
					contCheck = true;
					countUpLeft++;
				} else {
					contCheck = false;
				}
			}
		}
	}
	void AddObjecToBreak(int row, int col) {
		listObjectBreak[row, col] = matrixGameOject[row, col];
	}
	void BreakObject() {

		countObject = 0;
		countPiece1Break = 0;
		countPiece2Break = 0;
		countPiece3break = 0;
		countPiece4break = 0;
		countPiece5break = 0;
		countPiece6break = 0;
		for (int i = 0; i < 12; i++) {
			for (int j = 0; j < 8; j++) {
				if (listObjectBreak[i, j] != null) {
					switch (matrix[i, j]) {
						case 1: countPiece1Break++; break;
						case 2: countPiece2Break++; break;
						case 3: countPiece3break++; break;
						case 4: countPiece4break++; break;
						case 5: countPiece5break++; break;
						case 6: countPiece6break++; break;
					}
					countObject++;
					countObjectBreak++;
					ShowAnimationBreak(i, j);
					DespawnObjectPool(listObjectBreak[i, j]);
					AddPieceTweenAfterBreak(i, j);
					matrix[i, j] = 0;
					listObjectBreak[i, j] = null;
					matrixGameOject[i, j] = null;
				}
			}
		}

		if (countObjectBreak >= 3) {
			BOL_Battle_Screen.instance.CompAttack1();
		}
		if (countPiece1Break >= 3) { BOL_Battle_Screen.instance.Attack1(); }
		if (countPiece2Break >= 3) { BOL_Battle_Screen.instance.Attack2(); }
		if (countPiece3break >= 3) { BOL_Battle_Screen.instance.Attack3(); }
		if (countPiece4break >= 3) { BOL_Battle_Screen.instance.Attack4(); }
		if (countPiece5break >= 3) { BOL_Battle_Screen.instance.Attack5(); }
		if (countPiece6break >= 3) { BOL_Battle_Screen.instance.Attack6(); }

	}
	void AddPieceTweenAfterBreak(int row, int col) {
		int rowcheck = row - 1;
		int colcheck = col;
		if (matrix[rowcheck, col] != 0) {
			while (matrix[rowcheck, col] != 0) {
				listObjectTweenAfterBreak[rowcheck, col] = matrixGameOject[rowcheck, col];
				rowcheck--;
			}
		}
	}
	void TweenAfterBreak() {
		finishTween = false;
		for (int i = 11; i >= 0; i--) {
			for (int j = 7; j >= 0; j--) {
				if (listObjectTweenAfterBreak[i, j] != null) {
					int finishRow = GetRowFinishMove(i, j);
					int finishCol = j;
					LeanTween.moveLocalY(listObjectTweenAfterBreak[i, j], VectorRowFinish(GetRowFinishMove(i, j)), 0.5f);
					matrix[finishRow, finishCol] = matrix[i, j];
					matrixGameOject[finishRow, finishCol] = matrixGameOject[i, j];
					listTweenCheckAfterbreak[finishRow, finishCol] = matrix[finishRow, finishCol];
					matrix[i, j] = 0;
					matrixGameOject[i, j] = null;
					listObjectTweenAfterBreak[i, j] = null;
				}
			}
		}
		ShowMatrix();
		finishTween = true;
	}
	IEnumerator ScanMatrix() {
		for (int i = 0; i < 12; i++) {
			for (int j = 0; j < 8; j++) {
				if (listTweenCheckAfterbreak[i, j] != 0) {
					ScanOnePiece(i, j);
				}
			}
		}
		yield return Yielders.Get(0.1f);
		BreakObject();
		yield return Yielders.Get(0.1f);
		TweenAfterBreak();
		yield return Yielders.Get(0.2f);
		StopScanMatrix();
	}
	IEnumerator _scanMartrix;
	void StartScanMatrix() {
		if (_scanMartrix == null) {
			_scanMartrix = ScanMatrix();
			StartCoroutine(_scanMartrix);
		} else {
			StopCoroutine(_scanMartrix);
			_scanMartrix = null;
			_scanMartrix = ScanMatrix();
			StartCoroutine(_scanMartrix);
		}
	}
	void StopScanMatrix() {
		if (_scanMartrix != null) {
			StopCoroutine(_scanMartrix);
			_scanMartrix = null;
		}
	}
	void ScanOnePiece(int row, int col) {
		countup = 0;
		countdown = 0;
		countUpLeft = 0;
		countUpRight = 0;
		countDownLeft = 0;
		countDownRight = 0;
		ScanVertical(row, col);
		if ((countup + countdown) >= 2) {
			for (int i = 0; i <= countdown; i++) {
				AddObjecToBreak(row + i, col);
			}
			for (int i = 0; i <= countup; i++) {
				AddObjecToBreak(row - i, col);
			}
		}
		ScanHorizontal(row, col);
		if (countleft + countright >= 2) {
			for (int i = 0; i <= countleft; i++) {
				AddObjecToBreak(row, col - i);
			}
			for (int i = 0; i <= countright; i++) {
				AddObjecToBreak(row, col + i);
			}
		}
		ScanCrossUp(row, col);
		if (countUpRight + countDownLeft >= 2) {
			for (int i = 0; i <= countUpRight; i++) {
				AddObjecToBreak(row - i, col + i);
			}
			for (int i = 0; i <= countDownLeft; i++) {
				AddObjecToBreak(row + i, col - i);
			}
		}
		ScanCrossDown(row, col);
		if (countUpLeft + countDownRight >= 2) {
			for (int i = 0; i <= countUpLeft; i++) {
				AddObjecToBreak(row - i, col - i);
			}
			for (int i = 0; i <= countDownRight; i++) {
				AddObjecToBreak(row + i, col + i);
			}
		}

	}
	public int GetRowFinishMove(int row, int column) {
		int i = row;
		while (i < 11) {
			i++;
			if (matrix[i, column] != 0) {
				return i - 1;
			}
		}
		return i;
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
	public void DespawnObjectPool(GameObject objectprefab) {
		if (objectprefab == null) {
			return;
		}
		LeanPool.Despawn(objectprefab);
	}
	public Vector3 PointFinish(int row, int column) {
		return new Vector3(column * Rowfoot + Rowfoot / 2, row * Colfoot + Colfoot / 2);
	}
	public float VectorRowFinish(int row) {
		return row * Colfoot + Colfoot / 2;
	}
	public float VectorColumnFinish(int column) {
		return column * Rowfoot + Rowfoot / 2;
	}
	public int RowFinish(float valuePos) {
		return (int)((valuePos - Colfoot / 2) / Colfoot);
	}
	public int ColFinish(float valuePos) {
		return (int)((valuePos - Rowfoot / 2) / Rowfoot);
	}
	public void ShowFinish() {
		Debugs.LogBlue(" finish game");
		for (int i = 0; i < 3; i++) {
			currentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
		}
		StopMoveVertical();

		countNextPiece = 0;
		countPiece = 0;
		for (int i = 0; i < 12; i++) {
			for (int j = 0; j < 8; j++) {
				matrix[i, j] = 0;
				listObjectBreak[i, j] = null;
				LeanPool.Despawn(matrixGameOject[i, j]);
				matrixGameOject[i, j] = null;
			}
		}
		BOL_Battle_Screen.instance.SelfDestruction();
		BOL_MainControl_Offline.instance.Back2LastScene();
		//StartCoroutine(CountDownStart());
	}
	public bool finishGame = true;
	IEnumerator CountDownStart() {
		int i = 3;
		while (i > 1) {
			i--;
			textCountDown.text = i + "s";
			LeanTween.scale(textCountDown.gameObject, Vector3.one, 1).setOnComplete(() => {
				textCountDown.GetComponent<RectTransform>().localScale = Vector3.zero;
			});
			yield return Yielders.Get(0.8f);
		}
		finishGame = false;
		SpawnObject();
	}
	public void ShowAnimationBreak(int row, int col) {
		StartCoroutine(BreakObject(objectbreak, PointFinish(row, col), MatrixParent.transform));
	}
	IEnumerator BreakObject(GameObject prefab, Vector3 position, Transform parent) {
		GameObject piece = null;
		piece = SpawnObjectPools(prefab, position, parent);
		if (piece.GetComponent<ParticleSystem>() != null) {
			piece.GetComponent<ParticleSystem>().Play();
			yield return new WaitUntil(() => piece.GetComponent<ParticleSystem>().isStopped);
		} else {
			yield return Yielders.Get(1);
		}
		DespawnObjectPool(piece);
	}
}
