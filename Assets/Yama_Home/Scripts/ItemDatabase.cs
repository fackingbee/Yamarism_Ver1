using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using LitJson;
using System.IO;

#region メモ : アイテムデータベース作成手順
/// 1.Jsonデータを解析できるようにする(LitJsonを使用する)
/// 2.Itemクラスを作成する
#endregion

public class ItemDatabase : MonoBehaviour {

	private List<Item> itemList = new List<Item>();
	private JsonData   itemData;

	void Start () {

		// StreamingAssetsフォルダ直下のJsonデータを解析する
		itemData = JsonMapper.ToObject (File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));

		// Jsonデータを解析して値を格納する
		ConstructItemDatabase ();

	}
		
	void ConstructItemDatabase (){
		for(int i = 0; i < itemData.Count; i++){

			#region メモ : オーバーロードについて
			// Jsonデータはテキストなので、型変換に注意する
			// 特に同じ文字だからと言って無視してはいけないToString()変換(多分string型キャストでも同じこと)
			// bool型も同じようにキャスト出来る
			#endregion
			#region メモ : Jsonデータが入れ子になっている場合
			// [i]["stats"]["power"]と続けて書けば良い
			#endregion

			itemList.Add (
				new Item(
					(int)   itemData[i]["id"],
					(string)itemData[i]["itemName"],
					(int)   itemData[i]["price"],
					(int)   itemData[i]["stats"]["power"],
					(int)   itemData[i]["stats"]["speed"],
					(int)   itemData[i]["stats"]["performance"],
					(string)itemData[i]["description"],
					(bool)  itemData[i]["stackable"],
					(int)   itemData[i]["rarity"],
					(string)itemData[i]["slug"]

				)
			);
		}
	}


	 //IDによってアイテムを参照・取得するメソッド
	public Item FetchItemByID(int id){
		for(int i=0; i<itemList.Count; i++){
			if(itemList[i].ID == id){
				return itemList[i];
			}
		}
		// IDがなければnullを返す。これがないとバグる。
		return null;
	}
}


#region class : Item
public class Item {
	public int    ID          { get; set; }
	public string ItemName    { get; set; }
	public int    Price       { get; set; }
	public int    Power       { get; set; }
	public int    Speed       { get; set; }
	public int    Performance { get; set; }
	public string Description { get; set; }
	public bool   Stackable   { get; set; }
	public int    Rarity      { get; set; }
	public string Slug        { get; set; }
	public Sprite ItemIcon    { get; set; }

	#region メモ : SlugとSpriteの関連性について
	// このSlugでSpriteを紐づけている
	// SlugはResourcesフォルダにあるItemのSpriteの名前と一致させている
	// Slugの名称によってPassから引っ張ってくる
	// その為、Slugは「NO Spaces」でよろ
	#endregion
	public Item (int id, string itemName, int price, int power, int speed, int performance, string description, bool stackable, int rarity, string slug){
		this.ID          = id;
		this.ItemName    = itemName;
		this.Price       = price;
		this.Power       = power;
		this.Speed       = speed;
		this.Performance = performance;
		this.Description = description;
		this.Stackable   = stackable;
		this.Rarity      = rarity;
		this.Slug        = slug;
		this.ItemIcon = Resources.Load<Sprite> ("Sprites/Items/" + slug);
	}

	public Item (int id, string itemName, int price){
		this.ID          = id;
		this.ItemName    = itemName;
		this.Price       = price;
	}

	#region メモ : 引数無しコンストラクタを用意する理由と「-1」の理由
	// インスタンス化したとき(Item item = new<Item>())のエラー回避
	// 最初何も所持していない
	// ゴミ箱で削除したり、使用してアイテムが消滅したときのエラー回避
	// つまり、空という情報の為のIDとして−１を用意しておく
	#endregion
	public Item(){
		this.ID = -1;
	}
} #endregion
