using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IISDatabaseObject {
	string Name { get; set; }
	Sprite Icon { get; set; }
	void Clone ( IISDatabaseObject item );
	void OnGUI ();

}
