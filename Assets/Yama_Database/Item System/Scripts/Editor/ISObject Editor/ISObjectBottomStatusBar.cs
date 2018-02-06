using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YamaDelta.ItemSystem.Editor{

	public partial class ISObjectEditor  {

		void BottomStatusBar(){
			GUILayout.BeginHorizontal("BOX", GUILayout.ExpandWidth (true));
			GUILayout.Label("Status Bar");
			GUILayout.EndHorizontal();

		}
	}
}
