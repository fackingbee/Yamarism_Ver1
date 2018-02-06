
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace YamaDelta.ItemSystem {
	public class ISObject : IISObject,IISDatabaseObject {

		[SerializeField] string    _name;
		[SerializeField] Sprite    _icon;
		[SerializeField] int       _value;
		[SerializeField] int       _burden;
		[SerializeField] ISQuality _quality;

		public ISObject(){}

		public ISObject (ISObject item) {
			Clone(item);
		}


		#region 『public void Clone (ISObject item)』はこのままだとバグあり
//		public void Clone (ISObject item) {
//			_name    = item.Name;
//			_icon    = item.Icon;
//			_value   = item.Value;
//			_burden  = item.Burden;
//			_quality = item.Quality;
//		}
		#endregion
		#region 解決者のコメント
		/*
 		Hey guys, big rep for BurgZerg Arcade,
 		I've just completed the series, at least i saw this episode,
 		and i have found a problem which makes the ISWeapon, ISArmor, ...
		 attributes to not been showed/saved properly.

		The problem in short, the not the right one Clone function called.

		I figured out the solution,

		ISObject.cs - public void Clone(ISObject item) -> public virtual void Clone(ISObject item)

		And every inherited class do someting like this,
		the Clone function param type must be ISObject to match the virtual function signature.
		And then you can safely Upcast from ISObject to ISxxxxxx

        public override void Clone(ISObject weapon)
        {
            base.Clone(weapon);

            ISWeapon w = (ISWeapon)weapon;

            _minDamage = w._minDamage;

            _durability = w._durability;
            _maxDurability = w._maxDurability;

            equipmentSlot = w.equipmentSlot;
            _prefab = w._prefab;
        }﻿
		*/
		#endregion


		public virtual void Clone (ISObject item) {
			_name    = item.Name;
			_icon    = item.Icon;
			_value   = item.Value;
			_burden  = item.Burden;
			_quality = item.Quality;
		}
			
		public void Clone (IISDatabaseObject item) {
			_name = item.Name;
			_icon = item.Icon;
		}
			
		public string Name { 
			get { return _name;  }
			set { _name = value; }
		}

		public int Value {
			get { return _value;  }
			set { _value = value; }
		}

		public Sprite Icon {
			get { return _icon;  }
			set { _icon = value; }
		}

		public int Burden { 
			get { return _burden;  }
			set { _burden = value; }
		}

		public ISQuality Quality {
			get { return _quality;  }
			set { _quality = value; }
		}
			


// This code is going to be paced in a new class later on
#if UNITY_EDITOR

		ISQualityDatabase qdb;
		int qualitySelectedIndex = 0;
		string[] option;
		bool qualityDatabaseLoaded = false;


		// 本来直接ISWeaponに記載していたものを抽象メソッド化
		public virtual void OnGUI () {
			
			#region メモ : (int)によるキャストとSystem.Convert.ToInt32によるキャストに違い
			// System.Convert.ToInt32はnullだと0が返ってくるのに対し、
			// (int)は...何も返らなかったりnullだったり？
			#endregion
			//_value  = (int)EditorGUILayout.TextField("Name: ", _value.ToString());

			GUILayout.BeginVertical();
			_name   = EditorGUILayout.TextField("Name", _name);
			_value  = EditorGUILayout.IntField("Value",_value);
			_burden = EditorGUILayout.IntField("Burden",_burden);
			DisplayIcon();
			DisplayQuality();
			GUILayout.EndVertical();
		}


		public void DisplayIcon () {
			//GUILayout.Label ("Icon");
			_icon = EditorGUILayout.ObjectField("Icon", _icon, typeof(Sprite),false) as Sprite;

		}
			
		public int SelectedQualityID{
			get{ return qualitySelectedIndex; }
		}
			
		//public ISObject(){
			#region メモ : ここの処理を多少変更しました

			// 最近のUnityのバージョンにおけるAPIでは
			// シリアライズ化されたデータ（Scriptable Objectのこと）は、直接コンストラクタでは呼べず
			// OnEnableで呼ばなければいけないようなので
			// エディターが開いた直後、
			// DisplayQuality()が実行される段階でデータベースにアクセス（GetDatabase()）するようにしてみた

			
//			string DATABASE_NAME = @"YamaQualityDatabase.asset";
//			string DATABASE_PATH = @"Database";
//			qdb = ISQualityDatabase.GetDatabase<ISQualityDatabase>(DATABASE_PATH, DATABASE_NAME); 
//
//			// データベースの要素数の分だけ、optionの要素数を動的にインスタンス
//			option = new string[qdb.Count];
//
//			for(int cnt = 0;cnt < qdb.Count; cnt++){
//				option[cnt] = qdb.Get(cnt).Name;// Get : ScriptableObjectDatabaseで定義した関数
//
//			}

			#endregion
		//}

		public void LoadQualityDatabase (){

			string DATABASE_NAME = @"YamaQualityDatabase.asset";
			string DATABASE_PATH = @"Database";

			qdb = ISQualityDatabase.GetDatabase<ISQualityDatabase>(DATABASE_PATH, DATABASE_NAME); 

			// データベースの要素数の分だけ、optionの要素数を動的にインスタンス
			option = new string[qdb.Count];

			for(int cnt = 0;cnt < qdb.Count; cnt++){
				// Get : ScriptableObjectDatabaseで定義した関数
				option[cnt] = qdb.Get(cnt).Name;
			}

			qualityDatabaseLoaded = true;

		}


		public void DisplayQuality () {

		#region メモ : ビルド時にエラーが出るのでがっつり変更
			//GUILayout.Label ("Quality");

			//////////////////////////////////////////////////////////////////////////////////////////
//			string DATABASE_NAME = @"YamaQualityDatabase.asset";
//			string DATABASE_PATH = @"Database";
//
//			qdb = ISQualityDatabase.GetDatabase<ISQualityDatabase>(DATABASE_PATH, DATABASE_NAME); 
//
//			// データベースの要素数の分だけ、optionの要素数を動的にインスタンス
//			option = new string[qdb.Count];
//
//			for(int cnt = 0;cnt < qdb.Count; cnt++){
//				// Get : ScriptableObjectDatabaseで定義した関数
//				option[cnt] = qdb.Get(cnt).Name;
//			}
			//////////////////////////////////////////////////////////////////////////////////////////

		#endregion

			if(!qualityDatabaseLoaded){
				LoadQualityDatabase();
				return;
			}
				
			#region メモ : ポップアップウィンドウ形式

			// アイテムが選択された時のQualityの表示を登録されたもので表示させる
			// コメントアウトの方だとデータベースには登録されていても、表示は前のになってしまうから


//			Debug.Log ("Quality Index : " + qdb.GetIndex(_quality.Name));
//			qualitySelectedIndex = EditorGUILayout.Popup("Quality", qualitySelectedIndex, option);

			// 以前のバージョンだとエラーが出るので、変数を用意する仕様に変更
			//qualitySelectedIndex = EditorGUILayout.Popup("Quality", qdb.GetIndex(_quality.Name), option);


			// ローカル変数を用意がこのままだと、アイテムが一切無い場合はエラーが出る
			// 理由はqdbの初期値は-1だが、QualityDatabaseの要素数は0から始まるので、
			// null時に一致しない
			// そこでデータベースがnull時、新規登録する際は要素数0のQualityを表示するようにする
			// 要するにGetIndexを使って動的にindexを取得しようとした結果、null時の問題が出たので対処している

			#endregion

			int itemIndex = 0;

			if(_quality != null){
				itemIndex = qdb.GetIndex(_quality.Name);
			}

			if(itemIndex == -1){
				itemIndex = 0;
			}
				
			qualitySelectedIndex = EditorGUILayout.Popup("Quality", itemIndex, option);

			_quality = qdb.Get(SelectedQualityID);	

		}
#endif

	}
}

 