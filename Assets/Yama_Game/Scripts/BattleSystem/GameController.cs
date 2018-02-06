using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class GameController : MonoBehaviour {

	#region メモ : 停止スクリプトをここで一括管理しない理由
	// キャラ類のPowerGage系の停止が必須だが、
	// インスタンス化するまでInspectorに存在せず、Findしなければならないので、
	// 各スクリプトのUpdateに停止フラグを付けるようにした
	public static bool isPlaying = true;	// AudioManagerのif文管理用
	public static bool isPausing = false;	// 一時停止変数で
	public static bool isResult  = false;	// 勝敗後の評価画面に移行する合図(Lightを真ん中で止める)
	public        bool isStopped = false;
	#endregion

	#region GameController用変数
	[Header("Animator Setting")]
	public  Animator    cameraRotate;		// ゲームスタート時のカメラ回転
	public  Animator    pathRotate1;		// ゲームスタート時のレーン回転①
	public  Animator    pathRotate2;		// ゲームスタート時のレーン回転②
	public  Animator    pathRotate3;		// ゲームスタート時のレーン回転③
	public  Animator    pathRotate4;		// ゲームスタート時のレーン回転④
	[Header("Object Setting")]
	public  GameObject  pauseObj;			// 停止ボタン(Inspector上からアタッチ)
	public  GameObject  unPauseObj;			// 再生ボタン(Inspector上からアタッチ)
	public  GameObject  settleCutIn; 		// 決着カットイン
	public  GameObject  resultTransition;	// 評価画面遷移変数
	public  GameObject  results;			// 評価用オブジェクト
	private AudioSource gameAudio;			// オーディオソース変数
	[Header("Audio Setting")]
	public  float       audioLength;		// 楽曲の長さを秒数で格納
	public  float       delayTime;			// 曲の始まるタイミングを管理する変数
	[Header("Result Setting")]
	public ResultManager battleReult;		// 決着後、評価計算へ
	#endregion

	#region Awake() : ゲームを始める前の準備
	void Awake(){
		Application.targetFrameRate = 60;	// フレームレートは60に設定
		System.GC.Collect ();            	// Battle開始時に一旦メモリ解放
		Resources.UnloadUnusedAssets (); 	// 及び、使用していないアセットのアンロード
	}
	#endregion

	IEnumerator Start () {
		gameAudio   = GetComponent<AudioSource> (); // 取得
		while (gameAudio.clip == null)
			yield return null;
		unPauseObj.SetActive (false);				// 最初に再生ボタンの方は非表示にしておく
		audioLength = gameAudio.clip.length;		// 曲の長さを取得(曲が終わったらisPlayingをfalseにしたい)
		isPlaying   = true;							// AudioManagerのif内に入らないように(Audioが止まるとGameOverになるので、一時停止中もisPlayingはTrueにしておく)
		isPausing   = false;						// ゲーム開始時は一時停止ではない
		isResult    = false;
		isStopped   = false;

		Invoke ("GameStart",     8.0f);
		Invoke ("CameraRotateOn",3.6f);
		Invoke ("PathRotate1",   4.8f);
		Invoke ("PathRotate2",   5.4f);
		Invoke ("PathRotate3",   5.7f);
		Invoke ("PathRotate4",   6.0f);

		// デバッグ用(即座に終了)
		//Invoke("GameStop", 0.5f);

	}

	#region	メソッド : ゲーム開始時の演出アニメーション
	public void CameraRotateOn(){ cameraRotate.enabled = true; }
	public void PathRotate1()   { pathRotate1.enabled  = true; }
	public void PathRotate2()   { pathRotate2.enabled  = true; }
	public void PathRotate3()   { pathRotate3.enabled  = true; }
	public void PathRotate4()   { pathRotate4.enabled  = true; }
	#endregion

	void Update () {

		#region ポーズ・再開関連
		if(!isPausing && Input.GetKeyDown("q")){
			Pause ();
		} else if (isPausing && Input.GetKeyDown("q")){
			UnPause ();
		}
			
		// デバッグ用（曲の途中まで飛ばす）
//		if(Input.GetKeyDown("s")){
//			TimeManager.time += 20.0f;
//			gameAudio.time   += 20.0f;
//		}
		#endregion

		#region kii関連
		// デバッグ用（強制的にゲーム終了）
		if(Input.GetKeyDown("z")){

			// 強制的に経験値を100加算
			variableManage.currentExp += 100;

			// 強制的にゲームクリア
			GameDate.GagePoint = 512;	

			// 強制終了
			GameStop ();

			variableManage.levelUp ();

			// データを保存するかどうか
			bool svChk = KiiManage.saveKiiData ();

			Debug.Log ("variableManage.currentExp : " + variableManage.currentExp);
			Debug.Log ("variableManage.nextExp : " + variableManage.nextExp);
			Debug.Log ("svChk : " + svChk);

			// データを保存してシーン移動
			if(!svChk){
				Debug.Log ("データを保存します");
				svChk = KiiManage.saveKiiData ();
			}

			Invoke ("DebugBattleEnd",1f);

		}
		#endregion

//		// TimeManager.timeが曲の長さを超えたら止める
//		// +1.7fはGameOver等を遅らせて表示させる為
//		if(isPlaying && TimeManager.time >= audioLength + 1.7f){
//			GameStop ();
//		}


		// 少し早めに決着カットイン : ここにOnResultTransitionを書くと何個も生成されちゃうので他に移行しなければいけません
		if(isPlaying && !this.isStopped && TimeManager.time >= audioLength + delayTime - 1f){
			settleCutIn.SetActive (true);

			//一度Trannsitionを発生させたらもういらない
			this.isStopped = true;

			//Invoke ("OnResultTransition", 0.5f);
		}


		// ここを1.7fにすると、曲数分書き換えないといけないので変数化 (Music_01_Manager.csで時間管理)
		if(isPlaying && TimeManager.time >= audioLength + delayTime){
			gameAudio.Stop ();  // 楽曲を停止
			GameStop ();		// ゲーム終了判定
		}
	}
		
	public void GameStart(){

		// Inspector上では、TimeManagerスクリプトをOFFにしておく
		GetComponent<TimeManager> ().enabled = true;

		// 曲始まりのオフセット（スクリプトを分けて引数は変数化(Music_01_Manager.csで時間管理）
		// gameAudio.PlayDelayed (1.7f); と同義
		gameAudio.PlayDelayed (delayTime); 

	}


	// 曲が終わったら、isPlayingをfalseにするメソッド
	public void GameStop(){ 

		isPlaying = false;	// ゲームを止める
		isResult  = true;	// 評価画面移行を許可

		// デバッグ用
		Instantiate(settleCutIn);
		Invoke ("OnResultTransition", 0.5f);

		// 少し遅らせないと、ゲージのバリューが取れないのでInvokeする
		Invoke ("GameResult" , 5.5f);

//		results.SetActive (true);
	}

	private void OnResultTransition(){
		GameObject resultBack = Instantiate (resultTransition);
		resultBack.transform.SetAsLastSibling ();
	}
		
	public void GameResult(){

		results.SetActive (true);

		//Debug.Log ("味方の最終ゲージ : " + GameDate.playerFinalGageValue);
		//Debug.Log ("敵の最終ゲージ : "   + GameDate.enemyFinalGageValue );

		// 敗北 or 勝利
		if (GameDate.playerFinalGageValue <= GameDate.enemyFinalGageValue) {
			//Debug.Log ("戦闘評価 : Ｅ");
			//Debug.Log ("敗北");
		} else {
			//Debug.Log ("勝利");
		}



	}
		
	// デバッグ終了（KiiCloudの確認の為、経験値を与えて強制終了）
	public void DebugBattleEnd(){
		SceneManager.LoadScene("KiiStart");
		Debug.Log ("KiiStartへ遷移しました");

	}
	#region 一時停止メソッド
	public void Pause(){

		// 一瞬重なったりするので順番に気をつける
		pauseObj.SetActive   (false);
		unPauseObj.SetActive (true);

		// 再生されているかどうか
		isPausing = true;

		// Update自体を一時停止
		Time.timeScale = 0f;

		// と同時にオーディオを一時停止
		gameAudio.Pause ();
	}
	#endregion
	#region 再開メソッド
	public void UnPause(){

		// 一瞬重なったりするので順番に気をつける
		unPauseObj.SetActive (false);
		pauseObj.SetActive   (true);

		// 再生されているかどうか（されていない）
		isPausing = false;

		// Updateを再開
		Time.timeScale = 1.0f;

		// と同時に一時停止解除
		gameAudio.UnPause();
	}
	#endregion
}
