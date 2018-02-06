using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PathologicalGames; // オブジェクトプール用名前空間

public class ScoreHandler : MonoBehaviour {

	// スコアの配色を紐づける変数
	public Image backImage;
	public Color longTapColor;

	// 各ポイントのスプライト用変数 
	public enum PointTextKey { Miss, Bad, Good, Great, Perfect }

	private Vector3 defaultLongTapAnimScale = new Vector3 (2.0f, 2.0f, 2.0f);

	public  Sprite[]   textSprite;			//各ポイント評価用のテキストスプライト
	private GameObject buttonChild;   		//一度全て生成してから移動するような修正に使う（後にいらない）
	private GameObject longTapEndObj;		//ロングタップのアップ時のオブジェクト
	public  GameObject touchBar;	  		//TouchBar
	public  Animator   buttonAnim;	  		//ボタンアニメーションの修正に使う変数（※ここ重要）
	public  GameObject flickAnim;			//フリック成功時のアニメーション
	public  Vector3    flickVct;			//フリックの向きを正規化
	private Vector3    PositionInGameLong;	//ロングタップ用の変数
	private float      distancePoint;		//何度も使うのでOnScoreClick内の処理を関数化
	private float      longTapEndPoint;		//何度も使うのでOnScoreClick内の処理を関数化
	private int        flickCount     = 0; 	//フリックしたと見なされる回数
	private int        flickDirection = 6; 	//フリックの向き : フリックされていない時、つまりflickVct(0,0,0)のとき
	private int        tick = 79000;		//生成されてから削除されるまでのTick
	public  int        flickFlag;			//ScoreCreatorで生成されたフリックスコアの矢印の方向に番号をふる
	public  int        sh_SocreAttribute;	//スコアがもつ属性をScoreCreatorから受け取る

	// 固定座標キャッシュ用変数(newが増えるとゴミが増えてガベコレに影響する)
	private Transform  touchObject;
	private Transform  pointObj;
	private Transform  flickAnimObj;
	private Transform  longUptouchObject;
	private Vector3    touchObjectPos          = new Vector3 (0.0f, -22.5f, -4.9f);
	private Vector3    touchObjectScale        = new Vector3(0.9f, 0.9f, 0.9f);
  //private Vector3    longUptouchObjPos       = new Vector3(0f, 0f, 0f);
	private Vector3    longUptouchObjScale     = new Vector3(1f, 1f, 1f);
	private Vector3    pointObjPos             = new Vector3(0.6f, 14f, -2.4f);
	private Vector3    pointObjScale           = new Vector3(6f, 6f, 6f);
	private Vector3    flickAnimObjPos         = new Vector3(0f, -4f, -4.2f);
	private Quaternion touchObjectRotation     = Quaternion.Euler(-70.38f, 0, 0);
	private Quaternion longUptouchObjRotation  = Quaternion.Euler(-70.38f, 0, 0);
	private Quaternion longTapAnimObjRoatation = Quaternion.Euler(-64.3f, 0f, 0f);

	// フリックされた起点から移動点を司る変数
	private Vector3 flickStartPos;
	private Vector3 flickEndPos;
	private Vector3 startScreenPos;
	private Vector3 endScreenPos;
	private Vector3 startWorldPos;
	private Vector3 endWorldPos;

	private RectTransform myRect;							//毎回Findしていると重いので、静的メンバで保持
	private RectTransform endRect;							//毎回Findしていると重いので、静的メンバで保持
	private BoardMove     boardMove;						//タッチ判定で毎回GetComponemtして処理が重くなるのに対処

	[System.NonSerialized] public int  rand;				//いらん？
	[System.NonSerialized] public long longTapStartTick;	//ScoreHandlerより、tmp.tickが代入される
	[System.NonSerialized] public long longTapEndTick;		//ScoreHandlerより、ttmp.nextTickが代入される
	[System.NonSerialized] public bool isLongTap;			//ロングタップされているかどうかを管理する変数
	[System.NonSerialized] public bool isChecked;			//Update内のif文に入るかどうかを管理
	[System.NonSerialized] public int  tapType;				//ロングタップエンドかスタートかを見る
	[System.NonSerialized] public bool isEnemyGage;

