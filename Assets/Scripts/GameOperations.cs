using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOperations : MonoBehaviour {

	[HideInInspector] public List<GameObject> pu_currentUnits;

	public void StopAllUnitsMovement() {
		foreach (GameObject fe_unit in pu_currentUnits) {
			if (fe_unit != null) fe_unit.GetComponent<Movement> ().StopMovement ();
		}
	}

	public void ResumeAllUnitsMovement() {
		foreach (GameObject fe_unit in pu_currentUnits) {
			if (fe_unit != null) fe_unit.GetComponent<Movement> ().ResumeMovement ();
		}
	}
}

