using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinHitBehavior : MonoBehaviour {

	public float pu_jumpForce = 4f;

	private Rigidbody2D pr_rigidbody2D;
	private AudioSource pr_audioSource;

	void Start () {
		pr_rigidbody2D = GetComponent<Rigidbody2D> ();
		pr_audioSource = GetComponent<AudioSource> ();

		pr_rigidbody2D.velocity += new Vector2 (0, pu_jumpForce);
	}

	void FixedUpdate () {
		if (!pr_audioSource.isPlaying) {
			Destroy (gameObject);
		}
	}
}
