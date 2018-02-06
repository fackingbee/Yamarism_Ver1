using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yPrefsDelete : MonoBehaviour {

	void Awake () {
		PlayerPrefs.DeleteAll ();
	}
}
