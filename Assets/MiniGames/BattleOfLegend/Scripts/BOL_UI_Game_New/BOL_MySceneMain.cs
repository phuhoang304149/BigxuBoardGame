using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class BOL_MySceneMain : MonoBehaviour {
public enum UIScene{
ShowHeroAndSkill,
ChoiceHero,
ShowPlayer,
ShowBattle,
ShowFinish
}
	public virtual UIScene mySceneType {
		get {
			return UIScene.ShowHeroAndSkill;
		}
	}

	public UIScene myLastType { get; set; }
    public virtual void InitData() { }
    public virtual void ResetData() { }
    public virtual void RefreshData() { }
    public virtual void Show() { }
    public virtual void Hide() { }
    public virtual void SelfDestruction() {
        LeanPool.Despawn(gameObject);
    }
        public virtual void DestroyObject(){
		Destroy(gameObject);
    }
}
