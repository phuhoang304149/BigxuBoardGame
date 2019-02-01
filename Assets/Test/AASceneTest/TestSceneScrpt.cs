using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneScrpt : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void DoAction(){
		Debug.Log(">>>>> press here");
		OneHitAPI.LoginWithFacebookAccount(0, "EAAbYZC5sZAkmQBAOazBAc5VKewZBWgOiBRKIuvXisd9SdZCJfTHiJo5tZCq16Ac49WDww1ertw80lbjm9nKgtx63LNJ0rAradirDa7lFVo5TI7KYvpOYLGOckU2Jf9qo8ZCrA2JbANOWS5blSsMZBACtqZCdR08lIDZCgOAZAbX3TG2SHxBfEvZB9avUmJ8qxq3hz8ZD", 
			(_messageReceiving, _error)=>{
				sbyte _caseCheck = _messageReceiving.readByte (); 
				sbyte _databaseid = _messageReceiving.readByte();
				//////////////////////////////////////////////////////////////
				long _facebookid = _messageReceiving.readLong();
				string _token_for_business = _messageReceiving.readString();

				Debug.LogError(_caseCheck + " - " + _databaseid + " - " + _facebookid + " - " + _token_for_business);
		});
    }
}
