using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YamaDelta.ItemSystem{
	
	public interface IISDestructable {

		int Durability    { get;}
		int MaxDurability { get;}

		void TakeDamage (int amount);
		void Repair();
		void Break();

		// durability
		// takedamage
		 
	}
}
