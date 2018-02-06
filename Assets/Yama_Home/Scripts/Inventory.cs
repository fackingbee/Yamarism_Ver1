 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#region メモ : 発案者の考えとして
// ResourceからロードするよりInspectorからアサインする方がシンプルという考えらしい
// 余裕があればResourceからロードする方式も導入出来れば良いかもしれない
// どのようにしてたくさんのスロットを作成するかという方法を先に決定する必要がある
// 兎にも角にもIDを一致させて、アイテムとスロットが常にペアであるよう心がける
#endregion
#region メモ : スクリプトの実行順
// 先にItemDatabaseクラスを実行しないとSpriteを探せないので
// Project SettingのScirpt Exeution OrderでInventoryを作って
// ItemDatabaseより後にしておく
#endregion

public class Inventory : MonoBehaviour {

	public ItemDatabase itemDatabase;			// ItemDatabaseで解析されたものを格納
	public GameObject   inventoryPanel;			// ゲーム開始時にFindする
	public GameObject   slotPanel;				// ゲーム開始時にFindする
	public GameObject   inventorySlot;			// Inspectorでアサイン(スロットを作成)
	public GameObject   inventoryItem;			// Inspectorでアサイン(アイテム自体を作成し、スロットに格納)
	public int          slotAmount;				// スロットの個数を設定
	public int          inventoryCount;			// 現在のアイテム所持数
	public int          inventoryMax;			// アイテム所持上限数
	public bool         showInventory = true;	// ???
	public bool         inventoryFull = false; 	// アイテム所持数が上限に達しているかどうかを管理するフラグ

	public List<Item>       inventoryItems = new List<Item>();		 // Itemクラスの構造体を配列格納
	public List<GameObject> inventorySlots = new List<GameObject>(); // 生成されたスロットを配列格納

	void Start () {

		inventoryPanel = GameObject.Find ("Inventory Panel");
		slotPanel      = inventoryPanel.transform.Find ("Slot Panel").gameObject;
		slotAmount     = 20;
		inventoryMax   = slotAmount;
		itemDatabase   = GetComponent<ItemDatabase>();

		for(int i = 0; i < slotAmount; i++){
			#region メモ : ここでの処理
			// 空のスロットを指定した数だけ生成し、slotPanelの子要素として設定
			// new Item()のIDの初期値が−1なのでいきなりは何も生成されず、予期せぬ発生を回避する
			// またスロットの数だけItem型クラスをインスタンス化している
			// つまりここでは15のItem型クラスを追加している
			// 単純にforのiをIDとして使用している
			// ちなみに何も所持していなければスロットのIDはすべて『-1』
			#endregion
			inventoryItems.Add(new Item());
			inventorySlots.Add (Instantiate(inventorySlot));
			inventorySlots[i].GetComponent<DropZone>().id = i;
			inventorySlots[i].transform.SetParent(slotPanel.transform);
			inventorySlots[i].name = "EmptySlot" + i.ToString();
		}

		#region メモ : AddItemの引数
		// 結局購入するときも、そのIDがわかる限り、AddItemに渡してあげれば、スロットに追加される
		#endregion
//		AddItem(0);
//		AddItem(0);
//		AddItem(1);
//		AddItem(1);
//		AddItem(1);
//		AddItem(1);
//		AddItem(1);
//		AddItem(1);
		
	}


	void Update () {
		// デバッグ用
		if(Input.GetKeyUp(KeyCode.H)){
			AddItem(0);
		
		} else if (Input.GetKeyUp(KeyCode.G)){
			AddItem(1);
		}
	}
		


