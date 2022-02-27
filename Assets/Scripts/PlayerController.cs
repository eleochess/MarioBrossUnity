using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

	private Player pr_playerScript;
	private bool pr_jump = false;
	private float pr_verticalAxis, pr_horizontalAxis;
	private Movement pr_movementScript;

	void Start () {
		pr_playerScript = GetComponent<Player> ();
		pr_movementScript = GetComponent<Movement> ();
	}

	void Update () {
		if (!pr_movementScript.pu_isMoving) return;

		// Pause
		if (Input.GetKeyDown(KeyCode.P)) Time.timeScale = Time.timeScale == 1 ? 0 : 1;

		// Jump
		// Read the jump input in Update so button presses aren't missed.
		if (!pr_jump) pr_jump = CrossPlatformInputManager.GetButtonDown("Jump");
	}

	private void FixedUpdate()
	{
		if (!pr_movementScript.pu_isMoving) return;

		// Directionals
		pr_verticalAxis = CrossPlatformInputManager.GetAxis("Vertical");
		pr_horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal");

		// Pass all parameters to the character control script.
		pr_playerScript.Move(pr_horizontalAxis, pr_verticalAxis, pr_jump);
		pr_jump = false;
	}
}
