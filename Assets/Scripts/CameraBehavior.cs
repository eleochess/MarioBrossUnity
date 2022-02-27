using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

	public GameObject pu_limitDownLeft;
	public GameObject pu_limitTopRight;
	[HideInInspector] public GameObject pu_target = null;

	private float pr_cameraWidth = 0, pr_cameraHeight = 0;
	private float pr_xMaxLeft = 0, pr_xMaxRight = 0;
	private float pr_yMaxDown = 0, pr_yMaxRight = 0;
	private float pr_xPosition = 0, pr_yPosition = 0;
	private bool pr_noTarget = false;
	private Camera pr_camera;

	void Start () {
		pr_camera = GetComponent<Camera>();
		pr_cameraHeight = 2f * pr_camera.orthographicSize;
		pr_cameraWidth = pr_cameraHeight * pr_camera.aspect;

		pr_xMaxLeft = pu_limitDownLeft.transform.position.x + pr_cameraWidth / 2f;
		pr_xMaxRight = pu_limitTopRight.transform.position.x - pr_cameraWidth / 2f;
		pr_yMaxDown = pu_limitDownLeft.transform.position.y + pr_cameraHeight / 2f;
		pr_yMaxRight = pu_limitTopRight.transform.position.y - pr_cameraHeight / 2f;
	}

	void FixedUpdate () {
		if (pu_target == null) {
			pu_target = Utilities.GetPlayerObject ();
			if (pu_target == null)
				pr_noTarget = true;
		}

		if (pr_noTarget) return;

		computePosition ();
		transform.position = new Vector3 (pr_xPosition, pr_yPosition, transform.position.z);
	}

	void computePosition() {
		if (pu_target.transform.position.x < pr_xMaxLeft)
			pr_xPosition = pr_xMaxLeft;
		else if (pu_target.transform.position.x > pr_xMaxRight) {
			if (pr_xMaxLeft < pr_xMaxRight)
				pr_xPosition = pr_xMaxRight;
			else
				pr_xPosition = pr_xMaxLeft;
		}
		else
			pr_xPosition = pu_target.transform.position.x;

		if (pu_target.transform.position.y < pr_yMaxDown)
			pr_yPosition = pr_yMaxDown;
		else if (pu_target.transform.position.y > pr_yMaxRight) {
			if (pr_yMaxDown < pr_yMaxRight)
				pr_yPosition = pr_yMaxRight;
			else
				pr_yPosition = pr_yMaxDown;
		}
		else
			pr_yPosition = pu_target.transform.position.y;
	}
}
