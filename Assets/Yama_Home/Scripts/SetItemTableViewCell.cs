using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemDataTest {

	public string  iconName;	// アイコン名
	//public string  name;		// アイテム名
	//public int     price;		// 価格
	//public string  description;	// 説明
	//public string  Type;        // ショップのタイプ（道具、武具、etc...）

	// ShopItemDataのコンストラクタ化
	// この時点でハードコードで直接『new List<ShopItemData>() 』をインスタンス化することは出来ない。
//	public ShopItemData ( string iconName, string name, int price, string description, string Type ){
//
//		this.iconName    = iconName;
//		this.name        = name;
//		this.price       = price;
//		this.description = description;
//		this.Type        = Type;
//
//	}

	public ItemDataTest (string iconName){
		this.iconName    = iconName;
	}

	public ItemDataTest(){

	}
}

public class SetItemTableViewCell : TableViewCell<ItemDataTest>  {

	[SerializeField] private Image iconImage;	// アイコンを表示するイメージ

	public override void UpdateContent(ItemDataTest itemData) {


		// スプライトシート名とスプライト名を指定してアイコンのスプライトを変更する
		iconImage.sprite = SpriteSheetManager.GetSpriteByName("IconAtlas", itemData.iconName);

	}


}
