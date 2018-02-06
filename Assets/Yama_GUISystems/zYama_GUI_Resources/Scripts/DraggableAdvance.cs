using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

#region スロットとアイテムのタグについて
// Box内が空でないと機能しないので、ラベルなどは独立して設置すること
// インスタンス化の際にタグをつける必要がある
#endregion
#region CanvasGroupのアタッチについて
// CanvasGroupを必須にする(すでにコンポーネントが存在しているとエラーが出るので注意)
//[RequireComponent(typeof(CanvasGroup))]
// 起動時にはアタッチされず、ドラッグを開始する際に初めてアタッチされてしまうので
// すでにコンポーネントをアタッチさせておいて、
// ドラッグ時は他のアイテムは操作不能にしておく
#endregion
public class DraggableAdvance : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	// thisで自身であると指定する
	public static DraggableAdvance dragObject;

	// どの階層にあっても、イベント中は一度CanvasのTransformに差し替えるのに必要
	public  Transform parentTransform;

	// 実際の位置とドラッグ開始位置の差分を格納
	private Vector3 tapRefPosition;

	// 開始時にコンポーネントを追加する為の変数
	private CanvasGroup canvasGroup;

	// イベント中に差し替えたcanvasのTransformを格納
	private Transform canvasTransform;

	// null置換演算子：スタート時にCanvasGroupを追加
	//public CanvasGroup CanvasGroup {get{return canvasGroup ?? (canvasGroup = gameObject.AddComponent<CanvasGroup>());}}

	// null置換演算子：ドラッグ中は一度Canvas直属の子要素に移動（現在は孫、ひ孫で移動しないと描画順的に表示されない可能性があるので）
	public Transform   CanvasTransform {get{return canvasTransform ?? (canvasTransform = GameObject.Find("Canvas").transform);}}

	//private bool isStart;


	void Start(){

		canvasGroup = GetComponent<CanvasGroup> ();

		//isStart = true;

		// キーが存在すれば値を取得
		if (Item_Shimo.itemList.ContainsKey (gameObject.name)) {
			
			string parentName = Item_Shimo.itemList [gameObject.name];
		
			// 値が空でなければ親（ItemBox）を変更する
			if (!string.IsNullOrEmpty (parentName)) {
				GameObject parentObj = GameObject.Find (parentName);
				transform.SetParent (parentObj.transform);
			}
		}

		// 変数に親のTransformを代入（ヒュッって戻るときにつかう）
		parentTransform = transform.parent;

	}

	public void Update(){
		
		// 何も掴んでいないか、掴んでるアイテムを手放したとき
		if (dragObject == null ){
			#region メモ : 滑らかな移動
			//			// Sprite Rendererなら可能だが、UGUIは移動を得意とせず、
			//			// 描画順が制御出来ないので
			//			// 一瞬で移動する方式に変更する
			//
			//			// スタート時だけ瞬間移動
			//			if(isStart){
			//				
			//				transform.localPosition -= transform.localPosition / 1.0f;
			//
			//				// 初回以降は滑らかな移動
			//				isStart = false;
			//			}
			//
			//			//transform.localPosition -= transform.localPosition / 3.0f;
			#endregion
			transform.localPosition -= transform.localPosition / 1.0f;

		}
	} 


	public void OnBeginDrag(PointerEventData eventData) {

		// 実際の位置とドラッグ開始位置の差分を格納
		tapRefPosition = (Vector2)transform.position - eventData.position;

		// Staticで同じスクリプトをアタッチしているが、タッチされたオブジェクトについてだけ処理する
		dragObject = this;

		//dragObjectはstaticなので、Drop側からも見れる
		//Debug.Log ("現在ドラッグ中オブジェクトのTag : " + dragObject.tag);

		// 左オベランドはBox[]、右オベランドはCanvasになるので、親のTransformをCanvasに差し替える
		parentTransform = transform.parent;

		// 開始時に一度Canvasの子要素に移動
		transform.SetParent(CanvasTransform);

		// ドラッグ中は他のUI操作を禁止する
		canvasGroup.blocksRaycasts = false;

		// ドラッグ中の表示は薄くする
		canvasGroup.alpha = 0.5f;

	}

	public void OnDrag(PointerEventData eventData){
		transform.position = Input.mousePosition + tapRefPosition;
	}

	public void OnEndDrag(PointerEventData eventData){

		if (transform.parent == canvasTransform) {
			transform.SetParent(parentTransform);
		}

		dragObject = null;					// どのUI要素でもなくす
		canvasGroup.blocksRaycasts = true;	// 再度UI操作を許可
		canvasGroup.alpha = 1.0f;			// UIの色を通常に戻す
	}

}