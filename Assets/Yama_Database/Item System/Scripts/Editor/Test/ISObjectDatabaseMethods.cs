
//#if UNITY_EDITOR
using UnityEditor;
//#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ISObjectDatabaseMethods.cs

namespace YamaDelta.ItemSystem.Editor {

	//public class ISObjectDatabaseMethods : MonoBehaviour {
	public partial class ISObjectDatabaseType<D,T> where D : ScriptableObjectDatabase<T> where T : ISObject, new() {

		//#if UNITY_EDITOR

		public void Add (T item) {
			database.Item.Add (item);
			EditorUtility.SetDirty (database);
		}

		public void Insert (int index, T item) {
			database.Item.Insert (index, item);
			EditorUtility.SetDirty (database);
		}

		public void Remove (T item) {
			database.Item.Remove (item);
			EditorUtility.SetDirty (database);
		}

		public void Remove (int index) {
			database.Item.RemoveAt(index);
			EditorUtility.SetDirty (database);
		}
		//#endif

		//#if UNITY_EDITOR

		public void Replace (int index, T item) {
			database.Item[index] = item;
			EditorUtility.SetDirty (database);
		}
			
		#region 最初はpublic static U GetDatabase<U>を使用していたが、ジェネリックでデータベースとクラスを統一したので不要
//		public static U GetDatabase<U> (string dbPath, string dbName) where U : ScriptableObject {
//
//			string dbFullPath = @"Assets/Yama_Database/" + dbPath + "/" + dbName;
//
//			U db = AssetDatabase.LoadAssetAtPath(dbFullPath, typeof(U)) as U;
//
//			if(db == null){
//
//				// Check to see if the folder exists
//				if(!AssetDatabase.IsValidFolder(@"Assets/Yama_Database/" + dbPath)){
//					AssetDatabase.CreateFolder (@"Assets/Yama_Database",  dbPath);
//				}
//
//				// Create the database and refresh the AssetDatabase
//				db = ScriptableObject.CreateInstance <U>() as U;
//
//				AssetDatabase.CreateAsset (db,dbFullPath);
//				AssetDatabase.SaveAssets ();
//				AssetDatabase.Refresh ();
//
//			}
//			return db;
//		}
		#endregion

		//#endif


		private void LoadDatabase () {

			// フォルダパスを指定
			string dbFullPath = @"Assets/Yama_Database/" + dbPath + "/" + dbName;

			// データベースをロード
			database = AssetDatabase.LoadAssetAtPath(dbFullPath, typeof(D)) as D;

			// もしデータベースがなければ新規作成（その処理もCreateDatabaseとして関数化）
			if(database == null){
				CreateDatabase ( dbFullPath );
			}
		}
			
		private void CreateDatabase ( string dbFullPath ) {

			// もし、フォルダもアセットもなければ
			if(!AssetDatabase.IsValidFolder(@"Assets/Yama_Database/" + dbPath)){
				AssetDatabase.CreateFolder (@"Assets/Yama_Database",  dbPath);
			}

			// Create the database and refresh the AssetDatabase
			database = ScriptableObject.CreateInstance <D>() as D;
			AssetDatabase.CreateAsset (database,dbFullPath);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();

		}
	}
}
