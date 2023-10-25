using UnityEngine.AI;
using UnityEngine;


public enum EnemyState
{
    IDLE,
    CHASE
}

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private Transform Player;
    private AudioSource source;

    public Transform scoutRadie;
    public Transform hand;
    public Transform[] initial;
    public AudioClip dead;
    public LayerMask playerMask;


    [SerializeField] private int health = 100;

    [SerializeField] private EnemyState state;
    [SerializeField] private int damage = 25;
    [SerializeField] private float scoutSpeed = 0.5f;
    [SerializeField] private float range = 5f;

    private float pos_x;
    public float attackRadius = .5f;
 
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        pos_x = transform.position.x;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        Player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.speed = scoutSpeed;
        agent.SetDestination(initial[1].position);
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);

        if (isDead) return;

        if(state == EnemyState.IDLE)
        {
            agent.speed = scoutSpeed;
            if (Vector3.Distance(Player.position, scoutRadie.position) <= range) { source.Play(); state = EnemyState.CHASE; anim.SetBool("Chase", true); }
            else if (Vector3.Distance(initial[1].position, transform.position) <= 2) agent.SetDestination(initial[0].position);
            else if (Vector3.Distance(initial[0].position, transform.position) <= 2) agent.SetDestination(initial[1].position);

        }
        else
        {
            if(Vector3.Distance(Player.position, transform.position) < (attackRadius + .2f))
            {
                agent.isStopped = true;
                anim.SetBool("Attack", true);
                return;
            }
            agent.isStopped = false;
            anim.SetBool("Attack", false);
            agent.speed = scoutSpeed * 4;
            agent.destination = Player.position;
        }

        
    }

    public void Attack()
    {
        Collider[] coll = Physics.OverlapSphere(hand.position, attackRadius, playerMask);
        foreach (Collider collider in coll)
        {
            if(collider.CompareTag("Player"))
            {
                PlayerController controller = collider.GetComponent<PlayerController>();
                if(controller != null)
                {
                    controller.TakeDamage(damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(scoutRadie.position, range);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hand.position, attackRadius);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        anim.SetTrigger("Reaction");
        if (health <= 0 && !isDead)
        {
            source.clip = dead;
            source.Play();
            isDead = true;
            anim.SetTrigger($"Dead_{Random.Range(1,3)}");
            agent.SetDestination(transform.position);
            this.enabled = false;
            Destroy(gameObject, 5f);
            return;
        }
        else if((state == EnemyState.IDLE) && !isDead)
        {
            anim.SetTrigger("Reaction");
            source.Play();
            agent.isStopped = true; anim.SetBool("Chase", true);
            
        }
    }
}