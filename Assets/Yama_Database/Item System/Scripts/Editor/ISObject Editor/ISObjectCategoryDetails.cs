using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ISObjectCategoryDetails.cs

namespace YamaDelta.ItemSystem.Editor {

	#region public partial class ISObjectCategory { (particalクラスで一本化し抽象化してデータベースとエディター機能を統合)
	#endregion
	public partial class ISObjectDatabaseType<D,T> where D : ScriptableObjectDatabase<T> where T : ISObject, new() {

		public void ItemDetails () {

			GUILayout.BeginVertical("BOX",GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical(      GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));

			#region メモ : if文にしないといけない理由
			// 下記を見ればわかるように、CreateItemButton()が実行されるまで、tempArmorがインスタンス化されないので
			// トップバーでArmorを選んでも何も参照されておらず、エラーになる
			// よってCreateボタンが押されるまで（showDetailsがtrueになるまで）はtempArmor.OnGUI ()を実行しないようにする
			// このようにクラスをインスタンスしたりする場合、nullやエラー時の条件式による回避を意識しておく
			#endregion
			if(showDetails){
				tItem.OnGUI ();
			}

			GUILayout.EndVertical();
			GUILayout.Space (50);
			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			DisplayButtons ();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

		}

		void DisplayButtons () {

			if (showDetails) {
				ButtonSave ();

				#region : Deleteボタンの表示
				// Createボタンを押した際にDetailウィンドウが表示された時はまだ『_selectedIndex』は-1のままで
				// その時にはDeleteボタンは非表示
				// すでに登録されているアイテムのとき（つまりList Viewから選択したとき）だけ表示
				#endregion
				if(_selectedIndex > -1){
					ButtonDelete ();
				}

				ButtonCancel ();

			} else {
				ButtonCreate ();
			}
		}


		void ButtonCreate () {

			if (GUILayout.Button ("Create " + strItemType)) {

				#region 最終的にジェネリックで抽象化して統合
				// インスタンス
				//tItem = new ISArmor ();
				#endregion
				tItem = new T();

				// 次の画面に遷移する許可を出す
				showDetails = true;

			} 
		}


		void ButtonSave () {
			GUI.SetNextControlName("SaveButton");
			if (GUILayout.Button ("Save")) {

				// Save Item
				if (_selectedIndex == -1) {
					// すでに機能を追加してあるので指定する必要はなくなる
//					Database.Item.Add (tempArmor);
					 Add (tItem);
				} else {
//					Database.Replace (_selectedIndex, tempArmor);
					Replace (_selectedIndex, tItem);
				}
				
				showDetails    = false;
				_selectedIndex = -1;
				tItem          = null;
				GUI.FocusControl ("SaveButton");

			}
		}

		void ButtonCancel () {
			if (GUILayout.Button("Cancel")) {
				tItem          = null;
				showDetails    = false;
				_selectedIndex = -1;
				GUI.FocusControl ("SaveButton");
			}

		}

		void ButtonDelete () {

			if (GUILayout.Button ("Delete")) {

				if (EditorUtility.DisplayDialog (
					"Delete " + tItem.Name,
					"Are you sure that you want to delete 『 " + tItem.Name + " 』from the database", 
					"Delete", 
					"Cancel")) 
				{
					
					// データベースから削除
//					Database.Remove (_selectedIndex); 
					Remove (_selectedIndex);

					// 以下同様
					showDetails = false;
					tItem = null;
					_selectedIndex = -1;
					//state = DisplayState.NONE;
					GUI.FocusControl ("SaveButton");
				}
			}
		}
	}
}