using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This does not beling in this class or interface.
// We are going to remove it.

namespace YamaDelta.ItemSystem {
	
	public interface IISEquipmentSlot {

		string Name { get; set; }
		Sprite Icon { get; set; }

	}
}
