using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {
		
	public int playerColor = Main.WHITE;
	public Main mainInstance;
	public enum mAlgorithm {HillClimbing, BFS, GameAlgorithm};
	public mAlgorithm currentAlgorithm;

	// Use this for initialization
	void Start () {
		currentAlgorithm = mAlgorithm.GameAlgorithm;
	}
	
	// Update is called once per frame
	void Update () {
		if (mainInstance.currentPlayer == playerColor)
		{
			if (mainInstance.getAvailableLocation (playerColor) != null) 
			{
				mainInstance.getClickOnItemEvent (getBestPosition (currentAlgorithm));
			}
		}
	}

	// AI 下棋 -> 決定要下的位置
	GameObject getBestPosition(mAlgorithm algorithm)
	{	
		Vector2 location = new Vector2(0,0);
		switch (algorithm) {
			case mAlgorithm.HillClimbing:
				location = HillClimbing(playerColor);
				break;
			case mAlgorithm.BFS:
				location = BestFirstSearch(playerColor);
				break;
			case mAlgorithm.GameAlgorithm:
                location = GameAlgorithm(playerColor);
                break;
        }
		Debug.Log ("AI's move :" + (location.x+1) + "/" + (location.y+1));
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
			//newBoard[(int)location.x, (int)location.y] = player;
			boardUpdate((int)location.x , (int)location.y , newBoard , player);
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
            //newBoard[(int)location.x, (int)location.y] = player;
			boardUpdate((int)location.x , (int)location.y , newBoard , player);
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

        return bestPosition[UnityEngine.Random.Range(0, bestPosition.Count - 1)];
    }

    /*Vector2 GeneticAlgorithm(int player) 
	{
		List<Vector2> locations = mainInstance.getAvailableLocation(mainInstance.currentPlayer);
		int[,] initBoard = (int[,])mainInstance.mBoard.Clone();
		Vector2 bestPosition = new Vector2(0,0);
		List<int[]> fitness_values = new List<int[]>();

		foreach (Vector2 location in locations) 
		{
			int[,] newBoard = (int[,])initBoard.Clone();
			newBoard[(int)location.x, (int)location.y] = player;
			boardUpdate((int)location.x , (int)location.y , newBoard , player);
			fitness_values.Add (fitnessEvaluation (newBoard));
		}

		return bestPosition;
	}

	int [] fitnessEvaluation(int [,] tempBoard) 
	{
		int[] fitness = new int[14];
		int temp_f = Convert.ToInt32 (mainInstance.heuristicEvaluate (tempBoard));

		while (temp_f != 1) {
			fitness[count] = 
		}

		return fitness;
	}*/

    Vector2 GameAlgorithm(int player)
    {
		Debug.Log ("GA's turn");
        int[,] initBoard = (int[,])mainInstance.mBoard.Clone();
        int color = (player == 2) ? 1 : -1;
        GameAi ga = new GameAi(d2ArrayTo2d1Array(initBoard), color);
        int[] step = ga.bestMove();
        Vector2 bestPosition = new Vector2(step[0], step[1]);
        return bestPosition;
    }

    int[][] d2ArrayTo2d1Array(int[,] oriBoard)
    {
        int[][] newBoard = new int[8][];
        for (int i = 0; i < 8; i++)
        {
            newBoard[i] = new int[8];
            for (int j = 0; j < 8; j++)
            {
                switch (oriBoard[i, j])
                {
                    case 2:
                        newBoard[i][j] = 1;
                        break;
                    case 1:
                        newBoard[i][j] = -1;
                        break;
                    case 0:
                        newBoard[i][j] = 0;
                        break;
                }
            }
        }
        return newBoard;
    }

    void boardUpdate(int pos_x , int pos_y , int [,] temp_board , int p) {  // 更新盤面

		int temp_x , temp_y , adversary;

		if(p == Main.WHITE) {
			adversary = Main.BLACK;
		} else {
			adversary = Main.WHITE;
		}

		temp_board [pos_x, pos_y] = p;
		for(int i = 0; i < 8; i++) {

			temp_x = pos_x + mainInstance.direction [i, 0]; 
			temp_y = pos_y + mainInstance.direction [i, 1];
			renew (temp_x, temp_y, mainInstance.direction [i, 0], mainInstance.direction [i, 1], p , adversary ,temp_board); 		
		}
	}

	int renew (int base_x , int base_y , int dir_x , int dir_y , int p , int ad , int [,] tb) { // 變色機制

		if (base_x > -1 && base_x < 8 && base_y > -1 && base_y < 8) {  // 若在 8X8 邊界內

			if (tb[base_x, base_y] == ad) { // 和對方棋子相鄰 -> 繼續往相同方向檢查 -> 直到碰到邊界或空格

				tb [base_x, base_y] = renew (base_x + dir_x, base_y + dir_y, dir_x, dir_y, p, ad , tb);

			} else if (tb[base_x, base_y] == p) { // 若碰到自己顏色的棋子 -> 有包圍 -> 上一個變成白色

				return p;

			} else {
				return ad;
			}

			return tb [base_x, base_y];
		} else {
			return ad;
		}
	}

}
