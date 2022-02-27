using UnityEngine;
using System.Collections;

public class HitBoxBehavior : MonoBehaviour {

	public Transform[] pu_powerUp;
	public float pu_yHitVelocity = 1f;
	public bool pu_breakable;
	public GameObject pu_breakAnimation;
	[HideInInspector] public bool pu_isHit = false;

	private SpriteRenderer pr_spriteRenderer;
	private Rigidbody2D pr_rigidbody2D;
	private AudioSource pr_audioSource;
	private Animator pr_animator;
	private float pr_yStartPosition;
	private float pr_bottomCollisionTolerance = 0.015f;
	private float pr_bottomTriggerTolerance = 0.045f;
	private bool pr_isDead = false;
	private bool pr_isHit = false;
	private int pr_currentPowerUpIndex = 0;

	private string pr_anim_Dead = "Dead";

	void Start () {
		pr_spriteRenderer = GetComponent<SpriteRenderer> ();
		pr_rigidbody2D = GetComponent<Rigidbody2D> ();
		pr_audioSource = GetComponent<AudioSource> ();
		pr_animator = GetComponent<Animator> ();

		pr_rigidbody2D.isKinematic = true;
		pr_yStartPosition = transform.position.y;
	}

	void FixedUpdate () {
		if (pr_isHit && transform.position.y < pr_yStartPosition) {
			pr_rigidbody2D.isKinematic = true;
			pr_rigidbody2D.velocity = new Vector2();
			transform.position = new Vector3 (transform.position.x, pr_yStartPosition, transform.position.z);
			pr_isHit = false;
		}
	}

	void OnCollisionEnter2D (Collision2D pa_coll) {
		if (pa_coll.gameObject.tag == StringNaming.Tag_Player) {
			PlayerCollision (pa_coll.gameObject, pr_bottomCollisionTolerance);
		}
	}

	void OnTriggerEnter2D (Collider2D pa_coll) {
		if (pa_coll.gameObject.tag == StringNaming.Tag_Player) {
			PlayerCollision (pa_coll.gameObject, pr_bottomTriggerTolerance);
		}
	}

	void PlayerCollision(GameObject pa_player, float pa_tolerance) {
		float vl_yPlayerContact = pa_player.transform.position.y + pa_player.GetComponent<SpriteRenderer> ().bounds.size.y;
		float vl_yPlayerContactBefore = pa_player.GetComponent<Player> ().pu_backwardPosition.y + pa_player.GetComponent<SpriteRenderer> ().bounds.size.y;
		float vl_yHitboxLimit = transform.position.y - pr_spriteRenderer.bounds.size.y + pa_tolerance;

		if (vl_yPlayerContact < vl_yHitboxLimit && vl_yPlayerContactBefore < vl_yHitboxLimit) {
			pa_player.GetComponent<Rigidbody2D> ().velocity = new Vector2 (pa_player.GetComponent<Rigidbody2D> ().velocity.x, 0);

			if (pr_isDead) {
				pr_audioSource.Play ();
				return;
			}

			HitAnimation ();

			if (pr_currentPowerUpIndex < pu_powerUp.Length) {
				Instantiate (pu_powerUp [pr_currentPowerUpIndex], transform.position + new Vector3 (pr_spriteRenderer.bounds.size.x / 2, -pr_spriteRenderer.bounds.size.y - 0.03f, 0), Quaternion.identity);
				//Instantiate (pu_powerUp [pr_currentPowerUpIndex], transform.position, Quaternion.identity); <= Use when the hitbox pivot is at the middle bottom of the object
				pr_currentPowerUpIndex++;
			}

			if (pr_currentPowerUpIndex >= pu_powerUp.Length) {
				if (pu_breakable) {
					if (pa_player.GetComponent<Player> ().pu_form != Enumerators.playerForm.small) {
						if (transform.parent != null) {
							GameObject vl_colliderContent = transform.parent.gameObject;
							if (vl_colliderContent.tag == StringNaming.Tag_ColliderGenerator) {
								transform.parent = null;
								if (vl_colliderContent.transform.childCount > 0)
									vl_colliderContent.GetComponent<ColliderGenerator> ().pu_redoCollider = true;
								else
									Destroy (vl_colliderContent);
							}
						}
						Instantiate (pu_breakAnimation, transform.position, Quaternion.identity);
						Destroy (gameObject);
					}
				} else {
					pr_animator.SetBool (pr_anim_Dead, true);
					pr_isDead = true;
				}
			}
		}
	}

	void HitAnimation() {
		pr_audioSource.Play ();

		pr_rigidbody2D.isKinematic = false;
		pr_rigidbody2D.velocity = new Vector2 (0f, pu_yHitVelocity);
		pr_isHit = true;
	}
}
