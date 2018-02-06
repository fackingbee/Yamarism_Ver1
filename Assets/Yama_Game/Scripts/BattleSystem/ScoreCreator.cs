using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;//JSONに変換したMIDIを解析
using PathologicalGames;//オブジェクトプール用名前空間

public class ScoreCreator : MonoBehaviour {

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

	// 配色を配列で格納 : 月曜日と日曜日
	private List<Color> scoreColorList = new List<Color> {
		new Color (0.000f, 0.500f, 0.000f, 0.703f),	//木
		new Color (1.000f, 0.000f, 0.000f, 0.703f),	//火
		new Color (1.000f, 0.909f, 0.000f, 0.703f), //土
		new Color (0.837f, 0.792f, 0.890f, 0.703f), //金
		new Color (0.000f, 0.697f, 0.847f, 0.703f),	//水
	};

	// 曜日を配列で格納 : 後で静的フィールドにでも移動
	private string[] weekList = {"Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday"};

	// Jsonテキストデータ
//	[Header("JSON Setting")]
//	public TextAsset jsonDate;
//	public void SetJsonData( string data ){
//		Debug.Log ("実行");
//		this.cacheData = data;
//		Debug.Log (this.cacheData);
//	}
	// JsonDataBattleからロードされたJsonを受け取る
	private string cacheData;

	[Space(16)]
	public Color longTapEndColor;	// ロングタップアップ時の配色
	public int   doubleColorIndex;	// ダブルロングタップ時に配色を同じにする為の変数

	// スコアのXの位置
	private static float[] ScorePositionXList = new float[]{-375f, -132.5f, 132.5f, 375f};

	// ScoreHandlreからお引越し
	public static RectTransform touchBarRect;

	// MIDIのValueを第一引数に、レーンの番号を第二引数に渡して、KeyとvalueでロングタップのStartとEndを管理（連想配列という）
	Dictionary<int, int> randDic = new Dictionary<int, int>();

	#region 非オブジェクトプール時はこちらを使用
	//public GameObject      scorePrefab;
	//public GameObject      scorePrefabLongDown;
	//public GameObject      scorePrefabLongUp;
	//public GameObject[]    scoreFlickType;
	#endregion
	public Transform       scorePrefab;					// Scoreプレハブ（通常用）	 
	public Transform       scorePrefabLongDown;			// Scoreプレハブ（ロングタップ・ダウン用）
	public Transform       scorePrefabLongUp;	    	// Scoreプレハブ（ロングタップ・アップ用）
	public List<Transform> scoreFlickType;				// Scoreプレハブ（フリック用）
	public Animator[]      touchBarAnim;				// タッチバーアニメーションを配列化
	public List<MusicDate> scoreDate;					// 音楽データを格納する構造体の配列
	public GameObject[]    touchBars;					// TouchBar格納メンバ（4つのTouchBarをScoreHandlerに渡す）
	private int            longUpAttribute;				// 敵ゲージにスコア属性を渡す変数
	private int            longTapNum = 0;				// ロングタップのスタートエンドを紐付け
	private int            seed       = 100;			// 乱数を固定する為の変数
	private int            seedFlick  = 777;			// フリック方向乱数を固定する為の変数
	private int            seedColor  = 777;			// 配色乱数を固定する為の変数

	private PlayerAttackCreate   pac;					// PlayerTurn時の自動生成用
	//private CutInFillAmount      cutInFillAmount;		// CutInFillAmount開始用
	//private CutInFillAmountEnemy cutInFillAmountEnemy;  // CutInFillAmount（敵用）開始用

	[System.NonSerialized] public long tmpTick;			// tmp.Tickを一時保存する為の変数
	[System.NonSerialized] public int  tmpRand = -1;	// 重複回避用変数（0で初期化すると最初に重なる恐れがあるので負の数で初期化）
	[System.NonSerialized] public PointHandler       pointHandler;
	[System.NonSerialized] public GageHandler        gageHandler;
	[System.NonSerialized] public PowerProgress      powerProgress;
	[System.NonSerialized] public EnemyPowerProgress enemyPowerProgress;	// PlayerTurn中は敵のゲージを溜めない
	[System.NonSerialized] public AudioManager       audioManager;
	[System.NonSerialized] public SpawnPool          sc_spawnPool;
	[System.NonSerialized] public SpawnPool          sc_spawnPoolScore;
	[System.NonSerialized] public Transform          sc_touchedPrefabPool;
	[System.NonSerialized] public Transform          sc_pointTextPool;
	[System.NonSerialized] public Transform          sc_LongTapAnimPool;
	[System.NonSerialized] public Transform          sc_FlickAnimPool;


