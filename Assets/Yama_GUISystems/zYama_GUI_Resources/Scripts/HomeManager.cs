using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour {

	#region Awake() :  
	void Awake(){
		Application.targetFrameRate = 60;	// フレームレートは60に設定
		System.GC.Collect ();            	// Battle開始時に一旦メモリ解放
		Resources.UnloadUnusedAssets (); 	// 及び、使用していないアセットのアンロード
	}
	#endregion
	#region Start (off)
//	void Start () {
//		
//	}
	#endregion
	#region Update (off)
//	void Update () {
//		
//	}
	#endregion

}
