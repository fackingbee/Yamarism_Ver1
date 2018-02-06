using UnityEngine;
using System.Collections;

public class LightMove : MonoBehaviour {

	public float speed = 1f;
	public float width = 5f;

	private Transform myPos;

	void Start(){
		myPos = GetComponent<Transform>();
	}

	void LateUpdate () {

		// ゲーム終了まで常に振り子運動
		if(!GameController.isResult){
			myPos.position = 
				new Vector3(Mathf.Sin (Time.time * speed) * width, myPos.position.y , myPos.position.z);
		}

		// ゲーム終了時、だいたい真ん中くらいまできたら止まる
		if(GameController.isResult){
			myPos.position = 
				new Vector3(Mathf.Sin (Time.time * speed) * width, myPos.position.y , myPos.position.z);
			if(Mathf.Abs(this.myPos.localPosition.x)<10){
				this.enabled = false;
			}
		}
	}
}
