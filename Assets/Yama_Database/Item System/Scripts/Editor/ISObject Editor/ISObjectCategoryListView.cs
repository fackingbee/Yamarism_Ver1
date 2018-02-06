using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ISObjectCategoryListView.cs

namespace YamaDelta.ItemSystem.Editor {
	
	#region public partial class ISObjectCategory { (particalクラスでISObjectDatabaseTypeとして一本化し、抽象化してデータベースとエディター機能を統合)
	#endregion
	public partial class ISObjectDatabaseType<D,T> where D : ScriptableObjectDatabase<T> where T : ISObject, new() {
			
		private T        tItem;     // = new ISArmor();	// atemp holder for the item we are working on
		private Vector2 _scrollPos     = Vector2.zero;	// the pos of the scrollbar for the Listview
		private int     _selectedIndex = -1; 			// -1 means that we have nothing selected
		private bool     showDetails   = false;			// flat to show that we should be showing the item details


		public void ListView (Vector2 buttonSize ,int _listViewWidth) {

			_scrollPos =
				GUILayout.BeginScrollView (
					_scrollPos, "Box",GUILayout.ExpandHeight(true),GUILayout.Width(_listViewWidth)
				);

			for(int cnt = 0; cnt < database.Count; cnt++){
				if (GUILayout.Button (database.Get (cnt).Name, "box", GUILayout.Width (buttonSize.x), GUILayout.Height (buttonSize.y))) {
					
					_selectedIndex = cnt;
					tItem = new T();
					tItem.Clone(database.Get(cnt));
					showDetails = true; 

					#region バグ内容 : 直接ISWeapon等にアクセスしてはいけなくて、あくまでISObjectを経由する
					// これがないと、ListViewから選択された時に初期値しか表示されない
					// しかし、new T(database.Get (cnt))と記述しないと、値が保留されず直接データベースを書き換えてしまう
					// つまり、Cancelが効かない
					// だが、ジェネリックのコンストラクタは引数を持てない
					// さてどうしたものか。
					//tItem = database.Get(cnt);
					#endregion
					#region 修正内容 : そのため、ISObjectを経由して派生先で安全にキャストする方式とする
					#endregion

				}
			}
			GUILayout.EndScrollView ();
		}
	}
}