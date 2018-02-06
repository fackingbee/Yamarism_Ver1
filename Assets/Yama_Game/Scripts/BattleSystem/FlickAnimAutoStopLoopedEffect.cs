using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FlickAnimAutoStopLoopedEffect : MonoBehaviour {

	public  float effectDuration = 2.5f;
	private float d;

	// この関数はオブジェクトが有効/アクティブになったときに呼び出される。
	// 尚、コルーチンは不可
	void OnEnable(){
		d = effectDuration;
	}

//	void Start () {
//		
//	}
	
	void Update () {

		if(d > 0) {
			
			d -= Time.deltaTime;
			if(d <= 0) {
				
				this.GetComponent<ParticleSystem>().Stop(true);

				FlickAnimDirection translation = this.gameObject.GetComponent<FlickAnimDirection>();

				if(translation != null) {
					
					translation.enabled = false;

				}
			}
		}
	}
}
