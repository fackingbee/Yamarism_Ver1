using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlickAnimDirection : MonoBehaviour { 

	public Vector3   dir;
	public Transform myTrf;
	public int       flickFlagDir;


	void Start () {
		myTrf = GetComponent<Transform>();
	}
		

	void Update (){

		// 左
		if (flickFlagDir==0 && dir.x < 0 && dir.y >= -0.5f && dir.y <= 0.4f){

			myTrf.position += new Vector3 (-1f, 0f, 0f) * 2f;
			myTrf.Rotate (Vector3.forward, 10, Space.World);		// RotateでYを回転

		// 左上
		} else if (flickFlagDir==1 && dir.x < 0 && dir.y >= 0.2f && dir.y <= 0.9f){

			myTrf.position += new Vector3 (-1f,1f,-1f) * 2f;
			myTrf.Rotate (Vector3.forward, 10, Space.World);		// RotateでYを回転

		// 真上
		} else if (flickFlagDir==2 && dir.x >= -0.5f && dir.x <= 0.5f  && dir.y > 0){

			myTrf.position += new Vector3 (0f,1f,-1f) * 2f;
			myTrf.Rotate (Vector3.forward, 10, Space.World);		// RotateでYを回転

		// 右上
		} else if (flickFlagDir==3 && dir.x > 0 && dir.y >= 0.2f && dir.y <= 0.9f){

			myTrf.position += new Vector3 (1f,1f,-1f) * 2f;
			myTrf.Rotate (Vector3.forward, -10, Space.World);		// RotateでYを回転

		// 右
		} else if (flickFlagDir==4 && dir.x > 0 && dir.y >= -0.5f && dir.y <= 0.4f){

			myTrf.position += new Vector3 (1f,0f,0f) * 2f;
			myTrf.Rotate (Vector3.forward, -10, Space.World);		// RotateでYを回転

		}
	}
}
