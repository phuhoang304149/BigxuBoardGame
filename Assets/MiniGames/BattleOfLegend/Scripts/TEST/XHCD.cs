using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XHCD  {
	public long nextTimeProcess;
	public Pieces currentPieces, nextPieces;
	public Matrix matrix;

	public byte[,] matrixData;
    public void setup(){
		matrix.parent = this;
    }
	private bool isAddPieceToMatrix() { return false; }
    public void process(){
		if (MyConstant.currentTimeMilliseconds < nextTimeProcess)
			return;
		nextTimeProcess = MyConstant.currentTimeMilliseconds + 500;
        if(isAddPieceToMatrix()){
			Pieces _temp = currentPieces;
			nextPieces.joinInToMatrix();
			currentPieces = nextPieces;
			nextPieces = _temp;
            _temp.resetOutMatrix();
        }
    }
}
