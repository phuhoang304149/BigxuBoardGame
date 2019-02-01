using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TESTAAAA : MonoBehaviour {

    public GridLayoutGroup myGridLayoutGroup;
    public RectTransform myRectTransform;
    public Text txtTest;
    public LayoutElement layoutElement;
    public long goldAdd;

	[ContextMenu("TESTA")]
    public void TESTA(){
        string _t = "abcdefghijklmnop";
        Debug.Log(">>> " + _t.Substring(0, 12));
    }

    [ContextMenu("TESTB")]
    public void TESTB(){
       Debug.Log(goldAdd + " -- " + MyConstant.GetMoneyString(goldAdd));
    }

    [ContextMenu("TESTC")]
    public void TESTC(){
        Debug.Log(txtTest.rectTransform.rect.width + " - " + txtTest.rectTransform.rect.height);
       
        // layoutElement.preferredWidth = 200;
        // Debug.Log(txtTest.rectTransform.rect.width);
        // txtTest.rectTransform.sizeDelta = new Vector2(100,50);

        // Language a = new Language();
        // a.setupLanguage();
        // Debug.Log(a.getString(Language.asdas));
    }

    [ContextMenu("TESTD")]
    public void TESTD(){
        Debug.Log(myRectTransform.sizeDelta);
    }

    [ContextMenu("TESTE")]
    public void TESTE(){
        Debug.Log(myGridLayoutGroup.constraintCount + " - " + myGridLayoutGroup.cellSize.x + " - " + (myRectTransform.sizeDelta.x / myGridLayoutGroup.cellSize.x));
        
        // Vector2 _sz = myGridLayoutGroup.cellSize;
        // _sz.x = 240f;
        // _sz.y = 240f;
        // myGridLayoutGroup.cellSize = _sz;
    }

    [ContextMenu("TESTF")]
    public void TESTF(){
        List<int> a = new List<int>();
        a.Add(0);
        a.Add(3);
        a.Add(2);
        a.Add(1);
        a.Add(4);
        List<int> b = a;
        b[0] = 100;

        string _tmpA = string.Empty;
        for (int i = 0; i < a.Count; i++)
        {
            _tmpA += a[i] + "|";
        }
        Debug.Log(">>> " + _tmpA);

        string _tmpB = string.Empty;
        for (int i = 0; i < b.Count; i++)
        {
            _tmpB += b[i] + "|";
        }
        Debug.Log(">>> " + _tmpB);
    }

    [ContextMenu("TESTG")]
    public void TESTG(){
        LeanTween.color(myRectTransform, Color.yellow, 1f).setOnComplete(()=>{
           
        });
    }

    [ContextMenu("TESTH")]
    public void TESTH(){
        StartCoroutine(DoActionTestH());
    }

    IEnumerator DoActionTestH(){
        while(true){
            yield return Yielders.FixedUpdate;
            transform.TweenLookAt(Vector2.zero, 360f);
        }
    }

    [SerializeField] Sprite imgReplace;
    [SerializeField] bool canReplace;
    [ContextMenu("TESTI")]
    public void TESTI(){
        Object[] _o = FindObjectsOfType(typeof(Image));
        for(int i = 0; i < _o.Length; i ++){
            Image _i = (Image) _o[i];
            if(_i.isActiveAndEnabled && _i.sprite == null){
                Debug.Log(">>>>> " + _i.name);
                if(canReplace){
                    _i.sprite = imgReplace;
                }
            }
        }
    }
}
