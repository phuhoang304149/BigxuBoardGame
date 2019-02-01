using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hero4_Yasuo_controller : BOL_Hero_Controler
{
    public AudioClip sfx_sound_go;
    public GameObject ObjectMe;
    public Vector3 vectorMe;
    float timeWaitingSkill;
    public GameObject BeginBullet;
    public GameObject LegObject;
    public override void InitData()
    {
        base.InitData();
        if (chairPositions == Constant.CHAIR_LEFT)
        {
            vectorMe = ObjectMe.transform.GetChild(0).transform.position;
        }
        else if (chairPositions == Constant.CHAIR_RIGHT)
        {
            vectorMe = ObjectMe.transform.GetChild(0).transform.position;
        }
    }
    public override void Attack1()
    {
        base.Attack1();

        vectorMe = ObjectMe.transform.position;
        animatorHero.SetTrigger(Constant.dash1);
        animatorHero.SetTrigger(Constant.idle);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go);
        }
        LeanTween.move(ObjectMe, new Vector3(vector_position_ememy.x, vectorMe.y), 0.4f).setOnComplete(() =>
        {
            animatorHero.SetTrigger(Constant.attack1);
            animatorHero.SetTrigger(Constant.idle);

            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                LeanTween.delayedCall(0.7f, () => MyAudioManager.instance.PlaySfx(sfx_Attack2));
            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.6f, 1);
            Delay(1.4f, () =>
            {
                ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                Delay(1f, () =>
                {
                    animatorHero.SetTrigger(Constant.dash1);
                    animatorHero.SetTrigger(Constant.idle);
                    LeanTween.move(ObjectMe, vectorMe, 0.4f).setOnComplete(() =>
                    {
                        ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                        ResetData();
                    });
                });
            });
        });
    }
    public override void Attack2()
    {
        base.Attack2();
        vectorMe = ObjectMe.transform.position;
        animatorHero.SetTrigger(Constant.dash1);
        animatorHero.SetTrigger(Constant.idle);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go);
        }
        LeanTween.move(ObjectMe, new Vector3(vector_position_ememy.x, vectorMe.y), 0.4f).setOnComplete(() =>
        {
            animatorHero.SetTrigger(Constant.attack2);
            animatorHero.SetTrigger(Constant.idle);
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                LeanTween.delayedCall(1.2f, () => MyAudioManager.instance.PlaySfx(sfx_Attack2));

            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, BeginBullet.transform.position, 0.8f, 1.1f);
            Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f, 1.5f);
            Delay(2f, () =>
            {
                ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                Delay(0.5f, () =>
                {
                    animatorHero.SetTrigger(Constant.dash1);
                    animatorHero.SetTrigger(Constant.idle);
                    if (BOL_Manager.instance.CanPlayMusicAndSfx())
                    {
                        MyAudioManager.instance.PlaySfx(sfx_sound_go);
                    }
                    LeanTween.move(ObjectMe, vectorMe, 0.4f).setOnComplete(() =>
                    {
                        ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                        ResetData();
                    });
                });
            });
        });
    }
    public override void Attack_Q()
    {
        base.Attack_Q();
        vectorMe = ObjectMe.transform.position;
        animatorHero.SetTrigger(Constant.dash1);
        animatorHero.SetTrigger(Constant.idle);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go);
        }
        LeanTween.move(ObjectMe, new Vector3(vector_position_ememy.x, vectorMe.y), 0.5f).setOnComplete(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                LeanTween.delayedCall(1, () => MyAudioManager.instance.PlaySfx(sfx_AttackQ));

            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[2].gameObject, vector_competitor_parent, 0.7f, 2);
            animatorHero.SetTrigger(Constant.attackQ);
            animatorHero.SetTrigger(Constant.idle);
            Delay(1f, () =>
            {
                ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                Delay(0.5f, () =>
                {
                    animatorHero.SetTrigger(Constant.dash1);
                    animatorHero.SetTrigger(Constant.idle);
                    if (BOL_Manager.instance.CanPlayMusicAndSfx())
                    {
                        MyAudioManager.instance.PlaySfx(sfx_sound_go);
                    }
                    LeanTween.move(ObjectMe, vectorMe, 0.4f).setOnComplete(() =>
                    {
                        ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                        ResetData();
                    });
                });
            });
        });
    }
    public override void Attack_W()
    {
        base.Attack_W();
        Debug.Log(2);
        vectorMe = ObjectMe.transform.position;
        animatorHero.SetTrigger(Constant.dash1);
        animatorHero.SetTrigger(Constant.idle);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go);
        }
        LeanTween.move(ObjectMe, new Vector3(vector_position_ememy.x, vectorMe.y), 0.5f).setOnComplete(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                LeanTween.delayedCall(2.2f, () => MyAudioManager.instance.PlaySfx(sfx_AttackW));
            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 2, 3);
            animatorHero.SetTrigger(Constant.attackW);
            animatorHero.SetTrigger(Constant.idle);
            Delay(2.2f, () =>
            {
                ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                Delay(0.5f, () =>
                {
                    animatorHero.SetTrigger(Constant.dash1);
                    animatorHero.SetTrigger(Constant.idle);
                    if (BOL_Manager.instance.CanPlayMusicAndSfx())
                    {
                        MyAudioManager.instance.PlaySfx(sfx_sound_go);
                    }
                    LeanTween.move(ObjectMe, vectorMe, 0.4f).setOnComplete(() =>
                    {
                        ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                        ResetData();
                    });
                });
            });
        });
    }
    public override void Attack_E()
    {
        base.Attack_E();
        vectorMe = ObjectMe.transform.position;
        animatorHero.SetTrigger(Constant.dash1);
        animatorHero.SetTrigger(Constant.idle);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go);
        }
        LeanTween.move(ObjectMe, new Vector3(vector_position_ememy.x, vectorMe.y), 0.5f).setOnComplete(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                LeanTween.delayedCall(1, () => MyAudioManager.instance.PlaySfx(sfx_AttackE));
            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[3].gameObject, new Vector3(vector_competitor.x, vector_competitor.y - 0.5f), 0.8f, 2);
            animatorHero.SetTrigger(Constant.attackE);
            animatorHero.SetTrigger(Constant.idle);
            Delay(1.2f, () =>
            {
                ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                Delay(0.5f, () =>
                {
                    animatorHero.SetTrigger(Constant.dash1);
                    animatorHero.SetTrigger(Constant.idle);
                    if (BOL_Manager.instance.CanPlayMusicAndSfx())
                    {
                        MyAudioManager.instance.PlaySfx(sfx_sound_go);
                    }
                    LeanTween.move(ObjectMe, vectorMe, 0.4f).setOnComplete(() =>
                    {
                        ObjectMe.transform.localScale = new Vector3(ObjectMe.transform.localScale.x * -1, ObjectMe.transform.localScale.y);
                        ResetData();
                    });
                });
            });
        });
    }
    public override void ResetData()
    {
        base.ResetData();
    }


}
