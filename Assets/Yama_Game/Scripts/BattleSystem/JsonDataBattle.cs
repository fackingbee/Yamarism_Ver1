using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonDataBattle : MonoBehaviour {

	private bool isPlaying = false;
	public GameObject battleGame;
	public GameObject battleGUI;
	public GameObject battlePlayerTurn;
	public GameObject battlePoolObject;
	public GameObject battleCharacter;
	public static string loadJsonData;

	public void SetJsonData( string data ){
		loadJsonData = data;
		isPlaying = true;
	}
	
	void Update () {
		if(isPlaying){
			battleGame.gameObject.SetActive(true);
			battleGUI.gameObject.SetActive(true);
			battlePlayerTurn.gameObject.SetActive(true);
			battlePoolObject.gameObject.SetActive(true);
			battleCharacter.gameObject.SetActive(true);
			isPlaying = false;
			GetComponent<JsonDataBattle> ().enabled = false;
		}
	}
}
