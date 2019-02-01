using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSize : MonoBehaviour {
public static TestSize instance {
        get {
            return ins;
        }
    }
    public static TestSize ins;
	[SerializeField] RectTransform myCanvas;
    [SerializeField] RectTransform playbattle1;
	[SerializeField] RectTransform playbattle2;
	[SerializeField] RectTransform chair1;
	[SerializeField] RectTransform chair2;
	private void Awake() {
		ins= this;
	}
	void Start() {

	}
    public void InitData(){
    SetPosAgain();
    }
    void SetPosAgain(){
        Vector2 _szLeft = chair1.sizeDelta;
        _szLeft.x = (myCanvas.sizeDelta.x/2 - playbattle1.sizeDelta.x) / 2;
        chair1.sizeDelta = _szLeft;

        Vector2 _szRight = chair2.sizeDelta;
        _szRight.x = (myCanvas.sizeDelta.x/2 - playbattle2.sizeDelta.x) / 2;
        chair2.sizeDelta = _szRight;
    
    }
	
}
