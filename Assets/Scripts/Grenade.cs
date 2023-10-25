using UnityEngine;

public class Grenade : MonoBehaviour
{

    public int damage = 100;

    // Start is called before the first frame u
    void Start()
    {
        Destroy(gameObject , 2f);
    }

    void Update()
    {
        transform.Translate(10 * Vector3.forward * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null) enemy.TakeDamage(damage);

    }
}
