using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YamaDelta.ItemSystem{
	
	public interface IISObject {

		string    Name    { get; set; }	// name
		int       Value   { get; set; }	// value - gold value
		Sprite    Icon    { get; set; }	// icon
		int       Burden  { get; set; } // burden
		ISQuality Quality { get; set; } // qualitylevel

		// these got to other item interfaces 
		// questItem flag
	}
}
