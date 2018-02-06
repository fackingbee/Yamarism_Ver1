using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YamaDelta.ItemSystem {
	
	public interface IISWeapon {
		int MinDamage { get; set; }
		int Attack();
	}
}
 