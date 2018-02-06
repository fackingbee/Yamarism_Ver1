
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace YamaDelta.ItemSystem {
	
	[System.Serializable]
	public class ISWeapon : ISObject, IISWeapon, IISDestructable, IISGameObject {

		[SerializeField] int        _minDamage;
		[SerializeField] int        _durability;
		[SerializeField] int        _maxDurabiliry;
		[SerializeField] GameObject _prefab;

		//[SerializeField] ISEquipmentSlot _equipmentSlot;
		public EquipmentSlot equipmentSlot;


		// 初期値用コンストラクタ
		public ISWeapon () {

			#region newはサポートされてません
			//_equipmentSlot = new ISEquipmentSlot();
			//_prefab        = new GameObject();
			//_prefab        = new GameObject(); //コンストラクタ内でCreatGameObjectはサポートされていない
			#endregion

			_minDamage     = 0;
			_durability    = 1;
			_maxDurabiliry = 1;
			equipmentSlot  = EquipmentSlot.Feet;
		}


		#region コンストラクタの仕様を変更
//		public ISWeapon (int durabiliry, int maxDurabiliry, ISEquipmentSlot equipmentSlot, GameObject prefab) {
//			_durability    = durabiliry;
//			_maxDurabiliry = maxDurabiliry;
//			_equipmentSlot = equipmentSlot;
//			_prefab        = prefab;
//		}
		#endregion


		public ISWeapon (ISWeapon weapon) {
			#region Cloneメソッドへ関数化
//			// Cloneメソッドへ移行
//			_durability    = weapon.Durability;
//			_maxDurabiliry = weapon.MaxDurability;
//			equipmentSlot  = weapon.equipmentSlot;
//			_prefab        = weapon.Prefab;
			#endregion
			Clone (weapon);
		}


		#region メモ : Cloneメソッドのoverrideの違いについて
		// 引数の違うCloneメソッド(override)がISObjectにもあり、そちらはISObjectを引数にとる
		// 両者の違いは、こちらはアイテムのうち、武器に関するデータを取り扱うものであり、
		// ISObjectの方は、アイテム全般に共通のデータ(名前とか)を扱うものである。
		// baseは基本クラス(ISObjectの方)を参照しており、
		// Clone (ISWeapon weapon)を実行すると、同時にClone (ISObject item)が実行される。
		#endregion

		#region バグを修正 : overrideが必須
//		public void Clone (ISWeapon weapon) {
//			
//			base.Clone (weapon);
//
//			_minDamage     = weapon.MinDamage;
//			_durability    = weapon.Durability;
//			_maxDurabiliry = weapon.MaxDurability;
//			equipmentSlot  = weapon.equipmentSlot;
//			_prefab        = weapon.Prefab;
//
//		}
		#endregion

		public override void Clone (ISObject weapon) {

			base.Clone (weapon);

			ISWeapon w     = (ISWeapon)weapon;
			_minDamage     = w.MinDamage;
			_durability    = w.Durability;
			_maxDurabiliry = w.MaxDurability;
			equipmentSlot  = w.equipmentSlot;
			_prefab        = w.Prefab;

		}

		 
		// IISWeaponインターフェースより
		public int MinDamage {
			get{ return _minDamage;}
			set{_minDamage = value;}
		}


		public int Attack () {
			throw new System.NotImplementedException ();   
		}


		// IISDestructableインターフェースより
		public int Durability {
			get { return _durability; } 
		}


		public int MaxDurability { 
			get { return _maxDurabiliry; } 
		}


		public void TakeDamage (int amount){
			
			_durability -= amount;
//			_durability = _durability - amount;

			if(_durability < 0 ){
				_durability = 0;
			}

			// いつのまにか消えてる...
//			if(_durability == 0){
//				Break();
//			}

		}

		public void Repair () {
			
			_maxDurabiliry--;

			if(_maxDurabiliry > 0){
				_durability = _maxDurabiliry;
			}
		}

		// reduce the durability to 0.
		// not sure what to do with method.
		public void Break () {
			_durability = 0;
		}
			
//		// IISEquipableインターフェースより : 後に不要
//		public ISEquipmentSlot EquipmentSlot {
//			get { return _equipmentSlot; }
//		}

		// IISGameObjectインターフェースより
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


		// This code will go in to a new script later on
		#if UNITY_EDITOR

		public override void OnGUI () {
			
			base.OnGUI();

			_minDamage     = EditorGUILayout.IntField ( "Damage",         _minDamage     );
			_durability    = EditorGUILayout.IntField ( "Min Durability", _durability    );
			_maxDurabiliry = EditorGUILayout.IntField ( "Max Durability", _maxDurabiliry );

			DisplayEquipmentSlot();
			DisplayPrefab();
		}
			
		public void DisplayEquipmentSlot () {
			//GUILayout.Label ("Equipment Slot");
			equipmentSlot =	(EquipmentSlot)EditorGUILayout.EnumPopup("Equipment Slot", equipmentSlot);
		}
			
		public void DisplayPrefab () {
			//GUILayout.Label ("Prefab");
			_prefab = EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject),false) as GameObject;
		}

		#endif

	}
}
