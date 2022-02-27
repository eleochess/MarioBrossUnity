using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnd : MonoBehaviour {

	[HideInInspector] public GameObject pu_target = null;

	private bool pr_stop = false;
	private bool pr_noTarget = false;

	void FixedUpdate () {
		if (pr_stop) return;

		if (pu_target == null) {
			pu_target = Utilities.GetPlayerObject ();
			if (pu_target == null)
				pr_noTarget = true;
		}

		if (pr_noTarget) return;

		if (pu_target.transform.position.y < transform.position.y) {
			GameObject.FindGameObjectWithTag (StringNaming.Tag_Player).GetComponent<Player> ().Dead ();
			pr_stop = true;
		}
	}
}
