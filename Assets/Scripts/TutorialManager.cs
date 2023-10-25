using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUp;
    private int popUpIndex = 0;
    private float timeLeft = 2;
    private int time = 2;

    // Update is called once per frame
    void Update()
    {
        if (timeLeft <= 0) 
        {
            timeLeft = time;
            if (popUpIndex == 4) Destroy(gameObject);
            popUp[popUpIndex].SetActive(false); 
            popUpIndex += 1;
            popUp[popUpIndex].SetActive(true);
        }
        else { timeLeft -= Time.deltaTime; }
    }

}
