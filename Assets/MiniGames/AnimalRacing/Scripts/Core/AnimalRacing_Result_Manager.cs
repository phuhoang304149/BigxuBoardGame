using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRacing_Result_Manager : MonoBehaviour {

    public enum State{
        None,
        CountDown, 
        LetAnimalsRun,
    }
    public State currentState{get;set;}
    public AnimalRacing_RaceTrackController raceController;
    public AnimalRacing_PanelCountDown_Controller panelCountDown;
    public List<AnimalRacing_AnimalController> listAnimals{get;set;}
    
    [Header("Racetrack Info")]
    public Transform groupObjects;
    public float heightOfGroupObject = 4.705f;
	
    [Header("Setting")]
    public float timeCountDown;
    public short stepRun;

    long timeStartRun;
    AnimalRacing_AnimalController.AnimalType animalWin;

    // void Start(){
    //     Vector3 _pos = groupObjects.position;
    //     _pos.y = myCamera.transform.position.y - myCamera.sizeOfCamera.y / 2 + heightOfGroupObject / 2f;
    //     groupObjects.position = _pos;
    // }

    void ResetData(){
        currentState = State.None;
        raceController.ResetData();
        if(listAnimals == null || listAnimals.Count == 0){
            listAnimals = new List<AnimalRacing_AnimalController>();
            for(int i = 0; i < AnimalRacing_GamePlay_Manager.instance.listAnimalInfo.Count; i ++){
                listAnimals.Add(AnimalRacing_GamePlay_Manager.instance.listAnimalInfo[i].myController);
            }
        }
        for(int i = 0; i < listAnimals.Count; i ++){
            listAnimals[i].ResetData();
        }
        panelCountDown.ResetData();
    }

    public void Show(){
        gameObject.SetActive(true);

        Vector3 _posCam = AnimalRacing_GamePlay_Manager.instance.mainCamera.transform.position;
		_posCam.x = 0f;
		_posCam.y = -10f;
		AnimalRacing_GamePlay_Manager.instance.mainCamera.transform.position = _posCam;

        Vector3 _pos = groupObjects.position;
        _pos.y = _posCam.y - AnimalRacing_GamePlay_Manager.instance.mainCamera.sizeOfCamera.y / 2 + heightOfGroupObject / 2f;
        groupObjects.position = _pos;

        CoreGameManager.instance.DoVibrate();
    }

    public Coroutine StartRun(){
        InitData();
        return StartCoroutine(DoActionRun());
    }

    void InitData(){
        animalWin = AnimalRacing_GamePlay_Manager.instance.animalRacingData.currentResultData.animalWin;
        float _tmpS = 0;
        // long _tmpSa = 0;
        for(int i = 0; i < listAnimals.Count; i++){
            listAnimals[i].InitData(AnimalRacing_GamePlay_Manager.instance.animalRacingData.currentResultData.listAnimalRunData[i]);
            if(i == (int) animalWin){
                _tmpS = 0f;
                for(int j = 0; j < listAnimals[i].runData.Length; j++){
                    _tmpS += (((float) listAnimals[i].runData[j]) / 10f) * Time.fixedDeltaTime * stepRun;
                }
            }
            // _tmpSa = 0;
            // for(int j = 0; j < listAnimals[i].runData.Length; j++){
            //     _tmpSa += (((long) listAnimals[i].runData[j]));
            // }
            // Debug.LogError(">>> i = " + i + " -- " + _tmpSa);
        }
        raceController.InitData(_tmpS);
    }

    IEnumerator DoActionRun(){
        yield return DoActionCountDown();
        yield return DoActionLetAnimalsRun();
        yield return Yielders.Get(1.5f);
        yield return DoActionWaitToChangeScreen();
    }

    public void Hide(){
        StopAllCoroutines();
        ResetData();
        gameObject.SetActive(false);
    }

    IEnumerator DoActionCountDown(){
        currentState = State.CountDown;
        bool _isFinished = false;
        panelCountDown.InitData(timeCountDown, ()=>{
            _isFinished = true;
        });
        if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            panelCountDown.Show(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_BeepCountDown_00, AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_BeepCountDown_01);
        }else{
            panelCountDown.Show();
        }
        yield return new WaitUntil(()=> _isFinished);
    }

    IEnumerator DoActionLetAnimalsRun(){
        currentState = State.LetAnimalsRun;
        yield return null;
        for(int i = 0; i < listAnimals.Count; i++){
            listAnimals[i].SetUpRun();
        }
        
        int _indexVeclocity = 0;
        short _tmpStepRun = 0;

        float _currentVeclocity = 0f;
        float _tmpVeclocity = 0f;

		Vector3 _pos = Vector3.zero;
		float _targetPosX = 0f;
        int _maxLengeVelocity = listAnimals[0].runData.Length;

        if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_RunPlayBack);
        }
        if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_Run);
        }
        float _timeShowSfxRun = 0f;

		while(true){
			yield return Yielders.FixedUpdate;
			
            _timeShowSfxRun += Time.fixedDeltaTime;
            if(_timeShowSfxRun >= 0.9f){
                if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
                    MyAudioManager.instance.PlaySfx(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_Run);
                }
                _timeShowSfxRun = 0f;
            }

            _tmpStepRun ++;
            
			_targetPosX = listAnimals[0].transform.position.x;
            _currentVeclocity = 0f;
			for(int i = 0; i < listAnimals.Count; i++){
				_pos = listAnimals[i].transform.position;
                _tmpVeclocity = (((float)listAnimals[i].runData[_indexVeclocity])/10f) * Time.fixedDeltaTime;
				_pos.x += _tmpVeclocity;
                if(_currentVeclocity == 0){
                    _currentVeclocity = _tmpVeclocity;
                }
				listAnimals[i].transform.position = _pos;
                listAnimals[i].myAnimator.speed = ((float)listAnimals[i].runData[_indexVeclocity]) * 2.1f / 100f;
				if(_pos.x > _targetPosX){
					_targetPosX = _pos.x;
                    _currentVeclocity = _tmpVeclocity;
				}
			}
			
            if(AnimalRacing_GamePlay_Manager.instance.mainCamera != null){
				_pos = AnimalRacing_GamePlay_Manager.instance.mainCamera.transform.position;
                if(_pos.x < raceController.finishPoint.position.x){
                    _pos.x = _targetPosX + 2f;
                    if(_pos.x >= raceController.finishPoint.position.x){
                        _pos.x = raceController.finishPoint.position.x;
                        raceController.canMoveReverseCamera = false;
                    }
                    AnimalRacing_GamePlay_Manager.instance.mainCamera.transform.position = _pos;
                }
                raceController.UpdatePosAgain(_currentVeclocity);
            }

            if(_tmpStepRun >= stepRun){
                _indexVeclocity ++;
                _tmpStepRun = 0;
                if(_indexVeclocity == _maxLengeVelocity){
                    for(int i = 0; i < listAnimals.Count; i++){
                        listAnimals[i].myAnimator.speed = 0f;
                    }
                    if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
                        MyAudioManager.instance.PlaySfx(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_HighlightAnimalWin);
                    }
                    listAnimals[(int) animalWin].ShowGlowEffect();
                    break;
                }
            }
		}
    }

    IEnumerator DoActionWaitToChangeScreen(){
        int _indexVeclocity = listAnimals[0].runData.Length - 1;
		Vector3 _pos = Vector3.zero;
        float _tmpTime = 0f;
        if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_Run);
        }
        while(_tmpTime < 2f){
			yield return Yielders.FixedUpdate;
			
			for(int i = 0; i < listAnimals.Count; i++){
				_pos = listAnimals[i].transform.position;
				_pos.x += (((float)listAnimals[i].runData[_indexVeclocity])/10f) * Time.fixedDeltaTime;
				listAnimals[i].transform.position = _pos;
                listAnimals[i].myAnimator.speed = ((float)listAnimals[i].runData[_indexVeclocity]) * 2.3f / 100f;
			}
            _tmpTime += Time.fixedDeltaTime;
        }
        for(int i = 0; i < listAnimals.Count; i++){
            listAnimals[i].ResetAnimation();
        }
        yield return Yielders.Get(0.5f);
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }
}
