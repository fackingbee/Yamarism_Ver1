using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour {

	public static float time;				// 時間管理（どこからでも呼べるように静的メンバーstaticで宣言）(staticをつけるとpublicでもInspector上に表示されない)
	public static long  tick;				// MIDI時間管理用
	public static int   tempoSequence = 0;	// テンポシーケンス

	public static List<TempoStructure> tempo = new List<TempoStructure>();


	void Awake () {
		TimeManager.time            = 0;
		TimeManager.tick            = 0;
	}

//	void Start(){
//
//		if(this.enabled){
//			Debug.Log ("TimeManager ON");
//		}
//
//	}


	void Update () {

		// timeを更新
		#if UNITY_EDITOR
		TimeManager.time += Time.deltaTime;
		#else
		TimeManager.time += Time.unscaledDeltaTime;
		#endif

		// Tempoを更新
		if( TimeManager.tempo.Count > TimeManager.tempoSequence + 1 && TimeManager.time >= TimeManager.tempo[TimeManager.tempoSequence+1].changeTime ){
			TimeManager.tempoSequence++; 
		}

		// timeからtickを計算 (ProToolsで出力したMIDIは一拍9600tick)
		TimeManager.tick = (long)((TimeManager.time - TimeManager.tempo[TimeManager.tempoSequence].changeTime) * (TimeManager.tempo[TimeManager.tempoSequence].tempoValue * 9600f) / 60f) + TimeManager.tempo[TimeManager.tempoSequence].changeTick;
			
	}


	// テンポを格納する構造体
	public struct TempoStructure {

		public float tempoValue;
		public long  changeTick;
		public float changeTime;

		// コンストラクタ
		public TempoStructure( float tempoValue, long changeTick ){
			this.tempoValue = tempoValue; 
			this.changeTick = changeTick;
			this.changeTime = 0;
		}
	}
}
