using UnityEngine;

public class AnimationSwitchForm : StateMachineBehaviour {

	public GameObject pu_newPlayerForm;
	public Enumerators.playerState pu_playerState = Enumerators.playerState.normal;

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GameObject vl_instanceObject = Instantiate (pu_newPlayerForm, animator.gameObject.transform.position, animator.gameObject.transform.rotation);
		vl_instanceObject.GetComponent<Player> ().pu_facingRight = vl_instanceObject.transform.rotation.y == 0f;
		vl_instanceObject.GetComponent<Player> ().pu_state = pu_playerState;
		vl_instanceObject.GetComponent<Player> ().pu_currentPosition = animator.gameObject.GetComponent<Player> ().pu_currentPosition;
		vl_instanceObject.GetComponent<Player> ().pu_backwardPosition = animator.gameObject.GetComponent<Player> ().pu_backwardPosition;
		vl_instanceObject.GetComponent<Rigidbody2D> ().velocity = animator.GetComponent<Movement> ().pu_currentVelocity;
		animator.gameObject.GetComponent<Movement>().pu_operatorScript.ResumeAllUnitsMovement();
		GameObject.Destroy (animator.gameObject);
	}

}
