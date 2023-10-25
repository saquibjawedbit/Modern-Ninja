using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour
{
    
    public bool testMode = true;
    public static AdsManager instance;

    private readonly string videoId = "Interstitial_Android";

    private void Awake()
    {
        instance = this;
        Advertisement.Initialize("4705699", testMode);
    }

    // Start is called before the first frame update
    void Start()
    {
        Advertisement.Load(videoId);
    }

    // Update is called once per frame
    public void ShowVideoAds()
    {

        Advertisement.Show("Interstitial_Android");
    }
}
