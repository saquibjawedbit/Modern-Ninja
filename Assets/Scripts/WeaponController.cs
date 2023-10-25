using UnityEngine;
using Cinemachine;
using System.Collections;


public class WeaponController : MonoBehaviour
{
    public Weapon[] weapons;
    public GameObject flash;
    public GameObject boomSplash;
    public GameObject grenade;

    public UnityEngine.UI.Button LightingButon;

    public Transform hand;
    public Transform boomTip;
    public LayerMask gunMask;
    public ParticleSystem trails;
    private float activationTime = 15f;

    private Animator anim;
    private PlayerController controller;
    //private CharacterController charController;

    [SerializeField] private int weaponRange = 8;
    private int weaponIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
     //   charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    public void TriggerThrow()
    {
        controller.enabled = false;
        float w = anim.GetLayerWeight(1);
        if (w == 1) anim.SetLayerWeight(1, 0.1f);
        anim.SetTrigger("Throw");
        trails.Play();
    }

    public void TriggerBoom()
    {
        anim.SetTrigger("Boom");
    }

    public void Fire()
    {
        weapons[weaponIndex].muzzleFlash.Play();
        weapons[weaponIndex].source.Play();
        Instantiate(flash, weapons[weaponIndex].tip.position, Quaternion.identity);
        CastRay(weapons[weaponIndex].tip, weaponRange, weapons[weaponIndex].damage);    
    }

    private void CastRay(Transform tip, float range, int damage)
    {
        RaycastHit hit;
        if (Physics.Raycast(tip.position, transform.forward, out hit, range, gunMask))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    public void Throw()
    {
        Instantiate(grenade, hand.position, transform.rotation);
        if (anim.GetLayerWeight(1) == .1f) anim.SetLayerWeight(1, 1);
        controller.enabled = true;
    }

    public void Boom()
    {
        float w = anim.GetLayerWeight(1);
        if (w == 1) anim.SetLayerWeight(1, 0.1f);
        trails.Play();
        controller.enabled = false;
        boomSplash.SetActive(true);
        LightingButon.interactable = false;
        StartCoroutine(activateButton());
    }

    IEnumerator activateButton()
    {
        yield return new WaitForSeconds(activationTime);
        LightingButon.interactable = true;
    }

    IEnumerator BoomMove()
    {
        CastRay(boomTip, 10, 1000);
        ICinemachineCamera vCam = CameraManager.camManager.GetLiveCamera();
        Transform lookAt = vCam.LookAt;
        vCam.LookAt = null;
        transform.position += transform.forward * 10;
        yield return new WaitForSeconds(1.5f);
        controller.enabled = true;
        if (anim.GetLayerWeight(1) == .1f) anim.SetLayerWeight(1, 1);
        boomSplash.SetActive(false);
        vCam.LookAt = lookAt;
    }

    public void SetWeapon(int index)
    {
        print("Getting a Band New Pistol For You");
        weaponIndex = index - 1;
        if(weaponIndex == 0)
        {
            weapons[weaponIndex].gameObject.SetActive(true);
            anim.SetLayerWeight(1, 1);
        }
    }
}
