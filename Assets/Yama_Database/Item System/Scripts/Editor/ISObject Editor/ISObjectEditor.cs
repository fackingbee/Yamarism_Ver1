using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//ISQualityDatabaseEditorをコピーして同じように実装していく

namespace YamaDelta.ItemSystem.Editor{

	public partial class ISObjectEditor : EditorWindow {

		#region データベースとエディター機能を統合し、下記のような宣言方式に一本化
//		ISObjectCategory armorDatabase = new ISObjectCategory();
//		ISWeaponDatabase database;
//		const string DATABASE_NAME      = @"YamaWeaponDatabase.asset";
//		const string DATABASE_PATH      = @"Database";
//		const string DATABASE_FULL_PATH = @"Assets/Yama_Database/" + DATABASE_PATH + "/" + DATABASE_NAME;
		#endregion
		#region この段階で引数にアセット名を渡す(dbName = "weaponTest.asset" / "armorTest.asset")
		#endregion

		// the database that we will be using
		ISObjectDatabaseType<ISWeaponDatabase,ISWeapon> weaponDB = 
			new ISObjectDatabaseType<ISWeaponDatabase, ISWeapon>("weaponTest.asset");

		ISObjectDatabaseType<ISArmorDatabase,ISArmor> armorDB = 
			new ISObjectDatabaseType<ISArmorDatabase, ISArmor>("armorTest.asset");

//		ISObjectDatabaseType<ISQualityDatabase,ISQuality> qualityDB = 
//			new ISObjectDatabaseType<ISQualityDatabase, ISQuality>("YamaQualityDatabase.asset");

		// new vars for new System
		Vector2 buttonSize    = new Vector2(190, 25);
		int    _listViewWidth = 200;


		#region エディターウィンドウを作成
		// 『%#i』= shift + control + i
		// メニュー『YamaDelta』の項目を増やし、プルダウンでDatabaseを作成しようとすると
		// Windowが開く
		// minSizeが大きすぎるとTabでの格納がやりづらいからあとで微調整
		#endregion
		[MenuItem("Yama Delta/Database/Item System Editor %#i")]
		public static void Init () {
			ISObjectEditor window = EditorWindow.GetWindow<ISObjectEditor>();
			window.minSize        = new Vector2(800, 600);
			window.titleContent   = new GUIContent("Item System");
			window.Show();
		}
			
		void OnEnable () {
			#region データベースとエディター機能を統合し、下記のような宣言方式に一本化
//			if(database == null){
//				database = ISWeaponDatabase.GetDatabase<ISWeaponDatabase>(DATABASE_PATH, DATABASE_NAME);
//			}
//			armorDatabase.OnEnable ();
			#endregion
			weaponDB.OnEnable ("Weapon");
			armorDB. OnEnable ("Armor");
			tabState = TabState.QUALITY;
		}
			
		void OnGUI () {
			TopTabBar();
			GUILayout.BeginHorizontal();

			// トップタブ選択時のウィンドウ表示遷移を列挙で制御
			switch(tabState){

			case TabState.WEAPON:
				#region 別々にしていたListViewとDetailの名残で、後に機能を統合し一本化
//				ListView();
//				ItemDetails();
				#endregion
				weaponDB.OnGUI (buttonSize , _listViewWidth);
				break;
			case TabState.ARMOR:
				#region 別々にしていたListViewとDetailの名残で、後に機能を統合し一本化
//				armorDatabase.ListView (buttonSize , _listViewWidth);
//				armorDatabase.OnGUI (buttonSize , _listViewWidth);
				#endregion
				armorDB.OnGUI (buttonSize , _listViewWidth);
				break;
			case TabState.POTION:
				GUILayout.Label ("Potion");
				break;
			default:
				GUILayout.Label ("Default State - Quality : 現在何も実装されていません");
				break;
			}
			#region メモ : 開発段階でステート分けしていなかった名残で後に上記に移行
			//ListView();
			//ItemDetails();
			#endregion
			GUILayout.EndHorizontal();
			BottomStatusBar();
		}
	}
}
