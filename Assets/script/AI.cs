using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {
		
	public int playerColor = Main.WHITE;
	public Main mainInstance;
	public int mAlgorithm;
	const int HILL_CLIMBING = 1001;
    const int BFS = 1002;
	public GameObject chessSet;

	// Use this for initialization
	void Start () {
		mAlgorithm = BFS;
	}
	
	// Update is called once per frame
	void Update () {
		if (mainInstance.currentPlayer == playerColor) {
			mainInstance.getClickOnItemEvent(getBestPosition(mAlgorithm));
		}
	}

	// AI 下棋 -> 決定要下的位置
	GameObject getBestPosition(int algorithm)
	{	
		Vector2 location = new Vector2(0,0);
		switch (algorithm) {
			case HILL_CLIMBING:
				location = HillClimbing(playerColor);
				break;
			case BFS:
				location = BestFirstSearch(playerColor);
				break;
		}

		return GameObject.Find ((location.x + 1) + "-" + (location.y + 1));

	}

	// HillClimbing 方法
	Vector2 HillClimbing(int player)
	{
		Debug.Log ("HillClimbing's turn");
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

	// Best First Search 方法
    Vector2 BestFirstSearch(int player)
    {
		Debug.Log ("BFS's turn");
        List<Vector2> locations = mainInstance.getAvailableLocation(mainInstance.currentPlayer);
        int[,] initBoard = (int[,])mainInstance.mBoard.Clone();
        int max_value = -10000;
        List<Vector2> bestPosition = new List<Vector2>();
        List<int> value = new List<int>();

        foreach (Vector2 location in locations)
        {
            int[,] newBoard = (int[,])initBoard.Clone();
            newBoard[(int)location.x, (int)location.y] = player;
            int tempMax = (int)mainInstance.heuristicEvaluate(newBoard);
            value.Add(tempMax);
            max_value = Mathf.Max(tempMax, max_value);
        }

        foreach (int val in value)
        {
            if (val == max_value)
            {
                bestPosition.Add(locations[value.IndexOf(val)]);
                value[value.IndexOf(val)] = -10000;
            }
        }

        return bestPosition[Random.Range(0, bestPosition.Count - 1)];
    }
}
