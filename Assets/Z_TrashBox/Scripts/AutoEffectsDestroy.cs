using UnityEngine;
using System.Collections;

public class AutoEffectsDestroy : MonoBehaviour {

	void Start () {
		Invoke ("EffectsDestroy", 4f);
	}
		

	public void EffectsDestroy(){
		Destroy (gameObject);
	}
}
