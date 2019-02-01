using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class GlowScaleImage : MonoBehaviour {
	public GameObject imageVS;
	private void Start() {
    LeanTween.scale(imageVS,Vector3.one*1.2f,1).setLoopPingPong(-1).setEase(LeanTweenType.easeInBack);
	}
}
