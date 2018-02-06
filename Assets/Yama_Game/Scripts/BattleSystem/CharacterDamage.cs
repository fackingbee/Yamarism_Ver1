using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class CharacterDamage : MonoBehaviour {

	public  Animator    playerDamage;
	public  Animator    enemyDamage;
	public  GageHandler cd_gageHandler;
	private bool        isPlayerDamage;
	private bool        isPlayerNotDamage;
	private bool        isEnemyDamage;
	private bool        isEnemyNotDamage;
	const   float       degree = 0.08f;

	[System.NonSerialized] public  SpawnPool spawnPool;
	[System.NonSerialized] public  Transform playerDamageBomb;
	[System.NonSerialized] public  Transform enemyDamageBomb;

	void Start () {
		playerDamage      = GameObject.Find ("PlayerIcon").GetComponent<Animator>();
		enemyDamage       = GameObject.Find ("EnemyIcon") .GetComponent<Animator>();
		spawnPool         = GameObject.Find ("PoolObject").GetComponent<SpawnPool>();
		cd_gageHandler 	  = GameObject.Find ("BattleGages").GetComponent<GageHandler>();
		playerDamageBomb  = spawnPool.prefabPools ["PlayerDamageBomb"].prefab;
		enemyDamageBomb   = spawnPool.prefabPools ["EnemyDamageBomb"] .prefab;
		isPlayerDamage    = true;
		isPlayerNotDamage = true;
		isEnemyDamage     = true;
		isEnemyNotDamage  = true;
	}


	void OnTriggerEnter(Collider col){
		
		// PlayerDamage
		if(col.gameObject.tag == "EnemyAttack" ){
			Transform playerDamageBombObj = PoolManager.Pools ["PoolObject"].Spawn (playerDamageBomb);
			isPlayerDamage = true;
			playerDamage.SetBool ("isPlayerDamage",    isPlayerDamage);
			playerDamage.SetBool ("isPlayerNotDamage", isPlayerNotDamage);
			Shake ();
			PoolManager.Pools ["PoolObject"].Despawn (playerDamageBombObj, 4f);

			cd_gageHandler.setGageDamage (GameDate.enemyAttack);


		}

		// EnemyDamage
		if(col.gameObject.tag == "PlayerAttack" ){
			Transform enemyDamageBombObj = PoolManager.Pools ["PoolObject"].Spawn (enemyDamageBomb);
			isEnemyDamage = true;
			enemyDamage.SetBool ("isDamage",   isEnemyDamage);
			enemyDamage.SetBool ("isNotDamage",isEnemyNotDamage);
			Shake ();
			PoolManager.Pools ["PoolObject"].Despawn (enemyDamageBombObj, 4f);

			cd_gageHandler.setGageDamageEnemy (GameDate.playerAttack);
		}
	}

	void OnTriggerExit(Collider col_02){

		// PlayerDamage
		if(col_02.gameObject.tag == "EnemyAttack"){
			//cd_gageHandler.setGageDamage (GameDate.enemyAttack);
			isPlayerDamage = false;
			playerDamage.SetBool ("isPlayerDamage",isPlayerDamage);
		}

		// EnemyDamage
		if(col_02.gameObject.tag == "PlayerAttack"){
			//cd_gageHandler.setGageDamageEnemy (GameDate.playerAttack);
			isEnemyDamage = false;
			enemyDamage.SetBool ("isDamage",isEnemyDamage);
		}
	}

	#region メソッド : キャラクターダメージエフェクト
	public void Shake(){
		iTween.ShakePosition (gameObject,iTween.Hash
			("x", degree,"y", degree,"isLocal", false,"time", 1f));
	}
	#endregion
}
