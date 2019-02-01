using UnityEngine;
using System.Collections;

/**
 * Mô tả: Điểu khiển đuôi của đạn xoay đúng chiều của lớp cha
 * */
public class TrailParticleController : MonoBehaviour {
	public ParticleSystem myTrail;
	public Transform parent;

	void OnEnable(){
		StartCoroutine(DoActionCheckTrail());
	}

	IEnumerator DoActionCheckTrail(){
		while(true){
			yield return Yielders.FixedUpdate;
			if(myTrail.isPlaying){
				var main = myTrail.main;
        		main.startRotation = parent.rotation.eulerAngles.z * -1 * Mathf.Deg2Rad;
			}
		}
	}
}
