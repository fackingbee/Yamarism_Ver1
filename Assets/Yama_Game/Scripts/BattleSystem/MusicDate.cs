 using UnityEngine;
using System.Collections;


// 音楽譜面データを格納しておく構造体
public class MusicDate  {

//	public float tick;
	public long tick;			//Tick
	public int  value;			//value(note_onなら音程でset_tempoならBPMを司る)
	public bool isCreated;		//すでに譜面を生成したかどうかを判別するフラグ
	public int  tapType;		//タップなのかフリックなどの特殊な操作なのか
	public long nextTick;		//次のスコアのティック

	// コンストラクタ
	public MusicDate (long tick, int value) {

		this.tick  =  tick;		// 値を格納
		this.value =  value;	// 値を格納
		this.isCreated = false;	// 初期化

		//タップのタイプを識別
		if (value == 84 || value == 85) {
			
			// 1はフリック
			// フリックの総数
			// ここまでの総スコアを集計
			this.tapType = 1;
			GameDate.totalFlicklScore++;
			GameDate.totalScoreNum++; 

		} else if (value == 96 || value == 98 || value == 100 || value == 102) {
			
			// 3はロングタップ・ダウン（同時に４レーンに出せるようにしておく）
			// ロングスコア総数
			// ここまでの総スコアを集計
			this.tapType = 3;				
			GameDate.totalLongScore++;
			GameDate.totalScoreNum++;

		} else if (value == 97 || value == 99 || value == 101 || value == 103) {
			
			// 4はロングタップ・アップ（同時に４レーンに出せるようにしておく）
			// ロングタップとペアなのでトータルにはカウントしない
			// ロングタップアップスコア総数
			this.tapType = 4;
			GameDate.totalLongUpScore++;

		} else if (value == 36) {
			
			// 5はPlayerTurnStartフラグ
			this.tapType = 5;

		} else if (value == 35) {
			
			// 6はPlayerTurnEndフラグ
			this.tapType = 6;

		} else {
			
			// それ以外は今のところ通常のスコアを使用
			// 2は通常のタップ
			// ここまでの総スコアを集計
			// 通常スコア総数
			this.tapType = 2;
			GameDate.totalScoreNum++;
			GameDate.totalNormalScore++;

		}
	}
}
