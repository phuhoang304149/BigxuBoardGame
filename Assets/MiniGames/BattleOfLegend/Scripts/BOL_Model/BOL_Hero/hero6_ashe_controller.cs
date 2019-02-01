using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class hero6_ashe_controller : BOL_Hero_Controler
{

    public AudioClip sfx_sound_go;
    public GameObject bowObject;
    public Vector3 vectorBow;
    public List<GameObject> ListAura;
    Vector3 vectorMid_1, vectorMid_2;
    Vector3 vector_Rotation_Attack2, vector_Rotation_Attack2180;
    Vector3 vectorBegin1_Attack2, vectorBegin2_Attack2;
    Vector3 vectorBegin_Attack_W1, vectorBegin_Attack_W2, vectorBegin_Attack_W3, vectorBegin_Attack_W4;
    Vector3 vectorEnd_Attack_W1, vectorEnd_Attack_W2, vectorEnd_Attack_W3, vectorEnd_Attack_W4;
    public override void InitData()
    {
        base.InitData();
        if (chairPositions == Constant.CHAIR_LEFT)
        {
            vectorMid_1 = new Vector3(-2f, -0.9f, 0);
            vectorMid_2 = new Vector3(2, -0.9f, 0);
            vector_Rotation_Attack2 = new Vector3(0, 0, -7.277f);
        }
        else if (chairPositions == Constant.CHAIR_RIGHT)
        {
            vectorMid_1 = new Vector3(2f, -0.9f, 0);
            vectorMid_2 = new Vector3(-2, -0.9f, 0);
            vector_Rotation_Attack2 = new Vector3(0, 0, -7.277f);
        }
    }
    public override void Attack1()
    {
        base.Attack1();
        bulletPrefab.transform.eulerAngles = Vector3.zero;
        animatorHero.SetTrigger(Constant.attack1);
        Delay(1.1f, () =>
        {
            GameObject hero_bullet = CreateObjectPool(bulletPrefab, bowObject.transform.position);
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go);
            }
            hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
            //if (vector_competitor.x < 0) {
            //	hero_bullet.transform.localScale = new Vector3(hero_bullet.transform.localScale.x, hero_bullet.transform.localScale.y);
            //}

            LeanTween.move(hero_bullet, vector_competitor, 0.2f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(hero_bullet);
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.5f);
                Delay(0.5f, ResetData);
            });
        });
    }
    public override void Attack2()
    {
        base.Attack2();
        bulletPrefab.transform.eulerAngles = Vector3.zero;
        vectorBow = bowObject.transform.position;
        animatorHero.SetTrigger(Constant.attack2);
        Delay(0.5f, () =>
        {
            GameObject hero_bullet = CreateObjectPool(bulletPrefab, new Vector3(vectorBow.x, vectorBow.y + 0.5f));
            if (hero_bullet.transform.localScale.y < 0)
            {
                hero_bullet.transform.localScale = new Vector3(hero_bullet.transform.localScale.x, hero_bullet.transform.localScale.y * -1);
            }
            if (vector_competitor.x > 0 && hero_bullet.transform.localScale.x < 0)
            {
                hero_bullet.transform.localScale = new Vector3(hero_bullet.transform.localScale.x * -1, hero_bullet.transform.localScale.y);
            }
            else if (vector_competitor.x < 0 && hero_bullet.transform.localScale.x > 0)
            {
                hero_bullet.transform.localScale = new Vector3(hero_bullet.transform.localScale.x * -1, hero_bullet.transform.localScale.y);
            }

            hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[1];
            hero_bullet.transform.eulerAngles = vector_Rotation_Attack2;
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go);
            }
            LeanTween.move(hero_bullet, vector_competitor, 0.2f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(hero_bullet);
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
            }).setRepeat(5);
        });
        Delay(0.6f, () =>
        {
            GameObject hero_bullet2 = CreateObjectPool(bulletPrefab, new Vector3(vectorBow.x, vectorBow.y - 0.5f));
            if (hero_bullet2.transform.localScale.y > 0)
            {
                hero_bullet2.transform.localScale = new Vector3(hero_bullet2.transform.localScale.x, hero_bullet2.transform.localScale.y * -1);
            }
            if (vector_competitor.x > 0 && hero_bullet2.transform.localScale.x < 0)
            {
                hero_bullet2.transform.localScale = new Vector3(hero_bullet2.transform.localScale.x * -1, hero_bullet2.transform.localScale.y);
            }
            else if (vector_competitor.x < 0 && hero_bullet2.transform.localScale.x > 0)
            {
                hero_bullet2.transform.localScale = new Vector3(hero_bullet2.transform.localScale.x * -1, hero_bullet2.transform.localScale.y);
            }
            hero_bullet2.GetComponent<SpriteRenderer>().sprite = listBullet[1];
            hero_bullet2.transform.eulerAngles = vector_Rotation_Attack2;
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go);
            }
            LeanTween.move(hero_bullet2, vector_competitor, 0.2f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(hero_bullet2);
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
            }).setRepeat(5);
        });
        Delay(2, ResetData);
    }
    public override void Attack_Q()
    {
        base.Attack_Q();
        animatorHero.SetTrigger(Constant.attackQ);
        vectorBow = bowObject.transform.position;
        Delay(0.7f, () =>
        {
            GameObject fxskill = LeanPool.Spawn(ListFXSkill[1].gameObject, vectorBow, Quaternion.identity);
            if (vector_competitor.x < 0 && fxskill.transform.localScale.x > 0)
            {
                fxskill.transform.localScale = new Vector3(fxskill.transform.localScale.x * -1, fxskill.transform.localScale.y);
            }
            else if (vector_competitor.x > 0 && fxskill.transform.localScale.x < 0)
            {
                fxskill.transform.localScale = new Vector3(fxskill.transform.localScale.x * -1, fxskill.transform.localScale.y);
            }
            Delay(2, () =>
            {
                LeanPool.Despawn(fxskill);
            });
            //Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, vectorBow, 2);
        });
        AsheHeroQ(18, 0.7f, 0.5f, new Vector3(0, vectorBow.y, 0));
        AsheHeroQ(10, 0.7f, 0.5f, new Vector3(0, vectorBow.y + 0.5f, 0));
        AsheHeroQ(0, 0.7f, 0.5f, new Vector3(0, vectorBow.y + 1f, 0));
        AsheHeroQ(-10, 0.7f, 0.5f, new Vector3(0, vectorBow.y - 0.5f, 0));
        AsheHeroQ(-18, 0.7f, 0.5f, new Vector3(0, vectorBow.y - 1f, 0));
        Delay(1.5f, ResetData);
    }
    public override void Attack_W()
    {
        base.Attack_W();
        animatorHero.SetTrigger(Constant.attackW);
        animatorHero.SetTrigger(Constant.idle);
        Delay(1.3f, () =>
        {
            GameObject hero_bullet = CreateObjectPool(bulletPrefab, bowObject.transform.position);
            //if (vector_competitor.x < 0) {
            //	hero_bullet.transform.localScale = new Vector3(hero_bullet.transform.localScale.x * -1, hero_bullet.transform.localScale.y);
            //}
            hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[2];
            hero_bullet.transform.localScale = Vector3.one;
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go);
            }
            LeanTween.move(hero_bullet, vector_competitor, 0.5f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(hero_bullet);
                Auto_SelfDestruction_Object_Pool(ListFXSkill[2].gameObject, vector_competitor, 1f);
            });
            if (chairPositions == Constant.CHAIR_LEFT)
            {
                LeanTween.rotateX(hero_bullet, 180, 0.3f).setRepeat(-1);
            }
            else if (chairPositions == Constant.CHAIR_RIGHT)
            {
                LeanTween.rotateX(hero_bullet, 180, 0.3f).setRepeat(-1);
            }
        });
        ResetData();
    }
    public override void Attack_E()
    {
        base.Attack_E();
        bulletPrefab.transform.eulerAngles = Vector3.zero;
        animatorHero.SetTrigger(Constant.attackE);
        LeanTween.delayedCall(1.2f, () =>
        {
            GameObject hero_bullet = CreateObjectPool(bulletPrefab, bowObject.transform.position);
            //if (vector_competitor.x < 0) {
            //	hero_bullet.transform.localScale = new Vector3(hero_bullet.transform.localScale.x * -1, hero_bullet.transform.localScale.y);
            //}
            //GameObject animation_bullet = CreateObjectPool(listAnimationBullet[0], Vector3.zero, hero_bullet.transform);
            hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[3];
            if (BOL_Manager.instance.CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(sfx_sound_go);
            }
            LeanTween.move(hero_bullet, vector_competitor, 0.5f).setOnComplete(() =>
            {
                SelfDestruction_Object_Pool(hero_bullet);
                //SelfDestruction_Object_Pool(animation_bullet);
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_Attack1);
                }
                Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
                Attack_Q();
            });
            if (chairPositions == Constant.CHAIR_LEFT)
            {
                LeanTween.rotate(hero_bullet, new Vector3(180, 0, 0), 0.3f);
            }
            else if (chairPositions == Constant.CHAIR_RIGHT)
            {
                LeanTween.rotate(hero_bullet, new Vector3(180, 180, 0), 0.3f);
            }
        });
        //Delay(1.4f, () => {
        //	ResetData();
        //	Delay(0.5f, () => {
        //		Attack_Q();
        //	});
        //});
    }
    public void AsheHeroQ(float angleZ, float delayTime, float timeDuration, Vector3 vectorHeight)
    {
        GameObject hero_bullet = null;
        bulletPrefab.transform.eulerAngles = Vector3.zero;
        LeanTween.delayedCall(delayTime, () =>
        {
            hero_bullet = CreateObjectPool(bulletPrefab, bowObject.transform.position);
            hero_bullet.transform.eulerAngles = Vector3.zero;
            LeanTween.scale(hero_bullet, new Vector3(1, 1, 1), 0.01f).setOnComplete(() =>
            {
                vectorBow = bowObject.transform.position;
                hero_bullet.transform.position = vectorBow;
                if (chairPositions == Constant.CHAIR_LEFT)
                {
                    hero_bullet.transform.eulerAngles = new Vector3(0, 0, angleZ);
                }
                else if (chairPositions == Constant.CHAIR_RIGHT)
                {
                    hero_bullet.transform.eulerAngles = new Vector3(0, 180, angleZ);
                }
                hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
                LTBezierPath ltPath = new LTBezierPath(new Vector3[] { vectorBow, vectorHeight, vectorHeight, vector_competitor });
                if (BOL_Manager.instance.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(sfx_sound_go);
                }
                LeanTween.move(hero_bullet, ltPath.pts, timeDuration).setOnComplete(() =>
                {
                    SelfDestruction_Object_Pool(hero_bullet);
                    if (BOL_Manager.instance.CanPlayMusicAndSfx())
                    {
                        MyAudioManager.instance.PlaySfx(sfx_Attack1);
                    }
                    Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
                });
                LeanTween.rotateZ(hero_bullet, 0, timeDuration / 2).setOnComplete(() =>
                {
                    LeanTween.rotateZ(hero_bullet, -angleZ, timeDuration / 2).setOnComplete(() =>
                    {
                        hero_bullet.transform.eulerAngles = Vector3.zero;
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