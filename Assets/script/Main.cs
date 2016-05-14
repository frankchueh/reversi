using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	public const int BLACK = 2, WHITE = 1;
	public int currentPlayer , BLACK_NUM , WHITE_NUM;
	public int[,] mBoard;
	private int[,] heuristicValue = { 	{ 4, -3, 2, 2, 2, 2, -3, 4 },
									  	{ -3, -4, -1, -1, -1, -1, -4, -3 },
									  	{ 2, -1, 1, 0, 0, 1, -1, 2 },
										{ 2, -1, 0, 1, 1, 0, -2, 1 },
									  	{ 2, -1, 0, 1, 1, 0, -2, 1 },
									  	{ 2, -1, 1, 0, 0, 1, -1, 2 },
										{ -3, -4, -1, -1, -1, -1, -4, -3 },
										{ 4	, -3, 2, 2, 2, 2, -3, 4 }};
	public UpdateBoardUI mUpdateInstance;
	public GUIText finishText;

	int [,] direction = { { -1, 1 } , { 0, 1 } , { 1, 1 } , { -1, 0 },   // 棋子上下左右斜方所有方位位移量的集合
		{ 1, 0 } , { -1, -1 } , { 0, -1 } , { 1, -1 } };


	void Start()  // 遊戲初始化
	{	
		if(mUpdateInstance == null)
		{
			Debug.LogError("UpdateBoard instance should be locate.");
		}
		// 設置棋盤初始盤面 ( 2黑2白 )，並設定棋盤大小 ( 8X8 ) 及 heuristic 表
		mBoard = new int[8, 8];
		mBoard [3, 3] = WHITE;
		mBoard [4, 4] = WHITE;
		mBoard [3, 4] = BLACK;
		mBoard [4, 3] = BLACK;
		currentPlayer = BLACK;  // 黑棋先執

	}

	public void setBoardChess(int x, int y)
	{
		mBoard[x, y] = currentPlayer;
	}

	public int getBoardState(int x, int y)
	{
		return mBoard [x, y];
	}
		
	public void getClickOnItemEvent(GameObject item)
	{
		string[] position = item.name.Split('-');
		int x = Convert.ToInt32(position [0]);
		int y = Convert.ToInt32(position [1]);
		List<Vector2> available_positions = getAvailableLocation (currentPlayer);
		foreach (Vector2 pos in available_positions)
			if (Vector2.Distance (pos, new Vector2 (x - 1, y - 1)) == 0) {
				boardUpdate (x - 1, y - 1);
				mUpdateInstance.onChange = true;
				if (isFinish ()) {
					if(BLACK_NUM > WHITE_NUM)
					{
						finishText.text = "Black Win!";
					}
					else if(BLACK_NUM < WHITE_NUM)
					{
						finishText.text = "White Win!";
					}
					else if(BLACK_NUM == WHITE_NUM ){
						finishText.text = "Tie";	
					}
				}
				currentPlayer = (currentPlayer == BLACK) ? WHITE : BLACK;
			}
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
				if (mBoard [i, j] == target){   // 若找到的話，檢查其周遭的所有位置 -> 共8格

					for (int k = 0; k < 8; k++) {
						temp_x = i + direction [k, 0];    
						temp_y = j + direction [k, 1];
						// 若在棋盤範圍內且為對方顏色棋子 -> 繼續延伸尋找 ( refound )
						if (temp_x > -1 && temp_x < 8 && temp_y > -1 && temp_y < 8) {
							if ((mBoard [temp_x, temp_y] == ad_target)) {
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

					if (mBoard [i, j] == 0) {
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

			if (mBoard [center_x, center_y] == ad_target) { // 和對方棋子相鄰 -> 繼續往相同方向檢查 -> 直到碰到邊界或空格

				place = refound (center_x + move_x, center_y + move_y, move_x, move_y, ad_target);	

			} else if (mBoard [center_x, center_y] == 0) {  // 位置為空的 -> 可以放棋

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

	void boardUpdate(int pos_x , int pos_y) {  // 更新盤面

		int temp_x , temp_y , adversary;

		if(currentPlayer == WHITE) {
			adversary = BLACK;
			WHITE_NUM++;
		} else {
			adversary = WHITE;
			BLACK_NUM++;
		}

		mBoard [pos_x, pos_y] = currentPlayer;
		for(int i = 0; i < 8; i++) {

			temp_x = pos_x + direction [i, 0];  
			temp_y = pos_y + direction [i, 1];
			renew (temp_x, temp_y, direction [i, 0], direction [i, 1], currentPlayer, adversary); 		
		}
	}

	int renew (int base_x , int base_y , int dir_x , int dir_y , int p , int ad) { // 變色機制

		if (base_x > -1 && base_x < 8 && base_y > -1 && base_y < 8) {  // 若在 8X8 邊界內

			if (mBoard [base_x, base_y] == ad) { // 和對方棋子相鄰 -> 繼續往相同方向檢查 -> 直到碰到邊界或空格

				mBoard [base_x, base_y] = renew (base_x + dir_x, base_y + dir_y, dir_x, dir_y, p, ad);

			} else if (mBoard [base_x, base_y] == p) { // 若碰到自己顏色的棋子 -> 有包圍 -> 上一個變成白色
				
				return p;

			} else {
				return ad;
			}

			return mBoard [base_x, base_y];
		} else {
			return ad;
		}
	}

	bool isFinish() {  // 判斷遊戲結束

		bool end = true;
		int bn = 0, wn = 0;

		for (int i = 0; i < 8; i++) {

			for (int j = 0; j < 8; j++) {

				if (mBoard [i, j] == 0) {
					end = false;
					break;
				} else if (mBoard [i, j] == BLACK) {
					bn++;
				} else if (mBoard [i, j] == WHITE) {
					wn++;
				}
			}
			if (!end) {
				break;
			}
		}
		BLACK_NUM = bn;
		WHITE_NUM = wn;
		return end;
	}

	// 計算盤面的 heuristicEvaluate

	public double heuristicEvaluate(int [,] PB) {

		int adversary  , current_count = 0, adver_count = 0, temp_x , temp_y ,
		cur_front_count = 0, adv_front_count = 0 , static_value = 0, parity_value , front_value , mobility_value ;
		double h_value;

		if(currentPlayer == WHITE) {
			adversary = BLACK;
		} else {
			adversary = WHITE;
		}

		// 計算黑白棋各佔的數目 , 所佔位置的值 , 周圍是否有空格的棋的數量

		for (int i = 0; i < 8; i++) {
			
			for (int j = 0; j < 8; j++) {
				
				if (PB [i, j] == currentPlayer) {
					static_value += heuristicValue [i, j];
					current_count++;
				} else if (PB [i, j] == adversary) {
					static_value -= heuristicValue [i, j];
					adver_count++;
				}

				if (PB [i, j] != 0) {
					for (int k = 0; k < 8; k++) {
						
						temp_x = i + direction [k, 0];
						temp_y = i + direction [k, 1];
						if (temp_x > -1 && temp_x < 8 && temp_y > -1 && temp_y < 8 && PB[temp_x,temp_y] == 0) {
							if (PB [i, j] == currentPlayer) {
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

		current_count = getAvailableLocation (currentPlayer).Count;
		adver_count = getAvailableLocation (adversary).Count;

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

}