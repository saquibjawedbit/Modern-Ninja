using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;
    private AudioSource source;

    public Button jumpButton;
    public Button rightButton;
    public Button leftButton;
    public Slider healthProgress;
    public GameObject izanagi;
    public GameObject gameOverUI;

    public Transform tip;
    public Vector3 spawnPoint;

    public LayerMask groundmask;    


    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpheight = 5f;
    [SerializeField] private float radius = .1f;

    private float gravity = -9.8f;
    private int Health = 100;
    private Vector3 velocity;
    private float pos_x;

    private const string horizontalPrefix = "Vertical";

    //2.253828, 0.975, -2.255

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        pos_x = transform.position.x;
    }

    int GetInput()
    {

        if (rightButton.Pressed || Input.GetKey(KeyCode.W)) return 1;
        if (leftButton.Pressed || Input.GetKey(KeyCode.S)) return -1;
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        int horizontal = GetInput();
        anim.SetFloat(horizontalPrefix, horizontal);
        controller.Move(transform.forward * horizontal * speed * Time.deltaTime);

        bool isGround = Physics.CheckSphere(tip.position, radius, groundmask);
        if (velocity.y <= 0 && isGround)
        {
            velocity.y = -2;
        }

        if (jumpButton.Pressed && isGround)
        {
            anim.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(-2 * gravity * jumpheight);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);
    }

    bool isDead = false;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tip.position, radius);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        healthProgress.value = Health;
        if(Health <= 0  && !isDead)
        {
            Dead();
            return;
        }
        anim.SetTrigger("Damage");
    }

    void Dead()
    {
        source.Play();
        isDead = true;
        anim.SetTrigger("Dead");
        gameOverUI.SetActive(true);
        this.enabled = false;
    }

    public void Revive()
    {
        //Revive Player either showing rewarded Ads or at checkpoint
        transform.position = new Vector3(pos_x, spawnPoint.y, spawnPoint.z);
        Health = 100;
        anim.SetTrigger("Revive");
        healthProgress.value = Health;
        isDead = false;
        izanagi.SetActive(true);
        StartCoroutine(DisableIzanagi());
    }

    IEnumerator DisableIzanagi()
    {
        yield return new WaitForSeconds(3);
        izanagi.SetActive(false);
    }    


    public void Swing()
    {
        anim.SetTrigger("Rope");
        this.enabled = false;
    }
}
