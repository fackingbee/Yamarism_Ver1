using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#region メモ : Databaseフォルダを自動生成しているが...
// フォルダを先に作っておいて、固定とするならここまでする必要はないかもしれないが、
// あくまで全自動生成を目指しているので、このまま進むことにする
// とにかく、空のプロジェクト(もしくわデータベースのない途中のプロジェクト)にデータベースを作ろうとする際にこの拡張を使用する
#endregion

namespace YamaDelta.ItemSystem.Editor{

	public partial class ISQualityDatabaseEditor : EditorWindow {

//		ISObjectDatabaseType<> weaponDB = 
//			new ISObjectDatabaseType<>("QualityDatabase.asset");

		ISQualityDatabase qualityDatabase;
		Texture2D selectedTexture;
		int selectedIndex = -1;
		Vector2  _scrollPos;				// scroll position for the ListView

		const int    SPRITE_BUTTON_SIZE   = 46;
		const string DATABASE_NAME        = @"YamaQualityDatabase.asset";
		const string DATABASE_PATH        = @"Database";
//		const string DATABASE_FULL_PATH   = @"Assets/Yama_Database/" + DATABASE_PATH + "/" + DATABASE_NAME;


		#region エディターウィンドウを作成
		// 『%#i』= shift + control + w
		// メニュー『YamaDelta』の項目を増やし、プルダウンでDatabaseを作成しようとすると
		// Windowが開く
		#endregion
		[MenuItem("Yama Delta/Database/Quality Editor %#w")]
		public static void Init () {
			ISQualityDatabaseEditor window = EditorWindow.GetWindow<ISQualityDatabaseEditor>();
			window.minSize                 = new Vector2   (400, 300);
			window.titleContent            = new GUIContent("Quality Database");
			window.Show ();
		}


		#region OnEnableなのは...
		// データベース作成ウィンドウを開いたらときに呼ばれるということ
		#endregion
		void OnEnable(){

			// 基底クラスのGetDatabaseをstaticにしたのでこちらは不要
			//qualityDatabase = ScriptableObject.CreateInstance<ISQualityDatabase>();

			if(qualityDatabase == null){
				qualityDatabase = ISQualityDatabase.GetDatabase<ISQualityDatabase>(DATABASE_PATH, DATABASE_NAME);
			}
			#region メモ : ここでの流れ
			// 指定したフォルダにあるISQualityDatabaseを（この段階では単なるデータ）
			// ISQualityDatabase型オブジェクトとしてロード
			// 指定したフォルダにデータベース用データがなければ
			// 指定したフォルダに作成する
			// AssetDatabase.Refresh ();は今の所呪文のように覚えておく
			#endregion
			#region メモ : ここに記載したものはジェネリッククラスに変更した
//			qualityDatabase = AssetDatabase.LoadAssetAtPath (DATABASE_FULL_PATH, typeof(ISQualityDatabase)) as ISQualityDatabase;
//
//			if(qualityDatabase == null){
//
//				if(!AssetDatabase.IsValidFolder("Assets/Yama_Database/" + DATABASE_FOLDER_NAME)){
//					AssetDatabase.CreateFolder ("Assets/Yama_Database",  DATABASE_FOLDER_NAME);
//				}
//
//				qualityDatabase = ScriptableObject.CreateInstance<ISQualityDatabase>();
//				AssetDatabase.CreateAsset (qualityDatabase,DATABASE_FULL_PATH);
//				AssetDatabase.SaveAssets ();
//				AssetDatabase.Refresh ();
//
//			}
//
//			// ISQualityに値を格納するために、まず空の箱をインスタンス化
//			selectedItem = new ISQuality ();
			#endregion
		}


		// 開いたウィンドウの表示内容
		void OnGUI(){
			
			if(qualityDatabase == null){
				Debug.LogWarning ("qualityDatabase not loaded");
				return;
			}

			ListView ();
			GUILayout.BeginHorizontal("Box",GUILayout.ExpandWidth(true));
			BottomBar();
			GUILayout.EndHorizontal ();

		}


		void BottomBar(){

			// count : ウィンドウの下に格納しているアイテム数と追加ボタンを付加
			GUILayout.Label("Qualities : " + qualityDatabase.Count);

			// add button
			if(GUILayout.Button("Add")){
				qualityDatabase.Add (new ISQuality());
			}
		}
		// Qualityの情報をデータベースに追加するメソッド : 現在非使用
//		void AddQualityToDatabase(){
//			
//			#region メモ : まずNameとSpriteを表示させる
//			// タイプした名前をISQuality型のselectedItemという入れ物に値として保持させる。
//			// Texture選択はボタンを押した後、選択メニューウィンドでやるため、ここではボタンを使用する。
//			// EditorGUIUtility.ShowObjectPickerの段階では選択するウィンドウが開くだけでセーブ出来ていない。
//			// Repaint ();で反映させる
//			//"ObjectSelectorUpdated"は綴りを間違いやすいので注意。特に最後のdをつけ忘れる
//			#endregion
//			selectedItem.Name = EditorGUILayout.TextField ("Name : ", selectedItem.Name);
//
//			if (selectedItem.Icon) {
//				selectedTexture = selectedItem.Icon.texture;
//			} else {
//				selectedTexture = null; // これがないとTexture反映を空に出来ない
//			}
//
//			if (GUILayout.Button (selectedTexture, GUILayout.Width (SPRITE_BUTTON_SIZE), GUILayout.Height (SPRITE_BUTTON_SIZE))) {
//				int controllerID = EditorGUIUtility.GetControlID (FocusType.Passive);
//				EditorGUIUtility.ShowObjectPicker<Sprite> (null, true,null,controllerID);
//			}
//			string commandName = Event.current.commandName;
//
//			if(commandName ==  "ObjectSelectorUpdated"){
//				selectedItem.Icon = (Sprite)EditorGUIUtility.GetObjectPickerObject ();
//				Repaint ();
//			}
//
//			if (GUILayout.Button ("Save")) {
//				
//				if(selectedItem == null){
//					return;
//				}
//
//				#region メモ : 保存する際にISQualityDのdatabaseに格納する
//				// ScriptableObjectである、ISQualityD databaseの、Listであるdatabaseに格納保存
//				// qualityDatabase.database.Add (selectedItem); privateなのでこういう形ではアクセス出来ない
//				// もし名前をつけなければ、qualityDatabaseには加えずreturnする。
//				#endregion
//				if(selectedItem.Name == ""){
//					Debug.LogError ("名前を入力して下さい");
//					return;
//				}
//
//				qualityDatabase.Add(selectedItem);
//				int x = qualityDatabase.Count;
//
//				selectedItem = new ISQuality();
//			}
//		}
	}
}
