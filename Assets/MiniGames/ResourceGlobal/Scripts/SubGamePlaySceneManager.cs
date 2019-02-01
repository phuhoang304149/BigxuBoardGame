using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*   SubGamePlaySceneManager: class quản lý scence chơi sub subgame
*/
public class SubGamePlaySceneManager : IMySceneManager {

	public override Type mySceneType{
		get{
			return Type.SubGamePlayScene;
		}
	}
	public static SubGamePlaySceneManager instance;

	private void Awake()
    {
        instance = this;
        CoreGameManager.instance.currentSceneManager = instance;
    }

	void Start()
    {
        StartCoroutine(DoActionRun());
    }

	IEnumerator DoActionRun(){
		yield return null;
        yield return DoActionInitData();
		canShowScene = true;
	}

    IEnumerator DoActionInitData(){
		if(NetworkGlobal.instance.instanceRealTime != null){
			DataManager.instance.userData.sessionId = NetworkGlobal.instance.instanceRealTime.sessionId;
		}

		CoreGameManager.instance.currentSubGamePlay = Instantiate ((GameObject) DataManager.instance.miniGameData.currentSubGameDetail.myInfo.gameManagerPrefab.Load()).GetComponent<ISubGamePlayManager>();
		bool _initFinished = false;
		CoreGameManager.instance.currentSubGamePlay.InitData(true, false, ()=>{
			_initFinished = true;
		});
		yield return new WaitUntil(()=>_initFinished);
		yield return Yielders.Get(0.5f);
		yield return CoreGameManager.instance.currentSubGamePlay.Show();
    }

    private void OnDestroy() {
		DataManager.instance.miniGameData.currentSubGameDetail = null;
        CoreGameManager.instance.currentSubGamePlay = null;
    }
}
