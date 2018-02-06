using UnityEngine;

// ViewControllerクラスを継承する
public class TableViewCell<T> : ViewController {

	// セルの内容を更新するメソッド
	public virtual void UpdateContent(T itemData){}

	// セルに対応するリスト項目のインデックスを保持	
	public int DataIndex { get; set; }

	// セルの高さを取得、設定するプロパティ
	public float Height{

		get { return CachedRectTransform.sizeDelta.y; }

		set {
			Vector2 sizeDelta = CachedRectTransform.sizeDelta;
			sizeDelta.y = value;
			CachedRectTransform.sizeDelta = sizeDelta;
		}
	}

	// セルの幅を取得、設定するプロパティ
	public float Width{

		get { return CachedRectTransform.sizeDelta.x; }

		set {
			Vector2 sizeDelta = CachedRectTransform.sizeDelta;
			sizeDelta.x = value;
			CachedRectTransform.sizeDelta = sizeDelta;
		}
	}
		
	#region プロパティ : セルの上端の位置を取得、設定するプロパティ
	public Vector2 Top {
		get {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			return CachedRectTransform.anchoredPosition + 
				new Vector2(0.0f, corners[1].y);
		}

		set {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			CachedRectTransform.anchoredPosition = 
				value - new Vector2(0.0f, corners[1].y);
		}
	}
	#endregion

	#region プロパティ : セルの下端の位置を取得、設定するプロパティ
	public Vector2 Bottom{

		get {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			return CachedRectTransform.anchoredPosition + 
				new Vector2(0.0f, corners[3].y);
		}

		set {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			CachedRectTransform.anchoredPosition = 
				value - new Vector2(0.0f, corners[3].y);

		}
	}
	#endregion

	#region プロパティ : セルの左の位置を取得、設定するプロパティ
	public Vector2 Left {

		get {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			return CachedRectTransform.anchoredPosition + 
				new Vector2(corners[1].x, 0.0f);
		}

		set {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			CachedRectTransform.anchoredPosition = 
				value - new Vector2(corners[1].x, 0.0f);
		}
	}
	#endregion

	#region プロパティ : セルの下端の位置を取得、設定するプロパティ
	public Vector2 Right {

		get {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			return CachedRectTransform.anchoredPosition + 
				new Vector2(corners[3].x, 0.0f);
		}

		set {
			Vector3[] corners = new Vector3[4];
			CachedRectTransform.GetLocalCorners(corners);
			CachedRectTransform.anchoredPosition = 
				value - new Vector2(corners[3].x, 0.0f);
		}
	}
	#endregion
}
