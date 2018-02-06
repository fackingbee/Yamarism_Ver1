using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamaDelta.ItemSystem;

[DisallowMultipleComponent]
public class Weapon : MonoBehaviour {

	public Sprite Icon;
	public int Value;
	public int Burden;
	public Sprite Quality;
	public int Min_Damage;
	public int Durability;
	public int Max_Durability;
	public EquipmentSlot Equipment_Slot;

}
