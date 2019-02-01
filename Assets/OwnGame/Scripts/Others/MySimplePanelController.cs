using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySimplePanelController : MonoBehaviour {

	public virtual void InitData (System.Action _onFinished = null){}

	public virtual void LateInitData (){}

	public virtual void ResetData (){}

	public virtual void RefreshData (){}

	public virtual Coroutine Show (){return null;}

	public virtual Coroutine Hide (){return null;}
}
