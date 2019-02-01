using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using TMPro;
using System;

public class BOL_Battle_PlayerInGame : BOL_MySceneMain
{
    public const int CURRENT_PIECE = 1;
    public const int NEXT_PIECE = 2;
    public const int HEALTH = 3;
    public const int MANA = 4;
    public const int SHIELD = 5;
    public const float TIMETWEEN_VERTICAL = 0.05f;
    public const float TIMETWEEN_HORIZONTAL = 0.01f;
    const float Rowfoot = 0.25f;
    const float Colfoot = -0.25f;
    public BOL_FlashingSprite bOL_Flashing;
    public GameObject BGbattle;
    public GameObject playerBattle;
    public GameObject curentPiece;
    public PieceArrayControl _currentPieceCtrl;
    public GameObject netxtPiece;
    public PieceArrayControl _nextPieceCtrl;
    public GameObject breakParticle;
    public GameObject[,] matrix_Piece = new GameObject[12, 8];
    public List<DelayedAsset> listPiece;
    public GameObject MatrixMain;
    public GameObject txtShowHPMNSH;
    public GameObject hp;
    public long hpvalue;

    public GameObject mp;
    private long mpvalue;
    public long _mpvalue
    {
        get
        {
            return mpvalue;
        }
        set
        {
            mpvalue = value;
        }
    }
    public GameObject shield;
    public TextMeshPro textHP;
    public TextMeshPro textMN;
    public TextMeshPro textShield;
    public List<int> ListPlayerPlay;
    LTDescr TweenAlpha;
    LTDescr TweenVertical;
    LTDescr TweenHorizontal;
    [Header(" control pieces")]
    [Header("manager list spawn")]
    public List<GameObject> listObjectSpawn;
    float chair;
    public override void InitData()
    {

        if (playerBattle.transform.localScale.x > 0 && textShield.transform.localScale.x < 0)
        {
            chair = -1;
        }
        else if (playerBattle.transform.localScale.x < 0 && textShield.transform.localScale.x > 0)
        {
            chair = -1;
        }
        else chair = 1;
        listObjectSpawn = new List<GameObject>();
        bOL_Flashing.Initdata();
        textHP.transform.localScale = new Vector3(textHP.transform.localScale.x * chair, textHP.transform.localScale.y);
        textMN.transform.localScale = new Vector3(textMN.transform.localScale.x * chair, textMN.transform.localScale.y);
        textShield.transform.localScale = new Vector3(textShield.transform.localScale.x * chair, textShield.transform.localScale.y);
        MatrixMain.transform.localScale = new Vector3(MatrixMain.transform.localScale.x * chair, MatrixMain.transform.localScale.y);
        MatrixMain.transform.localPosition = new Vector3(MatrixMain.transform.localPosition.x * chair, MatrixMain.transform.localPosition.y);
        netxtPiece.transform.localScale = new Vector3(netxtPiece.transform.localScale.x * chair, netxtPiece.transform.localScale.y);

        MoveInPlayGame(playerBattle, playerBattle.transform.localPosition);
    }
    public override void SelfDestruction()
    {
        base.SelfDestruction();
    }
    public GameObject SpawnObjectPools(GameObject prefab, Vector3 position, Transform parent = null)
    {
        if (parent != null)
        {
            GameObject piece = LeanPool.Spawn(prefab, position, Quaternion.identity, parent);
            return piece;
        }
        else
        {
            GameObject piece = LeanPool.Spawn(prefab, position, Quaternion.identity);
            return piece;
        }
    }
    IEnumerator DelayDespawn(GameObject objectprefab, float time)
    {
        yield return Yielders.Get(time);
        SelfDestruction_Object_Pools(objectprefab);
    }
    public void SelfDestruction_Object_Pools(GameObject objectPool)
    {
        if (objectPool == null || !objectPool.activeSelf)
        {
            return;
        }
        LeanPool.Despawn(objectPool);
    }
    public Vector3 PointFinish(int row, int column)
    {
        return new Vector3(column * Rowfoot + Rowfoot / 2, row * Colfoot + Colfoot / 2);
    }
    float VectorRowFinish(int row)
    {
        return row * Colfoot + Colfoot / 2;
    }
    float VectorColumnFinish(int column)
    {
        return column * Rowfoot + Rowfoot / 2;
    }
    public void SetValuePiece(sbyte[] piecesValue, int stylePiece, int chairID = 0)
    {
        switch (stylePiece)
        {
            case CURRENT_PIECE:
                _currentPieceCtrl.piece1.sprite = null;
                _currentPieceCtrl.piece2.sprite = null;
                _currentPieceCtrl.piece3.sprite = null;
                switch (piecesValue.Length)
                {
                    case 1:
                        _currentPieceCtrl.piece1Value = piecesValue[0];
                        break;
                    case 2:
                        _currentPieceCtrl.piece1Value = piecesValue[0];
                        _currentPieceCtrl.piece2Value = piecesValue[1];
                        break;
                    case 3:
                        _currentPieceCtrl.piece1Value = piecesValue[0];
                        _currentPieceCtrl.piece2Value = piecesValue[1];
                        _currentPieceCtrl.piece3Value = piecesValue[2];
                        break;
                }
                break;
            case NEXT_PIECE:
#if TEST
                Debug.Log("setnextpiece" + piecesValue.Length);
#endif
                _nextPieceCtrl.piece1.sprite = null;
                _nextPieceCtrl.piece2.sprite = null;
                _nextPieceCtrl.piece3.sprite = null;
                switch (piecesValue.Length)
                {
                    case 1:
                        _nextPieceCtrl.piece1Value = piecesValue[0];
                        break;
                    case 2:
                        _nextPieceCtrl.piece1Value = piecesValue[0];
                        _nextPieceCtrl.piece2Value = piecesValue[1];
                        break;
                    case 3:
                        _nextPieceCtrl.piece1Value = piecesValue[0];
                        _nextPieceCtrl.piece2Value = piecesValue[1];
                        _nextPieceCtrl.piece3Value = piecesValue[2];
                        break;
                }
                break;

        }
        //if (stylePiece == CURRENT_PIECE) {
        //_currentPieceCtrl.piece1.sprite = null;
        //_currentPieceCtrl.piece2.sprite = null;
        //_currentPieceCtrl.piece3.sprite = null;
        //switch (piecesValue.Length) {
        //	case 1:
        //		_currentPieceCtrl.piece1Value = piecesValue[0];
        //		break;
        //	case 2:
        //		_currentPieceCtrl.piece1Value = piecesValue[0];
        //		_currentPieceCtrl.piece2Value = piecesValue[1];
        //		break;
        //	case 3:
        //		_currentPieceCtrl.piece1Value = piecesValue[0];
        //		_currentPieceCtrl.piece2Value = piecesValue[1];
        //		_currentPieceCtrl.piece3Value = piecesValue[2];
        //		break;
        //}
        //for (int i = 0; i < piecesValue.Length; i++) {
        //	int valuepiece = piecesValue[i];
        //	if (valuepiece != 0) {
        //		switch (i) {
        //			case 1:
        //				_currentPieceCtrl.piece2.sprite = _currentPieceCtrl.listSprite[valuepiece];
        //				break;
        //			case 2:
        //				_currentPieceCtrl.piece3.sprite = _currentPieceCtrl.listSprite[valuepiece];
        //				break;
        //			default:
        //				_currentPieceCtrl.piece1.sprite = _currentPieceCtrl.listSprite[valuepiece];
        //				break;
        //		}
        //	}
        //}
        //for (int i = 0; i < piecesValue.Length; i++) {
        //	curentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        //	int valuepiece = piecesValue[i];
        //	if (valuepiece != 0) {
        //		GameObject spriteValue = (GameObject)listPiece[valuepiece].Load();
        //		curentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
        //		spriteValue.GetComponent<SpriteRenderer>().sprite;
        //	} else {
        //		curentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        //	}
        //}
        //		} else if (stylePiece == NEXT_PIECE) {
        //#if TEST
        //			Debug.Log("setnextpiece" + piecesValue.Length);
        //#endif
        //_nextPieceCtrl.piece1.sprite = null;
        //_nextPieceCtrl.piece2.sprite = null;
        //_nextPieceCtrl.piece3.sprite = null;
        //switch (piecesValue.Length) {
        //	case 1:
        //		_nextPieceCtrl.piece1Value = piecesValue[0];
        //		break;
        //	case 2:
        //		_nextPieceCtrl.piece1Value = piecesValue[0];
        //		_nextPieceCtrl.piece2Value = piecesValue[1];
        //		break;
        //	case 3:
        //		_nextPieceCtrl.piece1Value = piecesValue[0];
        //		_nextPieceCtrl.piece2Value = piecesValue[1];
        //		_nextPieceCtrl.piece3Value = piecesValue[2];
        //		break;
        //}
        //for (int i = 0; i < piecesValue.Length; i++) {
        //	int valuepiece = piecesValue[i];
        //	if (valuepiece != 0) {
        //		switch (i) {
        //			case 1:
        //				_nextPieceCtrl.piece2.sprite = _nextPieceCtrl.listSprite[valuepiece];
        //				break;
        //			case 2:
        //				_nextPieceCtrl.piece3.sprite = _nextPieceCtrl.listSprite[valuepiece];
        //				break;
        //			default:
        //				_nextPieceCtrl.piece1.sprite = _nextPieceCtrl.listSprite[valuepiece];
        //				break;
        //		}
        //	}
        //}
        //for (int i = 0; i < 3; i++) {
        //	netxtPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        //}
        //for (int i = 0; i < piecesValue.Length; i++) {
        //	netxtPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        //	int valuepiece = piecesValue[i];
        //	if (valuepiece != 0) {
        //		GameObject spriteValue = (GameObject)listPiece[valuepiece].Load();
        //		netxtPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
        //		spriteValue.GetComponent<SpriteRenderer>().sprite;
        //	} else {
        //		netxtPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        //	}
        //}
        //}
    }
    public void SetPostionPiece(sbyte[] pieceValue, int stylePiece, int row, int col)
    {
        curentPiece.transform.localPosition = PointFinish(row, col);
        SetValuePiece(pieceValue, stylePiece);
    }
    public void AddBreakInMatrix(int row, int col, int piece = 0)
    {
        //GameObject breakPiece = SpawnObjectPools(breakParticle, PointFinish(row, col), MatrixMain.transform);
        //listObjectSpawn.Add(breakPiece);
        AutoSpawnAndDespawn(breakParticle, PointFinish(row, col), 1.5f, MatrixMain.transform);
        if (piece != 0)
        {
            GameObject spriteValue = (GameObject)listPiece[piece].Load();
            GameObject pieceBreak = SpawnObjectPools(spriteValue, PointFinish(row, col), MatrixMain.transform);
            listObjectSpawn.Add(pieceBreak);
            Piece_Control pieceControl = pieceBreak.GetComponent<Piece_Control>();
            pieceControl.InitData();
            pieceControl.FlashingObject(true);
        }
    }
    public void AddPieceInMatrix(int value, int row, int col)
    {
        if (matrix_Piece[row, col] != null)
        {
            SelfDestruction_Object_Pools(matrix_Piece[row, col]);
            matrix_Piece[row, col] = null;
        }
        if (value != 0)
        {
            GameObject spriteValue = (GameObject)listPiece[value].Load();
            GameObject piece = SpawnObjectPools(spriteValue, PointFinish(row, col), MatrixMain.transform);
            matrix_Piece[row, col] = piece;
            Piece_Control piece_Control = piece.GetComponent<Piece_Control>();
            piece_Control.InitData();
            listObjectSpawn.Add(piece);
        }
    }
    public void AutoSpawnAndDespawn(GameObject prefab, Vector3 position, float timeDestroy, Transform parent = null)
    {
        if (parent != null)
        {

            ParticleSystem fx = LeanPool.Spawn(prefab, position, Quaternion.identity, parent).GetComponent<ParticleSystem>();
            //GameObject fx = LeanPool.Spawn(prefab, position, Quaternion.identity, parent);
            //if (fx.GetComponent<ParticleSystem>() != null) {
            //	fx.GetComponent<ParticleSystem>().Play();
            //}
            if (fx != null)
            {
                fx.Play();
            }
            StartCoroutine(DelayDespawn(fx.gameObject, timeDestroy));
        }
        else
        {
            ParticleSystem fx = LeanPool.Spawn(prefab, position, Quaternion.identity).GetComponent<ParticleSystem>();
            //GameObject fx = LeanPool.Spawn(prefab, position, Quaternion.identity);
            //if (fx.GetComponent<ParticleSystem>() != null) {
            //	fx.GetComponent<ParticleSystem>().Play();
            //}
            if (fx != null)
            {
                fx.Play();
            }
            StartCoroutine(DelayDespawn(fx.gameObject, timeDestroy));
        }
    }
    public void SetValueHPorMP(int styleText, int valueresult, int valueDefault, int chairPostion)
    {
        switch (styleText)
        {
            case HEALTH:
                textHP.text = valueresult + "/" + valueDefault;
                hpvalue = valueresult;
                ScaleObject(hp, (valueresult), valueDefault);
                switch (chairPostion)
                {
                    case Constant.CHAIR_LEFT:

                        // BOL_ToastManager.Instance.CreateToast((valueresult - valueDefault).ToString(),
                        // BOL_PlaySkill_Controller.instance.info1.transform.position,
                        // Color.red,
                        // ToastController.maxSizeWDefault);
                        ShowHPMNSH((valueresult - valueDefault), BOL_Main_Controller.instance.chairLeft.transform.position,
                        Color.red);
                        break;
                    case Constant.CHAIR_RIGHT:
                        // BOL_ToastManager.Instance.CreateToast((valueresult - valueDefault).ToString(),
                        // BOL_PlaySkill_Controller.instance.info2.transform.position,
                        // Color.red,
                        // ToastController.maxSizeWDefault);
                        ShowHPMNSH((valueresult - valueDefault), BOL_Main_Controller.instance.chairRight.transform.position,
                        Color.red);
                        break;

                }
                break;
            case MANA:
                textMN.text = valueresult + "/" + valueDefault;


                if (BOL_Main_Controller.instance.ChairPosition == chairPostion)
                {
                    _mpvalue = valueresult;
                    BOL_PlaySkill_Controller.instance.btnSkillQ.CheckGlowSkill();
                    BOL_PlaySkill_Controller.instance.btnSkillW.CheckGlowSkill();
                    BOL_PlaySkill_Controller.instance.btnSkillE.CheckGlowSkill();
                }

                ScaleObject(mp, (valueresult), valueDefault);
                switch (chairPostion)
                {
                    case Constant.CHAIR_LEFT:
                        // BOL_ToastManager.Instance.CreateToast((valueresult - valueDefault).ToString(),
                        //    BOL_PlaySkill_Controller.instance.info1.transform.position,
                        //     Color.cyan, ToastController.maxSizeWDefault);
                        ShowHPMNSH((valueresult), BOL_Main_Controller.instance.chairLeft.transform.position,
                        Color.blue);
                        break;
                    case Constant.CHAIR_RIGHT:
                        //     BOL_ToastManager.Instance.CreateToast((valueresult - valueDefault).ToString(),
                        //    BOL_PlaySkill_Controller.instance.info2.transform.position,
                        //     Color.cyan, ToastController.maxSizeWDefault);
                        ShowHPMNSH((valueresult), BOL_Main_Controller.instance.chairRight.transform.position,
                            Color.blue);

                        break;
                }

                break;
            case SHIELD:
                textShield.text = valueresult + "/" + valueDefault;
                ScaleObject(shield, (valueresult), valueDefault);
                break;
        }
    }
    public void TweenPressMoveVertical(int row)
    {
        LeanTween.moveLocalY(curentPiece, VectorRowFinish(row), TIMETWEEN_VERTICAL);

    }
    public void TweenPressMoveHorizontal(int column)
    {
        LeanTween.moveLocalX(curentPiece, VectorColumnFinish(column), TIMETWEEN_HORIZONTAL);

    }
    public void TweenPressMoveVerticalFalling(int value, int col, int row, int rowfn)
    {
        if (matrix_Piece[row, col] != null)
        {
            SelfDestruction_Object_Pools(matrix_Piece[row, col]);
            matrix_Piece[row, col] = null;
        }
        if (value != 0)
        {
            GameObject spriteValue = (GameObject)listPiece[value].Load();
            GameObject piece = SpawnObjectPools(spriteValue, PointFinish(row, col), MatrixMain.transform);
            matrix_Piece[row, col] = piece;
            Piece_Control piece_Control = piece.GetComponent<Piece_Control>();
            piece_Control.InitData();
            listObjectSpawn.Add(piece);
            LeanTween.moveLocalY(piece, VectorRowFinish(rowfn), TIMETWEEN_VERTICAL);
        }

    }
    public void ScaleObject(GameObject gameobject, float value, float valueDefault)
    {
        float ratio = value / valueDefault;
        LeanTween.scaleX(gameobject, ratio, 0.1f);
        if (hp.transform.localScale.x < 0.5f)
        {
            bOL_Flashing.StartFlash();
        }
        else
        {
            bOL_Flashing.StopFlash();
        }
    }

