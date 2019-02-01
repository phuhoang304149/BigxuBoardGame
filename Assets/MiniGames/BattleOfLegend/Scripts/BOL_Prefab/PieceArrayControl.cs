using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceArrayControl : MonoBehaviour {
	public GameObject Pieces;
	public List<Sprite> listSprite;
	public List<Sprite> listSpriteHero;
	public SpriteRenderer piece1;
	public SpriteRenderer piece2;
	public SpriteRenderer piece3;
	[SerializeField]
	int _heroID;
	public int heroID {
		get {
			return _heroID;
		}
		set {
			_heroID = value;
		}
	}
	[SerializeField]
	int _piece1Value;
	int _piece2Value;
	int _piece3Value;

	public int piece1Value {
		get {
			return _piece1Value;
		}
		set {
			_piece1Value = value;
			if (_piece1Value == 2) {
				//piece1.sprite = listSpriteHero[_heroID];
                piece1.sprite = listSprite[_piece1Value];
			} else if (_piece1Value == 0) {
				piece1.sprite = null;
			} else {
				piece1.sprite = listSprite[_piece1Value];
			}
		}
	}
	public int piece2Value {
		get {
			return _piece2Value;
		}
		set {
			_piece2Value = value;
			if (_piece2Value == 2) {
				//piece2.sprite = listSpriteHero[_heroID];
                piece2.sprite = listSprite[_piece2Value];
			} else if (_piece1Value == 0) {
				piece2.sprite = null;
			} else {
				piece2.sprite = listSprite[_piece2Value];
			}
		}
	}
	public int piece3Value {
		get {
			return _piece3Value;
		}
		set {
			_piece3Value = value;
			if (_piece3Value == 2) {
				//piece3.sprite = listSpriteHero[_heroID];
                piece3.sprite = listSprite[_piece3Value];
			} else if (_piece1Value == 0) {
				piece3.sprite = null;
			} else {
				piece3.sprite = listSprite[_piece3Value];
			}
		}
	}

	// Use this for initialization
	void Start() {
		piece1.sprite = null;
		piece2.sprite = null;
		piece3.sprite = null;
	}
}
