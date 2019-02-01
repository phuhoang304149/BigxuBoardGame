using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
** AnimalRacing_RaceTrack_Col_Controller: Lớp control 1 collum của đường đua
**/
public class AnimalRacing_RaceTrack_Col_Controller : MonoBehaviour {

	public Vector2 mySize;
	
	public Vector3 startLocalPos;
	public float percentReverseVeclocity;

	[ContextMenu("Test For Get Start Pos")]
    void TestForGetStartPos(){
        startLocalPos = transform.localPosition;
    }

	public void ResetData(){
		transform.localPosition = startLocalPos;
	}
}
