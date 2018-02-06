using UnityEngine;
using System.Collections;

public class EnemyDamage : MonoBehaviour {

	private Animator   enemyDamage;
	public  GameObject enemyDamageBomb;
	private bool       isEnemyDamage;
	private bool       isEnemyNotDamage;

	// 攻撃を受けたらダメージエフェクトとしてブレ発生
	public DamageShake damageShake;


	void Start () {
		isEnemyDamage          = true;
		isEnemyNotDamage       = true;
		enemyDamage       = GetComponent<Animator> ();
		damageShake       = GetComponent<DamageShake> ();
	}

//	void Update () {
//
//	
//	}
		
	void OnTriggerEnter(Collider col){
		
		if(col.gameObject.tag == "PlayerAttack" ){
			
			Instantiate (enemyDamageBomb);

			isEnemyDamage    = true;

			enemyDamage.SetBool ("isDamage", isEnemyDamage);
			enemyDamage.SetBool ("isNotDamage", isEnemyNotDamage);

			damageShake.Shake ();

		}
	}


	void OnTriggerExit(Collider col_02){
		
		if(col_02.gameObject.tag == "PlayerAttack"){
			
			isEnemyDamage    = false;

			enemyDamage.SetBool ("isDamage", isEnemyDamage);

		}
	}
}
