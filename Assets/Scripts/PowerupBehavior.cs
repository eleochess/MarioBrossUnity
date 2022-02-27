using UnityEngine;
using System.Collections;

public class PowerupBehavior : MonoBehaviour {

	public Enumerators.PowerUpType pu_powerUp = Enumerators.PowerUpType.na;
	public bool pu_customParameters = false;
	public float pu_speed = 0.6f;
	public float pu_appearingSpeed = 0.2f;
	public bool pu_scrollUpOnInstantiate = true;
	[HideInInspector] public bool pu_isConsumableYet = false;
	[HideInInspector] public bool pu_itMoves = true;

	private float pr_upDistance = 0, pr_yPosition = 0;
	private bool pr_is_moving = true;
	private Vector3 pr_previus_position;
	private Movement pr_movementScript;
	private Rigidbody2D pr_rigidbody2D;
	private BoxCollider2D pr_boxCollider2D;

	void Start () {
		pr_rigidbody2D = GetComponent<Rigidbody2D>();
		pr_boxCollider2D = GetComponent<BoxCollider2D>();
		pr_movementScript = GetComponent<Movement> ();

		pr_upDistance = GetComponent<SpriteRenderer>().bounds.size.y;
		pr_yPosition = transform.position.y + pr_upDistance;
		pr_previus_position = transform.position;

		if (!pu_customParameters) {
			switch (pu_powerUp) {
			case Enumerators.PowerUpType.mushroom:
				PowerUpSettings (0.6f, 0.2f, true, true);
				break;
			case Enumerators.PowerUpType.flower:
				PowerUpSettings (0, 0.2f, true, true);
				break;
			default:
				break;
			}
		}

		if (pu_scrollUpOnInstantiate) {
			pr_boxCollider2D.isTrigger = true;
			pu_isConsumableYet = false;
			pr_rigidbody2D.isKinematic = true;
		}
	}

	void FixedUpdate () {
		if (!pr_movementScript.pu_isMoving) return;

		if (pu_scrollUpOnInstantiate) {
			pr_rigidbody2D.velocity = new Vector2 (pr_rigidbody2D.velocity.x, pu_appearingSpeed);

			if (transform.position.y > pr_yPosition / 3)
				pu_isConsumableYet = true;

			if (transform.position.y > pr_yPosition) {
				pr_boxCollider2D.isTrigger = false;
				pu_scrollUpOnInstantiate = false;
				pr_rigidbody2D.isKinematic = false;
			}
			return;
		}

		if (pu_itMoves) {
			pr_is_moving = pr_previus_position != transform.position;
			pr_previus_position = transform.position;

			if (!pr_is_moving) pu_speed = pu_speed * -1f;
			pr_rigidbody2D.velocity = new Vector2 (pu_speed, pr_rigidbody2D.velocity.y);
		}
	}

	void OnTriggerEnter2D (Collider2D pa_coll) {
		if (pa_coll.gameObject.tag.Equals(StringNaming.Tag_BodyCollider) && pu_isConsumableYet) {
			GameObject pr_player = pa_coll.transform.parent.gameObject;
			pr_player.GetComponent<Player> ().SwitchState (pu_powerUp);
			Destroy (gameObject);
		}
	}

	void PowerUpSettings(float pa_speed, float pa_appearingSpeed, bool pa_scrollUpOnInstantiate, bool pa_itMoves) {
		pu_speed = pa_speed;
		pu_appearingSpeed = pa_appearingSpeed;
		pu_scrollUpOnInstantiate = pa_scrollUpOnInstantiate;
		pu_itMoves = pa_itMoves;
	}
}