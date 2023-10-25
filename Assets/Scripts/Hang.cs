using UnityEngine;

public class Hang : StateMachineBehaviour
{

    private Vector3 wallPosition;
    private Button jumpButton;
    private PlayerController controller;

    public bool ropeSwing = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<PlayerController>();
        jumpButton = controller.jumpButton;
        controller.enabled = false;

        wallPosition = animator.GetBehaviour<Hanging>().wallPosition;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(jumpButton.Pressed)
        {
            animator.SetBool("hang", false);
            if (!ropeSwing) { animator.transform.position = wallPosition + Vector3.one * .1f; }
            else { animator.transform.position += animator.transform.forward; }
            animator.SetBool("Rope", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.enabled = true;
        if (animator.GetLayerWeight(1) == .01f) animator.SetLayerWeight(1, 1);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
