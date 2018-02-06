using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YamaDelta.ItemSystem.Editor{

	public partial class ISObjectEditor {

		enum TabState {
			WEAPON,
			ARMOR,
			POTION,
			QUALITY
		}

		TabState tabState;

		void TopTabBar(){
			GUILayout.BeginHorizontal ("BOX",GUILayout.ExpandWidth(true));
			WeaponTab();
			ArmorTab ();
			PotionTab();
			QualityTab ();
			GUILayout.EndHorizontal();
		}

		void WeaponTab () {
			if (GUILayout.Button ("Weapons")) {
				tabState = TabState.WEAPON;
			}
		}

		void ArmorTab  () {
			if (GUILayout.Button ("Armors")) {
				tabState = TabState.ARMOR;
			}
		} 

		void PotionTab () {
			if (GUILayout.Button ("Potions")) {
				tabState = TabState.POTION;
			}
		} 

		void QualityTab  () {
			if (GUILayout.Button ("Quality")) {
				tabState = TabState.QUALITY;
			}
		} 

	}
}
