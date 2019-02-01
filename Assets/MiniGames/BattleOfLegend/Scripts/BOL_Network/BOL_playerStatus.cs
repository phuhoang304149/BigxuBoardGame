using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOL_playerStatus : MonoBehaviour {
	public string nameshow;
	public short sessionid;
	public short userid;
	public bool Sitdown;
	public bool Standup;
	private bool _ischairStatus;
	public bool isChairStatus {
		get {
			return _ischairStatus;
		}
		set {
			_ischairStatus = value;
			if (isChairStatus) {
				Standup = true;
				Sitdown = false;
			} else {
				Standup = false;
				Sitdown = true;
			}
		}
	}
    public sbyte characterID;
    public sbyte spellID;
}
