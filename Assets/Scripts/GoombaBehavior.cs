using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaBehavior : MonoBehaviour {

	public float pu_speed = 20f;
	public bool pu_hasHeadWeakness = true;
	public float pu_playerRebound = 1.6f;

	private float pr_insideTimeTolerance = 0.25f;
	private float pr_currentInsideTime = 0;
	private float pr_dyingTime = 0.75f;
	private float pr_currentdyingTime = 0;
	private float pr_aditionalSpritePixelSize = 0.015f;
	private Rigidbody2D pr_rigidbody2D;
	private BoxCollider2D pr_boxCollider2D;
	private SpriteRenderer pr_spriteRenderer;
	private Animator pr_animator;
	private Movement pr_movementScript;
	private Enemy pr_enemy;
	private bool pr_isMoving;
	private bool pr_itStops;
	private bool pr_Dying = false;
	private Vector3 pr_previusPosition;

	private string pr_anim_Dead = "Dead";

	void Start () {
		pr_rigidbody2D = GetComponent<Rigidbody2D> ();
		pr_boxCollider2D = GetComponent<BoxCollider2D> ();
		pr_spriteRenderer = GetComponent<SpriteRenderer> ();
		pr_animator = GetComponent<Animator> ();
		pr_movementScript = GetComponent<Movement> ();
		pr_enemy = GetComponent<Enemy> ();

		pr_enemy.pu_enemyType = Enumerators.EnemyType.goomba;
		pr_previusPosition = transform.position;
		ActiveObject (false);
	}

	void FixedUpdate () {
		if (!pr_movementScript.pu_isMoving) return;

		if (!pr_isMoving) return;

		if (pr_Dying) {
			Dying ();
			return;
		}

		pr_itStops = pr_previusPosition != transform.position;
		pr_previusPosition = transform.position;

		if (!pr_itStops) pu_speed = pu_speed * -1f;
		pr_rigidbody2D.velocity = new Vector2(pu_speed * Time.deltaTime, pr_rigidbody2D.velocity.y);
	}

	void OnBecameVisible() {
		ActiveObject (true);
	}

	void ActiveObject(bool pa_isActive) {
		pr_rigidbody2D.isKinematic = !pa_isActive;
		pr_isMoving = pa_isActive;
	}

	void OnTriggerEnter2D (Collider2D pa_coll) {
		if (pa_coll.gameObject.tag == StringNaming.Tag_BodyCollider) {
			if (pu_hasHeadWeakness) {
				float vl_currentPlayerPosition = pa_coll.transform.parent.transform.position.y;
				float vl_backwardPlayerPosition = pa_coll.transform.parent.GetComponent<Player> ().pu_backwardPosition.y;
				float vl_enemyHeadLimit = transform.position.y + pr_spriteRenderer.bounds.size.y - pr_aditionalSpritePixelSize;

				if (vl_currentPlayerPosition > vl_enemyHeadLimit || vl_backwardPlayerPosition > vl_enemyHeadLimit) {
					Rigidbody2D vl_playerRigidbody2D = pa_coll.transform.parent.GetComponent<Rigidbody2D> ();
					vl_playerRigidbody2D.velocity = new Vector2 (vl_playerRigidbody2D.velocity.x, pu_playerRebound);
					GetDamage ();
				} else {
					DamagePlayerOnCollide (pa_coll);
				}
			} else {
				DamagePlayerOnCollide (pa_coll);
			}
		}
	}

	void OnTriggerStay2D (Collider2D pa_coll) {
		if (pa_coll.gameObject.tag == StringNaming.Tag_BodyCollider) {
			if (pu_hasHeadWeakness) {
				pr_currentInsideTime += Time.deltaTime;
				if (pr_currentInsideTime > pr_insideTimeTolerance) {
					DamagePlayerOnCollide (pa_coll);
				}
			} else {
				DamagePlayerOnCollide (pa_coll);
			}
		}
	}

	void OnTriggerExit2D (Collider2D pa_coll) {
		if (pa_coll.gameObject.tag == StringNaming.Tag_BodyCollider) {
			pr_currentInsideTime = 0;
		}
	}
	void DamagePlayerOnCollide(Collider2D pa_coll) {
		GameObject pr_player = pa_coll.transform.parent.gameObject;
		pr_player.GetComponent<Player> ().GetDamage (gameObject);
	}

	public void GetDamage() {
		pr_animator.SetBool (pr_anim_Dead, true);
		pr_rigidbody2D.velocity = new Vector2 ();
		pr_rigidbody2D.isKinematic = true;
		pr_boxCollider2D.enabled = false;
		pr_Dying = true;
	}

	void Dying() {
		pr_currentdyingTime += Time.deltaTime;
		if (pr_currentdyingTime > pr_dyingTime)
			Destroy (gameObject);
	}
}
