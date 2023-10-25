using UnityEngine;

public class Rope : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float velocity = 3f;
    [SerializeField] private Transform tip;

    public Button jumpButton;

    private float currentVelocity;

    private Transform player;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.angularVelocity = velocity * transform.right * Mathf.Sin(currentVelocity) * Time.deltaTime;
        currentVelocity += Time.deltaTime;

        if(player != null)
        {
            player.position = tip.position;
            player.rotation = tip.rotation;
        }

        if (jumpButton.Pressed && player != null)
        {
            player.rotation = Quaternion.Euler(Vector3.zero);
            player.position = new Vector3(2.264828f, player.position.y, player.position.z);
            player = null;
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();
        if(controller != null)
        {
            player = collision.transform;
            controller.Swing();
        }
    }

   

}
