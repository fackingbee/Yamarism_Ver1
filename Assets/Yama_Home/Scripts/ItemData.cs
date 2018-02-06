using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#region メモ : インターフェースについて
// ちなみにインタフェースを使用するときはpublicにしないとエラーが出るので注意
#endregion
//public class ItemData : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler {
public class ItemData : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {


	#region 変数
	public  int       amount;
	public  int       slot;
	public  Item      item;
	private Vector2   offSet;
	private Inventory inv;
	private Tooltip   tooltip;	// シーン上のInventoryオブジェクトにアタッチする
	#endregion


	#region MonoBehaviour
	void Start () {
		
		#region メモ : シンプルな仕様
		// このスクリプトはItemプレファブにアタッチされているが
		// ここでInventoryオブジェクトをFindしているので、コンポーネントにアクセス出来る
		// つまりプレファブを増やすことなく、ほぼInventoryとItemとSlotのプレファブだけでやっていく
		#endregion
		inv = GameObject.Find ("Inventory").GetComponent<Inventory>();
		tooltip = inv.GetComponent<Tooltip>();

	}
	#endregion

	#region Drag & Drop
//	public void OnBeginDrag(PointerEventData eventData){
//		if(item != null){
//			offSet = eventData.position - new Vector2 (this.transform.position.x,  this.transform.position.y);
//			#region メモ : parentが二つ
//			// 一つ目のparentはslotで(this.transform.parent)だと、スロットの子要素から動かない
//			// さらに一つ上の階層のInventory Panelの子要素に移動しないといけないので、(this.transform.parent.parent)となる
//			// 『UIの教科書』でワールド座標に変換してうんたらかんたらよりも
//			// Inventory Panelの子要素から出なくていいならこちらの方が楽
//			#endregion
//			this.transform.SetParent (this.transform.parent.parent);
//			this.transform.position = eventData.position;
//			GetComponent<CanvasGroup>().blocksRaycasts = false;
//		}
//	} 
//
//
//	public void OnDrag(PointerEventData eventData){
//		if(item != null){
//			this.transform.position = eventData.position - offSet;
//		} 
//	} 
//
//	public void OnEndDrag(PointerEventData eventData){
//
//		#region メモ : 移動先のスロットに格納する際...
//		// [slot]にはInventory.csで解析したitemsのIDを格納してあり、
//		// そのIDを保有するスロットの子要素に設置する
//		#endregion
//		this.transform.SetParent(inv.inventorySlots[slot].transform); 
//
//		#region メモ : この一文はスロットに格納する際は必ず必要であるが...
//		// このドラッグ中のアイテムをドロップする際にスロットの座標を渡す。
//		// そうしないとどこにでも置けてしまう。
//		// どこにでも置けてしまうということは、画像を任意の位置に移動させたい場合などに使えるかもしれない。
//		#endregion
//		this.transform.position = inv.inventorySlots[slot].transform.position;
//
//		// そしてタッチ可能にする
//		GetComponent<CanvasGroup>().blocksRaycasts = true;
//
//	}
	#endregion

	#region Pointer Enter & Exit
	public void OnPointerEnter (PointerEventData eventData){
		#region メモ : ここの(item)は...
		// ここでもInventory.csのAddItem()で追加されたItemクラスのitemToAddが追加されている
		// シーンに必ず存在するものを一つ設けて、ゲーム起動に支障をきたさない程度にFindすることでコードが散らからなくて済む
		#endregion
		tooltip.Activate (item);

	} 

	public void OnPointerExit (PointerEventData eventData){
		tooltip.Deactivate ();
	} 
	#endregion
}
