
// ScriptableObjectDatabaseを書き換え
// データベースとそのタイプを引数とするジェネリッククラスを作成して、同時に処理していく方式に変更
// paticalクラスでISObjectDatabaseMethodsを対と為す

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace YamaDelta.ItemSystem.Editor {

	public partial class ISObjectDatabaseType<D,T> where D : ScriptableObjectDatabase<T> where T : ISObject, new() {

		[SerializeField] D database;
		[SerializeField] string dbName;
		[SerializeField] string dbPath = @"Database";

		public string strItemType = "Item";


		public ISObjectDatabaseType (string n) {
			dbName = n;
		}

		public void OnEnable (string itemType) {
			strItemType = itemType;
			if(database == null){
				LoadDatabase ();
			}
		}


		public void OnGUI (Vector2 buttonSize ,int _listViewWidth) {
			ListView    (buttonSize, _listViewWidth);
			ItemDetails ();
		}
	}
}

