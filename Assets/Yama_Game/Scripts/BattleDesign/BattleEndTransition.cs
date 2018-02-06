using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndTransition : MonoBehaviour {

	// 背景の透明度を上げるアニメーション
	public Animator backAnim;

	#region Start() : ４秒後に透明度を下げる
	void Start () {
		Invoke ("OnBackAnim", 4.0f);
	}
	#endregion

	#region メソッド : 透明度を上げる
	public void OnBackAnim() {
		backAnim.enabled  = true;
	}
	#endregion

	// 評価画面遷移はAnimationのイベント関数コールから操作する //

}