	#region メソッド : アイテムを追加(ItemDatabase.FetchItemByID()のIDと一致する)
	public void AddItem(int id){

		Item itemToAdd = itemDatabase.FetchItemByID(id);

		if (inventoryCount < inventoryMax) {
			
			inventoryFull = false;

			for (int i = 0; i < inventorySlots.Count; i++) {

				if (inventorySlots[i].transform.childCount == 0) {
					GameObject itemObj = Instantiate (inventoryItem,new Vector3(-1000,-1000,-1000),Quaternion.identity);
					itemObj.GetComponent<ItemData>().item = itemToAdd;
					itemObj.GetComponent<ItemData> ().amount = 1;
					itemObj.transform.SetParent(inventorySlots[i].transform);
					itemObj.GetComponent<Image>().sprite = itemToAdd.ItemIcon;
					itemObj.name = itemToAdd.ItemName;
					inventoryCount++;
					if (inventoryCount == inventoryMax)
						inventoryFull = true; 
					break;
				}
			}
		} else if (inventoryCount == inventoryMax) {

			inventoryFull = true;
			Debug.Log("Inventory Full");

		}

		#region 過去の仕様
//		if (inventoryCount < inventoryMax) {
//
//			inventoryFull = false;
//
//			#region メモ : Part4のアイテムストック
//			// bool型のStackableでストックできるかどうか
//			// ストック数が違うアイテムに表示されないように、IDを一致させて、同スロットを取得する
//			// ゴミ箱機能を実装してからで、個人的な所持数に上限を設ける
//			// 目的の位置にストック数を表示させる
//			// 周知の通り、breakが必要な理由は一度追加したら抜ける為
//			// 同アイテムとIDが一致するスロットがすでに存在し
//			// 尚且つストック可能なアイテムなら新たにスロットを作らず
//			// 既にあるアイテムスロットのストック数に加算していく
//			// 何度もいうが、breakしないと全てのスロットにストック数が表示されてしまう
//			#endregion
//			if (itemToAdd.Stackable && CheckIfItemIsInInventory (itemToAdd)) {
//
//				for (int i = 0; i < inventoryItems.Count; i++) {
//					if (inventoryItems[i].ID == id) {
//						ItemData data = inventorySlots [i].transform.GetChild(0).GetComponent<ItemData> ();
//						data.amount++;
//						data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString ();
//						break;
//					}
//				}
//
//			} else {
//
//				#region メモ : アイテムを追加する際の考え方の手順
//				// 既にStart()でitemsにItemg型クラスを16個追加してあり
//				// その時点では全ての初期IDが-1である(空のスロット)
//				// IDが-1のときはFetchItemByIDで参照したアイテムを追加してやる
//				// ストックできないアイテム、あるいはまだそのアイテムがない場合は
//				// elseに進入してスロットを作りアイテムを格納
//				#endregion
//				for (int i = 0; i < inventoryItems.Count; i++) {
//
//					if (inventoryItems[i].ID == -1) {
//
//						//inventoryItems[i] = itemToAdd;
//
//						// 今度はアイテム自体を生成し格納していく。まず最初はからのアイテムからスタート。
//						//GameObject itemObj = Instantiate (inventoryItem); 
//
//						// 生成した空のアイテムにItemData.csにあるItem型のitemに解析されたItem型のitemToAddを格納する
//						//itemObj.GetComponent<ItemData>().item = itemToAdd;
//
//						// 二つあった場合『２』と表示ひないといけないが、実際は二つある場合の要素数が『０』『１』なので調整
//						//itemObj.GetComponent<ItemData>().amount = 1;
//
//						// ItemData.csにアイテムのIDを渡しておく。
//						//itemObj.GetComponent<ItemData>().slot = i;
//
//						// 生成したアイテムをスロットに格納
//						//itemObj.transform.SetParent (inventorySlots[i].transform);
//
//						// 位置を調整する場合はここでVectorを設定（最初に合わせておくとする必要はない）
//						//itemObj.transform.position = Vector3.zero;
//
//						// ItemDatabase.csで解析したIDに紐づくSpriteをイメージに反映
//						//itemObj.GetComponent<Image>().sprite = itemToAdd.ItemIcon;
//
//						// これがないと、Slot内のitemオブジェクト名が『Item(Cloen)』のまま
//						//itemObj.name = itemToAdd.ItemName;
//
//						#region メモ : breakが必要な理由
//						// 最初にスロットを16個分生成し、そのうちの一つにアイテムを追加する訳で、
//						// 一度スロットに追加したら残りのスロットには追加する必要がないので、
//						// ここでbreakすることでforを抜ける
//						// もしここでbreakしなければ、Addしたアイテムは全てのスロットに格納されてしまう
//						// IDが−1であるスロットという条件式だからね
//						#endregion
//						//break;
//					}
//				}
//			}
//
//		} else if (inventoryCount == inventoryMax){
//			
////			inventoryFull = true;
////			Debug.Log("Inventory Full");
//
//		}
		#endregion

	}
	#endregion 


	#region メソッド : インベントリー内アイテムとIDが一致するかどうか調べる
	public bool CheckIfItemIsInInventory (Item item) {
		for(int i = 0; i < inventoryItems.Count; i++){
			if(inventoryItems[i].ID == item.ID){
				return true;
			}
		}
		return false;
	} 
	#endregion
}
