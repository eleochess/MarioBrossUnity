using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	[HideInInspector] public Enumerators.EnemyType pu_enemyType = Enumerators.EnemyType.na;

	public void GetDamage() {
		switch (pu_enemyType) {
		case Enumerators.EnemyType.goomba:
			GetComponent<GoombaBehavior> ().GetDamage ();
			break;
		case Enumerators.EnemyType.turtle:
			break;
		case Enumerators.EnemyType.flower:
			break;
		case Enumerators.EnemyType.fireball:
			break;
		default:
			break;
		}
	}
}
