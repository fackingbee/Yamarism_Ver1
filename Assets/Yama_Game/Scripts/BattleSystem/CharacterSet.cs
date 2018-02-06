using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;


//public class CharacterSetManager {
//
//	// プレイヤーセット用変数
//	public static Dictionary<string, Dictionary<string,GameObject>> playerObject = 
//		new Dictionary<string, Dictionary<string, GameObject>>();
//
//	// 敵セット用変数
//	public static Dictionary<string, Dictionary<string,GameObject>> enemyObject = 
//		new Dictionary<string, Dictionary<string, GameObject>>();
//
//	// プレイヤーをロード
//	public static void CharacterLoad(string playerPath, string enemyPath) {
//		if(!playerObject.ContainsKey(playerPath)){
//			playerObject.Add (playerPath, new Dictionary<string, GameObject>());
//			enemyObject.Add  (enemyPath,  new Dictionary<string, GameObject>());
//		}
//
//		// Resourceフォルダ内のそれぞれのパスにあるオブジェクトを配列として格納
//		GameObject[] playerSet = Resources.LoadAll<GameObject> (playerPath);
//		GameObject[] enemySet  = Resources.LoadAll<GameObject> (enemyPath);
//	
//		// Resourceフォルダのプレイヤーに関するオブジェクトを追加
//		foreach(GameObject playerObj in playerSet){
//			if(!playerObject[playerPath].ContainsKey(playerObj.name)){
//				playerObject [playerPath].Add (playerObj.name, playerObj);
//			}
//		}
//
//		// Resourceフォルダの敵に関するオブジェクトを追加
//		foreach(GameObject enemyObj in enemySet){
//			if(!enemyObject[enemyPath].ContainsKey(enemyObj.name)){
//				enemyObject [enemyPath].Add (enemyObj.name, enemyObj);
//			}
//		}
//	}
//
//	public static GameObject GetCharacterByName(string path, string name){
//
//	}
//
//}

// 他シーンからキャラクターの状態を決める。
// 例えば、メニュー画面から遷移する際に難易度、敵のレベル、自分のレベルをセットする。
public static class CharacterData{
	public static string preEnemyName;
	public static int    prePlayerLV;
	public static int    preEnemyLV;
}

// プレイヤーデータクラス ------------------------------------------------------------------
public class PlayerData{

	public string iconName;
	public string objectName;
	public int    charaType;
	public int    charaLV;

	// コンストラクタ（引数なし）
	public PlayerData(){
	}

	// コンストラクタ（引数あり）
	public PlayerData(string iconName, string objectName, int charaType, int charaLV){
		this.iconName   = iconName;
		this.objectName = objectName;
		this.charaType  = charaType;
		this.charaLV    = charaLV;

	}
}

// 敵データクラス ------------------------------------------------------------------------
public class EnemyData{

	public string iconName;
	public string objectName;
	public int    stageType;
	public int    charaType;
	public int    charaLV;

	// コンストラクタ（引数なし）
	public EnemyData(){
	}

	// コンストラクタ（引数あり）
	public EnemyData(string iconName, string objectName, int stageType, int charaType, int charaLV){
		this.iconName   = iconName;
		this.objectName = objectName;
		this.stageType  = stageType;
		this.charaType  = charaType;
		this.charaLV    = charaLV;
	}
}

// ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
// ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

public class CharacterSet : MonoBehaviour {

	// プレイヤーセット用変数
	public Dictionary<string, Dictionary<string,GameObject>> playerObject = 
		new Dictionary<string, Dictionary<string, GameObject>>();

	// 敵セット用変数
	public Dictionary<string, Dictionary<string,GameObject>> enemyObject = 
		new Dictionary<string, Dictionary<string, GameObject>>();

	// キャラクターデータ格納変数
	public List<PlayerData>         playerDataList;
	public List<EnemyData>          enemyDataList;
	public List<SelectedEnemyData>  selectedEnemyList;

	// jsonファイル格納変数
	[Header("JSON Setting")]
	public  TextAsset  characterData;

	// 名前による敵選択変数
	[Space(16)]
	[Header("Select Setting")]
	public string enemySelect;

	#region キャラクターオブジェクト格納変数
	// デバッグの為Inspectorに表示
	// setPlayerObjectNameと一致しているか確認
	// 一旦インスペクターから設定出来るようにしておく
	#endregion
	[Space(16)]
	[Header("Enemy Setting")]
	public string setEnemyName;
	public string setEnemyObjectName;
	public int    setEnemyLV;
	[SerializeField] private GameObject enemyPrefab;