    public override void ResetData()
    {
        //for (int i = 0; i < 3; i++) {
        //	curentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        //	netxtPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
        //}
        //for (int i = 0; i < Constant.ROW; i++) {
        //	for (int j = 0; j < Constant.COL; j++) {
        //		if (matrix_Piece[i, j] != null) {
        //			SelfDestruction_Object_Pools(matrix_Piece[i, j]);
        //			matrix_Piece[i, j] = null;
        //		}
        //	}
        //}
        if (listObjectSpawn.Count > 0)
        {
            for (int i = 0; i < listObjectSpawn.Count; i++)
            {
                Destroy(listObjectSpawn[i]);
            }
        }
    }
    public void CheckWarning()
    {
        int countpiece = 0;
        for (int i = 0; i < Constant.COL; i++)
        {
            if (matrix_Piece[5, i] != null)
            {
                countpiece++;
            }
        }
        if (countpiece > 0)
        {
            ShowWaring(BGbattle, false);
        }
        else
        {
            ShowWaring(BGbattle, false);
        }
    }
    public void ShowWaring(GameObject gameObject, bool booleantween)
    {
        if (booleantween)
        {
            if (TweenAlpha != null)
            {
                TweenAlpha.resume();
            }
            else
            {
                TweenAlpha = LeanTween.alpha(gameObject, 0.1f, 2f).setRepeat(-1).setLoopPingPong();
            }
        }
        else
        {
            if (TweenAlpha != null)
            {
                TweenAlpha.pause();
                LeanTween.alpha(gameObject, 0.4f, 0.01f);
            }
        }
    }
    public void MoveInPlayGame(GameObject objectmove, Vector3 positon)
    {
#if TEST
        Debug.Log(">>>>>>" + positon);
#endif
        Vector3 tmp = Vector3.zero;
        if (objectmove.transform.localScale.x < 0)
        {
            tmp = new Vector3(positon.x + 5, positon.y);
            objectmove.transform.localPosition = tmp;
        }
        else
        {
            tmp = new Vector3(positon.x - 5, positon.y);
            objectmove.transform.localPosition = tmp;
        }
        LeanTween.moveLocal(objectmove, positon, 0.5f).setEase(LeanTweenType.easeInSine);
    }
    public void ShowHPMNSH(int valueShow, Vector3 posShow, Color colorShow)
    {
        GameObject objectShow = LeanPool.Spawn(txtShowHPMNSH, new Vector3(posShow.x, posShow.y + 1), Quaternion.identity);
        if (valueShow != 0)
        {
            objectShow.transform.GetComponent<TextMeshPro>().text = valueShow.ToString();
        }
        else
        {
            objectShow.transform.GetComponent<TextMeshPro>().text = string.Empty;
        }

        objectShow.transform.GetComponent<TextMeshPro>().color = colorShow;
        LeanTween.alpha(objectShow, 0, 0.01f).setOnComplete(() =>
        {
            LeanTween.moveLocalY(objectShow, objectShow.transform.localPosition.y + 0.5f, 0.5f).setEase(LeanTweenType.easeInBack);
            LeanTween.scale(objectShow, new Vector3(2, 2), 0.5f).setOnComplete(() =>
             {
                 LeanTween.scale(objectShow, new Vector3(0.2f, 0.2f), 0.5f);
             });
            LeanTween.alpha(objectShow, 1, 0.5f).setOnComplete(() =>
            {
                LeanTween.alpha(objectShow, 0, 0.5f).setOnComplete(() =>
                {
                    LeanPool.Despawn(objectShow);
                });
            });
        });
    }

}
