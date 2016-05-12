using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
		
	public int playerColor = Main.WHITE;
	public Main mainInstance;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (mainInstance.currentPlayer == playerColor) {

		}
	}

	GameObject getBestPosition(int algorithm)
	{
		return null;
	}
}
