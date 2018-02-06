using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CutInFillAmountSettlement : MonoBehaviour {
	
	public  Image    settlementImage; 			    // (Inspector)Imageコンポーネント取得用
	public  Animator settlemetCutInStart;		    // (Inspector)PlayerSpriteのAnimatorをON・OFF
	private bool     isSettlementCutInSpriteON;     // カットイン時のPlayerSpriteアニメーションONフラグ
	//private bool     isSettlementCutInSpriteOFF;    // カットイン時のPlayerSpriteアニメーションOFFフラグ

	[System.NonSerialized] public bool fadeIn;		// 左からフェードイン
	[System.NonSerialized] public bool fadeOut;		// 左へフェードアウト

	void Start () {
		settlementImage.fillAmount = 0f;			 // 開始時は何も映ってなくていいので0で初期化
		settlementImage.fillOrigin = 1;				 // 初期値はRight
		fadeIn                     = true;
		fadeOut                    = false;
		isSettlementCutInSpriteON  = true;
		//isSettlementCutInSpriteOFF = false;
	}

	void Update () {

		if(fadeIn){
			Invoke ("FadeIn",0f);					
		}

		if(settlementImage.fillAmount >= 0f && fadeIn){
			settlemetCutInStart.enabled  = true;	
			isSettlementCutInSpriteON    = true;	
			settlemetCutInStart.SetBool ("isSettlementCutInSpriteON", isSettlementCutInSpriteON);
		}
			
		if(settlementImage.fillAmount >= 1){
			settlementImage.fillOrigin   = 0;
			fadeIn  = false;	
			fadeOut = true;

		}
		#region 決着時はいらない
//		if(fadeOut && settlementImage.fillOrigin == 0 && settlementImage.fillAmount > 0){
//			//Invoke ("FadeOut",3.1f); // 3秒後にFadeOut、デクリメントし始める（その間にスプライトのアニメーションを終わらせる）
//		}
		#endregion
	}
	#region メソッド : フェードイン(FillAmountをインクリメント/この関数が呼ばれてもfillOrigin = 0じゃなければインクリメントしない)
	public void FadeIn(){
		if(settlementImage.fillAmount < 1 && settlementImage.fillOrigin == 1){
			settlementImage.fillAmount += 0.08f;
		}
	}
	#endregion
	#region メソッド : フェードアウト(決着時はいらない)
//	public void FadeOut(){
//
//		// アイドリング状態への遷移を許可
//		isSettlementCutInSpriteOFF = true;
//		settlemetCutInStart.SetBool ("isSettlementCutInSpriteOFF", isSettlementCutInSpriteOFF);
//
//		// 一度スプライトがカットインしたらもうアニメーションへ遷移しないようにする
//		isSettlementCutInSpriteON = false;
//		settlemetCutInStart.SetBool ("isSettlementCutInSpriteON", isSettlementCutInSpriteON);
//
//
//		//Debug.Log ("フェードアウト");
//		if(settlementImage.fillAmount > 0 && settlementImage.fillOrigin == 0){
//			settlementImage.fillAmount -= 0.037f;
//
//		}
//
//		if(settlementImage.fillAmount <= 0){
//			settlementImage.fillOrigin = 1;
//		}
//	}
	#endregion
}
