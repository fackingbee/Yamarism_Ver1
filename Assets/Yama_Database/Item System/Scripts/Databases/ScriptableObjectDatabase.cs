
#region ISObjectDatabaseMethodsへお引越し
#if UNITY_EDITOR
using UnityEditor;
 #endif
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // ElementAtが必要な為

namespace YamaDelta {

	public class ScriptableObjectDatabase<T> : ScriptableObject where T : class {

		[SerializeField] protected List<T> item = new List<T>();

		// 派生先で呼ぶ為のプロパティを設定
		public List<T> Item {
			get{ return item; }
		}

		// こちらはビルド時も必要
		public int Count {
			get{ return item.Count; }
		}

		public T Get (int index) {
			return item.ElementAt (index);
		}


		// これらの関数はEditor上で操作する為だけ必要で、SetDirty等はBuild時にエラーが出るので分ける
#if UNITY_EDITOR
		public void Add (T i) {
			item.Add (i);
			EditorUtility.SetDirty (this);
		}

		public void Insert (int index, T i) {
			item.Insert (index, i);
			EditorUtility.SetDirty (this);
		}

		public void Remove (T i) {
			item.Remove (i);
			EditorUtility.SetDirty (this);
		}

		public void Remove (int index) {
			item.RemoveAt(index);
			EditorUtility.SetDirty (this);
		}
#endif

// こちらも同様にアイテム登録は開発時にだけ必要で、ScriptableObjectはbuild時にエラーが出るので分ける
#if UNITY_EDITOR

		public void Replace (int index, T i) {
			item[index] = i;
			EditorUtility.SetDirty (this);
		}

		public static U GetDatabase<U> (string dbPath, string dbName) where U : ScriptableObject {

			string dbFullPath = @"Assets/Yama_Database/" + dbPath + "/" + dbName;

			U db = AssetDatabase.LoadAssetAtPath(dbFullPath, typeof(U)) as U;

			if(db == null){

				// Check to see if the folder exists
				if(!AssetDatabase.IsValidFolder(@"Assets/Yama_Database/" + dbPath)){
					AssetDatabase.CreateFolder (@"Assets/Yama_Database",  dbPath);
				}

				// Create the database and refresh the AssetDatabase
				db = ScriptableObject.CreateInstance <U>() as U;

				AssetDatabase.CreateAsset (db,dbFullPath);
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();

			}
			return db;
		}

#endif

	}
}
