using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Piece_Control : MySimplePoolObjectController {
	public BOL_FlashingSprite bOL_Flashing;
   	public override void ResetData() {
		StopAllCoroutines();
		LeanTween.cancel(gameObject);
	}
	public void InitData() {
		bOL_Flashing.Initdata();
	}
	public void FlashingObject(bool boolean) {
		if (boolean) {
			bOL_Flashing.StartFlash();
		} else bOL_Flashing.StopFlash();
	}
	public void SelfDestruction() {
		FlashingObject(false);
		LeanPool.Despawn(gameObject);
	}

}
