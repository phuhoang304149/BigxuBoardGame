using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class hero10_leesin_controller : BOL_Hero_Controler
{

    public AudioClip sfx_sound_go;
    public GameObject objecHeroMain;
    public GameObject objectBullet;
    public GameObject objecLeg;
    Vector3 vector_rotation_util;
    Vector3 vector_begin_attackQ;
    public GameObject objectParent;
    public override void InitData()
    {
        base.InitData();
        if (chairPositions == Constant.CHAIR_LEFT)
        {
            vector_rotation_util = new Vector3(0, 0, -4.31f);
        }
        else if (chairPositions == Constant.CHAIR_RIGHT)
        {
            vector_rotation_util = new Vector3(0, 180, -4.31f);
        }
    }
    public override void Attack1()
    {
        base.Attack1();
        animatorHero.SetTrigger(Constant.attack1);
        //Auto_SelfDestruction_Object_Pool(listAnimationBullet[1].gameObject, objectBullet.transform.position, 1,0.4f);
        Delay(1.4f, () =>
        {
            GameObject hero_bullet = CreateObjectPool(bulletPrefab, objectBullet.transform.position);
            hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
            hero_bullet.transform.localScale = Vector3.one * 2;
            RotateObject(hero_bullet);
            //GameObject animation_bullet = CreateObjectPool(listAnimationBullet[0], Vector3.zero, hero_bullet.transform);
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go);
            }
            LeanTween.move(hero_bullet, vector_competitor, 0.2f).setOnComplete(() =>
            {
                //SelfDestruction_Object_Pool(animation_bullet);
                SelfDestruction_Object_Pool(hero_bullet);
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                GameObject fxEffect = LeanPool.Spawn(ListFXSkill[0].gameObject, vector_competitor, Quaternion.identity);
                RotateObject(fxEffect);
                Delay(1f, () =>
                {
                    LeanPool.Despawn(fxEffect);
                });
                //Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
                Delay(0.5f, ResetData);
            });
        });
    }
    public override void Attack2()
    {
        base.Attack2();
        animatorHero.SetTrigger(Constant.attack2);
        animatorHero.SetTrigger(Constant.idle);
        GameObject hero_bullet = CreateObjectPool(bulletPrefab, objecLeg.transform.position);
        DelayObject(hero_bullet, 1f);
        //GameObject animation_bullet = CreateObjectPool(listAnimationBullet[0], Vector3.zero, hero_bullet.transform);
        hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[1];
        hero_bullet.transform.localScale = new Vector3(1.5f, 0, 0);
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go);
        }
        Vector3 vectorendAttack2 = new Vector3(-objecLeg.transform.position.x, objecLeg.transform.position.y, objecLeg.transform.position.z);
        LeanTween.move(hero_bullet, vectorendAttack2, 0.6f).setOnComplete(() =>
        {
            //SelfDestruction_Object_Pool(animation_bullet);
            SelfDestruction_Object_Pool(hero_bullet);
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_Attack2);
            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, vector_competitor - new Vector3(0, 0.5f), 1f);
        }).setDelay(1);
        LeanTween.scaleY(hero_bullet, 1.5f, 0.3f).setOnComplete(() =>
        {
            hero_bullet.transform.localScale = new Vector3(1.5f, 0, 0);
            LeanTween.scaleY(hero_bullet, 1.5f, 0.1f);
            ResetData();
        }).setDelay(1);
    }

    public override void Attack_Q()
    {
        base.Attack_Q();
        animatorHero.SetTrigger(Constant.attackQ);
        animatorHero.SetTrigger(Constant.idle);
        vector_begin_attackQ = objectBullet.transform.position;
        AttackQHero10(vector_begin_attackQ + new Vector3(0, -0.6f, 0), vector_competitor + new Vector3(0, -0.6f, 0), 0.5f, 1.3f);
        AttackQHero10(vector_begin_attackQ + new Vector3(0, 0, 0), vector_competitor + new Vector3(0, 0, 0), 0.5f, 1.4f);
        AttackQHero10(vector_begin_attackQ + new Vector3(0, -0.7f, 0), vector_competitor + new Vector3(0, -0.7f, 0), 0.5f, 1.5f);
        AttackQHero10(vector_begin_attackQ + new Vector3(0, 0.4f, 0), vector_competitor + new Vector3(0, 0.4f, 0), 0.5f, 1.6f);
        LeanTween.delayedCall(2, ResetData);
    }
    public override void Attack_W()
    {
        base.Attack_W();
        animatorHero.SetTrigger(Constant.attackW);
        animatorHero.SetTrigger(Constant.idle);
        LeanTween.delayedCall(1.9f, () =>
        {
            GameObject fxSkillW = CreateObjectPool(ListFXSkill[2].gameObject, myBody.transform.position);
            //DelayObject(fxSkillW, 1.9f);
            fxSkillW.transform.eulerAngles = new Vector3(-90, 0, 0);
            LeanTween.delayedCall(5, () =>
            {
                SelfDestruction_Object_Pool(fxSkillW);
                ResetData();
            });
        });

    }
    public override void Attack_E()
    {
        base.Attack_E();
        animatorHero.SetTrigger(Constant.attackE);
        animatorHero.SetTrigger(Constant.idle);
        GameObject hero_bullet = CreateObjectPool(bulletPrefab, objectBullet.transform.position);
        DelayObject(hero_bullet, 1.8f);
        //GameObject animation_bullet = CreateObjectPool(listAnimationBullet[0], Vector3.zero, hero_bullet.transform);
        hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[2];
        hero_bullet.transform.eulerAngles = vector_rotation_util;
        LeanTween.scale(hero_bullet, new Vector3(1, 1, 1), 0.01f).setOnComplete(() =>
        {
            hero_bullet.transform.position = objectBullet.transform.position;
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go);
            }
            LeanTween.move(hero_bullet, vector_competitor, 0.3f).setOnComplete(() =>
            {
                //SelfDestruction_Object_Pool(animation_bullet);

                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                SelfDestruction_Object_Pool(hero_bullet);
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
                ResetData();
            });
        }).setDelay(1.8f);

    }
    public void AttackQHero10(Vector3 vectorbg, Vector3 vectorfn, float moveTime, float timedelay)
    {
        GameObject hero_bullet = CreateObjectPool(bulletPrefab, vectorbg);
        DelayObject(hero_bullet, timedelay);
        hero_bullet.transform.localScale = new Vector3(1, 1, 1);
        hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[3];
        //GameObject animation_bullet = CreateObjectPool(listAnimationBullet[0], Vector3.zero, hero_bullet.transform);

        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(sfx_sound_go);
        }
        LeanTween.move(hero_bullet, vectorfn, moveTime).setOnComplete(() =>
        {
            //SelfDestruction_Object_Pool(animation_bullet);
            SelfDestruction_Object_Pool(hero_bullet);
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_Attack1);
            }
            Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vectorfn, 1f);
        }).setDelay(timedelay).setRepeat(2);
    }
    public override void ResetData()
    {
        base.ResetData();
    }

}
