using UnityEngine;

public class Weapon : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public AudioSource source;
    public Transform tip;

    public int damage = 25;
    public int bulletInGun;
    public int totalBullet;

}