	void Awake(){

		//mUniSmからロードされたJsonデータを格納
		cacheData = JsonDataBattle.loadJsonData;

		// スコア生成前にメモリを解放しておき、いらないアセットもアンロードしておく
		System.GC.Collect ();
		Resources.UnloadUnusedAssets ();

		// 本日の曜日
//		//Debug.Log (System.DateTime.Now.DayOfWeek);

		//使うか分からんが日付の取得
//		for(int i=0; i<weekList.Length; i++){
//			if("" + System.DateTime.Now.DayOfWeek == weekList[i]){
//				Debug.Log ("Today is " + weekList[i] + ".");
//			}
//		}
	}

	#region Start------------------------------------------------------------------------------------------
//	IEnumerator Start () {
//		while (string.IsNullOrEmpty (this.cacheData))
//			yield return null;
	void Start () {

		// インスタンス関連
		scoreFlickType = new List<Transform> ();
		scoreDate      = new List<MusicDate> ();	
		
		// タッチバーの位置をScoreHandlerに渡す（Static）
		touchBarRect      = GameObject.Find("TouchBar1").GetComponent<RectTransform>();
		sc_spawnPool      = GameObject.Find("PoolObject").GetComponent<SpawnPool>();
		sc_spawnPoolScore = GetComponent<SpawnPool>();

		// エフェクトプール関連
		scorePrefab          = sc_spawnPoolScore.prefabPools ["Score_Image"].prefab;
		scorePrefabLongDown  = sc_spawnPoolScore.prefabPools ["Score_Long"].prefab;
		scorePrefabLongUp    = sc_spawnPoolScore.prefabPools ["Score_LongUp"].prefab;
		scoreFlickType.Add    (sc_spawnPoolScore.prefabPools ["Score_Left"].prefab);
		scoreFlickType.Add    (sc_spawnPoolScore.prefabPools ["Score_Top_Left"].prefab);
		scoreFlickType.Add    (sc_spawnPoolScore.prefabPools ["Score_Top"].prefab);
		scoreFlickType.Add    (sc_spawnPoolScore.prefabPools ["Score_Top_Right"].prefab);
		scoreFlickType.Add    (sc_spawnPoolScore.prefabPools ["Score_Right"].prefab);

		// スコアプール関連
		sc_touchedPrefabPool = sc_spawnPool.prefabPools["ButterflyBroken"].prefab;
		sc_pointTextPool     = sc_spawnPool.prefabPools["PointTextContainer"].prefab;
		sc_LongTapAnimPool   = sc_spawnPool.prefabPools["LongTapAnim"].prefab;
		sc_FlickAnimPool     = sc_spawnPool.prefabPools["FlickAnim"].prefab;

		// 得点やゲージバーアニメーション関連
		pointHandler = FindObjectOfType<PointHandler> ();	 
		gageHandler  = FindObjectOfType<GageHandler>  ();
		audioManager = FindObjectOfType<AudioManager> ();

		//enemyPowerProgress = FindObjectOfType<EnemyPowerProgress> ();// 敵のゲージを溜めない為のフラグ管理用に取得
		//powerProgress      = FindObjectOfType<PowerProgress>();

		//Debug.Log ("powerProgress : " + powerProgress);
		//Debug.Log ("enemyPowerProgress : " + enemyPowerProgress);

		#region ランダム生成のシードをセット
		// ランダム生成のシードをセット（ここで固定しても他のスクリプトのUpdateが割り込んで固定されない）
		// Random.seed = 100;
		// ↓
		// ランダム生成のシードをセット //Random.seed = seed;
		#endregion
		Random.InitState (seed);

		#region IDictionaryについて
		// テキストデータを配列に変換 
		// DeserializeはJSONデータを読み込むメソッド 
		// IDictionaryはキー付きの配列 
		// 型変換も忘れずに（IDictionaryはオブジェクト型で返ってくる）
		#endregion
		IDictionary tmpDate = (IDictionary)Json.Deserialize(this.cacheData);
		//IDictionary tmpDate = (IDictionary)Json.Deserialize(jsonDate.text);

		// 値「”score”」に配列が格納されている
		List<object> arrayDate = (List<object>)tmpDate["score"];	

		long tick  = 0;
		int  value = 0;

		// ロングタップダウンとアップの紐付けの為（96と97のセットなど）新たに連想配列
		Dictionary<int,MusicDate> dic = new Dictionary<int, MusicDate>();

		// arrayDataを解析
		foreach(IDictionary val in arrayDate){

			// eventがnote_onの時のみ格納
			if((string)val["event"] == "note_on"){

				tick = (long)val["tick"];
				value = (int)(long)val["value"];

				// mdはつまり、スコアのことで、スコアはtickとvalueを持っている
				MusicDate md = new MusicDate (tick,value);

				// わざわざ一旦mdをインスタンス化したのは、md（つまり、ロングタップ）のtaptypeが見たいから
				scoreDate.Add(md);

				// もし、スコアのtaptypeがロングタップダウンだったら…
				if(md.tapType == 3){

					//（例としてMIDIが96だったら）96のKeyがあるかどうか判定
					if (dic.ContainsKey (value)) {

						// あれば、96keyにロングタップダウンのtickを格納
						dic [value] = md;

					// なければ…
					} else {

						// keyを96として、md、すなわちダウン時のtickを入れる
						dic.Add (value,md);
					}

				// もしMIDIが97だったら…
				}else if(md.tapType == 4){

					// 現在のアップのスコアが97なので紐づけたいのは-1されたMIDI96のダウンスコア
					// 96のtickは97が生成されて初めて分かり、↓の式だと96の次のスコアはたった今生成された97のスコア
					dic [value - 1].nextTick = tick;

					// 生成後は必ずRemove、通常の配列とは違い、前に詰めない。
					dic.Remove (value - 1);

					#region ※Dictionary型の連想配列はKeyとvalueをセットで格納
					// ※Dictionary型の連想配列はKeyとvalueをセットで格納
					// ※同じtickでもvalue96のロングタップなのか、98のロングタップなのかを判定し、
					// それと対になる97のアップなのか99のアップなのかを紐づける
					#endregion
				}
			}


			// eventがset_tempoの時はテンポ情報（↑『else ifでまとめられそう）
			// 整数系はlong型になってるので一旦変換して再度int型に変換する
			if( (string)val["event"] == "set_tempo" ){

				// 現状、♩=234と♩=236の2個を保持
				TimeManager.tempo.Add( new TimeManager.TempoStructure( (int)(long)val["value"], (long)val["tick"] ) );
			}
		}

		// Tempo再計算
		float prevTempo = -1;

		for( int i = 0; i < TimeManager.tempo.Count; i++ ){
			if (prevTempo > 0) {
				TimeManager.TempoStructure val = TimeManager.tempo [i];
				val.changeTime = TimeManager.tempo[i].changeTick / ( prevTempo * 9600f / 60f );
				TimeManager.tempo [i] = val;
			}
			prevTempo = TimeManager.tempo[i].tempoValue;
		}

		pac = GameObject.Find("PlayerTurn").GetComponent<PlayerAttackCreate>();

		//cutInFillAmount      = GameObject.Find("Cut_In_Player_BackGround").GetComponent<CutInFillAmount>();
		//cutInFillAmountEnemy = GameObject.Find("Cut_In_Enemy_BackGround").GetComponent<CutInFillAmountEnemy> ();

		// 現状ロングタップは全てプリロード（使いまわさない）仕様以外に方法がない
		sc_spawnPoolScore.prefabPools["Score_Long"].preloadAmount   = GameDate.totalLongScore;
		sc_spawnPoolScore.prefabPools["Score_LongUp"].preloadAmount = GameDate.totalLongUpScore;

	}
	#endregion

