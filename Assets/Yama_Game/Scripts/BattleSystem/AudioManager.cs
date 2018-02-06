using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#region メモ : Invokeを多用した理由
// 使いまわしたAnimationの発生タイミングを変更するのが面倒くさいので
// タイミング調整の為に、なるだけ関数化しておいて
// Invokeで微調整していく
#endregion
#region メモ : カットイン演出をここに置いた理由
// カットインのタイミングをサウンドと測る為
// 演出系は値や勝敗判定とは独立出来ると思う
#endregion

public class AudioManager : MonoBehaviour {
	
	public GameObject  GameClear; 		// GameClearアニメーション(現在未使用で追加があれば使用する)
	public GameObject  GameOver;		// GameOverアニメーション (現在未使用で追加があれば使用する)
	public AudioClip   onMiss;			// Miss時効果音 
	public AudioClip   onTouch;			// タッチが成功時効果音
	public AudioClip   onGameClear;		// ゲームクリア時効果音    
	public AudioClip   onGameOver;		// ゲームオーバー時効果音  
	public AudioSource audioSource; 	// オーディオ格納変数
	public bool        isTouch;     	// ScoreHandlerでフラグを管理
	public bool        isMiss;			// ScoreHandlerでフラグを管理


	void Start () {
		audioSource = GetComponent<AudioSource> ();
		#region 不要になったがメモ程度に一応残しておく
//		// オーディオを再生（GameControllerで管理するようになってからは必要なくなる）
//		gameObject.GetComponent<AudioSource>().PlayDelayed(1.7f);

//		// デバッグ用、再生位置を移動(曲の途中から始められるデバックで便利だが、スコアとは同期しない)
//		// 終わったらスコアは100％にする。
//		GameDate.GagePoint = 100;
//		gameObject.GetComponent<AudioSource>().time = gameObject.GetComponent<AudioSource>().clip.length -1f;
		#endregion
	}


	void Update () {

		#region 曲終了の合図としてこういう書き方もあるよってことで残しておく
//		if(!gameObject.GetComponent<AudioSource>().isPlaying){
//			Debug.Log ("終了");
//		}
		#endregion

		#region タッチ成功SE
		// 構文：public void PlayOneShot(AudioClip clip, float volumeScale = 1.0F);
		#endregion
		if(isTouch){
			audioSource.PlayOneShot(onTouch, 1f);
			isTouch = false;
		}
		#region タッチ失敗SE
		#endregion
		if(isMiss){
			audioSource.PlayOneShot(onMiss, 0.6f);
			isMiss = false;
		}

		#region ゲームの終了判定（音楽がいつ終了したかをチェック）
		// ゲームの終了判定（音楽がいつ終了したかをチェック）
		// オーディオが止まった時にゲーム終了
		// isPlayingはオーディオが再生されているか、いないか。
		//if(!gameObject.GetComponent<AudioSource>().isPlaying){
		// 今までは曲が終わったらisPlayingはfalseと同義だが、GameControllerで一時停止等を手動で管理
		#endregion
		if (GameController.isPlaying) {
			// 抜ける
			return;
		} else {
			if (GameDate.playerFinalGageValue > GameDate.enemyFinalGageValue) {
				Invoke ("OnGameClearSound", 1.3f);
			} else {
				Invoke ("OnGameOverSound",  1.3f);

				// 仮でゲームが終了したらHomeへシーン移動（後で削除する）
				//Invoke ("HomeScene", 5.0f);

			}
			#region  自身を停止してループを防ぐ
			// 自身を停止してループを防ぐ / Updateに記載しているので、終了してもゲーム（フレーム）が進むごとに上記が実行されてしまう 。
			// Gameごとfalseにしてしまうと、UIが消えるので今回はスクリプトを非アクティブにする。
			#endregion
			enabled = false;
		}
	}

	#region メソッド : ゲームクリア効果音
	private void OnGameClearSound(){

		// 効果音を鳴らす（GameClear）
		// 構文：public void PlayOneShot(AudioClip clip, float volumeScale = 1.0F);
		gameObject.GetComponent<AudioSource>().PlayOneShot(onGameClear, 1f);

		// GameClearアニメーションを表示
		//GameClear.SetActive (true);
	}
	#endregion
	#region メソッド : ゲームオーバー効果音
	private void OnGameOverSound(){

		// 効果音を鳴らす（GameOver）
		gameObject.GetComponent<AudioSource>().PlayOneShot(onGameOver, 0.7f);

		// ゲームオーバー時のカットイン
		//GameOver.SetActive(true);
	}
	#endregion
	#region メソッド : シーン移動

	// 仮でゲームが終了したらHomeへシーン移動（後で削除する）
	//public void HomeScene(){
	//	SceneManager.LoadScene ("GUISystem");
	//}

	#endregion


}