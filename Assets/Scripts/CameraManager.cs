using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] public CinemachineVirtualCamera[] vCam;
    [SerializeField] private float[] mPath;

    public static CameraManager camManager;

    private Transform Player;

    private int currentCam = 1;

    private void Awake()
    {
        camManager = this;
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public ICinemachineCamera GetLiveCamera()
    {
        return GetComponent<CinemachineBrain>().ActiveVirtualCamera;
    }

    void Update()
    {
        if (mPath.Length <= 0) return;

        for(int i=0; i < mPath.Length; i++)
        {
            if(Player.position.z > mPath[i])
            {
                vCam[i+1].Priority = 11;
                currentCam = i + 1;
            }
        }

        if(Player.position.z <= mPath[currentCam - 1])
        {
            vCam[currentCam].Priority = 0;
        }

    }

    void vCamChange(int index ,int Priority)
    {
        vCam[index].Priority = Priority;
    }
}
