using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour {
	public int value;
	public virtual void IntitData() { }
	public virtual void ResetData() { }
	public virtual void UpdateData() { }
	public virtual void PlayAnimation() { }
}
