using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesControl : MonoBehaviour {
	public static PiecesControl instance {
		get {
			return ins;
		}
	}
	static PiecesControl ins;
	void Awake() {
		ins = this;
	}
	public void SeflDestruction() {
		ins = null;
	}
	[Header(" NEXT PIECES")]
	public int numberValueNextPiece;
	public int nextPiece1;
	public int nextPiece2;
	public int nextPiece3;
	[Header(" CURRENT PIECES")]
	public int numberValueCurrentPiece;
	public int piece1;
	public int piece2;
	public int piece3;
	public bool finishSpawnNewObject;
	public bool finishCreateCurrentPiece;
	public void SpawnNewObject() {
		numberValueNextPiece = Random.Range(1, 4);
		switch (numberValueNextPiece) {
			case 1:
				nextPiece1 = Random.Range(1, 7);
				nextPiece2 = 0;
				nextPiece3 = 0;
				break;
			case 2:
				nextPiece1 = Random.Range(1, 7);
				nextPiece2 = Random.Range(1, 7);
				nextPiece3 = 0;
				break;
			case 3:
				nextPiece1 = Random.Range(1, 7);
				nextPiece2 = Random.Range(1, 7);
				nextPiece3 = Random.Range(1, 7);
				break;
		}
		finishSpawnNewObject = true;
	}
	public void CreateValueCurrentPiece() {
		piece1 = nextPiece1;
		piece2 = nextPiece2;
		piece3 = nextPiece3;
	}
	public void ResetPiece() {
		nextPiece1 = 0;
		nextPiece2 = 0;
		nextPiece3 = 0;
		piece1 = 0;
		piece2 = 0;
		piece3 = 0;
	}
}
