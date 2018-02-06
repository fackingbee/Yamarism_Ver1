using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

namespace YamaDelta.ItemSystem{
	
	public interface IISEquipable {

		ISEquipmentSlot EquipmentSlot{get;}
		bool Equip();
	}
}