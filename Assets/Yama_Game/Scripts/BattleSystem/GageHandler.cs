using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GageHandler : MonoBehaviour {

	#region レベル変数（現在非アクティブ）
	//private Slider mySlider;				// デバッグ用ゲージ増減変数
	//private float  increaseLvPoint;		// レベル差によるゲージ増減の補正(Perfect~Good)
	//private float  decreaseLvPoint;		// レベル差によるゲージ増減の補正(Miss)
	//private int    kariPlayerLV;			// 味方レベル
	//private int    kariEnemyLV;			// 敵レベル
	#endregion

	#region Inspector上からアタッチ
	// 増加と減少の変化が目まぐるしいので処理を分ける
	// 増加は徐々に増やすアニメーションをコルーチンで表現
	// ダメージ時は一瞬で減らし、減少アニメーションはコピーが担い、Updateで表現
	#endregion
	public  Slider enemySlider;				// エネミーのゲージ増減変数
	public  Slider playerSlider;			// プレイヤーのゲージ増減変数
	public  Image  enemyGageOriginal;		// エネミーの実ゲージイメージ
	public  Image  enemyGageCopy;			// エネミーのダメージゲージイメージ
	public  Image  playerGageOriginal;		// プレイヤーの実ゲージイメージ
	public  Image  playerGageCopy;			// プレイヤーのダメージゲージイメージ

	// GameDataより受理
	private float playerStartGageValue;		// プレイヤーのスタートのゲージ値
	private float enemyStartGageValue;		// エネミーのスタートのゲージ値
	private float playerGagePoint;			// 実際のゲージ増加基準値
	private float enemyGagePoint;			// 実際のゲージ増加基準値
	private float basePerfectPoint;			// Perfect増加量

	// GageHandler用変数
	private bool  isPlayerDamaged;			// ダメージ管理フラグ
	private bool  isEnemyDamaged;			// ダメージ管理フラグ
	private bool  isEndValue;				// ゲーム終了時の値受け渡し管理フラグ
	private float damagedEnemyGageValue;	// ダメージ後のSliderValueを再格納する変数
	private float damagedPlayerGageValue;	// ダメージ後のSliderValueを再格納する変数


	void Start() {
		
		#region ゲーム開始時のGageValueの仕様
		// ゲーム開始時のValueが可変する可能性があるので、必ずGameDateを介す。
		// 『setGageValue = 500』のときは約1.55
		// 『setGageValue = 1  』のときは約0.003
		// つまり、setGageValueによって増減の基準値が変わっている
		// 今回レベルの概念がなくなったので、Gageの増減係数はconstにして、
		// 攻撃を受けた際はこちらのゲージが減少、攻撃を仕掛けた際は相手のゲージを減少させる仕様とする
		#endregion
		enemyStartGageValue  = GameDate.enemySetGageValue;
		playerStartGageValue = GameDate.playerSetGageValue;
		enemySlider.value    = enemyStartGageValue;
		playerSlider.value   = playerStartGageValue;
		isEndValue           = false;

		#region Perfect増加量の仕様 : 4.60f
		// スコア数に応じてPerfect増加量を決める
		// 分母は定数で、攻撃力や属性によって今後微調整していく
		//basePerfectPoint = (mySlider.value * 1.4f) / GameDate.totalScoreNum;
		#endregion
		basePerfectPoint     = 1500f / GameDate.totalScoreNum;

		#region 過去の仕様でLVを加味したもの。
		// まず味方と敵のレベルを暫定で設定する(Lv1~99の差の最大値は98)
		//kariPlayerLV = 8;
		//kariEnemyLV  = 5;

		// レベル差をStart時に算出
		//increaseLvPoint = 1 + (kariPlayerLV - kariEnemyLV) * 0.01f;
		//decreaseLvPoint = 1 - (kariPlayerLV - kariEnemyLV) * 0.01f;
		//Debug.Log ("increaseLvPoint : " + increaseLvPoint);
		//Debug.Log ("decreaseLvPoint : " + decreaseLvPoint);
		#endregion
		playerGagePoint      = GameDate.playerPerformance / 100f;
		enemyGagePoint       = GameDate.enemyPerformance  / 100f;

		#region Debug.Log
		//Debug.Log ("setGageValue : "      + setGageValue);
		//Debug.Log ("mySlider.maxValue : " + mySlider.maxValue);
		//Debug.Log ("mySlider.value : "    + mySlider.value);
		//Debug.Log ("スコア総数 : " 		  + GameDate.totalScoreNum);
		//Debug.Log ("Perfect増加量 : " + basePerfectPoint);
		//Debug.Log ("gagePoint : " + gagePoint);
		#endregion
		#region メモ
		// ここまででゲージの基本アニメーションは終了
		// 次にこれに基づいて敵と味方で処理を分ける
		// まず敵はタッチバーを通過したら自動的にゲージ加算される
		// タッチバーを取得しよう
		// ScoreCreatorですでに取得しているが、先に消されると敵に値が渡らない
		// スコアを生成した瞬間に先に敵のゲージを上昇させておく
		#endregion
	}


	// ダメージ時のゲージ減少アニメーションはUpdateで管理
	void Update(){

		// エネミー : ゲージのコピーを作って、オリジナルが増加してる最中も減少を表現
		if(enemyGageOriginal.fillAmount < enemyGageCopy.fillAmount){
			enemyGageCopy.fillAmount -= 0.001f;
			if(enemyGageOriginal.fillAmount >= enemyGageCopy.fillAmount){
				enemyGageCopy.fillAmount = 0f;
			}
		}

		// プレイヤー : ゲージのコピーを作って、オリジナルが増加してる最中も減少を表現
		if(playerGageOriginal.fillAmount < playerGageCopy.fillAmount){
			playerGageCopy.fillAmount -= 0.001f;
			if(playerGageOriginal.fillAmount >= playerGageCopy.fillAmount){
				playerGageCopy.fillAmount = 0f;
			}
		}

		// ゲームが終了時点での各SliderValueを一旦GamaDataに渡す
		if(!GameController.isPlaying && !isEndValue){

			GameDate.playerFinalGageValue = playerSlider.value;
			GameDate.enemyFinalGageValue  = enemySlider.value;

			// 一度渡したら進入禁止
			isEndValue = true;
		}
	}
	#region プレイヤーのセットゲージ(タッチ判定)
	// プレイヤーのゲージセット
	public void setGage(int evaluation, int attribute){

		float point      = 0;	// 評価によるポイント計算用
		float finalPoint = 0;	// そのポイントに属性ボーナスを付加した最終値
	
		#region 先にポイントを計算
//		if (evaluation == 0){
//			point =  (5.0f * playerGagePoint) * basePerfectPoint * 0.14f; //Debug.Log ("Perfect : " + point);
//		}else if(evaluation == 1){
//			point =  (3.0f * playerGagePoint) * basePerfectPoint * 0.14f; //Debug.Log ("Great : "   + point);
//		}else if(evaluation == 2){
//			point =  (1.5f * playerGagePoint) * basePerfectPoint * 0.14f; //Debug.Log ("Good : "    + point);
//		}else if(evaluation == 3){
//			point =  (0.5f * playerGagePoint) * basePerfectPoint * 0.14f; //Debug.Log ("Cool : "    + point);
//		}else if(evaluation == 4){
//			point = -(5.0f * playerGagePoint) * basePerfectPoint * 0.14f; //Debug.Log ("Miss : "    + point);
//		}
		#endregion
		switch(evaluation){

			// Perfect
			case 0:
				point =  (5.0f * playerGagePoint) * basePerfectPoint * 0.14f;
				break;

			// Great
			case 1:
				point =  (3.0f * playerGagePoint) * basePerfectPoint * 0.14f;	
				break;
		
			//Good
			case 2:
				point =  (1.5f * playerGagePoint) * basePerfectPoint * 0.14f;
				break;

			// Cool
			case 3:
				point =  (0.5f * playerGagePoint) * basePerfectPoint * 0.14f;
				break;

			#region Miss
		    //減少させない仕様にするかどうか迷うところ
			#endregion
			case 4:
				point = -(5.0f * playerGagePoint) * basePerfectPoint * 0.14f;
				//point = -10f;
				break;
		}

		// 一旦最終値に格納しておき、下記の条件式を通らなければ(miss時)、そのままのポイントをゲージ渡す
		finalPoint = point;

		#region メモ : 敵及びプレイヤーとスコアの属性の一致性
		// 味方及び敵の属性とスコアの属性が一致していたら
		// 少しだけパフォーマンス（ゲージ上昇率）を上げる
		// 必要なものは、スコアの属性とキャラクターの属性
		// それらを条件式で分岐して、乗算もしくわ除算
		#endregion

		// タッチ成功時 : Miss時は減らさない仕様にするかどうかは検討中
		//if(evaluation != 4){

			#region 属性ボーナス値(+5%)
			// GameDataからプレイヤーの属性を受け取って
			// スコア属性とプレイヤー属性が一致していたら評価してボーナス値を算出
			// 現状試験的に5%アップとしておく
			#endregion
			if (attribute == GameDate.playerAttribute){
				finalPoint = point * 1.05f;	
			}

			// 一旦GameDataへ格納する(理由は最終的な値を最後の勝敗判定で使うから)
			GameDate.GagePoint += finalPoint;

			// アニメーション停止
			//StopCoroutine( "GageAnimation" );

			// アニメーション開始
			StartCoroutine(
				GageAnimation(GameDate.GagePoint,0.2f)
			);
		//}
	}
	#endregion
	#region 敵のゲージセット(タッチ判定)
	public void setGage_Enemy(int attribute){

		float point      = 0;
		float finalPoint = 0;

		point            = (5.0f * enemyGagePoint) * basePerfectPoint * 0.14f;
		finalPoint       = point;

		// スコアと属性が違えばそのまま加算して、一致すればボーナス値を加算
		if(attribute == GameDate.enemyAttribute){
			finalPoint = point * 1.05f;	
		}

		// 一旦GameDataへ格納する
		GameDate.enemyGagePoint += finalPoint;

		// アニメーション停止
		//StopCoroutine( "GageAnimation_Enemy" );

		// アニメーション開始
		StartCoroutine(
			GageAnimation_Enemy(
				GameDate.enemyGagePoint,
				0.2f
			)
		);
	}
	#endregion
	#region プレイヤーのセットゲージ（ダメージ時）
	public void setGageDamage(float damage){
		
		float point      = 0;	// 評価によるポイント計算用
		float finalPoint = 0;	// そのポイントに属性ボーナスを付加した最終値

		point = damage;

		#region pointとfinalPointの間でボーナス値等の処理を挟む為の交換のアルゴリズ
		#endregion

		finalPoint                = point;
		playerGageCopy.fillAmount = playerGageOriginal.fillAmount;
		playerSlider.value       -= finalPoint;
		damagedPlayerGageValue    = playerSlider.value;
		isPlayerDamaged           = true;
	}
	#endregion
	#region エネミーのセットゲージ（ダメージ時）
	public void setGageDamageEnemy(float damage){
		
		float point      = 0;	// 評価によるポイント計算用
		float finalPoint = 0;	// そのポイントに属性ボーナスを付加した最終値

		point = damage;

		#region pointとfinalPointの間でボーナス値等の処理を挟む為の交換のアルゴリズ
		#endregion

		finalPoint               = point;
		enemyGageCopy.fillAmount = enemyGageOriginal.fillAmount;
		enemySlider.value       -= finalPoint;
		damagedEnemyGageValue    = enemySlider.value;
		isEnemyDamaged           = true;
	}
	#endregion
	#region コルーチン : プレイヤーのゲージアニメーション
	private IEnumerator GageAnimation (float point, float time) {

		float startTime  = TimeManager.time;	// アニメーション開始時間
		float endTime    = startTime + time;	// アニメーション終了時間
		float startValue = playerSlider.value;	// アニメーション開始時のゲージ

		#region  受けたダメージを元にゲージ開始値を再設定
		// これがないとタイミングによってはダメージを受けてもゲージが減らない
		// 理由はstartValueが何らかの理由で元のSlider.Valueで上書きされる
		// 一度進入したら、次にダメージを受けるまで呼ばれなくて良い
		#endregion
		if(isPlayerDamaged){
			startValue      = damagedPlayerGageValue;
			isPlayerDamaged = false;
		}

		// 1フレームごとに数値を上昇させる
		while (TimeManager.time < endTime){

			// アニメーション中の今の経過時間を計算
			float t = (TimeManager.time - startTime) / time;

			// 数値を更新
			playerSlider.value = startValue + (point * t);

			// 1フレーム待つ
			yield return null;

			// もし1秒経ってもループしてたら強制的にブレイク
			if (TimeManager.time > startTime + 2.0f) {
				break;
			}

			// 上限に達してもしばらくコルーチンが動きMissをしてもvalueが減らない現象を防ぐ為
			if( playerSlider.value >= 1000f && point * t != 0 ){

				#region 数値を最終値に合わせる
				// tはフレームによって誤差が出るので、ムラなくきちんと目的の値に着地するために
				// 最終値を再度設定してあげる
				// 要するに上記の処理はアニメーションなのであって
				// 目的は出た値をゲージに反映するというこt
				#endregion
				playerSlider.value = startValue + point;

				#region GameDate.GagePointを初期化
				// SliderValueが1000以上のときは強制的にブレイクするので
				// ここでもGameDate.GagePointを0に初期化する必要がある
				#endregion
				GameDate.GagePoint = 0;

				#region「point * t != 0」が必要な理由
				// 一度1000に達すると必ず「yield break」してしまうが、
				// コルーチン開始時は「point * t = 0」なので
				// point * t != 0」とすることで進入を回避出来るし、
				// 次のフレームでは「point * t > 0」だが、
				// 今度は「mySlider.value < 1000」なのでやはり進入を回避出来る。
				#endregion
				yield break;
			}
		} 

		#region 数値を最終値に合わせる
		// tはフレームによって誤差が出るので、ムラなくきちんと目的の値に着地するために
		// 最終値を再度設定してあげる
		// 要するに上記の処理はアニメーションなのであって
		// 目的は出た値をゲージに反映するというこt
		#endregion
		playerSlider.value = startValue + point;

		// コルーチンが終わったら初期化する(SliderValueが1000未満のとき)
		GameDate.GagePoint = 0;
	}
	#endregion
	#region コルーチン : 敵のゲージアニメーション
	private IEnumerator GageAnimation_Enemy (float point, float time) {

		float startTime  = TimeManager.time;	// アニメーション開始時間
		float endTime    = startTime + time;	// アニメーション終了時間
		float startValue = enemySlider.value;	// アニメーション開始時のゲージ

		#region  受けたダメージを元にゲージ開始値を再設定
		// これがないとタイミングによってはダメージを受けてもゲージが減らない
		// 理由はstartValueが何らかの理由で元のSlider.Valueで上書きされる
		// 一度進入したら、次にダメージを受けるまで呼ばれなくて良い
		#endregion
		if(isEnemyDamaged){
			startValue     = damagedEnemyGageValue;	
			isEnemyDamaged = false;	
		}

		// 1フレームごとに数値を上昇させる
		while (TimeManager.time < endTime){

			// アニメーション中の今の経過時間を計算
			float t = (TimeManager.time - startTime) / time;

			// 数値を更新
			enemySlider.value = startValue + (point * t);

			// 1フレーム待つ
			yield return null;

			// もし1秒経ってもループしてたら強制的にブレイク
			if (TimeManager.time > startTime + 2.0f) {
				break;
			}

			// 上限に達してもしばらくコルーチンが動きMissをしてもvalueが減らない現象を防ぐ為
			if( enemySlider.value >= 1000f && point * t != 0 ){
				
				#region 数値を最終値に合わせる
				// tはフレームによって誤差が出るので、ムラなくきちんと目的の値に着地するために
				// 最終値を再度設定してあげる
				// 要するに上記の処理はアニメーションなのであって
				// 目的は出た値をゲージに反映するというこt
				#endregion
				enemySlider.value = startValue + point;

				#region GameDate.GagePointを初期化
				// SliderValueが1000以上のときは強制的にブレイクするので
				// ここでもGameDate.GagePointを0に初期化する必要がある
				#endregion
				GameDate.enemyGagePoint = 0;

				#region「point * t != 0」が必要な理由
				// 一度1000に達すると必ず「yield break」してしまうが、
				// コルーチン開始時は「point * t = 0」なので
				// point * t != 0」とすることで進入を回避出来るし、
				// 次のフレームでは「point * t > 0」だが、
				// 今度は「mySlider.value < 1000」なのでやはり進入を回避出来る。
				#endregion
				yield break;
			}
		} 

		#region 数値を最終値に合わせる
		// tはフレームによって誤差が出るので、ムラなくきちんと目的の値に着地するために
		// 最終値を再度設定してあげる
		// 要するに上記の処理はアニメーションなのであって
		// 目的は出た値をゲージに反映するというこt
		#endregion
		enemySlider.value = startValue + point;

		// コルーチンが終わったら初期化する
		GameDate.enemyGagePoint = 0;
	}
	#endregion
}
	