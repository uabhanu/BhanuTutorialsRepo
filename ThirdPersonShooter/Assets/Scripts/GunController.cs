using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public float range = 100f; // The maximum distance that the gun can shoot
    public float fireRate = 1f; // The number of shots that the gun can fire per second
    public int maxAmmo = 30; // The maximum number of bullets that the gun can hold
    public int currentAmmo; // The current number of bullets in the gun
    public float reloadTime = 2f; // The amount of time it takes to reload the gun
    public float accuracy = 0.1f; // The accuracy of the gun
    public float bulletForce = 100f;
    public float impactForce = 100f;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private float _nextTimeToFire;
    private bool _isReloading;

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
        Vector3 direction = transform.forward + Random.insideUnitSphere * accuracy;
        
        if(Physics.Raycast(transform.position, direction, out hit, range))
        {
            Debug.Log(hit.transform.name);

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(transform.forward * bulletForce, ForceMode.Impulse);

        // Define the distance the bullet travels before being destroyed
        float bulletTravelDistance = 100f;
        StartCoroutine(DestroyBulletAfterDistance(bullet, bulletTravelDistance));
    }

    private IEnumerator DestroyBulletAfterDistance(GameObject bullet, float distance)
    {
        Vector3 initialPosition = bullet.transform.position;
        float traveledDistance = 0f;

        while(traveledDistance < distance)
        {
            traveledDistance = Vector3.Distance(initialPosition, bullet.transform.position);
            yield return null;
        }

        Destroy(bullet);
    }
}