using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#region 注意！ : 『勝利』のカットインと使い回す
// 名前がややこしくなるのは我慢する
// enemyの方と若干書き方が違うが特に意味はない
// 昔はあまりわかっていなかったので手探りでやってしまっただけ
// 統合してもいいが、動くし面倒なのでそのまま使う
#endregion

public class CutInFillAmount : MonoBehaviour {

	public  Image    enemyImage; 			  	 // Imageコンポーネント取得用
	public  Animator playerCutInStart;		  	 // PlayerSpriteのAnimatorをON・OFF
	private bool     isPlayerCutInSpriteON;   	 // カットイン時のPlayerSpriteアニメーションONフラグ
	private bool     isPlayerCutInSpriteOFF;  	 // カットイン時のPlayerSpriteアニメーションOFFフラグ
	private bool	 inFadeIn;				  	 // 特にこうする必要もなかったが、昔の名残でそのまま使う

	[System.NonSerialized] public  bool fadeIn;	 // 右からフェードイン
	[System.NonSerialized] public  bool fadeOut; // 右へフェードアウト

	void Start () {
		enemyImage.fillAmount  = 0f;			// 開始時は何も映ってなくていいので0で初期化
		enemyImage.fillOrigin  = 0;				// 初期値はLeft
		fadeIn                 = false;
		fadeOut                = false;
		inFadeIn               = false;
		isPlayerCutInSpriteON  = true;
		isPlayerCutInSpriteOFF = false;
	}
		
	void Update () {

		// ScoreCreaterからFadeInフラグをONにする
		if(fadeIn){

			fadeIn = false;
		
			// このスクリプトがオンになってから、1秒後にFadeIn()を実行
			Invoke("FadeIn", 0f);

		}
			
		// フェードイン中だけ通る
		if (inFadeIn) {
			// enemyImage.fillAmountが1を超えるまで毎フレーム加算、超えたらフラグをOFFる
			if (enemyImage.fillAmount < 1 && enemyImage.fillOrigin == 0) {
				enemyImage.fillAmount += 0.05f;
			} else {
				inFadeIn = false;
			}
		}
			
		// 目的の数値（max＝1）に達したら、fillOrigin＝1（right）にする
		if(enemyImage.fillAmount >= 1){
			enemyImage.fillOrigin    = 1;		// この時点でFadeInが呼ばれても、fillOrifginが0じゃないのでインクリメントが実行されない。
			playerCutInStart.enabled = true;	// fillOrigin＝1になったら、アニメーションさせるスプライトのAnimatorをオンにする
			isPlayerCutInSpriteON    = true;	// カットインアニメーションへの遷移を許可

			// カットインアニメーションを遷移させる
			playerCutInStart.SetBool ("isPlayerCutInSpriteON", isPlayerCutInSpriteON);

			// 帯を描写しきったら、もうFadeIn関数にはいらないようにする
			//fadeIn = false;

			fadeOut = true; // FadeOutにはいれるようにする

		}

		if(fadeOut && enemyImage.fillOrigin == 1 && enemyImage.fillAmount > 0){
			// 3秒後にFadeOut、デクリメントし始める（その間にスプライトのアニメーションを終わらせる）
			Invoke ("FadeOut",3.1f);

		}
	}
	#region メソッド : フェードイン(このメソッドでは、フェードイン中フラグをONにするだけ)
	public void FadeIn() {
		inFadeIn = true;
	}
	#endregion
	#region メソッド : フェードアウト(FillAmountをデクリメント/この関数が呼ばれても、fillOrigin＝1じゃなければデクリメントしない)
	public void FadeOut(){

		// アイドリング状態への遷移を許可
		isPlayerCutInSpriteOFF = true;
		playerCutInStart.SetBool ("isPlayerCutInSpriteOFF", isPlayerCutInSpriteOFF);

		// 一度スプライトがカットインしたらもうアニメーションへ遷移しないようにする
		isPlayerCutInSpriteON = false;
		playerCutInStart.SetBool ("isPlayerCutInSpriteON", isPlayerCutInSpriteON);

		//Debug.Log ("フェードアウト");
		if(enemyImage.fillAmount > 0 && enemyImage.fillOrigin == 1){
			enemyImage.fillAmount -= 0.037f;
		}

		if(enemyImage.fillAmount <= 0){
			enemyImage.fillOrigin = 0;
		}
	}
	#endregion
}
