using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class hero7_Jhiin_controller : BOL_Hero_Controler
{


    public AudioClip sfx_sound_go_start;
    [Header(">>>>Hero only<<<<")]
    public GameObject bulletBegin;
    public GameObject legHero;
    GameObject bulletHero;
    GameObject circleBulletHero;
    LTDescr TweenRotation;
    public override void InitData()
    {
        base.InitData();
    }
    public override void Attack1()
    {
        base.Attack1();
        animatorHero.SetTrigger(Constant.attack1);
        Delay(0.5f, () =>
        {
            GameObject bullet = CreateObjectPool(bulletPrefab, bulletBegin.transform.position);
            bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
            }
            LeanTween.move(bullet, vector_competitor, 0.3f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(bullet);
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.5f);
            });
        });
        Delay(1.4f, ResetData);
    }
    public override void Attack2()
    {
        base.Attack2();
        animatorHero.SetTrigger(Constant.attack2);
        Delay(1.4f, () =>
        {
            GameObject bullet = CreateObjectPool(bulletPrefab, bulletBegin.transform.position);
            bullet.GetComponent<SpriteRenderer>().sprite = listBullet[1];
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
            }
            LeanTween.move(bullet, vector_competitor, 0.3f).setOnComplete(() =>
            {
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                SelfDestruction_Object_Pool(bullet);
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1.5f);
                ResetData();
            });
        });
    }
    public override void Attack_Q()
    {
        base.Attack_Q();
        animatorHero.SetTrigger(Constant.attackQ);
        LeanTween.delayedCall(1, () =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
            }
            GameObject bulet = CreateObjectPool(ListFXSkill[1].gameObject, bulletBegin.transform.position);
            bulet.transform.localScale = new Vector3(1, 1);
            //bulet.GetComponent<SpriteRenderer>().sprite = listBullet[2];
            //bulet.transform.localScale = Vector3.one * 1.5f;
            LeanTween.delayedCall(1, () =>
            {
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_AttackQ);
                }
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 2f);
                LeanTween.delayedCall(1, ResetData);
            });
        });
    }
    public override void Attack_W()
    {
        base.Attack_W();
        animatorHero.SetTrigger(Constant.attackW);
        Delay(0.5f, () =>
        {
            if (bulletHero == null)
            {
                CreateBullet();
            }
            else
            {
                SelfDestruction_Object_Pool(circleBulletHero);
                SelfDestruction_Object_Pool(bulletHero);
                circleBulletHero = null;
                bulletHero = null;
                CreateBullet();
            }
            int tweenR = LeanTween.rotateZ(bulletHero, 720, 0.5f).setRepeat(-1).id;
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
            }
            LeanTween.move(bulletHero, new Vector3(vector_competitor.x, vector_competitor.y - 0.5f), 0.5f).setOnComplete(() =>
            {
                Delay(0.2f, () =>
                {
                    bulletHero.GetComponent<SpriteRenderer>().sprite = listBullet[4];
                    circleBulletHero.transform.localScale = Vector3.one;
                    LeanTween.cancel(bulletHero, tweenR);
                    tweenR = LeanTween.rotateZ(bulletHero, -720, 1f).setRepeat(-1).id;
                    if (BOL_Manager.instance.CanPlayMusicAndSfx())
                    {
                        MyAudioManager.instance.PlaySfx(sfx_AttackW);
                    }
                    Auto_SelfDestruction_Object_Pool(ListFXSkill[2].gameObject, new Vector3(vector_competitor.x, vector_competitor.y - 0.5f), 1, 2);
                    Delay(1.2f, () =>
                    {
                        LeanTween.cancel(bulletHero, tweenR);
                        SelfDestruction_Object_Pool(circleBulletHero);
                        SelfDestruction_Object_Pool(bulletHero);
                        circleBulletHero = null;
                        bulletHero = null;
                        ResetData();
                    });
                });
            });
        });
    }
    public override void Attack_E()
    {
        base.Attack_E();
        animatorHero.SetTrigger(Constant.attack1);
        Delay(0.5f, () =>
        {
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
            }
            GameObject bullet = CreateObjectPool(bulletPrefab, bulletBegin.transform.position);
            bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
            LeanTween.move(bullet, vector_competitor, 0.3f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(bullet);
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_AttackE);
                }
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.5f);
                Delay(0.2f, () =>
                {
                    animatorHero.SetTrigger(Constant.attackE);
                    Delay(2f, () =>
                    {
                        bullet = CreateObjectPool(bulletPrefab, bulletBegin.transform.position);
                        bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
                        if (BOL_Manager.instance.CanPlayMusicAndSfx())
                        {
                            MyAudioManager.instance.PlaySfx(sfx_sound_go_start);
                        }
                        LeanTween.move(bullet, vector_competitor, 0.3f).setOnComplete(() =>
                        {
                            if (BOL_Manager.instance.CanPlayMusicAndSfx())
                            {
                                MyAudioManager.instance.PlaySfx(sfx_AttackE);
                            }
                            SelfDestruction_Object_Pool(bullet);
                            Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.5f);
                            Delay(1, ResetData);
                        });
                    });
                });
            });
        });
    }


    void CreateBullet()
    {
        bulletHero = CreateObjectPool(bulletPrefab, bulletBegin.transform.position);
        bulletHero.GetComponent<SpriteRenderer>().sprite = listBullet[3];
        bulletHero.transform.eulerAngles = new Vector3(80, 0, 0);
        circleBulletHero = CreateObjectPool(bulletPrefab, Vector3.zero, bulletHero.transform);
        circleBulletHero.transform.localScale = Vector3.zero;
        circleBulletHero.transform.eulerAngles = new Vector3(80, 0, 0);
        circleBulletHero.GetComponent<SpriteRenderer>().sprite = listBullet[5];
    }
    void RotationOject(int LR)
    {

    }
}
