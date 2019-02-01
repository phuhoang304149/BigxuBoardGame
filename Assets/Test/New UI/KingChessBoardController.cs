using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingChessBoardController : MonoBehaviour {

	[SerializeField] RectTransform myCanvas;
	[SerializeField] RectTransform board;
    [SerializeField] RectTransform holderLeft;
    [SerializeField] RectTransform holderRight;

	private void Awake() {
		// Debug.Log(board.sizeDelta + " - " + myCanvas.sizeDelta);
		SetPosAgain();
	}

	[ContextMenu("Set Pos Again")]
	void SetPosAgain(){
		Vector2 _szLeft = holderLeft.sizeDelta;
        _szLeft.x = (myCanvas.sizeDelta.x - board.sizeDelta.x) / 2;
        holderLeft.sizeDelta = _szLeft;

        Vector2 _szRight = holderRight.sizeDelta;
        _szRight.x = (myCanvas.sizeDelta.x - board.sizeDelta.x) / 2;
        holderRight.sizeDelta = _szRight;
	}
}
