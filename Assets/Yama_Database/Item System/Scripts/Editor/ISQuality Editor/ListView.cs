using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#region メモ : 
// 一つのスクリプトが膨大になってきた時、一度に全て見る必要がない場合に、クラスを分割定義する為に、partialを使用する
#endregion

namespace YamaDelta.ItemSystem.Editor{

	public partial class ISQualityDatabaseEditor {

		// List all of the stored qualities in the dagabase
		void ListView () {
			
			//GUILayout.Label ("Displayed");

			#region メモ : スクロールを可能にする為に...
			// 右オペランドだけでは表示するだけで、
			// _scrollPosに代入することで初めてスクロールが可能となる
			#endregion
			_scrollPos = EditorGUILayout.BeginScrollView (_scrollPos, GUILayout.ExpandHeight(true));
			DisplayQualities ();
			EditorGUILayout.EndScrollView ();

		}

		#region メモ : qualityDatabaseに追加・保存された情報の一覧を表示する
		// partialクラスなのでISQualityDatabaseEditorのグローバル変数qualityDatabaseを呼べる
		// ISQualityDatabaseEditorのAddQualityToDatabaseと同じ要領で作る
		// そのままだと選択したIconがすべてのデータに反映されてしまうので、
		// selectedIndex = -1 を設けて回避する
		#endregion
		void DisplayQualities () {
			
			for(int cnt = 0; cnt < qualityDatabase.Count; cnt++){

				// 引数でスタイルを指定する
				GUILayout.BeginHorizontal ("Box");

				// sprite
				if (qualityDatabase.Get (cnt).Icon) {
					selectedTexture = qualityDatabase.Get(cnt).Icon.texture;
				} else {
					selectedTexture = null;
				}

				if(GUILayout.Button (selectedTexture, GUILayout.Width (SPRITE_BUTTON_SIZE), GUILayout.Height (SPRITE_BUTTON_SIZE))){
					int controllerID = EditorGUIUtility.GetControlID (FocusType.Passive);
					EditorGUIUtility.ShowObjectPicker<Sprite> (null, true,null,controllerID);
					selectedIndex = cnt;
				}

				string commandName = Event.current.commandName;

//*				// We will do it later when we create the state-machine.

				if(commandName ==  "ObjectSelectorUpdated"){
					
					if(selectedIndex != -1){

						qualityDatabase.Get(selectedIndex).Icon = (Sprite)EditorGUIUtility.GetObjectPickerObject ();
						//selectedIndex = -1;
						
					}
					Repaint ();
				}

				GUILayout.BeginVertical();
				// name
				qualityDatabase.Get(cnt).Name = GUILayout.TextField(qualityDatabase.Get(cnt).Name);

				// delete button
				if(GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(25))){
					if(EditorUtility.DisplayDialog ( "Delete Quality",
													 "Are you sure that you want to delete" + qualityDatabase.Get(cnt).Name + "from the database", 
													 "Delete", 
													 "Cancel" ))
					{
						qualityDatabase.Remove(cnt);
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical  ();
			}
		}
	}
}
