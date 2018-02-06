using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YamaDelta.ItemSystem {
	
	[System.Serializable]
	public class ISQuality : IISQuality, IISDatabaseObject {

		[SerializeField] string _name;
		[SerializeField] Sprite _icon;

		#region メモ : ここでのコンストラクタはウィンドウ表示の情報
		// 『YamaDelta.ItemSystemの名前空間内なら』どこからでも呼べるように
		// ウィンドウを開いた時のデフォルトの名前
		#endregion
		public ISQuality(){
			_name = "";
			_icon = new Sprite (); 
		}

		public ISQuality(string name, Sprite icon){
			this._name = name;
			this._icon = icon;
		}


		#region IISDatabaseObject implementation
		public void Clone (IISDatabaseObject item) {
			_name = item.Name;
			_icon = item.Icon;
		}

		public void OnGUI () {
			throw new System.NotImplementedException ();
		}

		public string Name { 
			get { return _name; } 
			set { _name = value; }
		}

		public Sprite Icon{ 
			get{return _icon;}
			set{_icon = value;}
		}
		#endregion

	}
}
