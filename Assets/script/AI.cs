using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {
		
	public int playerColor = Main.WHITE;
	public Main mainInstance;
	public int mAlgorithm = HILL_CLIMBING;
	const int HILL_CLIMBING = 1001;
	public GameObject chessSet;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (mainInstance.currentPlayer == playerColor) {
			mainInstance.getClickOnItemEvent(getBestPosition(mAlgorithm));
		}
	}

	GameObject getBestPosition(int algorithm)
	{	
		Vector2 location = new Vector2(0,0);
		switch (algorithm) {
		case HILL_CLIMBING:
			location = HillClimbing(playerColor);
			break;
		}

		return GameObject.Find ((location.x + 1) + "-" + (location.y + 1));

	}

	Vector2 HillClimbing(int player)
	{
		List<Vector2> locations = mainInstance.getAvailableLocation (mainInstance.currentPlayer);
		int[,] initBoard = (int[,]) mainInstance.mBoard.Clone();
		double max_value = -10000;
		Vector2 bestPosition = new Vector2(0,0);
		foreach (Vector2 location in locations)
		{
			int[,] newBoard = (int[,])initBoard.Clone();
			newBoard[(int)location.x, (int)location.y] = player;
			double value = mainInstance.heuristicEvaluate(newBoard);
			if (max_value < value)
			{
				bestPosition = location;
				max_value = value;
			}
		}
		return bestPosition;

	}
}
