using UnityEngine;

public class flash : MonoBehaviour
{
    private const float speed = 30f;

    private void Start()
    {
        Destroy(gameObject, .8f);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);     
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }
}