	#region Update-----------------------------------------------------------------------------------------
	void Update () {
		if (scoreDate == null)
			return;

//		if(powerProgress != null && !powerProgress.isProgress){
//			powerProgress.isProgress = true;
//		}
//
//		if(enemyPowerProgress!= null && !enemyPowerProgress.isProgress){
//			enemyPowerProgress.isProgress = true;
//		}

		// ゲーム終了時に停止
		if(!GameController.isPlaying){ 
			this.enabled = false; 
		}

		// scoreDataをチェック
		foreach( MusicDate tmp in scoreDate ){

			// 指定したTickを超えたものから生成 (『!tmp.isCreated』がないと、既に生成したものに対して再度生成してしまう)
			if (tmp.isCreated) {

				continue;

			} else if (TimeManager.tick >= tmp.tick){

				// 生成フラグをセット（※ここ重要）
				tmp.isCreated = true;

				//GameObject scoreObject = null;
				Transform  scoreObject = null;

				// tapType = 1 : value = 84 : FlickType
				if (tmp.tapType == 1) {

					//Random.seed = seedFlick++;
					Random.InitState (seedFlick++);

					// StartでSeedを設けてるので、一定のRandom性で生成する番号を決める。
					//int typeIndex = Random.Range (0, scoreFlickType.Length);
					int typeIndex = Random.Range (0, scoreFlickType.Count);

					// 配列のscoreFlickTypeに格納したプレハブをtypeIndexに基づいて生成
					//scoreObject = (GameObject)Instantiate (scoreFlickType[typeIndex]);
					scoreObject = PoolManager.Pools ["ScorePool"].Spawn (scoreFlickType[typeIndex]);

					// return mPageObjectPool.Instance.Get(this.content, this.scrollRect.content)
					//第一引数 = プールするプレハブ , 第二引数 = 親のオブジェクト
					// mUUレシーバーがいる
					// 

					// まずScoreCreatorが、生成したスコアのHandlerを取得
					ScoreHandler scoreHandler = scoreObject.GetComponent<ScoreHandler>();

					// 生成されたフリックプレハブの名前に応じて、flickFlgに番号をつける 
					// (0:Left, 1:Top_Left, 2:Top, 3:Top_Right, 4:Right)
					scoreHandler.flickFlag = typeIndex;
					scoreHandler.tapType   = tmp.tapType;

					// 生成したスコアのScoreHandlerにScoreCreatorで取得したpointHandlerたちを渡す事で、
					// ScoreHandler側でのFindを避けている
					scoreHandler.pointHandler         = this.pointHandler;
					scoreHandler.gageHandler          = this.gageHandler;
					scoreHandler.powerProgress        = this.powerProgress;
					scoreHandler.audioManager         = this.audioManager;
					scoreHandler.sh_spawnPool         = this.sc_spawnPool;
					scoreHandler.sh_touchedPrefabPool = this.sc_touchedPrefabPool;
					scoreHandler.sh_pointTextPool     = this.sc_pointTextPool;
					scoreHandler.sh_FlickAnimPool     = this.sc_FlickAnimPool;

					// 配色をランダムで決定（seedは固定）
					Random.InitState (seedColor++);
					int colorIndex = Random.Range (0, scoreColorList.Count);
					scoreHandler.backImage.color = scoreColorList [colorIndex];

					// 生成時に敵ゲージを増やす(属性も渡す)
					gageHandler.setGage_Enemy (colorIndex);

					// 生成時にScoreHandlerに属性を渡す
					scoreHandler.sh_SocreAttribute = colorIndex;

				//  LongTapDawn
				} else if (tmp.tapType == 3) {

					//scoreObject = Instantiate (scorePrefabLongDown);
					scoreObject = PoolManager.Pools ["ScorePool"].Spawn (scorePrefabLongDown);

					ScoreHandler scoreHandler = scoreObject.GetComponent<ScoreHandler>();

					// オリジナルをアクティブにしたければこれでいけるけど、多分使わない。
					//scoreObject.gameObject.transform.FindChild ("Score_Image").gameObject.SetActive (true);

					scoreHandler.longTapStartTick = tmp.tick;
					scoreHandler.longTapEndTick   = tmp.nextTick;
					scoreHandler.tapType          = tmp.tapType;

					// 生成されたscoreObjectのScorteHandlerを取得して、ScoreCreatorでFindしておいたHandler群を渡してあげる
					scoreHandler.pointHandler         = this.pointHandler;
					scoreHandler.gageHandler          = this.gageHandler;
					scoreHandler.powerProgress        = this.powerProgress;
					scoreHandler.audioManager         = this.audioManager;
					scoreHandler.sh_spawnPool         = this.sc_spawnPool;
					scoreHandler.sh_touchedPrefabPool = this.sc_touchedPrefabPool;
					scoreHandler.sh_pointTextPool     = this.sc_pointTextPool;
					scoreHandler.sh_LongTapAnimPool   = this.sc_LongTapAnimPool;

					Shadow_AfterImage saImg = scoreObject.GetComponent <Shadow_AfterImage>();
					saImg.longTapEndPosY    = tmp.nextTick * 0.05f + 3000;
					longTapNum++;

					// ローカルスコープで仕様する変数 : グローバルで宣言したものも有り
					int colorIndex;

					// "違うティック or シングル"
					if(tmp.tick != tmpTick){

						// カラーをランダムで決定（seedは固定）
						Random.InitState (seedColor++);

						// カラーのIndexを設定
						colorIndex = Random.Range (0, scoreColorList.Count);

						// カラーIndexをグローバルで保有
						doubleColorIndex = colorIndex;

						// 色を設定
						scoreHandler.backImage.color = scoreColorList [colorIndex];

						// ロングタップエンドの色をスタートと同じにする
						longTapEndColor = scoreColorList [colorIndex];

						// 生成時にScoreHandlerに属性を渡す
						scoreHandler.sh_SocreAttribute = doubleColorIndex;
					}

					//"違うティック or シングル"の場合は多少冗長だが、もう一度同じ色を決め直す
					//同じティック、すなわちダブルの時は、一度選んだ配色を選択する
					scoreHandler.backImage.color = scoreColorList [doubleColorIndex];

					// 生成時にScoreHandlerに属性を渡す
					scoreHandler.sh_SocreAttribute = doubleColorIndex;

					// アップ生成時にゲージを上昇させるのでここでは属性だけ格納する
					longUpAttribute = doubleColorIndex;

				// LongTapUp
				} else if (tmp.tapType == 4) {

					//scoreObject = Instantiate (scorePrefabLongUp);
					scoreObject = PoolManager.Pools ["ScorePool"].Spawn (scorePrefabLongUp);
					ScoreLongUpController scoreLUC = scoreObject.GetComponent<ScoreLongUpController> ();
					scoreLUC.tapType = tmp.tapType;
					longTapNum--;


					// 絶対なのはここでカラーを渡す時、ロングタップ開始時の色と同じであること
					scoreLUC.longTapColor.color = longTapEndColor;

					// 生成時に敵ゲージを増やす(属性も渡す)
					gageHandler.setGage_Enemy (longUpAttribute);
				
				// CutIn
				} else if (tmp.tapType == 5) {

					pac.StartSpawn();						// 味方ターン時に攻撃開始
					enemyPowerProgress.enemyTurn = false;	// 敵のゲージ加算はPlayerTurn時は停止
					//cutInFillAmount.fadeIn       = true;	// 味方カットイン開始
					/*後にカットイン用のMIDIのValueを設けた方が発生タイミングを制御しやすいだろう*/
				
				// Turn
				} else if (tmp.tapType == 6) {

					pac.StopSpawn();						// 味方の攻撃停止
					enemyPowerProgress.enemyTurn = true;	// 敵のゲージ加算を再始動
					//cutInFillAmountEnemy.enabled = true;	// 敵のカットインスクリプトON
					//cutInFillAmountEnemy.fadeIn  = true;	// 敵のカットイン開始
					/* 後にカットイン用のMIDIのValueを設けた方が発生タイミングを制御しやすいだろう */
				
				// NormalScore
				} else {

					//scoreObject = Instantiate(scorePrefab);
					scoreObject = PoolManager.Pools ["ScorePool"].Spawn (scorePrefab);

					ScoreHandler scoreHandler = scoreObject.GetComponent<ScoreHandler>();

					scoreHandler.tapType              = tmp.tapType;
					scoreHandler.pointHandler         = this.pointHandler;
					scoreHandler.gageHandler          = this.gageHandler;
					scoreHandler.powerProgress        = this.powerProgress;
					scoreHandler.audioManager         = this.audioManager;
					scoreHandler.sh_spawnPool         = this.sc_spawnPool;
					scoreHandler.sh_touchedPrefabPool = this.sc_touchedPrefabPool;
					scoreHandler.sh_pointTextPool     = this.sc_pointTextPool;

					// カラーをランダムで決定（seedは固定）
					Random.InitState (seedColor++);
					int colorIndex = Random.Range (0, scoreColorList.Count);
					scoreHandler.backImage.color = scoreColorList [colorIndex];

					#region プレイヤー用にScoreHandlerに属性を渡す
					// 現在「土=2」が２投目に設定されており、この値をGageHandlerにも渡す
					//gageHandler.scoreAttribute = colorIndex;
					// ↑
					// しかし、ここで渡すとタッチする前に次に生成されたスコアの属性に上書きされてしまうので
					// scoreHandlerに一旦渡し、socreが個々に属性の情報を持つようにする
					#endregion
					scoreHandler.sh_SocreAttribute = colorIndex;

					// 生成時に敵ゲージを増やす(属性も渡す)
					gageHandler.setGage_Enemy (colorIndex);

				}
					
				// 譜面のヒエラルキーを移動
				if (scoreObject != null){
					
					scoreObject.transform.SetParent(transform);

					#region ランダム生成のシードを再セット
					//// ランダム生成のシードを再セット / ここでインクリメントしないと乱数が固定しない（※超重要）////
					//// 音ゲーでは毎回同じスコア配置にしたい。スコアの位置を決めるときにシードを100、101、102…と渡すと固定する ////
					//// 基本一つのUpdateが走っているときは他のスクリプトのUpdateは待機している ////
					//Random.seed = seed++;
					#endregion
					Random.InitState (seed++);

					int rand = CreateRand (tmp);

					#region randの生成をメソッド化
//					// xの位置を決めるため、0~3の乱数を生成
//					int rand = Random.Range(0, ScoreCreator.ScorePositionXList.Length);
//
//					// ロングタップエンドは、必ずロングタップスタートと同じラインに生成する
//					// 先に下記のtapType==3を見に行く
//					// taptype==3でrandDic[key=96、第○レーン]と指定しておく。そしてtapType==4で生成されたときに…
//					if (tmp.tapType == 4) {
//
//						// この97のスコアは-1のrandDic[96]のレーンであると指定
//						rand = randDic[tmp.value - 1];
//
//						// randDic[96]のレーンと97のレーンを紐づけた段階でいらないのでRemoveしておく
//						randDic.Remove (tmp.value - 1);
//
//						// longTapNumが0じゃないということは、つまりロングタップが生成されているということ
//					} else if (longTapNum != 0) {
//
//						// ロングタップ中は、その他の音符はロングタップと違うラインに生成する → もし重なったらRondom.Seedで再度選び直す処理
//						// randDicのvalueがある間はループする。falseになって抜けるのは、taptype==4が生成されて、randDic.Removeされたとき
//						while (randDic.ContainsValue(rand)){
//
//							rand = Random.Range(0, ScoreCreator.ScorePositionXList.Length);
//
//						}
//					}
//
//					// tick数が同じ且つ、randが同じ場合は、乱数を再生成
//					if (tmp.tick == tmpTick && rand == tmpRand) {
//
//						int count = 0;
//
//						while (tmpRand == rand) {
//							rand = Random.Range(0, ScoreCreator.ScorePositionXList.Length);
//							count++;
//							if (count >= 10) {
//								break;
//							}
//						}
//					}
					#endregion

					// 生成した乱数を格納
					tmpRand = rand;

					// ボタンアニメーションの修正(Shimo)
					scoreObject.tag = (rand + 1).ToString();

					// 譜面のXの位置を決定
					float x = ScoreCreator.ScorePositionXList[rand];

					// 譜面のYの位置を決定
					float y = tmp.tick * 0.05f + 3000;

					// 譜面の位置を移動※Y軸はボードの移動によって決定
					scoreObject.transform.localPosition = new Vector3 (x, y, 0);

					// 譜面のスケールをリセット
					scoreObject.transform.localScale = Vector3.one;

					//出現したものの表示順を最奥に設定
					scoreObject.transform.SetAsFirstSibling ();

					//TouchBarを渡す
					if (tmp.tapType != 4) {

						ScoreHandler sh = scoreObject.GetComponent<ScoreHandler>();

						sh.touchBar = touchBars [rand];

						TouchBarAnim (sh, scoreObject.tag);

						#region タッチバーアニメーションを配列・関数化
						// スコアのタグと同じレーンのTouchBarのアニメーションを開始
//						if(      scoreObject.tag == "1"){
//							sh.buttonAnim = touchBarAnim1;
//						}else if(scoreObject.tag == "2") {
//							sh.buttonAnim = touchBarAnim2;
//						}else if(scoreObject.tag == "3") {
//							sh.buttonAnim = touchBarAnim3;
//						}else if(scoreObject.tag == "4") {
//							sh.buttonAnim = touchBarAnim4;
//						}
						#endregion

						#region オブジェクトプール時のスコアの注意
						// ScoreHandlerのStartは一回しか呼ばれないので、
						// ここで一旦isCheckedをfalseにしておかないと
						// ScoreHandlerのupdateでisChecked = trueのときに呼ばれる筈の
						// ShowText()等が呼ばれなくなる
						// ScoreHandler側にOnEnable()を追加して
						// そちらでアクティブ時に切り替えてもいいかもしれないが
						// 出来るだけScoreHandlerの仕事は減らす為にここで処理しておく
						#endregion

						sh.isChecked   = false;
						sh.isEnemyGage = false;
					}

					// 生成された音符のtickを一時保存
					tmpTick = tmp.tick;

					if (tmp.tapType == 3) {
						
						#region ここでの処理概要
						// 特定のキーや値を持つ項目がハッシュテーブルに格納されているかどうかを調べるには、
						// ContainsKeyメソッドあるいはContainsValueメソッドを使用する
						// 「96というvalueがあれば、ifに入り、randDic[96]に第○レーンを指定」
						// 「なければ、elseにはいり、randDicにkeyを96としてセットし、第○レーンと指定」
						#endregion

						if (randDic.ContainsKey (tmp.value)) {
							
							randDic[tmp.value] = rand;
						} else {
							randDic.Add (tmp.value, rand);
							// taptype == 4へ戻る
						}
					}
				}
			} else if (tmp.tick > (TimeManager.tick + 9600)) {
				break;
			}
		}
	}
	#endregion

