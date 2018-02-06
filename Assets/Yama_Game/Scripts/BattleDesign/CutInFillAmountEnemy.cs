using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#region 注意！ : 『敗北』のカットインと使い回す
// 名前がややこしくなるのは我慢する
#endregion

public class CutInFillAmountEnemy : MonoBehaviour {

	public  Image    enemyImage; 			    // (Inspector)Imageコンポーネント取得用
	public  Animator enemyCutInStart;		    // (Inspector)PlayerSpriteのAnimatorをON・OFF
	private bool     isEnemyCutInSpriteON;      // カットイン時のPlayerSpriteアニメーションONフラグ
	private bool     isEnemyCutInSpriteOFF;     // カットイン時のPlayerSpriteアニメーションOFFフラグ

	[System.NonSerialized] public bool fadeIn;	// 左からフェードイン
	[System.NonSerialized] public bool fadeOut;	// 左へフェードアウト

	void Start () {
		enemyImage.fillAmount = 0f;				 // 開始時は何も映ってなくていいので0で初期化
		enemyImage.fillOrigin = 1;				 // 初期値はRight
		fadeIn                = true;
		fadeOut               = false;
		isEnemyCutInSpriteON  = true;
		isEnemyCutInSpriteOFF = false;
	}

	void Update () {

		// このスクリプトがオンになってから、2秒後にFadeIn
		if(fadeIn){
			Invoke ("FadeIn",0f);	
		}
			
		// 目的の数値（max＝1）に達したら、fillOrigin＝1（right）にする
		if(enemyImage.fillAmount >= 1){
			
			enemyImage.fillOrigin   = 0;		// この時点でFadeInが呼ばれても、fillOrifginが0じゃないのでインクリメントが実行されない。
			enemyCutInStart.enabled = true;		// fillOrigin＝1になったら、アニメーションさせるスプライトのAnimatorをオンにする
			isEnemyCutInSpriteON    = true;		// カットインアニメーションへの遷移を許可

			// カットインアニメーションを遷移させる
			enemyCutInStart.SetBool ("isEnemyCutInSpriteON", isEnemyCutInSpriteON);

			fadeIn  = false;					// 帯を描写しきったら、もうFadeIn関数にはいらないようにする
			fadeOut = true;						// FadeOutにはいれるようにする

		}
			
		if(fadeOut && enemyImage.fillOrigin == 0 && enemyImage.fillAmount > 0){
			Invoke ("FadeOut",3.1f); // 3秒後にFadeOut、デクリメントし始める（その間にスプライトのアニメーションを終わらせる）
		}
	}
	#region メソッド : フェードイン(FillAmountをインクリメント/この関数が呼ばれてもfillOrigin = 0じゃなければインクリメントしない)
	public void FadeIn(){
		if(enemyImage.fillAmount < 1 && enemyImage.fillOrigin == 1){
			enemyImage.fillAmount += 0.05f;
		}
	}
	#endregion
	#region メソッド : フェードアウト(FillAmountをデクリメント/この関数が呼ばれても、fillOrigin＝1じゃなければデクリメントしない)
	public void FadeOut(){

		// アイドリング状態への遷移を許可
		isEnemyCutInSpriteOFF = true;
		enemyCutInStart.SetBool ("isEnemyCutInSpriteOFF", isEnemyCutInSpriteOFF);

		// 一度スプライトがカットインしたらもうアニメーションへ遷移しないようにする
		isEnemyCutInSpriteON = false;
		enemyCutInStart.SetBool ("isEnemyCutInSpriteON", isEnemyCutInSpriteON);


		//Debug.Log ("フェードアウト");
		if(enemyImage.fillAmount > 0 && enemyImage.fillOrigin == 0){
			enemyImage.fillAmount -= 0.037f;

		}

		if(enemyImage.fillAmount <= 0){
			enemyImage.fillOrigin = 1;
		}
	}
	#endregion
}
