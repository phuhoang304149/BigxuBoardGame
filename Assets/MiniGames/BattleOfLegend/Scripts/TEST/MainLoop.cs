using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoop : MonoBehaviour {
	public XHCD Player, Comp;
	public byte state;

	public const byte STATE_CHOOSE_HERO = 8;
    public const byte STATE_FIGHTING = 9;
    public const byte STATE_RESULT = 10;
    
    
	void Start () {
        Player = new XHCD();
		Comp = new XHCD();
	}
	
	void Update () {
    
        if(state==STATE_FIGHTING){
			if (MyConstant.currentTimeMilliseconds > Player.nextTimeProcess)
				Player.process();
            if(MyConstant.currentTimeMilliseconds>Comp.nextTimeProcess)
                Comp.process();
        }
	}
}
