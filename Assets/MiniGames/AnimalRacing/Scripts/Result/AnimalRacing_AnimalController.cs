using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AnimalRacing_AnimalController : MonoBehaviour
{
    public enum AnimalType{
        Rabbit = 0,
        Dog,
        Pig,
        Panther,
        Buffalo,
        Horse,
        Antelope,
        Ostrich,
        Camel
    }
    public AnimalType myType;

    public Vector3 startLocalPos;

    public Animator myAnimator;
    [SerializeField] GameObject glowEffect;
    public sbyte[] runData;
    
    [ContextMenu("Test For Get Start Pos")]
    void TestForGetStartPos(){
        startLocalPos = transform.localPosition;
    }

    public void ResetData(){
        transform.localPosition = startLocalPos;
        glowEffect.SetActive(false);
    }

    public void ResetAnimation(){
        myAnimator.speed = 1f;
        myAnimator.SetTrigger("Idle");
        myAnimator.Update(0.1f);
    }

    public void InitData(sbyte[] _runData){
        runData = _runData;
    }

    public void SetUpRun() {
        myAnimator.SetTrigger("Run");
    }

    public void ShowGlowEffect(){
        glowEffect.SetActive(true);
    }

}
