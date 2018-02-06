using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YamaDelta.ItemSystem{
	public interface IISStackable {
		
		int Stack   (int amount);	// default value of 0
		int MaxStack(int amount);

 		 
	}
}
