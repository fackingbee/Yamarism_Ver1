using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PointHandler : MonoBehaviour {

	public Text pointText;
	public Text comboText;

	void Start(){

	}

	#region ポイントセット
	public void setPoint(long point){

		// アニメーションを止める
		StopCoroutine("PointAnimation");

		// アニメーションスタート
		StartCoroutine(
			PointAnimation(
				long.Parse(pointText.text),
				point,
				0.2f
			)
		);
	}
	#endregion

	#region コルーチン : ポイントアニメーション
	private IEnumerator PointAnimation(long start, long end, float time){

		// アニメーション開始時間
		float startTime = TimeManager.time;

		// アニメーション終了時間
		float endTime = startTime + time;

		// 1フレームごとに数値を上昇させる
		do{
			// アニメーション中の今の経過時間を計算
			float t = (TimeManager.time - startTime) / time;

			// 数値を更新
			long updateValue = (long)( ((end - start) * t) + start );

			// テキストを更新
			//GetComponent<Text>().text = updateValue.ToString();
			pointText.text = updateValue.ToString();


			// 1フレーム待つ
			yield return null; 
		} while (TimeManager.time < endTime);

		// 数値を最終値に合わせる
		//GetComponent<Text>().text = end.ToString();
		pointText.text = end.ToString();

	}
	#endregion

	#region コンボセット
	public void setCombo(int combo){

		// アニメーションを止める
		StopCoroutine("PointAnimation");

		// アニメーションスタート
		StartCoroutine(
			ComboAnimation(
				int.Parse(comboText.text),
				combo,
				0.2f
			)
		);
	}
	#endregion

	#region コルーチン : コンボアニメーション
	private IEnumerator ComboAnimation(int start, int end, float time){

		// アニメーション開始時間
		float startTime = TimeManager.time;

		// アニメーション終了時間
		float endTime = startTime + time;

		// 1フレームごとに数値を上昇させる
		do{
			// アニメーション中の今の経過時間を計算
			float t = (TimeManager.time - startTime) / time;

			// 数値を更新
			int updateValue = (int)( ((end - start) * t) + start );

			// テキストを更新
			comboText.text = updateValue.ToString();

			// 1フレーム待つ
			yield return null;

		}while(TimeManager.time < endTime);

		// 数値を最終値に合わせる
		comboText.text = end.ToString();
	}
	#endregion
}
