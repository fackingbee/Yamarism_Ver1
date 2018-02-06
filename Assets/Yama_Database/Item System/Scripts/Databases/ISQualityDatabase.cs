
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#region メモ : 用語
// EditorUtility.SetDirty : アセットが更新されたことをエディターに通知する
// つまり、アセットの値を変更した時はかならずEditorUtility.SetDirtyを呼び出す
#endregion

namespace YamaDelta.ItemSystem{

	public class ISQualityDatabase : ScriptableObjectDatabase<ISQuality> {

		#region メモ : 基底クラスScriptableObjectDatabase.csを作成し、型を指定して継承するジェネリッククラスに変更
//		//[SerializeField]
//		//public List<ISQuality> database = new List<ISQuality>();
//
//		[SerializeField]
//		List<ISQuality> database = new List<ISQuality>();
//
//		public void Add (ISQuality item) {
//			database.Add (item);
//			EditorUtility.SetDirty (this);
//		}
//
//		public void Insert (int index, ISQuality item) {
//			database.Insert (index, item);
//			EditorUtility.SetDirty (this);
//		}
//
//		public void Remove (ISQuality item) {
//			database.Remove (item);
//			EditorUtility.SetDirty (this);
//		}
//
//		public void Remove (int index) {
//			database.RemoveAt(index);
//			EditorUtility.SetDirty (this);
//		}
//
//		public int Count {
//			get{ return database.Count; }
//		}
//
//		public ISQuality Get (int index) {
//			return database.ElementAt (index);
//		}
//
//		public void Replace (int index, ISQuality item) {
//			database [index] = item;
//			EditorUtility.SetDirty (this);
//		}
		#endregion

		#region メモ : protected修飾子
		// protectedメンバーは派生クラスから見るとpublicだが、それ以外のメンバーからはprivate扱いになる
		// ジェネリックで定義したListのdatabaseをISQualityで弄っても、ISObject等で継承したdatabaseには干渉しない
		#endregion

		/*
		 * have a method that checks to see if we have a data base created
		 * if we do not, then create it and add at least one entry.
		 */

		public int GetIndex ( string name ) {
			
			#region メモ : ラムダ式で記述
			// データベースのある要素の名前が、指定された名前と同じなら
			// メソッドの型からaはint型だとわかる
			// 要素数aのNameとnameが同じなら、aというindexを返してあげる
			#endregion
			return item.FindIndex( a => a.Name==name );

		}

	}
}
