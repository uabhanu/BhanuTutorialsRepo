using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private bool _isReloading;
    private float _nextTimeToFire;
    
    [SerializeField] private float range = 100f; // The maximum distance that the gun can shoot
    [SerializeField] private float fireRate = 1f; // The number of shots that the gun can fire per second
    [SerializeField] private int maxAmmo = 30; // The maximum number of bullets that the gun can hold
    [SerializeField] private int currentAmmo; // The current number of bullets in the gun
    [SerializeField] private float reloadTime = 2f; // The amount of time it takes to reload the gun
    //[SerializeField] private float accuracy = 0.1f; // The accuracy of the gun
    [SerializeField] private float bulletForce = 100f;
    [SerializeField] private float impactForce = 100f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrelTransform;

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

        if(Input.GetButtonDown("Fire1") && Time.time >= _nextTimeToFire)
        {
            _nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }
    
    private IEnumerator DestroyBulletAfterDistance(GameObject bullet , float distance)
    {
        Vector3 initialPosition = bullet.transform.position;
        
        float traveledDistance = 0f;

        while(traveledDistance < distance)
        {
            traveledDistance = Vector3.Distance(initialPosition , bullet.transform.position);
            yield return null;
        }

        Destroy(bullet);
    }
    
    private IEnumerator Reload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        _isReloading = false;
    }

    private void Shoot()
    {
        currentAmmo--;

        RaycastHit hit;

        // Use the barrelTransform's right direction instead of forward direction
        Vector3 direction = barrelTransform.right;

        if(Physics.Raycast(barrelTransform.position , direction , out hit , range))
        {
            Debug.Log(hit.transform.name);

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }

        GameObject bullet = Instantiate(bulletPrefab , barrelTransform.position , Quaternion.identity);
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

        // Use the barrelTransform's right direction instead of forward direction
        bulletRB.AddForce(barrelTransform.right * bulletForce , ForceMode.Impulse);

        // Define the distance the bullet travels before being destroyed
        float bulletTravelDistance = 100f;
        StartCoroutine(DestroyBulletAfterDistance(bullet , bulletTravelDistance));
    }
}