using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxObjectController : MySimplePoolObjectController {
	[SerializeField] AudioSource sfxSource;

	public void Play(AudioClip _audioClip){
		sfxSource.clip = _audioClip;
		StartCoroutine(DoActionPlay());
	}

	IEnumerator DoActionPlay(){
		sfxSource.Play();
		yield return new WaitUntil(()=>!sfxSource.isPlaying);
		SelfDestruction();
	}
}