	[Space(16)]
	[Header("Player Setting")]
	public string setPlayerName;
	public string setPlayerObjectName;
	public int    setPlayerLV;
	[SerializeField] private GameObject playerPrefab;

	// 解析されたjson中のプレイヤーと敵のデータ用変数
	private PlayerData playerData;
	private EnemyData  enemyData;


// ------------------------------------------------------------------------------------------------------
	void Awake () {

		#region デバッグ用ハードコードテーブルデータ
//		// 一旦ハードコードでプレイヤーデータを格納
//		playerDataList = new List<PlayerData> () {
//			new PlayerData{iconName="ルシウス",objectName="PlayerLV1",charaType=0,charaLV=1},
//			new PlayerData{iconName="ルシウス",objectName="PlayerLV2",charaType=0,charaLV=2},
//			new PlayerData{iconName="ルシウス",objectName="PlayerLV3",charaType=0,charaLV=3},
//			new PlayerData{iconName="ルシウス",objectName="PlayerLV4",charaType=0,charaLV=4}
//		};
//
//		// 一旦ハードコードで敵データを格納
//		enemyDataList = new List<EnemyData> () {
//			new EnemyData{iconName="レディー・グレイ",objectName="LadyGlayLV1",stageType=1,charaType=1,charaLV=1},
//			new EnemyData{iconName="レディー・グレイ",objectName="LadyGlayLV2",stageType=2,charaType=1,charaLV=2},
//			new EnemyData{iconName="レディー・グレイ",objectName="LadyGlayLV3",stageType=3,charaType=1,charaLV=3},
//			new EnemyData{iconName="レディー・グレイ",objectName="LadyGlayLV4",stageType=4,charaType=1,charaLV=4},
//			new EnemyData{iconName="ジャンヌ",objectName="JanneLV1",stageType=1,charaType=1,charaLV=1},
//			new EnemyData{iconName="ジャンヌ",objectName="JanneLV2",stageType=2,charaType=1,charaLV=2},
//			new EnemyData{iconName="ジャンヌ",objectName="JanneLV3",stageType=3,charaType=1,charaLV=3},
//			new EnemyData{iconName="ジャンヌ",objectName="JanneLV4",stageType=4,charaType=1,charaLV=4}
//		}; 


//		enemySelect = CharacterData.preEnemyName;
//		setPlayerLV = CharacterData.prePlayerLV;
//		setEnemyLV  = CharacterData.preEnemyLV;
		#endregion

		// キャラクターデータをjsonから解析して追加
		CharacterJsonDataLoad ();
			
		// キャラクターをResourcesフォルダからロード (引数は現状定数)
		CharacterLoad ("CharacterPlayer","CharacterEnemy");

		// ロードされた敵データを選択 : デバッグではInspectorから設定
		SelectEnemy (enemySelect);

	}
		

// -----------------------------------------------------------------------------------------
	void Start () {
		
		// キャラクターをセット
		SetCharacter();

	}


// キャラクターをロードするメソッド -------------------------------------------------------------
	public void CharacterLoad(string playerPath, string enemyPath) {
		if(!playerObject.ContainsKey(playerPath)){
			playerObject.Add (playerPath, new Dictionary<string, GameObject>());
			enemyObject.Add  (enemyPath,  new Dictionary<string, GameObject>());
		}

		// Resourceフォルダ内のそれぞれのパスにあるオブジェクトを配列として格納
		GameObject[] playerSet = Resources.LoadAll<GameObject> (playerPath);
		GameObject[] enemySet  = Resources.LoadAll<GameObject> (enemyPath);

		// Resourceフォルダのプレイヤーに関するオブジェクトを追加
		foreach(GameObject playerObj in playerSet){
			if(!playerObject[playerPath].ContainsKey(playerObj.name)){
				playerObject [playerPath].Add (playerObj.name, playerObj);
			}
		}

		// Resourceフォルダの敵に関するオブジェクトを追加
		foreach(GameObject enemyObj in enemySet){
			if(!enemyObject[enemyPath].ContainsKey(enemyObj.name)){
				enemyObject [enemyPath].Add (enemyObj.name, enemyObj);
			}
		}
	}


// キャラクターのJsonファイルをロードするメソッド -------------------------------------------------------------
	public void CharacterJsonDataLoad(){

		string jsonIconName;
		string jsonObjectName;
		int    jsonStageType;
		int    jsonCharaType;
		int    jsonCharaLV;

		playerDataList = new List<PlayerData> ();
		enemyDataList  = new List<EnemyData> ();

		IDictionary charaDataDic  = (IDictionary)Json.Deserialize (characterData.text);
		List<object> arrayData    = (List<object>)charaDataDic["CharacterData"];

		foreach(IDictionary charaVal in arrayData){

			// ["charaType"] == 0（プレイヤーの識別番号）
			// long型に直してからint型に変換する必要があるのは、どうやら仕様らしい
			if((int)(long)charaVal["charaType"] == 0){

				jsonIconName   = (string)charaVal["iconName"];
				jsonObjectName = (string)charaVal["objectName"];
				//jsonStageType  = (int)(long)charaVal["stageType"];
				jsonCharaType  = (int)(long)charaVal["charaType"];
				jsonCharaLV    = (int)(long)charaVal["charaLV"];

				playerData = new PlayerData (jsonIconName, jsonObjectName, jsonCharaType, jsonCharaLV);
				playerDataList.Add (playerData);
			}

			// ["charaType"] == 1（敵の識別番号）
			if((int)(long)charaVal["charaType"] == 1){

				jsonIconName   = (string)charaVal["iconName"];
				jsonObjectName = (string)charaVal["objectName"];
				jsonStageType  = (int)(long)charaVal["stageType"];
				jsonCharaType  = (int)(long)charaVal["charaType"];
				jsonCharaLV    = (int)(long)charaVal["charaLV"];

				enemyData = new EnemyData (jsonIconName, jsonObjectName, jsonStageType, jsonCharaType, jsonCharaLV);
				enemyDataList.Add (enemyData);
			}
		}
	}


// 先に敵を選択するメソッド -----------------------------------------------------------------
	public void SelectEnemy(string select){

		string name = select;

		selectedEnemyList = new List<SelectedEnemyData> ();

		// EnemyDataから選択された敵データをSelectedEnemyDataにコピー
		for(int i = 0; i < enemyDataList.Count; i++){
			if(enemyDataList[i].iconName == name){
				selectedEnemyList.Add (
					new SelectedEnemyData (
						enemyDataList[i].iconName, 
						enemyDataList[i].objectName, 
						enemyDataList[i].charaLV
					)
				);
			}
		}
	}


// キャラクターをセットするメソッド ----------------------------------------------------------------
	public void SetCharacter(){

		// プレイヤーリストからLVに応じたオブジェクト名をセット
		for(int i = 0; i < playerDataList.Count; i++){
			if(playerDataList[i].charaLV == setPlayerLV){
				setPlayerObjectName = playerDataList [i].objectName;
				setPlayerName       = playerDataList [i].iconName;
				break;
			}
		}

		// 『選択された』敵リスト(selectedEnemyList)からLVに応じたオブジェクト名をセット
		for(int i = 0; i < selectedEnemyList.Count; i++){
			if(selectedEnemyList[i].selectedEnemyLV == setEnemyLV){
				setEnemyObjectName = selectedEnemyList [i].selectedEnemyObjectName;
				setEnemyName       = selectedEnemyList [i].selectedEnemyName;
				break;
			}
		}
		// それぞれのLVに応じたオブジェクト名を渡す
		SetCharacterObject (setPlayerObjectName,setEnemyObjectName);
	}


// オブジェクト名からキャラクターを生成するメソッド -----------------------------------------------------
	public void SetCharacterObject(string playerObjName, string enemyObjName){
		
		// キャラクターを格納 : 第二引数を変数化
		playerPrefab = GetCharaObjByName ("CharacterPlayer", playerObjName);
		enemyPrefab  = GetCharaObjByName ("CharacterEnemy",  enemyObjName );

		// キャラクターを生成
		GameObject player = Instantiate (playerPrefab);
		GameObject enemy  = Instantiate (enemyPrefab );

		// キャラクターを子要素としてセット
		player.transform.parent = transform;
		enemy. transform.parent = transform;
	}


// ロードしたキャラクターオブジェクトを返すメソッド ----------------------------------------------------
	public GameObject GetCharaObjByName(string path, string name){
		if(playerObject.ContainsKey(path) && playerObject[path].ContainsKey(name)){
			return playerObject [path] [name];
		}
		if(enemyObject.ContainsKey(path) && enemyObject[path].ContainsKey(name)){
			return enemyObject [path] [name];
		}
		return null;
	}


// 選択された敵の情報だけを一旦構造体にコピーする -------------------------------------------------------
	public struct SelectedEnemyData{

		public string selectedEnemyName;
		public string selectedEnemyObjectName;
		public int    selectedEnemyLV;

		public SelectedEnemyData(string selectedEnemyName, string selectedEnemyObjectName, int selectedEnemyLV){
			this.selectedEnemyName       = selectedEnemyName;
			this.selectedEnemyObjectName = selectedEnemyObjectName;
			this.selectedEnemyLV         = selectedEnemyLV;

		}
	}
}
