using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {

	public Text   battlePointText;		// 戦闘点数を文字列として反映
	public Text   touchPointText;		// 音技点数を文字列として反映
	public Text   totalPointText;		// 総合点数を文字列として反映
	public Image  battleRankFill;		// 戦闘ランクをFillでアニメーション
	public Image  touchRankFill;		// 音技ランクをFillでアニメーション
	public Image  totalRankFill;		// 総合ランクをFillでアニメーション
	public float  resultTime;			// 評価開始後の時間管理
	public float  battlePoint;			// 戦闘評価(点数)
	public float  touchPoint;			// 音技評価(点数)
	public float  totalPoint;			// 総合評価(点数)
	public string battleRank;			// 戦闘評価(ランク)
	public string touchRank;			// 音技評価(ランク)
	public string totalRank;			// 総合評価(ランク)
	public bool   isBattlePointCheck;	// 戦闘アニメーション開始をUpdateで監視
	public bool   isTouchPointCheck;	// 音技アニメーション開始をUpdateで監視
	public bool   isTotalPointCheck; 	// 総合アニメーション開始をUpdateで監視
	public bool   isSettlementCheck;	// 勝敗カットインをUpdateで監視

	public GameObject winObject;

	void Awake () {
		resultTime = 0f;
	}

	void Start () {

		Debug.Log ("プレイヤーのゲージ : " + GameDate.playerFinalGageValue + "点");
		Debug.Log ("エネミーのゲージ　 : " + GameDate.enemyFinalGageValue  + "点");

		// 結果を計算し、評価を受け取る
		BattleCalculation();
		TouchCalculation ();
		TotalCalculation ();
		
	}

	#region	Update () : ゲーム終了時の時間管理
	void Update () {
		
		// timeを更新
		#if UNITY_EDITOR
		resultTime += Time.deltaTime;
		#else
		resultTime += Time.unscaledDeltaTime;
		#endif

		// 戦闘ランクを表示
		if(isBattlePointCheck && battleRankFill.fillAmount < 1){
			battleRankFill.fillAmount += 0.03f;
			if(battleRankFill.fillAmount == 1){
				SetTouchResultPoint();
				isBattlePointCheck = false;
			}
		}

		// 音技ランクを表示
		if(isTouchPointCheck && touchRankFill.fillAmount < 1){
			touchRankFill.fillAmount += 0.03f;
			if(touchRankFill.fillAmount == 1){
				SetTotalResultPoint();
				isTouchPointCheck = false;
			}
		}

		// 総合ランクを表示
		if(isTotalPointCheck && totalRankFill.fillAmount < 1){
			totalRankFill.fillAmount += 0.03f;
			if(totalRankFill.fillAmount == 1){
				isTotalPointCheck = false;
			}
		}

		// 評価が終了したら、勝利 or 敗北シンボルを表示
		if(!isSettlementCheck && battleRankFill.fillAmount == 1 && touchRankFill.fillAmount == 1 && totalRankFill.fillAmount == 1){
			Invoke ("OnSettleAnim",0.5f);
			isSettlementCheck = true;
		}

	}
	#endregion
	#region メソッド : 戦闘評価
	public void BattleCalculation () {

		#region 勝利
		// やはり点数は音ゲーらしく少し大きめの桁にしておく
		#endregion
		if (GameDate.playerFinalGageValue > GameDate.enemyFinalGageValue) {

			Debug.Log ("勝利");

			if(GameDate.playerFinalGageValue >= 900){
				battleRank = "S";
			} else if (GameDate.playerFinalGageValue >= 800){
				battleRank = "A";
			} else if (GameDate.playerFinalGageValue >= 700){
				battleRank = "B";
			}else if (GameDate.playerFinalGageValue >= 600){
				battleRank = "C";
			} else {
				battleRank = "D";
			}
			#region 敗北
			// 敗北時は強制的にRankはEにして、点数も0点にする
			#endregion
		} else {
			Debug.Log ("敗北");
			battleRank = "E";
			battlePoint = 0.0f;
		}

		// 実際に表示する値は音ゲーらしく大きめの単位にしておく
		battlePoint = GameDate.playerFinalGageValue * 10;

		Debug.Log ("戦闘点数 : " + battlePoint);
		Debug.Log ("戦闘点数・カンマ区切り : " + battlePoint.ToString("N0") + "点");
		Debug.Log ("戦闘評価 : " + battleRank);

		// 評価アニメーションはTransitionアニメーションが終了するまで待つ
		Invoke ("SetBattleResultPoint", 1.1f);
	}
	#endregion
	#region メソッド : タッチ評価の計算
	private void TouchCalculation(){

		#region GameDataから受理
		// 計算した結果だけあれば良いので、ローカル変数化
		// キャストが面倒臭いのでfloat型にしておく
		// coolは使わないかもしれないし、どっちにしろ係数は0なので除外
		// 係数は定数にしてグローバル化しておくと、後々の微調整で便利
		// ここでタッチ関連は計算して、結果を文字列として変数に格納してUIに表示する
		#endregion
		float totalScore = (float)GameDate.totalScoreNum;
		float maxCombo   = (float)GameDate.totalScoreNum;
		float perfect    = (float)GameDate.perfectNum;
		float great      = (float)GameDate.greatNum;
		float good       = (float)GameDate.goodNum;
		float miss       = (float)GameDate.missNum;
		float combo      = (float)GameDate.combo;

		// タッチ計算(点数 : ％表示)
		float touchCalculation =
			(
				(perfect * 5) + (great * 3) + (good * 1) - (miss * 5) + combo 
			)

			/ 

			(
				(totalScore * 5) + maxCombo
			) 

			* 100;


		// タッチ評価(Rank)
		if(touchCalculation >= 90.0f){
			touchRank = "S";
		} else if(touchCalculation >= 80.0f){
			touchRank = "A";
		} else if (touchCalculation >= 70.0f){
			touchRank = "B";
		} else if (touchCalculation >= 60.0f){
			touchRank = "C";
		} else if (touchCalculation >= 50.0f){
			touchRank = "D";
		} else {
			touchRank = "E";
		}


		// 実際に表示する値は音ゲーらしく大きめの単位にしておく
		touchPoint = touchCalculation * 100;

		#region Debug.Log
		Debug.Log ("総スコア数 : " + totalScore);
		Debug.Log ("最大コンボ : " + maxCombo);
		Debug.Log ("perfect数 : " + perfect);
		Debug.Log ("great数 : "   + great);
		Debug.Log ("good数 : "    + good);
		Debug.Log ("miss数 : "    + miss);
		Debug.Log ("コンボ数 : "   + combo);
		Debug.Log ("タッチ率(%) : " + touchCalculation + "%");
		Debug.Log ("タッチ点数 : " + touchPoint + "点");
		Debug.Log ("タッチ点数・カンマ区切り : " + touchPoint.ToString("N0") + "点");
		Debug.Log ("タッチ評価 : " + touchRank);
		#endregion

	}
	#endregion
	#region メソッド : 総合評価の計算
	private void TotalCalculation () {

		float totalScore = (float)GameDate.totalScoreNum;
		float maxCombo   = (float)GameDate.totalScoreNum;
		float perfect    = (float)GameDate.perfectNum;
		float great      = (float)GameDate.greatNum;
		float good       = (float)GameDate.goodNum;
		float miss       = (float)GameDate.missNum;
		float combo      = (float)GameDate.combo;

		// タッチの点数を先に計算
		float touchTotalScoreNum =
			(
				(perfect * 5) + (great * 3) + (good * 1) - (miss * 5) + combo 
			);

		// 総合評価(点数 : %表示)
		float totalCalculation = 
			(
				GameDate.playerFinalGageValue + touchTotalScoreNum
			) 

			/ 

			(
				1000f + ((totalScore * 5) + maxCombo)
			)

			* 100;


		if (totalCalculation >= 90.0f) {
			totalRank = "S";
		} else if (totalCalculation >= 80.0f) {
			totalRank = "A";
		} else if (totalCalculation >= 70.0f) {
			totalRank = "B";
		} else if (totalCalculation >= 60.0f) {
			totalRank = "C";
		} else if (totalCalculation >= 50.0f) {
			totalRank = "D";
		} else {
			totalRank = "E";
		}

		// 点数が総合評価だけでよい場合
		//totalPoint = totalCalculation * 100;

		// 別案(タッチ点数と戦闘点数の合計にコンボボーナスを不可)
		totalPoint = (battlePoint + touchPoint) * ( 1 + (combo / maxCombo) );

		#region Debug.Log
		Debug.Log(("(総合内)(combo / maxCombo) : " + combo / maxCombo));
		Debug.Log ("(総合内)戦闘点数 : " + battlePoint);
		Debug.Log ("(総合内)タッチ点数 : " + touchPoint);
		Debug.Log ("総合率(%) : " + totalCalculation + "%");
		Debug.Log ("総合点数(%) : " + totalPoint + "点");
		Debug.Log ("総合点数・カンマ区切り : " + totalPoint.ToString("N0") + "点");
		Debug.Log ("総合評価 : " + totalRank);
		#endregion
	}
	#endregion
	#region メソッド : 評価用ポイントセット
	public void SetBattleResultPoint(){

		// 戦闘、タッチ、総合評価で使い回す
		float setPoint    = battlePoint;
		int   setResultID = 1;

		// アニメーション停止
		//StopCoroutine("ResultPointAnim");

		// アニメーション開始
		StartCoroutine(
			ResultPointAnim(0, setPoint, 1f, setResultID)
		);
	}

	public void SetTouchResultPoint(){

		// 戦闘、タッチ、総合評価で使い回す
		float setPoint    = touchPoint;
		int   setResultID = 2;

		// アニメーション停止
		//StopCoroutine("ResultPointAnim");

		// アニメーション開始
		StartCoroutine(
			ResultPointAnim(0, setPoint, 1f, setResultID)
		);
	}

	public void SetTotalResultPoint(){

		// 戦闘、タッチ、総合評価で使い回す
		float setPoint    = totalPoint;
		int   setResultID = 3;

		// アニメーション停止
		//StopCoroutine("ResultPointAnim");

		// アニメーション開始
		StartCoroutine(
			ResultPointAnim(0, setPoint, 1f, setResultID)
		);
	}
	#endregion
	#region メソッド : 勝利 or 敗北時のシンボル
	private void OnSettleAnim(){
		if(battleRank != "E"){
			winObject.SetActive (true);
		} else {
			// 敗北

		}
	}
	#endregion
	#region コルーチン : 評価用ポイントアニメーション
	private IEnumerator ResultPointAnim(float startPoint, float endPoint, float time, int resultID){

		// アニメーション開始時間
		float startTime = resultTime;

		// アニメーション終了時間
		float endTime = startTime + time;

		// 1フレームごとに数値を上昇させる
		do{
			// アニメーション中の今の経過時間を計算
			float t = (resultTime - startTime) / time;

			// 数値を更新
			long updateValue = (long)( ((endPoint - startPoint) * t) + startPoint );

			if(resultID == 1){
				// テキストを更新
				battlePointText.text = updateValue.ToString("N0");
			} else if (resultID == 2) {
				touchPointText.text = updateValue.ToString("N0");
			} else if (resultID == 3) {
				totalPointText.text = updateValue.ToString("N0");
			}

			// 1フレーム待つ
			yield return null; 

		} while (resultTime < endTime);

		// 数値を最終値に合わせる
		if (resultID == 1) {
			battlePointText.text = endPoint.ToString ("N0");
			isBattlePointCheck = true;
		} else if (resultID == 2) {
			touchPointText.text = endPoint.ToString ("N0");
			isTouchPointCheck = true;
		} else if (resultID == 3) {
			totalPointText.text = endPoint.ToString ("N0");
			isTotalPointCheck = true;
		}
	}
	#endregion
}
