// データベースにアクセスする為のデモ
// データベースからアイテムを生成する

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamaDelta.ItemSystem;

[DisallowMultipleComponent]
public class IS_Demo1 : MonoBehaviour {

	public ISWeaponDatabase database;

	void OnGUI () {
		for(int cnt = 0; cnt < database.Count; cnt++){
			if (GUILayout.Button("Spawn : " + database.Get(cnt).Name)){
				Spawn (cnt);
			}
		}
	}
	 
	void Spawn (int index) {
		
		//Debug.Log (database.Get(index).Name);

		ISWeapon isw      = database.Get(index);
		GameObject weapon = Instantiate (isw.Prefab);
		weapon.name       = isw.Name;
		Weapon myWeapon   = weapon.AddComponent<Weapon> ();

		/* Weapon.csにて定義
		public Sprite        Icon;
		public int           Value;
		public int           Burden;
		public Sprite        Quality;
		public int           MinDamage;
		public int           Durability;
		public int           Max_Durability;
		public EquipmentSlot Equipment_Slot;
		*/
		myWeapon.Icon           = isw.Icon;
		myWeapon.Value          = isw.Value;
		myWeapon.Burden         = isw.Burden;
		myWeapon.Quality        = isw.Quality.Icon;
		myWeapon.Min_Damage     = isw.MinDamage;
		myWeapon.Durability     = isw.Durability;
		myWeapon.Max_Durability = isw.MaxDurability;
		myWeapon.Equipment_Slot = isw.equipmentSlot;
	}
}
