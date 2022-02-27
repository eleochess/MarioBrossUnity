using UnityEngine;

public class Movement : MonoBehaviour {

	[HideInInspector] public bool pu_isMoving = true;
	[HideInInspector] public Vector2 pu_currentVelocity;
	[HideInInspector] public GameOperations pu_operatorScript;

	private Rigidbody2D pr_rigidbody2D;

	void Start() {
		pr_rigidbody2D = GetComponent<Rigidbody2D> ();
		pu_operatorScript = GameObject.FindGameObjectWithTag (StringNaming.Tag_GameOperator).GetComponent<GameOperations> ();

		pu_operatorScript.pu_currentUnits.Add (gameObject);
	}

	void OnDestroy() {
		pu_operatorScript.pu_currentUnits.Remove (gameObject);
	}

	public void StopMovement() {
		pu_currentVelocity = pr_rigidbody2D.velocity;
		pr_rigidbody2D.velocity = new Vector2 (0, 0);
		pr_rigidbody2D.isKinematic = true;
		pu_isMoving = false;
	}

	public void ResumeMovement() {
		pr_rigidbody2D.isKinematic = false;
		pr_rigidbody2D.velocity = pu_currentVelocity;
		pu_isMoving = true;
	}
}
