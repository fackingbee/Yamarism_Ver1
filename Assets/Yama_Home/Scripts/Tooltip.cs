using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour,IActivate<Item>,IDeactivate {

	private string      data;
	private bool	    isActivate;
	private Item        item;
	private GameObject  tooltip;
	private CanvasGroup canvasGroup;

	void Start () {
		#region メモ : 起動時は非アクティブだが...
		// 但し、Tooltipオブジェクトのアクティブボタンが非アクティブだとそもそも探せなくなるので
		// シーン配置時はアクティブにしておく必要がある
		// Tooltip表示時はRayが届かないように、CanvasGroupをAddする
		#endregion
		tooltip = GameObject.Find ("Tooltip");
		canvasGroup = tooltip.GetComponent<CanvasGroup>();
		//tooltip.SetActive (false);
	}
		
	void Update () {
		if(tooltip.activeSelf && isActivate){
			tooltip.transform.position = Input.mousePosition;
			TooltipAnimStart ();
			isActivate = false;
		}
	}


	public void Activate (Item item) {
		this.item = item;
		ConstructDataString();
		TooltipAnimStart ();
		//tooltip.SetActive(true);
		isActivate = true;
	}

	public void Deactivate () {
		//tooltip.SetActive (false);
		TooltipAnimEnd();
	}
		
	public void ConstructDataString () {
		//data = "<color=#87CEEB><b>" +  item.Title + "</b></color>\n\n" + item.Description + "\nPower:" + item.Power;
		data = "<color=#87CEEB><b>" +  item.ItemName + "</b></color>\n\n" + item.Description; 
		tooltip.transform.GetChild (0).GetComponent<Text>().text = data;
	}

	private void TooltipAnimStart () {

		iTween.ScaleTo (
			tooltip, 
			iTween.Hash(
				"x"   , 1.0f,
				"y"   , 1.0f,
				"z"   , 1.0f,
				"time", 0.5f,
				"easetype",iTween.EaseType.easeInOutBack
			)
		);

		iTween.ValueTo (
			tooltip,
			iTween.Hash(
				"from", 0.0f,
				"to"  , 1.0f,
				"time", 0.5f,
				"onupdatetarget", gameObject,
				"onupdate", "OnUpdateAlpha"
			)
		);
	}

	private void TooltipAnimEnd () {
		iTween.ScaleTo (
			tooltip, 
			iTween.Hash(
				"x"   , 0.0f,
				"y"   , 0.0f,
				"z"   , 0.0f,
				"time", 0.5f,
				"oncompletetarget", gameObject,
				"oncomplete", "OnComplete"
			)
		);

		iTween.ValueTo (
			tooltip,
			iTween.Hash(
				"from", 1.0f,
				"to"  , 0.0f,
				"time", 0.3f,
				"onupdatetarget", gameObject,
				"onupdate", "OnUpdateAlpha"
			)
		);
	}

	private void OnUpdateAlpha (float alpha) { canvasGroup.alpha = alpha; }
	private void OnComplete () { canvasGroup.alpha = 0.0f; }
}
