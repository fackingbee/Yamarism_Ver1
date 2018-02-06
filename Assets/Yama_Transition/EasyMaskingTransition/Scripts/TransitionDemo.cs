using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionDemo : MonoBehaviour {

	// 『Canvas_EMT』はどのシーンにも一つだけ存在するようにする
	public  EMTransition transition1;
	public  EMTransition transition2;
	public  Button       button;
	public  float        delay        = 1.0f;
	private bool         isCloseScene = true;

	void Start(){
		Invoke ("OnStartAnimation", delay);
	}

	#region 同一シーン内でTransitionだけさせる場合は自動で開閉する
	public void OnTransitionStart() {
		if(!button) return;
		if(isCloseScene) {
			button.gameObject.SetActive(false);
		}
	}

	public void OnTransitionComplete(){
		if(!button) return;
		if(isCloseScene) {
			isCloseScene = false;
			Invoke ("OnStartAnimation", delay);
		} else {
			button.gameObject.SetActive(true);
			isCloseScene = true;
		}
	}
	#endregion

	// Transitionを開始するメソッド
	public void OnStartAnimation(){
		transition1.Play();
		transition2.Play();
	}
	
}
