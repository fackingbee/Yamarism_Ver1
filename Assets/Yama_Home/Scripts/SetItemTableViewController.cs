using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;


[RequireComponent(typeof(ScrollRect))]
public class SetItemTableViewController : TableViewControllerHorizontal<ItemDataTest> {


	// インスタンスのロード時に呼ばれる【TableViewControllerにてvirtualで許可】
	protected override void Awake(){

		// ベースクラスのAwakeメソッドを呼ぶ【TableViewControllerのvirtual Awake】
		base.Awake();

		// アイコンのスプライトシートに含まれるスプライトをキャッシュしておく【何度も呼ばない、一度呼んだら使い回す】
		SpriteSheetManager.Load("IconAtlas");

	}


	// インスタンスのロード時Awakeメソッドの後に呼ばれる
	protected override void Start(){

		// ベースクラスのStartメソッドを呼ぶ
		base.Start();

		// リスト項目のデータを読み込む
		LoadData();

//		//アイテム一覧画面をナビゲーションビューに対応させる
//		if(navigationView != null) {
//
//			// ナビゲーションビューの最初のビューとして設定する
//			navigationView.Push(this);
//
//		}

	}

	
	private void LoadData(){

		tableData_H = new List<ItemDataTest> () { 

			new ItemDataTest { iconName="drink1"}, 
			new ItemDataTest { iconName="drink2"}, 
			new ItemDataTest { iconName="drink3"}, 
			new ItemDataTest { iconName="drink4"}, 
			new ItemDataTest { iconName="drink5"}, 
			new ItemDataTest { iconName="drink6"}, 
			new ItemDataTest { iconName="fruit1"}, 
			new ItemDataTest { iconName="fruit2"}, 
			new ItemDataTest { iconName="fruit3"}, 
			new ItemDataTest { iconName="fruit4"}, 
			new ItemDataTest { iconName="fruit5"}, 
			new ItemDataTest { iconName="fruit6"}, 
			new ItemDataTest { iconName="gun1"}, 
			new ItemDataTest { iconName="gun2"}, 
			new ItemDataTest { iconName="gun3"}, 
			new ItemDataTest { iconName="gun4"}, 
			new ItemDataTest { iconName="gun5"}, 
			new ItemDataTest { iconName="gun6"}, 
			
		};

		UpdateContents_H();

	}

	protected override float CellWidthAtIndex(int index) {

//		// ここは自身のゲームのUIに合わせて、微調整しなければいけない
//		// 下記は完全に冗長だが、セルの高さは視認性上変えないつもりでも、
//		// ランクなど価格によってなにか条件をつけるかもしれないので、現状このままにしておく。
//		if(index >= 0 && index <= tableData.Count-1){
//
//			if(tableData[index].price >= 1000){
//				// 価格が1000以上のアイテムを表示するセルの高さを返す
//				return 140.0f;
//			}
//
//			if(tableData[index].price >= 500){
//				// 価格が500以上のアイテムを表示するセルの高さを返す
//				return 140.0f;
//			}
//		}

		return 100.0f;
	}
}
