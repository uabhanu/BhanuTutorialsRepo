using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private float _nextTimeToFire = 0f;
    public float range = 100f; // The maximum distance that the gun can shoot
    public float fireRate = 15f; // The number of shots that the gun can fire per second
    public int maxAmmo = 30; // The maximum number of bullets that the gun can hold
    public int currentAmmo; // The current number of bullets in the gun
    public float reloadTime = 2f; // The amount of time it takes to reload the gun
    public float accuracy = 0.1f; // The accuracy of the gun
    public float bulletForce = 100f;
    public float impactForce = 100f;
    //public ParticleSystem muzzleFlash; // A particle system that shows the muzzle flash when the gun is fired
    public GameObject bulletPrefab;
    //public GameObject impactEffect; // A visual effect that is shown where the bullet hits a target
    //public AudioSource gunshotAudio; // An audio source that plays the gunshot sound when the gun is fired
    //public AudioClip gunshotClip; // The audio clip for the gunshot sound
    //public AudioSource reloadAudio; // An audio source that plays the reload sound when the gun is reloaded
    //public AudioClip reloadClip; // The audio clip for the reload sound
    public Transform shootPoint;

    private bool _isReloading = false;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        if(_isReloading)
            return;

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            if(Time.time >= _nextTimeToFire)
            {
                _nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }
    
    private IEnumerator Reload()
    {
        _isReloading = true;
        //reloadAudio.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        _isReloading = false;
    }

    private void Shoot()
    {
        currentAmmo--;
        //muzzleFlash.Play();
        //gunshotAudio.PlayOneShot(gunshotClip);

        RaycastHit hit;
        Vector3 direction = transform.forward + Random.insideUnitSphere * accuracy;
        
        if(Physics.Raycast(transform.position, direction, out hit, range))
        {
            Debug.Log(hit.transform.name);
            //GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(impactGO, 2f);

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }

        GameObject bullet = Instantiate(bulletPrefab , shootPoint.position , Quaternion.identity);
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(transform.forward * bulletForce , ForceMode.Impulse);
        Destroy(bullet , 2f);
    }
}