	#region tmpから適切なrandを生成するメソッド（無限ループ対策してません）------------------------------------------
	private int CreateRand(MusicDate tmp) {
		
		int rand;

		// ロングタップエンドのとき
		if (tmp.tapType == 4) {
			rand = randDic[tmp.value - 1];
			randDic.Remove(tmp.value - 1);
		} 

		// フリックのとき
		else if (tmp.tapType == 1) {
			rand = Random.Range(1, ScoreCreator.ScorePositionXList.Length-1);
		} 

		// その他
		else {
			rand = Random.Range(0, ScoreCreator.ScorePositionXList.Length);
		}

		// 同tickのスコアとrandが被らなくなるまで再帰（自分で自分を呼ぶ ← whileの代わり）
		if (tmp.tick == tmpTick && rand == tmpRand) {
			rand = CreateRand(tmp);
		}

		// ロングタップでレーンが占有されていたら別レーンになるまで再帰
		if (longTapNum != 0 && randDic.ContainsValue(rand)) {
			rand = CreateRand(tmp);
		}
		return rand;
	}
	#endregion

	#region タッチバーにアニメーションを渡すメソッド : 配列にすべきだが数が固定で少ないので現状このまま-------------------
	private void TouchBarAnim(ScoreHandler sh_Anim, string tag){
		switch (tag) {
			case "1":
				sh_Anim.buttonAnim = touchBarAnim [0];
				break;
			case "2":
				sh_Anim.buttonAnim = touchBarAnim [1];
				break;
			case "3":
				sh_Anim.buttonAnim = touchBarAnim [2];
				break;
			case "4":
				sh_Anim.buttonAnim = touchBarAnim [3];
				break;
		}
	}
	#endregion
}
