using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class ToGame : MonoBehaviour {

	private Scene          currentScene;			// 動作しているシーンを格納する変数
	private AsyncOperation nextScene;				// ロードシーンを格納する変数
	private int            loadState    = 0;		// ロード前
	private bool           isLoad       = false;    // ロードフラグ

	void Start() {

		currentScene = SceneManager.GetActiveScene ();

		Debug.Log ("現在のシーンのインデックス : " + currentScene.buildIndex);
		Debug.Log ("現在のシーン名 : " + currentScene.name);

		// 完了しているかどうか
		if(currentScene.isLoaded){
			Debug.Log ("完了");
		}

		// テスト
		//Invoke ("hoge",5f);

	}

	void Update() {

		// ロードが終了したら
		if(loadState == 2 && isLoad){
			
			isLoad = false;

			// ここの遅延は遷移の微調整の為に儲けた
			Invoke ("SceneMove", 2f);

		}
	}


	// この関数をシーン別に量産し、指定して使い分ける
	public void toGame() {

		//ここで遷移トランジェントが走る

		// 一度シーン読み込みを開始したら、もう呼ばない（引数は関数化しておこうか）
		if(loadState > 0){
			return;
		}

		// こルーチンに引数を渡して配列から取得して管理
		StartCoroutine(LoadNextScene(7));

	}
		
//	public void hoge(){
//		StartCoroutine (LoadNextScene("GUISystem"));
//	}



	// オーバーロード① : シーンのインデックスで指定
	IEnumerator LoadNextScene(int sceneIndex){

		//Debug.Log ("インデックスで移動");

		float start = Time.realtimeSinceStartup;
	
		// ロード中
		loadState = 1;

		// 移動先を格納（色々考えるのがめんどくさいので破棄してからの移動でもうええやん）
		nextScene = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

		// ロードが終わっても、Trueになるまでシーン移動は行わない
		nextScene.allowSceneActivation = false;

		// ロード進行が90％を超えるまで、且つ２秒経過するまで
		while(nextScene.progress < 0.9f  && Time.realtimeSinceStartup - start < 1 ){
			//Debug.Log (nextScene.progress * 100 + "%");
			yield return null;
		}
			
		// 読み込みが終わったら
		loadState = 2;
		isLoad = true;

// 簡単な方法ならこちら
//		float start = Time.realtimeSinceStartup;
//		nextScene = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Single);
//		nextScene.allowSceneActivation = false;
//
//		while (Time.realtimeSinceStartup - start < 3 ){
//			yield return null;
//		}
//		nextScene.allowSceneActivation = true;

	}



	// オーバーロード② : シーン名で指定（微妙に演出を変えたい場合に使うと良いかも）
	IEnumerator LoadNextScene(string sceneName){

		//Debug.Log ("名前で移動");

		float start = Time.realtimeSinceStartup;

		// ロード中
		loadState = 1;

		// 移動先を格納（色々考えるのがめんどくさいので破棄してからの移動でもうええやん）
		nextScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

		// ロードが終わっても、Trueになるまでシーン移動は行わない
		nextScene.allowSceneActivation = false;

		// ロード進行が90％を超えるまで、且つ２秒経過するまで
		while(nextScene.progress < 0.9f  && Time.realtimeSinceStartup - start < 2 ){
			//Debug.Log (nextScene.progress * 100 + "%");
			yield return null;
		}

		// 読み込みが終わったら
		loadState = 2;
		isLoad = true;

	}
		
	// 最終的な移動の許可
	public void SceneMove(){
		nextScene.allowSceneActivation = true; 
	}

}
