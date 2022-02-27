using UnityEngine;

public class Hitbox_Hit : StateMachineBehaviour {

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		HitBoxBehavior vl_HitboxScript = animator.gameObject.GetComponent<HitBoxBehavior> ();
		vl_HitboxScript.pu_isHit = false;
		//vl_HitboxScript.pu_animationHitOn = false;
	}

}