	[System.NonSerialized] public PointHandler  pointHandler;		
	[System.NonSerialized] public GageHandler   gageHandler; 		
	[System.NonSerialized] public PowerProgress powerProgress;
	[System.NonSerialized] public AudioManager  audioManager;
	[System.NonSerialized] public SpawnPool     sh_spawnPool;
	[System.NonSerialized] public Transform     sh_touchedPrefabPool;
	[System.NonSerialized] public Transform     sh_pointTextPool;
	[System.NonSerialized] public Transform     sh_LongTapAnimPool;
	[System.NonSerialized] public Transform     longTapAnimObj;
	[System.NonSerialized] public Transform     sh_FlickAnimPool;


// *******************************************************************************************************************
	#region Start ()
	// ゲーム開始時に取得と初期化 : キャッシュ
	void Start () {
		
//		Invoke ( "AutoDestroy",
//			(60 * tick * 2) / (TimeManager.tempo[TimeManager.tempoSequence].tempoValue * 9600f) 
//		); 

		// キャッシュ＆初期化
		touchObject       = GetComponent<Transform>();
		pointObj          = GetComponent<Transform>();
		flickAnimObj      = GetComponent<Transform>();
		longTapAnimObj    = GetComponent<Transform>();
		longUptouchObject = GetComponent<Transform>();
		myRect            = GetComponent<RectTransform>();
		boardMove         = GetComponentInParent<BoardMove>();
		longTapEndObj     = null;
		endRect		      = null;

		// ロングタップのオブジェクトプールは必要な数だけプリロードするのでStartは必ず呼ばれ、
		// ここで一旦ロングタップの配色をコピーしておく
		if(tapType == 3){
			// ここで取得したカラーをLongTapEndに渡す
			longTapColor = backImage.color;
		}
	}
	#endregion
// *******************************************************************************************************************
	#region OnEnable()
	// アクティブ（再利用）時に初期化
	void OnEnable() {

		#region メモ : 配色のRGBA
		//木
		//backImage.color = new Color (0.000f, 0.500f, 0.000f, 0.409f);
		//火
		//backImage.color = new Color (1.000f, 0.000f, 0.000f, 0.409f);
		//土
		//backImage.color = new Color (1.000f, 0.909f, 0.000f, 0.409f);
		//金
		//backImage.color = new Color (0.837f, 0.792f, 0.890f, 0.408f);
		//水
		//backImage.color = new Color (0.000f, 0.697f, 0.847f, 0.409f);
		#endregion

		tick = 79000;

		if(tapType == 3){
			longTapEndObj = null;
			endRect		  = null;
		}

		if(tapType == 1){
			flickDirection    = 6;
			flickCount        = 0;
			flickVct          = new Vector3(0,0,0);
		}
	}
	#endregion
// *******************************************************************************************************************
	#region Update()
	void Update() {

		if (myRect.position.y <= ScoreCreator.touchBarRect.position.y && !isChecked && tapType != 3) {
			showText(0);
			TouchResult (0);
			audioManager.isMiss = true;

			//生成されたスコアに関しては一度呼ばれたらもう呼ばない(false初期化はScoreHandlerで行なっている)
			isChecked = true;

		}

		if (tapType == 3 && TimeManager.tick > (longTapEndTick + 9600)) {

			if (longTapEndObj == null) {
				
				// 先に下記のFindLongTapEndObj関数を見る
				longTapEndObj = FindLongTapEndObj();
				endRect       = longTapEndObj.GetComponent<RectTransform>();

			} else {

				// !isCheckということは、このif内にまだ入っていないということ。一度入ったらもう入らない
				// こうする事で（isCheckedでフラグ管理する事で）、二度Missが表示されるといった事象を防ぐ
				if (endRect.position.y <= ScoreCreator.touchBarRect.position.y && !isChecked) {

					isChecked = true;
					showText(0);
					TouchResult (0);
					audioManager.isMiss = true;

					#region マージを設けて ロングタップスコアをある一定の時間が過ぎたら削除
					//Destroy( gameObject,    (60 * tick * 2) / (TimeManager.tempo[TimeManager.tempoSequence].tempoValue * 9600f) );
					#endregion
					PoolManager.Pools["ScorePool"].Despawn(this.gameObject.transform,  (60 * tick * 2) / (TimeManager.tempo[TimeManager.tempoSequence].tempoValue * 9600f));

					#region あまり速すぎると、ロングタップエンドスコアが画面上から急に消えるので、マージを設ける
					//Destroy( longTapEndObj, (60 * tick * 2) / (TimeManager.tempo[TimeManager.tempoSequence].tempoValue * 9600f) );
					#endregion
					PoolManager.Pools["ScorePool"].Despawn(longTapEndObj.transform, ((60 * tick * 2) / (TimeManager.tempo[TimeManager.tempoSequence].tempoValue * 9600f) - 4.0f));

					if(isLongTap){
						longTapAnimObj.localScale = defaultLongTapAnimScale;
						PoolManager.Pools ["PoolObject"].Despawn (longTapAnimObj);
					}
				}
			}
		}
	} 
	#endregion
// *******************************************************************************************************************
	#region メソッド : ノーマルスコア・タッチ判定
	public void OnScoreClick(){

		audioManager.isTouch    = true;

		// Board外の位置を計算 / 要するに、タッチした座標を取得（Shimo）/ GetComponentInParentは親のコンポーネントを取得する(Yama)
		Vector3 PositionInGame = boardMove.transform.localPosition + transform.localPosition;

		// 取得した座標をもとに、タッチバーからの距離を数値化（単位として正規化）
		distancePoint = GetDistancePoint (PositionInGame);

		// ポイントが0より大きければ譜面を削除
		if(distancePoint > 0){

			// タッチに成功したオブジェクトを削除して、画面のスコアとゲージを更新
			TouchResult(distancePoint);

			// 評価用テキスト表示
			showText(distancePoint);

		}
	}
	#endregion
// *******************************************************************************************************************
	#region メソッド : フリック判定メソッド
	public void OnScoreFlick(){

		//移動するとは、何度もフリックしていると同じ事なのでフリックの回数を加算
		flickCount++;

		// フリックしたときだけ呼ばれればいい。（これがないと何度も呼ばれて重くなる）
		if(flickCount == 1){

			audioManager.isTouch    = true;

			// 画面をタッチした位置
			startScreenPos = Input.mousePosition;
			flickStartPos  = boardMove.transform.localPosition + transform.localPosition;
			flickEndPos    = flickStartPos;

		}

		// 取得した座標をもとに、タッチバーからの距離を点数化
		distancePoint = GetDistancePoint (flickStartPos);

		//  点数が0より大きければ、フリックの成功判定（フリック範囲、要微調整）
		if(distancePoint > 0){

			// タッチして動いた後の座標
			endScreenPos = Input.mousePosition;

			// フリックした向きを正規化して取得（当初はここで変数宣言していたが、他のスクリプトから参照するためpublic化）
			flickVct = (endScreenPos - startScreenPos).normalized;

			// フリックの移動量flickDistanceを算出
			flickEndPos = boardMove.transform.localPosition + transform.localPosition;

			float flickDistance = Vector3.Distance (flickStartPos, flickEndPos);

			// 上のコードと同じ内容で使えるかもしれないので残しておく。
			// float flickDistance = (flickStartPos-flickEndPos).magnitude;

			// flickDistance = 0の時は方向もくそもないので、評価しない。
			// フリック後（flickDistance > 0）、flickFlag = flickDirectionのときはもう一度評価する必要もない
			if(flickFlag != flickDirection && flickDistance != 0){
				
				//左
				if (flickFlag==0 && flickVct.x < 0 && flickVct.y >= -0.5f && flickVct.y <= 0.4f){	
					
					flickDirection = 0;

				//左上
				} else if (flickFlag==1 && flickVct.x < 0 && flickVct.y >= 0.2f && flickVct.y <= 0.9f){	

					flickDirection = 1;

				//真上
				} else if (flickFlag==2 && flickVct.x >= -0.5f && flickVct.x <= 0.5f && flickVct.y  > 0){	
					
					flickDirection = 2;
				
				//右上
				} else if (flickFlag==3 && flickVct.x > 0 && flickVct.y >= 0.2f && flickVct.y <= 0.9f){	
					
					flickDirection = 3;
				
				//右
				} else if (flickFlag==4 && flickVct.x > 0 && flickVct.y >= -0.5f && flickVct.y <= 0.4f){
					
					flickDirection = 4;
				
				//ノーフリックと上記以外の方向
				} else {
					
					flickDirection = 5;

				}
			}

			//Debug.Log ("フリック検証" + flickFlag);
			//Debug.Log ("flickVct : " + flickVct);
			//Debug.Log ("flickDirection : " + flickDirection);
			//Debug.Log ("flickCount : " + flickCount);
			//Debug.Log ("flickDistance : " + flickDistance);

			// フリック開始から、規定フレーム数以内に、規定移動量を満たし、フリックした方向とflickFlagの向きが一致した場合、フリック成功
			if (flickFlag == flickDirection && flickCount <= 5 && flickDistance > 30.0f) {
				
				TouchResult (distancePoint);
				showText (distancePoint);
				FlickAnim ();

			}
		}
	}
	#endregion
// *******************************************************************************************************************
	#region メソッド : フリック時のアニメーション（フリックした方向にパーティクルを飛ばす）
	public void FlickAnim(){

		// まずは生成（問題なくレーンごとに生成される）
		//GameObject flickAnimObj = Instantiate(flickAnim);
		//Transform  flickAnimObj = PoolManager.Pools ["PoolObject"].Spawn (sh_FlickAnimPool);
		flickAnimObj = PoolManager.Pools ["PoolObject"].Spawn (sh_FlickAnimPool);

		FlickAnimDirection flickAnimDirection = flickAnimObj.GetComponent<FlickAnimDirection> ();

		//flickAnimObj.GetComponent<FlickAnimDirection> ().enabled = true;
		flickAnimDirection.enabled = true;

		// 高さなど、細かい座標調整
		flickAnimObj.position = touchBar.transform.position + flickAnimObjPos;

		// CFX_Demo_Translateに正規化された方向を渡す
		//flickAnimObj.GetComponent<FlickAnimDirection> ().dir          = flickVct;
		//flickAnimObj.GetComponent<FlickAnimDirection> ().flickFlagDir = flickFlag;
		flickAnimDirection.dir          = flickVct;
		flickAnimDirection.flickFlagDir = flickFlag;

		PoolManager.Pools ["PoolObject"].Despawn (flickAnimObj, 1.5f);

	}
	#endregion
// *******************************************************************************************************************
	#region メソッド : ロングタップ判定（Start）
	public void OnScoreLongTapStart() {

		audioManager.isTouch = true;
		PositionInGameLong   = boardMove.transform.localPosition + transform.localPosition;
		distancePoint        = GetDistancePoint(PositionInGameLong);

		if (distancePoint > 0) {

			// タップされたかどうか
			isLongTap = true;

			// ロングタップアニメーションスタート（FlickAnimと同じ要領で考える）
			//longTapAnimObj = Instantiate (longTapAnim);
			longTapAnimObj = PoolManager.Pools ["PoolObject"].Spawn (sh_LongTapAnimPool);

			ParticleSize particleSize = longTapAnimObj.GetComponent<ParticleSize>();

			// longTapStartTick、longTapEndTickはスコアが生成されたら既に値を持つ。その値をparticleSizeに渡す
			particleSize.startTick    = (float)longTapStartTick;
			particleSize.endTick      = (float)longTapEndTick;

			// タッチバーの位置に生成 : キャッシュ
			//longTapAnimObj.transform.position = touchBar.transform.position + new Vector3(0f, -7.0f, -7.0f);
			//longTapAnimObj.transform.position = touchBar.transform.position;
			//longTapAnimObj.transform.rotation = Quaternion.Euler (-64.3f, 0f, 0f);
			longTapAnimObj.position = touchBar.transform.position;
			longTapAnimObj.rotation = longTapAnimObjRoatation;
		}
	} 
	#endregion
// *******************************************************************************************************************
	#region メソッド : ロングタップエンドのオブジェクトを返す
	private GameObject FindLongTapEndObj() {

		GameObject[] objcts  = GameObject.FindGameObjectsWithTag(gameObject.tag);
		GameObject   findObj = null;

		int maxIndex = -1;
		int myIndex  = transform.GetSiblingIndex();

		foreach (GameObject obj in objcts) {

			// ScoreLongUpControllerを持っていたら『tapType = 4』、つまりLongTapUpである。
//			if (obj.name == "Score_LongUp(Clone)") {
			if(obj.GetComponent<ScoreLongUpController>()){	
				int tmpIndex = obj.transform.GetSiblingIndex();
				if (tmpIndex > myIndex) {
					continue;
				}
				if (tmpIndex > maxIndex) {
					maxIndex = tmpIndex;
					findObj = obj;
				}
			}
		}
		return findObj;
	} 
	#endregion
// *******************************************************************************************************************
	#region メソッド : ロングタップ判定（End）
	public void OnScoreLongTapEnd() {

		// Upされたとき、その直前のDownプレハブがちゃんと成功していたら、if文に入り評価する。
		if (!isChecked && isLongTap) {

			audioManager.isTouch = true;

			isChecked = true;

			if (longTapEndObj != null) {
				Vector3 PositionInGame = boardMove.transform.localPosition + longTapEndObj.transform.localPosition;

				distancePoint = GetDistancePoint(PositionInGame);
			} else {
				distancePoint = 0;
			}

			// TouchResultメソッドでDestroy（ここではDownプレハブを消すという処理）
			TouchResult(distancePoint);

			// 評価
			showText(distancePoint);

			// 自身（ロングタップアップ）を削除
//			Destroy(longTapEndObj);
			PoolManager.Pools["ScorePool"].Despawn(longTapEndObj.transform);

			//  ロングアップダウンのイベントで管理する
			//GameObject longUptouchObject = Instantiate(touchRingPrefab);
			//Transform longUptouchObject = PoolManager.Pools ["PoolObject"].Spawn (sh_touchedPrefabPool);
			longUptouchObject = PoolManager.Pools ["PoolObject"].Spawn (sh_touchedPrefabPool);

			// 他のタップタイプと同じオフセットを設けてアップされた瞬間の場所（longTapEndObj.transform.position.y）に発生させる
			longUptouchObject.position = new Vector3(
				transform.position.x,  
				longTapEndObj.transform.position.y + (-22.5f),  
				transform.position.z + -5.0f 
				);

			// スケールと角度を決め、アニメーションを開始
			longUptouchObject.localScale    = longUptouchObjScale;
			longUptouchObject.localRotation = longUptouchObjRotation;

			// アニメーション再生
			longUptouchObject.GetComponent<Animator>().Play(0);

			// タッチエフェクトプレファブを非アクティブ
			PoolManager.Pools ["PoolObject"].Despawn (longUptouchObject, 0.4f);
		}
	}
	#endregion
// *******************************************************************************************************************
	#region メソッド : ロングタップアニメーションを止める(EventTriggerでUp時に実行される)
	public void LongTapAnimStop() {

		//Destroy (longTapAnimObj); 

		#region ロングタップアニメーション時の注意
		// ここで一旦isLongTapがTrueかどうか見る
		// 成功していれば、エフェクトが発生し、失敗ならそもそもDespawnしようとしない
		// ロングタップが成功していないとlongTapAnimObjがSpawnされていないので、
		// DespawnしようとしてもNullエラーになる
		#endregion
		if (isLongTap) {

			// スケールをリセット
			longTapAnimObj.localScale = defaultLongTapAnimScale;

			#region 非アクティブ時の注意
			// 非アクティブにする
			// ここだけだとタップ成功時、アップスコアが通り過ぎてmissしてもエフェクトが残るので
			// update内でその場合も吟味する
			#endregion
			PoolManager.Pools ["PoolObject"].Despawn (longTapAnimObj);
		} 
	}
	#endregion
// *******************************************************************************************************************
	#region メソッド : 評価用テキスト作成する
	private void showText(float distancePoint){

		//PointTextプレハブ（エフェクトオブジェクトを生成
		//GameObject pointObj = Instantiate(pointText);
		//Transform pointObj = PoolManager.Pools ["PoolObject"].Spawn (sh_pointTextPool);
		pointObj = PoolManager.Pools ["PoolObject"].Spawn (sh_pointTextPool);


		//（修正）ScoreCreatorで生成されたxの位置を受け取って（touchBar）、タッチバーにプレハブを出現させる
		pointObj.position    = touchBar.transform.position + pointObjPos;
		pointObj.localScale  = pointObjScale;

		// メソッドを使って評価を生成
		int evaluation = CreateEvaluation (distancePoint);

		// ポイントに応じて画像を切替(Badは『point > 0』で0にしておかないとタッチバーより上でmissが表示されてしまう)
		if (evaluation == 0) {

			pointObj.GetComponentInChildren<SpriteRenderer> ().sprite = textSprite [(int)PointTextKey.Perfect];

			// Perfectの時はその数をカウント
			GameDate.perfectCount++;

			// タップに成功したら、Perfect係数をPowerProgressにわたす。（係数が固定だと使いにくいので後に変数化）
			powerProgress.PlayerValueChange(distancePoint * 0.03f);

			// perfectするたびに１づつ加算
			GameDate.perfectNum++;

			// 以下同義
		} else if (evaluation == 1) {
			pointObj.GetComponentInChildren<SpriteRenderer> ().sprite = textSprite [(int)PointTextKey.Great];
			powerProgress.PlayerValueChange(distancePoint * 0.03f);
			GameDate.greatNum++;
		} else if (evaluation == 2) {
			pointObj.GetComponentInChildren<SpriteRenderer> ().sprite = textSprite [(int)PointTextKey.Good];
			powerProgress.PlayerValueChange(distancePoint * 0.03f);
			GameDate.goodNum++;
		} else if (evaluation == 3) {
			pointObj.GetComponentInChildren<SpriteRenderer> ().sprite = textSprite [(int)PointTextKey.Bad];
			powerProgress.PlayerValueChange(distancePoint * 0.03f);
			GameDate.badNum++;
		} else if(evaluation == 4) {	// point＝0ならmissと表示させる
			pointObj.GetComponentInChildren<SpriteRenderer>().sprite  = textSprite [(int)PointTextKey.Miss];
			powerProgress.PlayerValueChange(distancePoint * 0.03f);

			// pointが0なのは、ゲーム中なのか、そうじゃないのか。ゲーム中以外はパラメーターの増減は加味しない。
			powerProgress.isMissed = true;

			GameDate.missNum++;
		}

		// アニメーションを開始
		pointObj.GetComponentInChildren<Animator>().Play( 0 );

		// コンボカウント
		if (distancePoint > 0) {
			GameDate.combo++;
		} else {
			GameDate.combo = 0;
		}
			
		// 最初maxは０なので、一回でも成功したら、現在のコンボ数がmaxCombo
		if(GameDate.combo > GameDate.maxCombo){
			// maxComboをcomboで更新、つまりmissしたら0に戻る
			GameDate.maxCombo = GameDate.combo;
		}

		// コンボ表示（コンボを更新）
		pointHandler.setCombo(GameDate.combo);

		// 最後にDespawnするがここでの秒数はpointTextのAnimationClipのLengthを参照した定数(18フレーム目にDespawn)
		PoolManager.Pools ["PoolObject"].Despawn (pointObj, 0.333f);
	} 
	#endregion
// *******************************************************************************************************************
	#region 文字列は処理が重くなるのでint型に変換
// distancePointを元に評価を作成し、文字列で返すメソッド
//	private string CreateEvaluation(float distancePoint){
//
//		string evaluation = null;
//
//		if (distancePoint> 0.7f) {
//			evaluation = "Perfect";
//		} else if (distancePoint > 0.4f) {
//			evaluation = "Great";
//		} else if (distancePoint > 0.2f) {
//			evaluation = "Good";
//		} else if (distancePoint > 0) {
//			evaluation = "Bad";
//		} else {
//			evaluation = "Miss";
//		}
//		return evaluation;
//	}
	#endregion
	#region メソッド : distancePointを元に評価を作成し、数値で返すメソッドに変更
	private int CreateEvaluation(float distancePoint){

		// "Perfect"=0, "Great"=1, Good"=2, "Bad"=3, "Miss"=4

		int evaluation;

		if (distancePoint> 0.7f) {
			evaluation = 0;
		} else if (distancePoint > 0.4f) {
			evaluation = 1;
		} else if (distancePoint > 0.2f) {
			evaluation = 2;
		} else if (distancePoint > 0) {
			evaluation = 3;
		} else {
			evaluation = 4;
		}
		return evaluation;
	}
	#endregion
// *******************************************************************************************************************
	#region メソッド :  タッチに成功したオブジェクトを削除して、画面のスコアとゲームオーバーを更新する
	private void TouchResult(float distancePoint){

		// ロングタップ時はダップダウンの位置にButterfly Brokenを発生させない（ロングタップ用は別途実装）
		if (!isLongTap && distancePoint > 0) {

			// エフェクトオブジェクトを生成 (touchuRingPrefab = Butterfly Broken)
			//GameObject touchObject = Instantiate (touchRingPrefab);
			//Transform touchObject = PoolManager.Pools ["PoolObject"].Spawn (sh_touchedPrefabPool);
			touchObject = PoolManager.Pools ["PoolObject"].Spawn (sh_touchedPrefabPool);

			#region エフェクトの位置の移動とサイズをリセット
			// エフェクトの位置の移動とサイズをリセット
			// ※BrokenEffectの大きさを微調整 / ButterFlyBorkenはタッチバーから出現する必要はない
			// BrokenEffectの大きさはtransform.positionとの兼ね合いもあって、破綻するかもしれないので要検討
			#endregion
			touchObject.position      = transform.position + touchObjectPos;
			touchObject.localScale    = touchObjectScale;
			touchObject.localRotation = touchObjectRotation;

			// アニメーションを開始
			touchObject.GetComponent<Animator> ().Play (0);

			// タッチフェクトプレファブを非アクティブ
			PoolManager.Pools ["PoolObject"].Despawn (touchObject, 0.4f);

			// 譜面 （自身）の削除
			//Destroy (gameObject);
			PoolManager.Pools["ScorePool"].Despawn(this.gameObject.transform);

		} else {

			// isChecked = Trueの時はロングスコア時のMiss / isChecked = Falseの時はロングスコア以外のMiss
			if (isChecked) {

				//Destroy (gameObject);
				PoolManager.Pools["ScorePool"].Despawn(this.gameObject.transform);

			} else {	

				// ロングタップ以外のMiss時は画面外でDestroyする為に遅らせる
				//Destroy (gameObject,0.3f);
				PoolManager.Pools["ScorePool"].Despawn(this.gameObject.transform, 0.3f);
			}
		}
			
		// GameDataのscoreにポイントを設定 （pointの基準値が変わったのでその分を*4.4f乗算）
		GameDate.score += (int)(distancePoint * 1000 * 4.4f);

		// ポイント表示(スコアを更新)
		pointHandler.setPoint(GameDate.score);

		int evaluation = CreateEvaluation (distancePoint);

		// タッチ評価とスコア属性をGageHandlerに渡す
		gageHandler.setGage (evaluation, sh_SocreAttribute);
	} 
	#endregion
// *******************************************************************************************************************
	#region メソッド : タッチバーとタッチした座標の間の距離を数値化（単位として正規化）して返す
	private float GetDistancePoint(Vector3 touchPos){

		// タッチバーからの距離を計算
		int Distance = (int)Mathf.Abs(touchPos.y * 0.4f + 160);

		// 距離からポイントを計算し正規化
		distancePoint = (100 - Distance) / 100f;

		// ポイントの最小値は0とする（マイナス不可）/ポイントが0以下の時はポイントを0にする
		if (distancePoint < 0){
			distancePoint = 0;
		}
		return distancePoint;
	} 
	#endregion
// *******************************************************************************************************************
	#region  メソッド : 自動で削除（miss時の処理）: オブジェクトプーリング時は不要
// 自動で削除（miss時の処理）: オブジェクトプーリング時は不要
//	public void AutoDestroy(){
//		if (tapType != 3) {
//			Destroy(gameObject);
//		}
//	} 
	#endregion
	#region メソッド : レンズフレア・メソッド
	//タッチした瞬間レンズフレア発生
	public void TouchEffect() {
		buttonAnim.SetTrigger ("Touch"); 
	}
	#endregion
}
