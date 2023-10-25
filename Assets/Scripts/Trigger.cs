using UnityEngine;

public class Trigger : MonoBehaviour
{

    public GameObject levelUI;
    public GameObject arrows;


    [SerializeField] private bool isGate = false;
    [SerializeField] private bool isLava;
    [SerializeField] private bool isTrap = false;
    [SerializeField] private bool spawnPoint = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isGate)
        {
            other.GetComponent<PlayerController>().enabled = false;
            levelUI.SetActive(true);
            int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            if (currentScene == PlayerPrefs.GetInt("LEVEL", 1))
            {
                PlayerPrefs.SetInt("LEVEL", currentScene + 1);
            }

            AdsManager.instance.ShowVideoAds();

        }
        else if (isLava)
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null) controller.TakeDamage(100);
        }
        else if(isTrap)
        {
            arrows.SetActive(true);
        }
        else if(spawnPoint)
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null) controller.spawnPoint = transform.position;
        }    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            PlayerController controller = collision.collider.GetComponent<PlayerController>();
            if (controller != null) controller.TakeDamage(100);
        }
    }
}
