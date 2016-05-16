using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class GameAi {
	int[,] heuristicValue = { 	{ 4, -3, 2, 2, 2, 2, -3, 4 },
		{ -3, -4, -1, -1, -1, -1, -4, -3 },
		{ 2, -1, 1, 0, 0, 1, -1, 2 },
		{ 2, -1, 0, 1, 1, 0, -2, 1 },
		{ 2, -1, 0, 1, 1, 0, -2, 1 },
		{ 2, -1, 1, 0, 0, 1, -1, 2 },
		{ -3, -4, -1, -1, -1, -1, -4, -3 },
		{ 4	, -3, 2, 2, 2, 2, -3, 4 }};
	int [,] direction = { { -1, 1 } , { 0, 1 } , { 1, 1 } , { -1, 0 },   // 棋子上下左右斜方所有方位位移量的集合
		{ 1, 0 } , { -1, -1 } , { 0, -1 } , { 1, -1 } };
	const int WHITE = -1;
	const int BLACK = 1;



	public Main mainInstance;
    static int maxlevel = 1; //預測層數 ***level
    int curLevel; //現在層數
    int curColor; //現在顏色
    int stepCounts; //可走的步數
    int[][] curChessboard; //現在棋盤
    List<Data> possibleStep = new List<Data>(); //所有可走的步


    /*
        1.判斷下棋者(黑/白)
        2.搜尋所有可走的步
        3.for所有可走的步{
        4.如果為底層return weight
        5.如果不為底層recursive搜尋 (傳走過上一步的chessboard)}
    */


    public GameAi(int[][] board, int color)
    {
        curLevel = 0;
        curChessboard = board;
        curColor = color;
    }

    public GameAi(int level, int[][] board, int color)
    {
        /*
        Console.WriteLine(level);
        for (int i = 0; i < 8; i++) { 
            for (int j = 0; j < 8; j++) { 
                if(board[i][j]==-1)
                    Console.Write("X ");
                else if(board[i][j] == 1)
                    Console.Write("●");
                else if(board[i][j]==2)
                    Console.Write("__");
                else
                    Console.Write("○");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        */


        curLevel = level;
        curChessboard = board;
        curColor = color;
    }
        
    //get best move step
    public int[] bestMove()
    {
		double bestWeight = -10000;
        if (curLevel == maxlevel)
            return new int[] { -1, -1};
        findPossibleStepAI();
		if (stepCounts == 0)
			return new int[] { -1, -1 };
        List<int[]> bestStep = new List<int[]>();
        bestStep.Add(new int[]{ -1, -1});
        foreach (Data data in possibleStep)
        {
            //往下找 新的AI為curlevel+1 , 棋盤下一步 , 顏色變換
            double subWeight = new GameAi(curLevel + 1, chessboardAfterStep(data.get(), copyBoard(curChessboard)), -curColor).weight();
            //Console.WriteLine("curLevel : "+curLevel + " row : " + data.getRow() + " column :  " + data.getColumn() + " weight : " + subWeight);
			Debug.Log("weight "+subWeight);
			if (subWeight >= bestWeight)    //maximize (Random select)
            {
				if (subWeight > bestWeight)
                    bestStep.Clear();
                int[] temp = new int[2];
                temp[0] = data.getRow();
                temp[1] = data.getColumn();
                bestStep.Add(temp);
            }
        }
        System.Random random = new System.Random();
        return bestStep[random.Next(bestStep.Count)];
    }
    //recursive find best step weight (called by bestMove) 
    public double weight()
    {
        if (curLevel == maxlevel)
            return countChess();
        findPossibleStepAI();
        if(stepCounts == 0)
            return countChess();
        double bestWeight = -10000;
        double worstWeight = 10000;
        foreach(Data data in possibleStep)
        {
            //往下找 新的AI為curlevel+1 , 棋盤下一步 , 顏色變換
            double subWeight = new GameAi(curLevel + 1, chessboardAfterStep(data.get(), copyBoard(curChessboard)), -curColor).weight();
            //Console.WriteLine("curLevel : " + curLevel + " row : " + data.getRow() + " column :  " + data.getColumn() + " weight : " + subWeight);
            //maximize
            if (subWeight > bestWeight)
                bestWeight = subWeight;
            //minimize
            if (subWeight < worstWeight)
                worstWeight = subWeight;
        }
        //如果為偶數(自己下棋的回合則maximize，對手則minimize)
        if (curLevel % 2 == 0)  //maximize
            return bestWeight;
        else                    //minimize
            return worstWeight;
    }
    //count black and white chess
    private double countChess()
	{
		int adversary  , current_count = 0, adver_count = 0, temp_x , temp_y ,
		cur_front_count = 0, adv_front_count = 0 , static_value = 0, parity_value , front_value , mobility_value ;
		double h_value;

		adversary = -curColor;

		// 計算黑白棋各佔的數目 , 所佔位置的值 , 周圍是否有空格的棋的數量
		for (int i = 0; i < 8; i++) {

			for (int j = 0; j < 8; j++) {

				if (curChessboard [i] [j] == curColor) {
					static_value += heuristicValue [i, j];
					current_count++;
				} else if (curChessboard [i] [j] == adversary) {
					static_value -= heuristicValue [i, j];
					adver_count++;
				}
				if (curChessboard [i] [j] != 0) {
					for (int k = 0; k < 8; k++) {

						temp_x = i + direction [k, 0];
						temp_y = i + direction [k, 1];
						if (temp_x > -1 && temp_x < 8 && temp_y > -1 && temp_y < 8 && curChessboard[temp_x] [temp_y] == 0) {
							if (curChessboard [i] [ j] == curColor) {
								cur_front_count++;
							} else {
								adv_front_count++;
							}
							break;
						}
					}
				}
			}
		}

		// 所佔個數 heuristic 值
		if (current_count > adver_count) {
			parity_value = (100 * current_count) / (current_count + adver_count);
		} else if (current_count < adver_count) {
			parity_value = -(100 * adver_count) / (current_count + adver_count);
		} else {
			parity_value = 0;
		}
		// 周圍是否有空格的 heuristic 值
		if (cur_front_count > adv_front_count) {
			front_value = -(100 * cur_front_count) / (cur_front_count + adv_front_count);
		} else if (cur_front_count < adv_front_count) {
			front_value = (100 * adv_front_count) / (cur_front_count + adv_front_count);
		} else {
			front_value = 0;
		}

		// 可下位置數目的 heuristic 值
		current_count = getAvailableLocation (curColor).Count;
		adver_count = getAvailableLocation (adversary).Count;
		Debug.Log("Count "+current_count+" "+adver_count);
		if (current_count > adver_count) {
			mobility_value = (100 * current_count) / (cur_front_count + adver_count);
		} else if (current_count < adv_front_count) {
			mobility_value = - (100 * adver_count) / (current_count + adver_count);
		} else {
			mobility_value = 0;
		}

		// 加權
		h_value = (10.0 * parity_value) + (74.396 * front_value) + (20 * static_value) + (78.922 * mobility_value);

		return h_value;
    }

	// 輪到該 player 執棋時 -> 找出所有可放棋的位置
	public List<Vector2> getAvailableLocation(int player) {

		int target , ad_target , temp_x , temp_y;    // target : player的棋子顏色 , ad_target : 對手的棋子顏色
		int[] temp;
		bool unfound = true;
		Vector2 temp_v;
		List<Vector2> result = new List<Vector2>();  // 可放棋子的位置集合

		if (player == WHITE) {   // 設定判斷依據 ( 目標棋子顏色 )
			target = WHITE;
			ad_target = BLACK;
		} else {
			target = BLACK;
			ad_target = WHITE;
		}

		for (int i = 0; i < 8; i++) {  // 掃過整個盤面，尋找 player 所有的棋子

			for (int j = 0; j < 8; j++) {
				if (curChessboard [i][ j] == target){   // 若找到的話，檢查其周遭的所有位置 -> 共8格

					for (int k = 0; k < 8; k++) {
						temp_x = i + direction [k, 0];    
						temp_y = j + direction [k, 1];
						// 若在棋盤範圍內且為對方顏色棋子 -> 繼續延伸尋找 ( refound )
						if (temp_x > -1 && temp_x < 8 && temp_y > -1 && temp_y < 8) {
							if ((curChessboard [temp_x][ temp_y] == ad_target)) {
								temp = refound (temp_x, temp_y, direction [k, 0], direction [k, 1], ad_target);  // 尋找可放棋位置
							} else {
								temp = null;
							}
							// 若有找到可下位置
							if (temp != null) {
								unfound = false;
								temp_v = new Vector2 (temp [0], temp [1]);
								if (!result.Contains (temp_v)) {
									result.Add (temp_v);
								}
							}
						}
					}
				}	
			}
		}
		// 若找不到可下位置 -> 讓子 ( 可下任一空白處 )
		if(unfound) {
			for (int i = 0; i < 8; i++) {

				for (int j = 0; j < 8; j++) {

					if (curChessboard [i][ j] == 0) {
						result.Add (new Vector2 (i, j));
					}
				}
			}
		}

		return result;
	}

	// 以棋子為起點向該方位 ( movex , movey) 延伸去找尋可放置棋子的位子 
	int [] refound(int center_x , int center_y , int move_x , int move_y , int ad_target) {

		int[] place = new int[2];

		if (center_x > -1 && center_x < 8 && center_y > -1 && center_y < 8) {  // 若在 8X8 邊界內

			if (curChessboard [center_x][ center_y] == ad_target) { // 和對方棋子相鄰 -> 繼續往相同方向檢查 -> 直到碰到邊界或空格

				place = refound (center_x + move_x, center_y + move_y, move_x, move_y, ad_target);	

			} else if (curChessboard [center_x][ center_y] == 0) {  // 位置為空的 -> 可以放棋

				place [0] = center_x;
				place [1] = center_y;
			} else {
				place = null;
			}
		} else {
			place = null;	// 沒有位置可放 -> 碰到邊界或位置全部已佔滿	
		}

		return place;
	}


	private int[,] changeBoard(){
		int[,] newBoard = new int[8, 8];
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				newBoard [i, j] = curChessboard [i] [j];
			}
		}
		return newBoard;
	}

    //get chessboard after move chess
    private int[][] chessboardAfterStep(int[] step,int[][] newBoard)
    {
        int row = step[0];
        int col = step[1];
        newBoard[row][col] = curColor;
        newBoard = change(newBoard ,row, col, -1, -1);
        newBoard = change(newBoard, row, col, -1,  0);
        newBoard = change(newBoard, row, col, -1,  1);
        newBoard = change(newBoard, row, col,  0, -1);
        newBoard = change(newBoard, row, col,  0,  1);
        newBoard = change(newBoard, row, col,  1, -1);
        newBoard = change(newBoard, row, col,  1,  0);
        newBoard = change(newBoard, row, col,  1,  1);
        clearPossiblePosition(newBoard);
        return newBoard;
    }
    //recursive change the chessboard after move chess (called by chessboardAfterStep)
    private int[][] change(int[][] board, int ori_row, int ori_col, int offset_row, int offset_col)
    {
        int row = ori_row + offset_row;
        int col = ori_col + offset_col;
        int count = 1;
        while (row >= 0 && row < 8 && col >= 0 && col < 8)
        {
            if (board[row][col] == 0)
                break;
            if (board[row][col] == curColor)
            {
                for (int i = 1; i < count; i++)
                {
                    board[ori_row + offset_row * i][ori_col + offset_col * i] = curColor;
                }
                break;
            }
            row += offset_row;
            col += offset_col;
            count++;
        }
        return board;
    }
    //
    private void clearPossiblePosition(int[][] chessboard)
    {
        for (int i = 0; i < chessboard.Length; i++)
        {
            for (int j = 0; j < chessboard[0].Length; j++)
            {
                //判斷是否為可下棋子
                if (chessboard[i][j] == 2)
                {
                    chessboard[i][j] = 0;
                }
            }
        }
    }

    //find all possible steps ***原為搜尋黑棋 可改為搜尋空
    private void findPossibleStepAI()
    {
        possibleStep = new List<Data>();
        stepCounts = 0;
        for (int i = 0; i < curChessboard.Length; i++)
        {
            for (int j = 0; j < curChessboard[0].Length; j++)
            {
                //判斷是否為黑色旗子
                if (curChessboard[i][j] == curColor)
                {
                    //如果是則找周圍8個方位可下位置
                    checkAI(i, j, -1, -1);
                    checkAI(i, j, -1, 0);
                    checkAI(i, j, -1, 1);
                    checkAI(i, j, 0, -1);
                    checkAI(i, j, 0, 1);
                    checkAI(i, j, 1, -1);
                    checkAI(i, j, 1, 0);
                    checkAI(i, j, 1, 1);
                }
            }
        } 
    }
    //recursive find all possible steps
    private int checkAI(int ori_row, int ori_col, int offset_row, int offset_col)
    {

        int row = ori_row + offset_row;
        int col = ori_col + offset_col;
        //超出棋盤
        if (row < 0 || row > 7 || col < 0 || col > 7)
            return 0;
        //為可下的棋子
        if (curChessboard[row][col] == 2)
            return 0;
        //為自己旗子
        if (curChessboard[row][col] == curColor)
            return 0;
        //為空
        if (curChessboard[row][col] == 0)
            return 1;
        //鄰近點為對手旗子 (如果為1代表在recursive中遇到空，所以為可下的棋)
        if(checkAI(row, col, offset_row, offset_col) == 1)
        {
            curChessboard[row + offset_row][col + offset_col] = 2;
            stepCounts++;
            Data data = new Data(row + offset_row, col + offset_col);
            possibleStep.Add(data);
        }
        return 2;
    }
    
    public static int[][] copyBoard(int[][] board)
    {
        int[][] newboard = new int[board.Length][];
        for(int i = 0; i < board.Length; i++)
        {
            newboard[i] = new int[board[0].Length];
            for(int j = 0; j < board[0].Length; j++)
            {
                newboard[i][j] = board[i][j];
            }
        }
        return newboard;
    }

    public static void setLevel(int level)
    {
        maxlevel = level;
    }

    public static int getLevel()
    {
        return maxlevel;
    }
}
