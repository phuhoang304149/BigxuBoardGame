using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Lean.Pool;
public class hero5_zed_controller : BOL_Hero_Controler
{

    public AudioClip sfx_sound_go_start;

    [FormerlySerializedAs("ObjectHero")]
    public GameObject ObjectMe;
    public Vector3 vectorMe;
    public GameObject beginBullet1;
    public GameObject beginBullet2;
    Vector3 vector_begin_attack1 = new Vector3();
    Vector3 vector_end_attack1 = new Vector3(3, -1.236383f, 0);
    Vector3 vector_begin_attackQ = new Vector3(-3.3f, -1.15f, 0);
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
        vectorMe = ObjectMe.transform.GetChild(0).transform.position;
        animatorHero.SetTrigger(Constant.dash1);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
        }
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vectorMe, 0.9f, 1);
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_position_ememy, 1.1f, 1);
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_position_ememy, 3.5f, 1);
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vectorMe, 3.7f, 1);
        LeanTween.move(ObjectMe, vector_position_ememy, 0.01f).setOnComplete(() =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_Attack1);
            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, vector_competitor, 1.2f, 2);
            animatorHero.SetTrigger(Constant.attack1);
            LeanTween.delayedCall(2.5f, () =>
            {
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
                }
            });
            LeanTween.move(ObjectMe, vectorMe, 0.01f).setOnComplete(() =>
            {
                Delay(1, ResetData);
            }).setDelay(2.5f);
        }).setDelay(1f);
    }
    public override void Attack2()
    {
        base.Attack2();
        vectorMe = ObjectMe.transform.position;
        animatorHero.SetTrigger(Constant.dash1);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
        }
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vectorMe, 0.9f, 1);
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_position_ememy, 1.1f, 1);
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_position_ememy, 4, 1);
        Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vectorMe, 4.2f, 1);
        LeanTween.move(ObjectMe, vector_position_ememy, 0.01f).setOnComplete(() =>
        {
            animatorHero.SetTrigger(Constant.attack2);
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_Attack2);
            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[2].gameObject, vector_competitor, 1.2f, 1);
            LeanTween.delayedCall(3, () =>
            {

                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
                };

                LeanTween.move(ObjectMe, vectorMe, 0.01f).setOnComplete(() =>
                {
                    Delay(1, ResetData);
                }).setDelay(3f);
            }).setDelay(1f);
        });
	}
    public override void Attack_Q()
    {
        base.Attack_Q();
        animatorHero.SetTrigger(Constant.attackQ);
        Delay(0.6f, () =>
        {
            GameObject bullet = CreateObjectPool(bulletPrefab, beginBullet1.transform.position);
            GameObject animationBullet = CreateObjectPool(bulletPrefab, Vector3.zero, bullet.transform);
            GameObject bullet2 = CreateObjectPool(bulletPrefab, beginBullet2.transform.position);
            GameObject animationBullet2 = CreateObjectPool(bulletPrefab, Vector3.zero, bullet2.transform);
            bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
            bullet2.GetComponent<SpriteRenderer>().sprite = listBullet[0];
            animationBullet.GetComponent<SpriteRenderer>().sprite = listBullet[1];
            animationBullet2.GetComponent<SpriteRenderer>().sprite = listBullet[1];
            bullet.transform.localScale = new Vector3(0.5f, 0.5f);
            bullet.transform.eulerAngles = Vector3.zero;
            animationBullet.transform.localScale = new Vector3(1, 1);
            bullet2.transform.localScale = new Vector3(0.5f, 0.5f);
            bullet2.transform.eulerAngles = Vector3.zero;
            animationBullet2.transform.localScale = new Vector3(1, 1);
            LeanTween.rotateZ(bullet, 720, 0.5f).setRepeat(-1);
            LeanTween.rotateZ(bullet2, -720, 0.5f).setRepeat(-1);
            LeanTween.move(bullet, vector_competitor, 0.8f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(bullet);
                SelfDestruction_Object_Pool(animationBullet);
                ResetData();
            });
            LeanTween.move(bullet2, vector_competitor, 0.7f).setOnComplete(() =>
            {
                Auto_SelfDestruction_Object_Pool(ListFXSkill[3].gameObject, vector_competitor, 4); SelfDestruction_Object_Pool(bullet2);
                Auto_SelfDestruction_Object_Pool(ListFXSkill[4].gameObject, vector_competitor, 2);
                SelfDestruction_Object_Pool(animationBullet2);
            });
        });
    }
    public override void Attack_W()
    {
        base.Attack_W();
        Attack1();
        Delay(5, () =>
        {
            Attack2();
        });

    }
    public override void Attack_E()
    {
        base.Attack_E();
        vectorMe = ObjectMe.transform.position;
        Attack_Q();
        LeanTween.delayedCall(2f, () =>
        {
            animatorHero.SetTrigger(Constant.dash1);
            LeanTween.move(ObjectMe, vector_position_ememy, 0.01f).setOnComplete(() =>
            {
                LeanTween.delayedCall(1f, () =>
                {
                    GameObject obtmp = null;
                    Animator animatortmp = null;
                    switch (chairPositions)
                    {
                        case Constant.CHAIR_LEFT:
                            obtmp = LeanPool.Spawn(ObjectMe, vector_position_ememy + new Vector3(2, 0), Quaternion.identity);
                            animatortmp = obtmp.transform.GetChild(0).GetComponent<Animator>();
                            obtmp.transform.eulerAngles = new Vector3(0, 180, 0);
                            animatortmp.SetTrigger(Constant.attackE);
                            break;
                        case Constant.CHAIR_RIGHT:
                            obtmp = LeanPool.Spawn(ObjectMe, vector_position_ememy + new Vector3(-2, 0), Quaternion.identity);
                            animatortmp = obtmp.transform.GetChild(0).GetComponent<Animator>();
                            obtmp.transform.eulerAngles = new Vector3(0, 0, 0);
                            animatortmp.SetTrigger(Constant.attackE);
                            break;
                    }
                    LeanTween.delayedCall(1f, () =>
                    {
                        Destroy(obtmp);
                    });
                });
                animatorHero.SetTrigger(Constant.attackE);
                LeanTween.delayedCall(1f, () =>
                {
                    Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, vector_competitor, 2);
                    LeanTween.delayedCall(1f, () =>
                    {
                        animatorHero.SetTrigger(Constant.dash1);
                        LeanTween.delayedCall(1f, () =>
                        {
                            LeanTween.move(ObjectMe, vectorMe, 0.01f).setOnComplete(() =>
                            {
                                ResetData();
                            });
                        });
                    });
                });
            }).setDelay(1f);
        });
    }
    public override void ResetData()
    {
        base.ResetData();
    }
}
