
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YamaDelta.ItemSystem {

	[System.Serializable]
	public class ISArmor : ISObject, IISArmor, IISDestructable, IISGameObject {

		[SerializeField] int        _curArmor;
		[SerializeField] int        _maxArmor;
		[SerializeField] int        _durability;
		[SerializeField] int        _maxDurabiliry;
		[SerializeField] GameObject _prefab;
		public EquipmentSlot        equipmentSlot;

		public ISArmor(){
			_curArmor      = 0;
			_maxArmor      = 0;
			_durability    = 1;
			_maxDurabiliry = 1;
			equipmentSlot  = EquipmentSlot.Feet;
		}

		public ISArmor (ISArmor armor) {
			Clone (armor);
		}

		#region バグを修正 : overrideが必須
//		public void Clone (ISArmor armor) {
//
//			base.Clone (armor);
//
//			_curArmor      = armor._curArmor;
//			_maxArmor      = armor._maxArmor;
//			_durability    = armor.Durability;
//			_maxDurabiliry = armor.MaxDurability;
//			equipmentSlot  = armor.equipmentSlot;
//			_prefab        = armor.Prefab;
//
//		}
		#endregion

		public override void Clone (ISObject armor) {

			base.Clone (armor);

			ISArmor a      = (ISArmor)armor;
			_curArmor      = a._curArmor;
			_maxArmor      = a._maxArmor;
			_durability    = a.Durability;
			_maxDurabiliry = a.MaxDurability;
			equipmentSlot  = a.equipmentSlot;
			_prefab        = a.Prefab;

		}





		#region IISGameObject implementation

		public GameObject Prefab{
			get {
				#region メモ : タッチするとヒエラルキーにオブジェクトを作ってしまうのでnewしない
//				if(!_prefab){
//					_prefab = new GameObject();
//				}
				#endregion
				return _prefab;
			}
		}

		#endregion

		#region IISDestructable implementation

		public void TakeDamage (int amount){
			_durability -= amount;
			if(_durability < 0 ){
				_durability = 0;
			}
		}

		public void Repair () {
			_maxDurabiliry--;
			if(_maxDurabiliry > 0){
				_durability = _maxDurabiliry;
			}
		}
			
		public void Break () {
			_durability = 0;
		}

		public int Durability {
			get { return _durability; } 
		}
			
		public int MaxDurability { 
			get { return _maxDurabiliry; } 
		}

		#endregion

		#region IISArmor implementation

		public Vector2 Armor {
			get {
				return new Vector2(_curArmor, _maxArmor);
			}
			set {
				
				// cur is never less than zero.
				// cur is never greater than the max.
				// max is always greater than zero.

				if(value.x < 0){
					value.x = 0;
				}

				if(value.y < 1){
					value.y = 1;
				}

				if(value.x > value.y){
					value.x = value.y;
				}

				_curArmor = (int)value.x;
				_maxArmor = (int)value.y;
			}
		}

		#endregion

		// This code will go in to a new script later on
		#if UNITY_EDITOR

		public override void OnGUI () {

			base.OnGUI();
			_curArmor      = EditorGUILayout.IntField ( "Cur Armor",      _curArmor      );
			_maxArmor      = EditorGUILayout.IntField ( "Max Armor",      _maxArmor      );
			_durability    = EditorGUILayout.IntField ( "Cur Durability", _durability    );
			_maxDurabiliry = EditorGUILayout.IntField ( "Max Durability", _maxDurabiliry );

			DisplayEquipmentSlot();
			DisplayPrefab();
		}

		public void DisplayEquipmentSlot () {
			equipmentSlot =	(EquipmentSlot)EditorGUILayout.EnumPopup("Equipment Slot", equipmentSlot);
		}

		public void DisplayPrefab () {
			_prefab = EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject),false) as GameObject;
		}
		#endif


	}
}
