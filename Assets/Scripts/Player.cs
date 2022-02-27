using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float pu_speed = 2.8f;
	public float pu_jumpForce = 150f;
	public float pu_raycastBottomDistance = 0.01f;
	public float pu_raycastTopDistance = -0.0018f;
	public float pu_raycastEdgesGap = 0.02f;
	public float pu_invincibleTime = 2.5f;
	public float pu_feetColliderHeight = 0.02f;
	public bool pu_airControl = true;
	public LayerMask pu_whatIsStepable;
	public Enumerators.playerForm pu_form = Enumerators.playerForm.small;
	public Enumerators.playerState pu_state = Enumerators.playerState.normal;
	public PhysicsMaterial2D pu_material;
	public GameObject pu_deadPrefab;
	[HideInInspector] public bool pu_facingRight = true;
	[HideInInspector] public Vector3 pu_currentPosition = new Vector3();
	[HideInInspector] public Vector3 pu_backwardPosition = new Vector3();

	private float pr_colliderGap = 0.03f;
	private float pr_currentTimeInvincible = 0f;
	private bool pu_grounded = false;
	private bool pu_isCrouching = false;
	private SpriteRenderer pr_spriteRenderer;
	private Animator pr_animator;
	private Rigidbody2D pr_rigidbody2D;
	private BoxCollider2D pr_boxCollider2D, pr_bodyCollider2D, pr_feetCollider2D;
	private Vector2 pr_bottomLeftDirection, pr_bottomRightDirection; //, pr_topLeftDirection, pr_topRightDirection;
	private Vector2 pr_bottomLeftSpot, pr_bottomRightSpot; //, pr_topLeftSpot, pr_topRightSpot;
	private Vector2 pr_vector2Position;
	private Vector2 pr_velocity;
	private RaycastHit2D[] pr_raycastFloor, pr_raycastHead;
	private Movement pr_movementScript;
	private HitBoxBehavior pr_hitboxScript;

	private string pr_anim_vSpeed = "vSpeed";
	private string pr_anim_hSpeed = "hSpeed";
	private string pr_anim_Ground = "Ground";
	private string pr_anim_Crouch = "Crouch";
	private string pr_anim_GrowUp = "GrowUp";
	private string pr_anim_Shrink = "Shrink";
	private string pr_anim_Flower = "Flower";
	private string pr_anim_Invincible = "Invincible";

	void Start () {
		pr_spriteRenderer = GetComponent<SpriteRenderer>();
		pr_animator = GetComponent<Animator>();
		pr_rigidbody2D = GetComponent<Rigidbody2D>();
		pr_movementScript = GetComponent<Movement> ();

		BoxCollider2DCreation ();
		RaycastSpotSettings ();
	}

	void FixedUpdate () {
		if (!pr_movementScript.pu_isMoving) return;

		// Getting the last position (NOT CURRENT)
		pu_backwardPosition = pu_currentPosition;
		pu_currentPosition = transform.position;

		InvincibleVerification ();
		Vector2PositionsUpdate ();
		GizmosRaycastShowUp ();

		pr_raycastFloor = RaycastFloorActivate();
		pu_grounded = pr_raycastFloor.Length > 0;
		pr_animator.SetBool(pr_anim_Ground, pu_grounded);

		/*pr_raycastHead = RaycastHeadActivate();
		foreach (RaycastHit2D fe_ray in pr_raycastHead) {
			pr_hitboxScript = null;
			pr_hitboxScript = fe_ray.transform.GetComponent<HitBoxBehavior> ();
			if (pr_hitboxScript != null) {
				if (!pr_hitboxScript.pu_isHit) {
					pr_hitboxScript.pu_isHit = true;
					pr_rigidbody2D.velocity -= new Vector2 (0f, pr_rigidbody2D.velocity.y);
					Debug.Log ("Hitting Hitbox");
				}
			}
		}*/

		pr_animator.SetFloat(pr_anim_hSpeed, Mathf.Abs(pr_rigidbody2D.velocity.x));
		pr_animator.SetFloat(pr_anim_vSpeed, pr_rigidbody2D.velocity.y);
	}

	// ***** Start methods: INI
	void BoxCollider2DCreation() {
		pr_boxCollider2D = gameObject.AddComponent<BoxCollider2D> ();
		pr_boxCollider2D.size -= new Vector2 (1f, 1f) * pr_colliderGap;
		pr_boxCollider2D.sharedMaterial = pu_material;
		pr_boxCollider2D.sharedMaterial.friction = 0f;

		GameObject vl_bodyCollider = Instantiate (new GameObject ("BodyCollider"), transform.position + new Vector3(0f, pr_spriteRenderer.bounds.size.y / 2, 0f), Quaternion.identity, transform);
		vl_bodyCollider.tag = StringNaming.Tag_BodyCollider;
		pr_bodyCollider2D = vl_bodyCollider.AddComponent<BoxCollider2D> ();
		pr_bodyCollider2D.size = pr_boxCollider2D.size;
		pr_bodyCollider2D.isTrigger = true;

		/*GameObject vl_feetCollider = Instantiate (new GameObject ("FeetCollider"), transform.position - new Vector3(0f, pr_colliderGap / 2 + pu_feetColliderHeight / 2, 0f), Quaternion.identity, transform);
		vl_feetCollider.tag = StringNaming.Tag_FeetCollider;
		pr_feetCollider2D = vl_feetCollider.AddComponent<BoxCollider2D> ();
		pr_feetCollider2D.size = new Vector2 (pr_boxCollider2D.size.x, pu_feetColliderHeight);
		pr_feetCollider2D.isTrigger = true;*/
	}

	void RaycastSpotSettings() {
		float vl_width = pr_spriteRenderer.bounds.size.x;
		//float vl_height = pr_spriteRenderer.bounds.size.y;

		// Bottom spots
		pr_bottomLeftDirection = new Vector2 ((vl_width / 2) * -1 + pu_raycastEdgesGap, -pu_raycastBottomDistance);
		pr_bottomRightDirection = new Vector2 (vl_width / 2 - pu_raycastEdgesGap, -pu_raycastBottomDistance);

		// Top spots
		//pr_topLeftDirection = new Vector2 ((vl_width / 2) * -1 + pu_raycastEdgesGap, pu_raycastTopDistance + vl_height);
		//pr_topRightDirection = new Vector2 (vl_width / 2 - pu_raycastEdgesGap, pu_raycastTopDistance + vl_height);
	}
	// ***** Start methods: END

	// ***** Update methods: INI
	void Vector2PositionsUpdate() {
		pr_vector2Position = Utilities.GetVector2 (transform.position);
		pr_bottomLeftSpot = pr_vector2Position + pr_bottomLeftDirection;
		pr_bottomRightSpot = pr_vector2Position + pr_bottomRightDirection;
		//pr_topLeftSpot = pr_vector2Position + pr_topLeftDirection;
		//pr_topRightSpot = pr_vector2Position + pr_topRightDirection;
	}

	RaycastHit2D[] RaycastFloorActivate() {
		return Physics2D.LinecastAll(pr_bottomLeftSpot, pr_bottomRightSpot, pu_whatIsStepable);
	}

	/*RaycastHit2D[] RaycastHeadActivate() {
		return Physics2D.LinecastAll(pr_topLeftSpot, pr_topRightSpot, pu_whatIsHitable);
	}*/

	void Flip()
	{
		pu_facingRight = !pu_facingRight;
		transform.RotateAround(pr_boxCollider2D.bounds.center, Vector3.up, 180f);
	}

	void InvincibleVerification() {
		if (pu_state == Enumerators.playerState.invincible) {
			pr_currentTimeInvincible += Time.deltaTime;
			if (!pr_animator.GetBool (pr_anim_Invincible))
				pr_animator.SetBool (pr_anim_Invincible, true);
			if (pr_currentTimeInvincible > pu_invincibleTime) {
				pu_state = Enumerators.playerState.normal;
				pr_currentTimeInvincible = 0f;
				pr_animator.SetBool (pr_anim_Invincible, false);
			}
		}
	}
	// ***** Update methods: END

	// ***** Gizmos drawing: INI
	void GizmosRaycastShowUp() {
		Debug.DrawLine (pr_bottomLeftSpot, pr_bottomRightSpot);
		//Debug.DrawLine (pr_topLeftSpot, pr_topRightSpot);
	}
	// ***** Gizmos drawing: END

	// ***** Public methods: INI
	public void Move(float pa_horizontalAxis, float pa_verticalAxis, bool pa_jump)
	{
		// Crouch control
		if (pu_grounded) pu_isCrouching = pa_verticalAxis < 0;
		pr_animator.SetBool (pr_anim_Crouch, pu_isCrouching);

		// Horizontal movement control
		if ((pu_grounded && !pu_isCrouching) || pu_airControl) {
			pr_rigidbody2D.velocity = new Vector2 (pa_horizontalAxis * pu_speed, pr_rigidbody2D.velocity.y);
		}

		// Flip control
		if ((pa_horizontalAxis > 0 && !pu_facingRight) || (pa_horizontalAxis < 0 && pu_facingRight)) Flip ();

		// Jump control
		if (pu_grounded && pa_jump)
		{
			pu_grounded = false;
			pr_animator.SetBool(pr_anim_Ground, false);
			pr_rigidbody2D.AddForce(new Vector2(0f, pu_jumpForce));
		}
	}

	public void SwitchState(Enumerators.PowerUpType pa_powerUp) {
		switch (pa_powerUp) {
		case Enumerators.PowerUpType.mushroom:
			if (pu_form == Enumerators.playerForm.small) {
				pr_animator.SetTrigger (pr_anim_GrowUp);
				pr_movementScript.pu_operatorScript.StopAllUnitsMovement ();
			} else {
				// Get points
			}
			break;
		case Enumerators.PowerUpType.flower:
			if (pu_form == Enumerators.playerForm.small) {
				pr_animator.SetTrigger (pr_anim_GrowUp);
				pr_movementScript.pu_operatorScript.StopAllUnitsMovement();
			} else if (pu_form == Enumerators.playerForm.big) {
				pr_animator.SetTrigger (pr_anim_Flower);
				pr_movementScript.pu_operatorScript.StopAllUnitsMovement();
			} else {
				// Get points
			}
			break;
		default:
			break;
		}
	}

	public void GetDamage(GameObject pa_damagedBy) {
		bool vl_ignoreDamage = false;

		switch (pu_state) {
		case Enumerators.playerState.invincible:
			vl_ignoreDamage = true;
			break;
		case Enumerators.playerState.star:
			vl_ignoreDamage = true;
			pa_damagedBy.GetComponent<Enemy> ().GetDamage ();
			break;
		default:
			break;
		}

		if (vl_ignoreDamage) return;

		switch (pu_form) {
		case Enumerators.playerForm.small:
			Dead();
			break;
		case Enumerators.playerForm.big:
			pr_animator.SetBool (pr_anim_Invincible, true);
			pr_animator.SetTrigger (pr_anim_Shrink);
			pu_state = Enumerators.playerState.invincible;
			pr_movementScript.pu_operatorScript.StopAllUnitsMovement();
			break;
		case Enumerators.playerForm.flower:
			pr_animator.SetBool (pr_anim_Invincible, true);
			pr_animator.SetTrigger (pr_anim_Shrink);
			pu_state = Enumerators.playerState.invincible;
			pr_movementScript.pu_operatorScript.StopAllUnitsMovement();
			break;
		default:
			break;
		}
	}

	public void Dead() {
		Instantiate (pu_deadPrefab, transform.position, Quaternion.identity);
		Destroy (gameObject);
	}
	// ***** Public methods: END
}
