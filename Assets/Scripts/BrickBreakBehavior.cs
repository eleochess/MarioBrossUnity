using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBreakBehavior : MonoBehaviour {

	public float pu_upForceA = 5f;
	public float pu_sideForceA = 0.5f;
	public float pu_upForceB = 2.5f;
	public float pu_sideForceB = 1f;

	public GameObject pu_debrisA;
	public GameObject pu_debrisB;

	private AudioSource pr_audioSource;
	private GameObject pr_fallDead;
	private GameObject pr_debrisA1, pr_debrisA2, pr_debrisB1, pr_debrisB2;
	private int pr_deadCount = 0;

	// Use this for initialization
	void Start () {
		pr_audioSource = GetComponent<AudioSource> ();
		pr_fallDead = GameObject.FindGameObjectWithTag (StringNaming.Tag_FallDead);

		pr_debrisA1 = Instantiate (pu_debrisA, transform.position, Quaternion.identity, transform);
		pr_debrisA2 = Instantiate (pu_debrisA, transform.position, Quaternion.identity, transform);
		pr_debrisB1 = Instantiate (pu_debrisB, transform.position, Quaternion.identity, transform);
		pr_debrisB2 = Instantiate (pu_debrisB, transform.position, Quaternion.identity, transform);

		pr_debrisA1.GetComponent<Rigidbody2D> ().velocity += new Vector2 (-pu_sideForceA, pu_upForceA);
		pr_debrisA2.GetComponent<Rigidbody2D> ().velocity += new Vector2 (pu_sideForceA, pu_upForceA);
		pr_debrisB1.GetComponent<Rigidbody2D> ().velocity += new Vector2 (-pu_sideForceB, pu_upForceB);
		pr_debrisB2.GetComponent<Rigidbody2D> ().velocity += new Vector2 (pu_sideForceB, pu_upForceB);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		DebrisLife (pr_debrisA1);
		DebrisLife (pr_debrisA2);
		DebrisLife (pr_debrisB1);
		DebrisLife (pr_debrisB2);

		if (pr_deadCount >= 4) {
			Destroy (gameObject);
		}
	}

	void DebrisLife (GameObject pa_debris) {
		if (pa_debris != null) {
			if (pa_debris.transform.position.y < pr_fallDead.transform.position.y) {
				pr_deadCount++;
				Destroy (pa_debris);
			}
		}
	}
}
