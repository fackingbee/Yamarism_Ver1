using UnityEngine;
using System.Collections;

public class ParticleSize : MonoBehaviour {

	public  float   startTick;			// ロングタップ時のエフェクト用で、ダウン時のTick
	public  float   endTick;			// ロングタップ時のエフェクト用で、アップ時のTick
	private float   scaleRate;			// ロングタップ時のエフェクト用で、長さに合わせて縮小率を変える
	private float   baseTick;
	private Vector3 baseLocalScale;
	private Vector3 defaultSize = new Vector3(2.0f, 2.0f, 2.0f);

	void Start () {
		baseTick = TimeManager.tick;
		baseLocalScale = this.transform.localScale;
	}
	
	void Update () {

		// （構文）縮小率を決める
		scaleRate = (TimeManager.tick - baseTick) / (endTick - startTick);

		// 0.2以下になるまで縮小する
		if (this.transform.localScale.x > 0.2f) {
			this.transform.localScale = baseLocalScale * (1f - scaleRate * 0.7f);
		}

		// 目的の大きさまで縮小したら初期化
		// 本来のInstanteateは生成された時のtickが入るが、オブジェクトプールだと前のtickが残るので、
		// 目的の大きさまで縮小したら、その時のtickを格納しておき、サイズも初期化する
		if(this.transform.localScale.x <= 0.2f){
			baseTick = TimeManager.tick;
			this.transform.localScale = defaultSize;
		}
	}
}
