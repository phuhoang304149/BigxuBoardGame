using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class MyAudioManager : MonoBehaviour {

	public static MyAudioManager instance{
		get{return ins;}
	}
	static MyAudioManager ins;

	[SerializeField] AudioSource musicSource;
	[SerializeField] Transform pool;

	[Header("Prefabs")]
	[SerializeField] GameObject sfxObjectPrefab;

	MySimplePoolManager sfxObjectPoolManager;

	LTDescr tweenVolumeMusic;
	
	void Awake() {
        if (ins != null && ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(this.gameObject);

		InitData();
    }

	void InitData(){
		sfxObjectPoolManager = new MySimplePoolManager();
	}

	#region Music, Playback
	public void SetMusic(AudioClip _audioClip){
		if (_audioClip.Equals (musicSource.clip) && musicSource.isPlaying) {
			return;
		}
		musicSource.clip = _audioClip;
	}
	public void PlayMusic (AudioClip _audioClip){
		if (_audioClip == null) {
			#if TEST
			Debug.LogError ("Audio Clip sound not found");
			#endif
			return;
		}

		SetMusic(_audioClip);

		if(DataManager.instance.musicStatus == 0){
			return;
		}
		musicSource.Play ();
		if(tweenVolumeMusic != null){
			LeanTween.cancel(tweenVolumeMusic.uniqueId);
			tweenVolumeMusic = null;
		}
		tweenVolumeMusic = LeanTween.value(gameObject, musicSource.volume, 1f, 0.1f).setOnComplete(()=>{
			tweenVolumeMusic = null;
		});
	}

	public void RestartMusic (){
		if(musicSource.clip == null){
			return;
		}
		if(DataManager.instance.musicStatus == 0){
			return;
		}
		musicSource.Play ();
		if(tweenVolumeMusic != null){
			LeanTween.cancel(tweenVolumeMusic.uniqueId);
			tweenVolumeMusic = null;
		}
		tweenVolumeMusic = LeanTween.value(gameObject, musicSource.volume, 1f, 0.1f).setOnComplete(()=>{
			tweenVolumeMusic = null;
		});
	}

	public void PauseMusic (){
		if(musicSource.clip == null){
			return;
		}
		musicSource.Pause ();
	}

	public void ResumeMusic (){
		if(musicSource.clip == null){
			return;
		}
		if(DataManager.instance.musicStatus == 0){
			return;
		}
		musicSource.volume = 1f;
		if(musicSource.isPlaying){
			musicSource.UnPause ();
		}else{
			musicSource.Play();
		}
	}

	public void StopMusic (){
		if(musicSource.clip == null){
			return;
		}
		if(tweenVolumeMusic != null){
			LeanTween.cancel(tweenVolumeMusic.uniqueId);
			tweenVolumeMusic = null;
		}
		tweenVolumeMusic = LeanTween.value(gameObject, musicSource.volume, 0f, 0.1f).setOnComplete(()=>{
			tweenVolumeMusic = null;
			musicSource.Stop ();
		});
	}
	#endregion

	#region SFX
	public void PlaySfx(AudioClip _audioClip){
		if(DataManager.instance.sfxStatus == 0){
			return;
		}
		if (_audioClip == null) {
			#if TEST
			Debug.LogError ("Audio Clip sound not found");
			#endif
			return;
		}

		// Spawn object chứa audioclip
		SfxObjectController _sfxObject = LeanPool.Spawn(sfxObjectPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<SfxObjectController>();
		sfxObjectPoolManager.AddObject(_sfxObject);
		_sfxObject.Play(_audioClip);
	}

	public void StopAllSfx (){
		sfxObjectPoolManager.ClearAllObjectsNow();
	}
	#endregion

	public void PauseAll(){
		PauseMusic();
		StopAllSfx();
	}

	public void ResumeAll(){
		ResumeMusic();
	}

	public void StopAll(){
		StopMusic();
		StopAllSfx();
	}
}
