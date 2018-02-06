using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class TableViewControllerHorizontal<T> : ViewController {

	#region 変数
	// リスト項目のデータを保持
	protected List<T> tableData_H = new List<T>();

	// セルを保持
	private LinkedList<TableViewCell<T>> cells_H = new LinkedList<TableViewCell<T>>();

	private ScrollRect cachedScrollRect_H;						// Scroll Rectコンポーネントをキャッシュ
	public  ScrollRect CachedScrollRect_H{
		get {
			if(cachedScrollRect_H == null) { 
				cachedScrollRect_H = GetComponent<ScrollRect>(); }
			return cachedScrollRect_H;
		}
	}			
	private Rect       visibleRect_H = new Rect(0,0,0,0);		// リスト項目をセルとして表示する範囲を示す矩形
	private Vector2    prevScrollPos_H;							// 前回のスクロール位置を保持	

	[SerializeField] private RectOffset padding_H;				// スクロールさせる内容のパディング
	[SerializeField] private GameObject cellBase_H;				// コピー元のセル
	[SerializeField] private RectOffset visibleRectPadding_H;	// visibleRect_Hのパディング
	[SerializeField] private float      spacingWidth = 0.0f;	// 各セルの間隔
	#endregion

	protected virtual void Awake(){}

	protected virtual void Start() {
		cellBase_H.SetActive(false);
		CachedScrollRect_H.onValueChanged.AddListener(OnScrollPosChanged_H);
	}

	protected virtual float CellWidthAtIndex(int index){ return 0.0f; }

	// スクロールさせる内容全体の幅を更新するメソッド
	protected void UpdateContentSize_H(){

		// スクロールさせる内容全体の高さを算出する
		float contentWidth = 0.0f;

		for(int i=0; i<tableData_H.Count; i++){

			contentWidth += CellWidthAtIndex(i);

			if(i > 0) { 
				contentWidth -= spacingWidth; 
			}
		}
		Vector2 sizeDelta   = CachedScrollRect_H.content.sizeDelta;
		sizeDelta.x = padding_H.left + contentWidth + padding_H.left;
		CachedScrollRect_H.content.sizeDelta = sizeDelta;
	}


	// セルを作成するメソッド
	private TableViewCell<T> CreateCellForIndex_H(int index){

		// コピー元のセルから新しいセルを作成する
		GameObject obj_H = Instantiate(cellBase_H) as GameObject;

		// 元となるセルは非アクティブにしておくのでクローンはアクティブにする
		obj_H.SetActive(true);

		// TableViewCell.csのプロパティを取得
		TableViewCell<T> cell_H = obj_H.GetComponent<TableViewCell<T>>();

		// 親要素の付け替えをおこなうとスケールやサイズが失われるため、変数に保持しておく
		Vector3 scale     = cell_H.transform.localScale;
		Vector2 sizeDelta = cell_H.CachedRectTransform.sizeDelta;
		Vector2 offsetMin = cell_H.CachedRectTransform.offsetMin;
		Vector2 offsetMax = cell_H.CachedRectTransform.offsetMax;

		cell_H.transform.SetParent(cellBase_H.transform.parent);

		// セルのスケールやサイズを設定する
		cell_H.transform.localScale          = scale;
		cell_H.CachedRectTransform.sizeDelta = sizeDelta;
		cell_H.CachedRectTransform.offsetMin = offsetMin;
		cell_H.CachedRectTransform.offsetMax = offsetMax;


		// 指定したインデックスのリスト項目に対応するセルとして内容を更新する
		UpdateCellForIndex_H(cell_H, index);

		cells_H.AddLast(cell_H);

		return cell_H;
	}

	private void UpdateCellForIndex_H(TableViewCell<T> cell_H, int index) {


		// セルに対応するリスト項目のインデックスを設定する【waterのindexは0でそれをcell.DataIndexに代入】
		cell_H.DataIndex = index;

		// セルの項目数（この場合は18個）を超えたら、入らない
		if(cell_H.DataIndex >= 0 && cell_H.DataIndex <= tableData_H.Count-1){

			// セルに対応するリスト項目があれば、セルをアクティブにして…高さを設定する
			cell_H.gameObject.SetActive(true);

			// 内容を更新し…
			cell_H.UpdateContent(tableData_H[cell_H.DataIndex]);

			// 高さを設定する。
			cell_H.Width = CellWidthAtIndex(cell_H.DataIndex);

		} else {

			// セルに対応するリスト項目がなかったら、セルを非アクティブにして表示しない
			cell_H.gameObject.SetActive(false);
		}
	}

	#region visibleRectを更新するためのメソッド
	private void UpdateVisibleRect_H () {

		// visibleRectの位置はスクロールさせる内容の基準点からの相対位置
		visibleRect_H.x =  CachedScrollRect_H.content.anchoredPosition.x + visibleRectPadding_H.left;
		visibleRect_H.y = -CachedScrollRect_H.content.anchoredPosition.y + visibleRectPadding_H.top;

		// visibleRectのサイズはスクロールビューのサイズ + パディング
		visibleRect_H.width  = CachedRectTransform.rect.width  + visibleRectPadding_H.left + visibleRectPadding_H.right;
		visibleRect_H.height = CachedRectTransform.rect.height + visibleRectPadding_H.top  + visibleRectPadding_H.bottom;

	}
	#endregion

	#region
	protected void UpdateContents_H(){

		// スクロールさせる内容のサイズを更新する
		UpdateContentSize_H();

		// visibleRectを更新する : 無限ループバグ有り
		UpdateVisibleRect_H();


		// セルが1つもない場合、visibleRectの範囲に入る最初のリスト項目を探して、それに対応するセルを作成する
		if(cells_H.Count < 1){

			// 最初表示領域内の右端と上端からどれくらいの距離に作るか【現在は右から3.0f、上から-16.0fの位置に生成】
			Vector2 cellLeft = new Vector2( (-padding_H.left - 2.0f), 3.0f  );

			for(int i=0; i<tableData_H.Count; i++){

				float cellWidth = CellWidthAtIndex(i);

				Vector2 cellRight = cellLeft + new Vector2(-cellWidth, 0.0f);

				if( (cellLeft.x <= visibleRect_H.x && 
					cellLeft.x >= visibleRect_H.x - visibleRect_H.width)
					|| 
					(cellRight.x <= visibleRect_H.x && 
						cellRight.x >= visibleRect_H.x - visibleRect_H.width) 
				){
					TableViewCell<T> cell_H = CreateCellForIndex_H(i);
					cell_H.Left = cellLeft;
					break;
				}
				cellLeft = cellRight + new Vector2(spacingWidth,0.0f);
			}

			// visibleRectの範囲内に空きがあればセルを作成する
			FillVisibleRectWithCells_H();

		} else {

			// すでにセルがある場合、先頭のセルから順に対応するリスト項目の
			// インデックスを設定し直し、位置と内容を更新する
			LinkedListNode<TableViewCell<T>> node = cells_H.First;


			UpdateCellForIndex_H(node.Value, node.Value.DataIndex);
			node = node.Next;

			while(node != null) {

				UpdateCellForIndex_H(node.Value, node.Previous.Value.DataIndex + 1);

				node.Value.Left = node.Previous.Value.Right + new Vector2(-spacingWidth, 0.0f);

				node = node.Next;

			}

			// visibleRectの範囲内に空きがあればセルを作成する
			FillVisibleRectWithCells_H();

		}
	}
	#endregion

	private void FillVisibleRectWithCells_H() {

		//Debug.Log ("Horizontala");

		// セルがなければ何もしない
		if(cells_H.Count < 1) { 
			return; 
		}

		// 表示されている最後のセルに対応するリスト項目の次のリスト項目があり、         【Beerの次にはCOCKTAILがある】
		// かつ、そのセルがvisibleRectの範囲内に入るようであれば、対応するセルを作成する【スクロールしたら範囲内に入る】

		// まずは関数内変数宣言【表示されうる一番下のセル？】
		TableViewCell<T> lastCell_H = cells_H.Last.Value;

		// 表示されている最後のセルの次のセルのIndex
		int nextCellDataIndex = lastCell_H.DataIndex + 1;

		// 最後のセルから下端余白分
		Vector2 nextCellLeft = lastCell_H.Right + new Vector2(-spacingWidth, 0.0f);

		// 全項目数より少ない要素番号、尚且つ、次に表示されるべきセルの上端が表示可能領域に入ったら
		while(nextCellDataIndex < tableData_H.Count && nextCellLeft.x <= -(visibleRect_H.x - visibleRect_H.width)) {

			//Debug.Log ("while2");
			//Debug.Log ("nextCellLeft.x : " + nextCellLeft.x);
			//Debug.Log ("visibleRect_H.x - visibleRect_H.width : " + (visibleRect_H.x - visibleRect_H.width));


			TableViewCell<T> cell_H = CreateCellForIndex_H(nextCellDataIndex);

			cell_H.Left       = nextCellLeft;
			lastCell_H        = cell_H;
			nextCellDataIndex = lastCell_H.DataIndex + 1;
			nextCellLeft      = lastCell_H.Right + new Vector2(-spacingWidth, 0.0f);

		}
	}

	// スクロールビューがスクロールされたときに呼ばれる
	public void OnScrollPosChanged_H(Vector2 scrollPos_H) {

		// visibleRectを更新する
		UpdateVisibleRect_H();

		// スクロールした方向によって、セルを再利用して表示を更新する【三項演算子】
		ReuseCells_H((scrollPos_H.x > prevScrollPos_H.x)? -1: 1);

		// この次の処理の際に、現在のスクロール位置を前のスクロール位置として保持
		prevScrollPos_H = scrollPos_H;

	}


	// セルを再利用して表示を更新するメソッド
	private void ReuseCells_H(int scrollDirection_H) {

		if(cells_H.Count < 1){
			return;
		}

		if( scrollDirection_H < 0 ) {

			TableViewCell<T> firstCell_H = cells_H.First.Value;

			while( -firstCell_H.Right.x > visibleRect_H.x){
				TableViewCell<T> lastCell_H = cells_H.Last.Value;
				UpdateCellForIndex_H(firstCell_H, lastCell_H.DataIndex + 1);
				firstCell_H.Left = lastCell_H.Right + new Vector2(-spacingWidth, 0.0f);
				cells_H.AddLast(firstCell_H);
				cells_H.RemoveFirst();
				firstCell_H = cells_H.First.Value;
			}

			FillVisibleRectWithCells_H();

		} else if( scrollDirection_H > 0 ) {

			TableViewCell<T> lastCell_H = cells_H.Last.Value;

			while(-(lastCell_H.Left.x) < visibleRect_H.x - visibleRect_H.width){

				TableViewCell<T> firstCell_H = cells_H.First.Value;
				UpdateCellForIndex_H(lastCell_H, firstCell_H.DataIndex - 1);
				lastCell_H.Right = firstCell_H.Left + new Vector2(spacingWidth, 0.0f);
				cells_H.AddFirst(lastCell_H);
				cells_H.RemoveLast();
				lastCell_H = cells_H.Last.Value;

			}
		}
	}


}
