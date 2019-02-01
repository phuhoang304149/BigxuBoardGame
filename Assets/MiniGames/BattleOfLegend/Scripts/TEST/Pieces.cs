using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour {
	public XHCD parent;

	public byte xMatrix;
	public byte yMatrix;
	int numberValue;
	byte[] piecesData;
	float xScreen;
	float yScreen;
    
	
    public void joinInToMatrix(){}
    public void resetOutMatrix(){}
    
    public void moveLeft(){}
    public void moveRight(){}
    public void moveDown(){}
}
