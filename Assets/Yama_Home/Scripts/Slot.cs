using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour,IDropHandler {

	#region 変数
	public  int       id;	// スロット生成時に各スロットにIDを付加しておく
	private Inventory inv;	// Findするか、publicでInspectorからアタッチするか
	#endregion

	#region MonoBehavior
	void Start () {
		inv = GameObject.Find ("Inventory").GetComponent<Inventory>();
	}
	#endregion

	#region メソッド : OnDrop
	public void OnDrop(PointerEventData eventData){

		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData> ();

		// Inventory内のドロップ先スロットが空の場合はそのまま格納
		if (inv.inventoryItems[id].ID == -1) {

			// ドロップ先の空のスロットにItemクラスをインスタンス
			inv.inventoryItems[droppedItem.slot] = new Item ();

			// ここでのidはドロップ先のスロットの要素番号で、
			inv.inventoryItems[id] = droppedItem.item;
			droppedItem.slot = id;// IDを格納

			//Debug.Log (droppedItem.slot);
			//Debug.Log (id);

		
		// Inventory内のドロップ先スロットに既にアイテムがある場合は交換
		} else if (droppedItem.slot != id) {

			//Debug.Log ("droppedItem.slot : " + droppedItem.slot);
			//Debug.Log ("id : " + id);

			
			Transform item = this.transform.GetChild (0);
			item.GetComponent<ItemData>().slot = droppedItem.slot;
			item.transform.SetParent (inv.inventorySlots[droppedItem.slot].transform);
			item.transform.position = inv.inventorySlots[droppedItem.slot].transform.position;
			droppedItem.slot = id;
			droppedItem.transform.SetParent(this.transform); 
			droppedItem.transform.position = this.transform.position;
			inv.inventoryItems[droppedItem.slot] = item.GetComponent<ItemData> ().item;
			inv.inventoryItems[id] = droppedItem.item;

		}
	}
	#endregion
}
