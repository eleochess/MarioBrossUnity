using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioDiying : MonoBehaviour {

	public float pu_timeBeforeJump = 0.8f;
	public float pu_JumpVelocity = 4f;

	private bool pr_jumpHappened = false;
	private float pr_currentTime = 0f;
	private Rigidbody2D pr_rigidbody2D;
	private AudioSource pr_audioSource;

	void Start () {
		pr_rigidbody2D = GetComponent<Rigidbody2D> ();
		pr_audioSource = GetComponent<AudioSource> ();

		pr_rigidbody2D.isKinematic = true;
		GameObject.FindGameObjectWithTag (StringNaming.Tag_Camera).GetComponent<AudioSource> ().Stop ();
	}

	void FixedUpdate () {
		pr_currentTime += Time.deltaTime;

		if (pr_currentTime > pu_timeBeforeJump && !pr_jumpHappened) {
			pr_jumpHappened = true;
			pr_rigidbody2D.isKinematic = false;
			pr_rigidbody2D.velocity = new Vector2 (0f, pu_JumpVelocity);
		}

		if (!pr_audioSource.isPlaying) {
			// Restart level
			Destroy (gameObject);
		}
	}
}
