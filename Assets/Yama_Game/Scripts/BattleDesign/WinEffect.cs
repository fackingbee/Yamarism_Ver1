using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinEffect : MonoBehaviour {

	public TypefaceAnimator typefaceAnimator;
	public GameObject       winObject;
	public Image            winImage;
	public Text             winText;
	public float            degree = 0f;


	void Start  () {}

	void Update () {

		if(winObject.transform.localScale.x > 1){
			winObject.transform.localScale -= new Vector3 (0.3f,0.3f,0.3f);
			Shake ();
		}

		if(winObject.transform.localScale.x < 2 && winImage.color.a < 0.584f){
			winImage.color += new Color (winImage.color.r, winImage.color.g, winImage.color.b, 0.08f);;
		}

		if(winObject.transform.localScale.x < 1.5f && winText.color.a < 1f){
			winText.color += new Color (winText.color.r, winText.color.g, winText.color.b, 0.08f);
		}

		if(!typefaceAnimator.enabled && winText.color.a >= 0.6f){
			winText.color = new Color (245f/255f, 255f/255f, 79f/255f, 255f/255f);
			typefaceAnimator.enabled = true;
		}
	}
		
	public void Shake(){
		iTween.ShakePosition (
			gameObject,iTween.Hash (
				"x", degree, "y", degree, "isLocal", true ,"time", 0.5f
			)
		);
	}
}
