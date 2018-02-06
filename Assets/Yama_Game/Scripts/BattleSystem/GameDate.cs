 using UnityEngine;
using System.Collections;

// リズムゲーム時のパラメーターを格納しておく構造体
// Awakeで初期化したりするので、MonoBehaviourを継承しておく
public class GameDate : MonoBehaviour {
	
	#region ゲージ関連 : GageHandler.cs
	public static float	GagePoint;				// ゲージポイント : プレイヤー用
	public static float playerPowerGagePoint;	// playerPowerGageアニメーション用
	public static float playerSetGageValue;		// ゲーム開始時のプレイヤーゲージ値
	public static float playerFinalGageValue;	// ゲーム終了時のプレイヤーの最終的なゲージ値
	public static float enemyGagePoint;			// ゲージポイント : 敵用
	public static float enemyPowerGagePoint;	// enemyPowerGageアニメーション用
	public static float enemySetGageValue;		// ゲーム開始時の敵ゲージ値
	public static float enemyFinalGageValue;	// ゲーム終了時のエネミーの最終的なゲージ値
	#endregion
	#region キャラクターステータス関連 : CharacterSet.cs
	public static int   playerAttribute;		// プレイヤーの属性
	public static int   enemyAttribute;			// 敵の属性
	public static float playerAttack;			// プレイヤーの攻撃力
	public static float enemyAttack;			// エネミーの攻撃力
	public static float playerPerformance;      // プレイヤーゲージの増減基準値
	public static float enemyPerformance;		// エネミーゲージの増減基準値
	#endregion
	#region スコア関連 : ScoreCreator.cs、ScoreHandler.cs
	public static long  score;					// ポイントスコア
	public static int   perfectNum;				// perfec総数
	public static int   greatNum;				// great総数
	public static int   goodNum;				// good総数
	public static int   badNum;					// bad総数
	public static int   missNum;				// miss総数
	public static int   totalScoreNum;			// 総スコア数格納変数
	public static int   maxCombo     = 0;		// Combo用変数(最大コンボ数はトータルスコア数と同じ)
	public static int   combo        = 0;		// Combo用変数(実際のコンボ数)
	public static int   perfectCount = 0;		// Combo用変数(というより現在はプレイヤーターンの威力変数)
	public static int   totalNormalScore;		// 通常スコア総数
	public static int   totalFlicklScore;		// フリックスコア総数
	public static int   totalLongScore;			// ロングタップスコア総数
	public static int   totalLongUpScore;		// ロングタップアップスコア総数
	#endregion

	void Awake () {

		// バトル開始時に明示的に初期化
		score                = 0;
		GagePoint            = 0f;
		enemyGagePoint       = 0f;
		playerPowerGagePoint = 0f;
		totalScoreNum        = 0;
		perfectNum           = 0;
		greatNum             = 0;
		goodNum              = 0;
		badNum               = 0;
		missNum              = 0;
		maxCombo             = 0;
		combo	             = 0;
		perfectCount         = 0;

		// 各スコアは初期化すると使えなくなるので、ゲームが終了してから初期化するようにする
		//totalNormalScore     = 0;
		//totalFlicklScore     = 0;
		//totalLongScore       = 0;
		//totalLongUpScore     = 0;


		// 以下、デバッグ用
		// 暫定でプレイヤーと敵のパフォーマンス（験力）を設定
		playerPerformance = 103f;
		enemyPerformance  = 97f;

		// 暫定でプレイヤーの属性を"木"、敵の属性を"土"としておく
		playerAttribute = 0;
		enemyAttribute  = 2;

		// そろそろキャラクターステータスは構造体にする方がよくなってきた
		// 一旦暫定でキャラクターの攻撃力を設定しておく
		playerAttack = 30f;
		enemyAttack  = 30f;

		// 後にJSONファイルに項目を追加して、外部から値を引っ張ってくる可能性がある
		// 暫定でゲージの開始位置を設定(開始時の値は難易度や装備品で初期値が変わる可能性がある)
		//playerSetGageValue = 850.23f;
		//enemySetGageValue  = 500.06f;

		// ゲーム終了時のゲージ値をセット
		//playerFinalGageValue = playerSetGageValue;
		//enemyFinalGageValue  = enemySetGageValue;
	}

	// デバッグ用 : 評価計算の実験
	void Start () {
		// デバッグ用にスコア値を設定①
		//totalScoreNum = 100;
		//perfectNum    = 70;
		//greatNum      = 15;
		//goodNum       = 10;
		//badNum        = 0;
		//missNum       = 5;
		//maxCombo      = 100;
		//combo	      = 60;

		// デバッグ用にスコア値を設定②
		//totalScoreNum = 326;
		//perfectNum    = 284;
		//greatNum      = 37;
		//goodNum       = 0;
		//badNum        = 1;
		//missNum       = 4;
		//maxCombo      = 326;
		//combo	      = 175;

		// デバッグ用にスコア値を設定③
//		totalScoreNum = 326;
//		perfectNum    = 296;
//		greatNum      = 27;
//		goodNum       = 2;
//		badNum        = 0;
//		missNum       = 1;
//		maxCombo      = totalScoreNum;
//		combo	      = 210;
	}
}