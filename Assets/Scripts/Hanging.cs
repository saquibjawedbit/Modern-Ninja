using UnityEngine;

public class Hanging : StateMachineBehaviour
{

    public LayerMask wallMask;
    [SerializeField] private float radius = .1f;
    [SerializeField] private Transform handIk;

    [HideInInspector]
    public Vector3 wallPosition;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        handIk = animator.transform.GetComponent<WeaponController>().hand;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool isWall = Physics.CheckSphere(handIk.position, radius, wallMask);
        if(isWall)
        {
            Collider[] coll = Physics.OverlapSphere(handIk.position, radius, wallMask);
            foreach (Collider c in coll)
            {
                if (c != null) wallPosition = c.transform.position;
            }
            animator.transform.position = wallPosition - 2 * Vector3.up;
            animator.SetBool("hang", true);
            if(animator.GetLayerWeight(1) == 1) animator.SetLayerWeight(1, .01f);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
