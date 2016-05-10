using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	public const int BLACK = 2, WHITE = 1;
	public int currentPlayer;
	private int[,] mBoard;
	private int[,] heuristicValue;
	public UpdateBoardUI mUpdateInstance;
	public GUIText finishText;

	private List<Vector2> place_location = new List<Vector2>();
	int [,] direction = { { -1, 1 } , { 0, 1 } , { 1, 1 } , { -1, 0 },   // 棋子上下左右斜方所有方位位移量的集合
		{ 1, 0 } , { -1, -1 } , { 0, -1 } , { 1, -1 } };


	void Start()
	{	
		if(mUpdateInstance == null)
		{
			Debug.LogError("UpdateBoard instance should be locate.");
		}
		mBoard = new int[8, 8];
		heuristicValue = new int[8, 8];
		mBoard [3, 3] = WHITE;
		mBoard [4, 4] = WHITE;
		mBoard [3, 4] = BLACK;
		mBoard [4, 3] = BLACK;
		currentPlayer = BLACK;
		finishText.enabled = false;
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
		boardUpdate (x - 1, y - 1);
		mUpdateInstance.onChange = true;
		if (isFinish ()) {
			finishText.enabled = true;
			finishText.text = "XXXXX win~~~";
		}
		currentPlayer = (currentPlayer == BLACK) ? WHITE : BLACK;
	}

	// 輪到該 player 執棋時 -> 找出所有可放棋的位置
	public List<Vector2> getAvailableLocation(int player) {

		int target , ad_target , temp_x , temp_y;    // target : player的棋子顏色 , ad_target : 對手的棋子顏色
		int[] temp;
		bool unfound = true;
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
						if (mBoard [temp_x, temp_y] == ad_target) {
							temp = refound (temp_x, temp_y, direction [k, 0], direction [k, 1], ad_target);  // 尋找可放棋位置
						} else {
							temp = null;
						}
						if(temp != null) {
							unfound = false;
							result.Add(new Vector2(temp[0],temp[1]));
						}
					}
				}	
			}
		}

		if(unfound) {
			return null;
		} else {
			return result;
		}
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
		} else {
			adversary = WHITE;
		}

		mBoard [pos_x, pos_y] = currentPlayer;
		for(int i = 0; i < 8; i++) {

			temp_x = pos_x + direction [i, 0];  
			temp_y = pos_y + direction [i, 1];
			renew (temp_x, temp_y, direction [i, 0], direction [i, 1] , currentPlayer ,adversary); 																
		}
	}

	int renew (int base_x , int base_y , int dir_x , int dir_y , int p , int ad) { // 變色機制

		if (base_x > -1 && base_x < 8 && base_y > -1 && base_y < 8) {  // 若在 8X8 邊界內

			if (mBoard [base_x, base_y] == ad) { // 和對方棋子相鄰 -> 繼續往相同方向檢查 -> 直到碰到邊界或空格

				mBoard [base_x, base_y] = renew (base_x + dir_x, base_y + dir_y, dir_x, dir_y , p , ad);	
			} else if (mBoard [base_x, base_y] == p) { // 若碰到自己顏色的棋子 -> 有包圍 -> 上一個變成白色

				return p;

			} else {
				return ad;
			}

			return mBoard[base_x, base_y];
		}
		return -1;
	}

	bool isFinish() {  // 判斷遊戲結束

		bool end = true;

		for (int i = 0; i < 8; i++) {

			for (int j = 0; j < 8; j++) {

				if (mBoard [i, j] == 0) {
					end = false;
					break;
				}
			}
			if (!end) {
				break;
			}
		}

		return end;
	}

}