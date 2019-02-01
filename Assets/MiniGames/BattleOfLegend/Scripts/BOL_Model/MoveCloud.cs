using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class MoveCloud : MonoBehaviour {
	public bool isMoveCloud;
	public bool isMoveDe;
	public RectTransform objectMain;
	public GameObject cloud1;
	public GameObject cloud2;
	public GameObject cloud3;
	[SerializeField]
	Vector3 localPosition1;
	[SerializeField]
	Vector3 localPosition2;
	[SerializeField]
	Vector3 localPosition3;
	[SerializeField]
	Vector3 localTmp;

	public GameObject dedung;


	private void Start() {
		MovingCloud();
		MovingDedung();
	}

	public void MovingCloud() {
		localPosition2 = Vector3.zero;
		if (objectMain != null) {
			localPosition1 = new Vector3(localPosition2.x - objectMain.rect.width, 0);
			localPosition3 = new Vector3(localPosition2.x + objectMain.rect.width, 0);
		}
		if (cloud1 != null) {
			LeanTween.moveLocalX(cloud1, localPosition3.x, 30).setLoopCount(-1);
		}
		if (cloud2 != null) {
			LeanTween.moveLocalX(cloud2, localPosition3.x, 30).setLoopCount(-1).setDelay(15);
		}
		if (cloud3 != null) {
			LeanTween.moveLocalX(cloud3, localPosition3.x, 40).setLoopCount(-1).setDelay(10);
		}
	}
	public void MovingDedung() {
		if (dedung != null) {
			LeanTween.moveLocalY(dedung, dedung.transform.localPosition.y - 10, 1).setLoopPingPong(-1);
		}
	}
}
