using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehavior : MonoBehaviour {

	public float pu_speed = 5f;
	public float pu_rotation = 20f;
	public LayerMask pu_whatIsStepable;

	private Rigidbody2D pr_rigidbody2D;
	private CircleCollider2D pr_circleCollider2D;
	private SpriteRenderer pr_spriteRenderer;
	private Vector2 pr_bottomLeftDirection, pr_bottomRightDirection;
	private Vector2 pr_vector2Position;
	private Vector2 pr_bottomLeftSpot, pr_bottomRightSpot;

	// Use this for initialization
	void Start () {
		pr_rigidbody2D = GetComponent<Rigidbody2D> ();
		pr_circleCollider2D = GetComponent<CircleCollider2D> ();
		pr_spriteRenderer = GetComponent<SpriteRenderer> ();

		RaycastSpotSettings ();
		GizmosRaycastShowUp ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector2PositionsUpdate ();
		RaycastFloorActivate ();


		pr_rigidbody2D.velocity = new Vector2 (pu_speed, pr_rigidbody2D.velocity.y);
		transform.Rotate (0, 0, pu_rotation * -1);
	}

	void RaycastSpotSettings() {
		float vl_width = pr_spriteRenderer.bounds.size.x;

		// Bottom spots
		pr_bottomLeftDirection = new Vector2 (-pr_circleCollider2D.radius, -pr_circleCollider2D.radius);
		pr_bottomRightDirection = new Vector2 (-pr_circleCollider2D.radius, -pr_circleCollider2D.radius);
	}

	void Vector2PositionsUpdate() {
		pr_vector2Position = Utilities.GetVector2 (transform.position);
		pr_bottomLeftSpot = pr_vector2Position + pr_bottomLeftDirection;
		pr_bottomRightSpot = pr_vector2Position + pr_bottomRightDirection;
	}

	RaycastHit2D[] RaycastFloorActivate() {
		return Physics2D.LinecastAll(pr_bottomLeftSpot, pr_bottomRightSpot, pu_whatIsStepable);
	}

	void GizmosRaycastShowUp() {
		Debug.DrawLine (pr_bottomLeftSpot, pr_bottomRightSpot);
	}
}
