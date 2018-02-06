using UnityEngine;
using System.Collections;

public class BoardMove : MonoBehaviour {

	void Update () {
		
		//時間の更新
		transform.localPosition = new Vector3 (0, - TimeManager.tick * 0.05f, 0);

		// ゲーム終了時に停止
		if(!GameController.isPlaying){ this.enabled = false; }
	}
}