using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// 正直なところ、スタート時のアニメーション案は演出的に若干無駄なので、不採用

public class SetStartGage : MonoBehaviour {

	private Slider      startValue;
	private Slider      setValue;
	public  GageHandler gageHandler;

	private bool isFinished;

	void Start () {
		gageHandler         = FindObjectOfType<GageHandler> ();
		gageHandler.enabled = false;
		startValue          = GetComponent<Slider> ();
		setValue            = GetComponent<Slider> ();
		startValue.value    = 0f;
		isFinished          = true;
	}
	 
	void Update () {

		if(setValue.value < startValue.maxValue/2 ){
			setValue.value += 2.0f;
		}

		if(setValue.value == startValue.maxValue/2 && isFinished){

			gageHandler.enabled         = true;
			isFinished                  = false;
			GameDate.playerSetGageValue = startValue.maxValue / 2;

			this.enabled = false;
		}
	}
}
