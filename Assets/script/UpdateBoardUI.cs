using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UpdateBoardUI : MonoBehaviour {
	public Sprite Black, White, Hint;
	public GameObject chessSet;
	private SpriteRenderer[] chesses;
	public bool onChange;
	public Main mainInstance;

	// Use this for initialization
	void Start () {
		chesses = chessSet.GetComponentsInChildren<SpriteRenderer> ();
		onChange = true;
		if (mainInstance == null) {
			Debug.LogError("Main instance should be locate:" + this.gameObject.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (onChange) {
			foreach (SpriteRenderer chess in chesses) {
				string[] tem = chess.gameObject.name.Split ('-');
				int x = Convert.ToInt32 (tem[0]);
				int y = Convert.ToInt32 (tem[1]);
				switch (mainInstance.getBoardState (x - 1, y - 1)) {
				case Main.BLACK:
					chess.sprite = Black;
					break;
				case Main.WHITE:
					chess.sprite = White;
					break;
				default:
					chess.sprite = null;
					break;
				}
			}
			onChange = false;

			List<Vector2> locations = mainInstance.getAvailableLocation (mainInstance.currentPlayer);
			foreach (Vector2 position in locations) {
				GameObject chess = GameObject.Find((position.x + 1) + "-" + (position.y + 1));
				chess.GetComponent<SpriteRenderer>().sprite = Hint;
			}

		}
	}
}
