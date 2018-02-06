using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler {

	private Inventory inventory;
	public  int       id;

	void Start(){
		inventory = GameObject.Find ("Inventory").GetComponent<Inventory>();
	}

	void Update(){
		if(this.gameObject.transform.childCount == 2){
			Debug.Log ("重複した");
			Debug.Log (this.gameObject.transform.GetChild(1));
			for (int i = 0; i < inventory.inventorySlots.Count; i++) {
				if (inventory.inventorySlots[i].transform.childCount == 0) {
					Debug.Log (i+"番目が空き");
					this.gameObject.transform.GetChild(1).SetParent (inventory.inventorySlots[i].transform);
					break;
				}
			}
		}
	}

	public GameObject Item {
		
		get {
			
			// ドロップエリアに既にオブジェクトがある場合（つまりchildCount = 1）
			if (transform.childCount > 0){
				
				// ドラッグ中のオブジェクトを反映させる
				return transform.GetChild(0).gameObject;

			}
			// ドロップ先が空の場合はそのままドラッグ中のオブジェクトを返す
			return null;
		}
	}


	public void OnDrop(PointerEventData eventData) {

		// ドラッグ中のtagがドロップゾーンのタグと一致するか
		if(gameObject.tag == DraggableAdvance.dragObject.tag){

			// ドロップ先が空の場合
			if (Item == null){
				
				// ドラッグ中のオブジェクトをそのままセット
				DraggableAdvance.dragObject.transform.SetParent(transform);

			// ドロップ先に既にオブジェクトがある場合は配置交換 
			} else {
				
				// ドラッグが開始された元のBoxに既にあるオブジェクトを移動
				Item.transform.SetParent(DraggableAdvance.dragObject.parentTransform);

				// ドロップ先のBoxにドラッグ中のオブジェクトを配置
				DraggableAdvance.dragObject.transform.SetParent(transform);

			}
			Item_Shimo.itemList[DraggableAdvance.dragObject.name] = gameObject.name;
		}
	}
}