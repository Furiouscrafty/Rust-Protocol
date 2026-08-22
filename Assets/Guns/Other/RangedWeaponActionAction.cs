using System.Collections;
using UnityEngine;
using TMPro;

public class RangedWeaponActionAction : MonoBehaviour
{
    // GUN DATA
    [Header("Gun Settings")]
    public float damage = 25f;
    public float range = 100f;
    public Camera playerCamera;

    [Header("Ammo Settings")]
    public int magazineSize = 10; // Bullets per mag
    public int totalMagazines = 3; // Total spare mags
    public float reloadTime = 1.5f; // Reload duration in seconds

    private int currentAmmo;
    private bool isReloading = false;

    [Header("Cooldown Settings")]
    public float fireCooldown = 0.5f; // Time between shots
    private float nextTimeToFire = 0f;

    [Header("Burst Settings")]
    public int burstCount = 3; // Number of shots in a burst
    public float burstDelay = 0.1f; // Delay between burst shots

    [Header("Shotgun Settings")]
    public int pelletCount = 8; // number of pellets per shot
    public float spreadAngle = 10f; // Spread angle in degrees

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudioSource;

    [Header("Animation")]
    public Animator gunAnimator;

    public ItemSO Gun;

    private bool isBursting = false;

    private const string ENEMY_TAG = "Enemy";

    [Header("Ammo UI")]
    public TMP_Text ammoText;
    [SerializeField] private string formatString = "{0} | {0}";
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowAmmoColor = Color.red;
    [SerializeField] private int lowAmmoThreshold = 3;

    void Start()
    {
        //Default Data
        Gun.DefaultTotalMag = totalMagazines;
        Gun.DefaultMag = magazineSize;

        currentAmmo = magazineSize;
        UpdateAmmoUI();
    }

    void Update()
    {
        

        if (Gun.WasInInventory)
            ResetGunData();

        if (isReloading || isBursting)
            return;

        // Automatic
        if (Gun.Type == GunType.Automatic)
        {
            if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireCooldown;
                Shoot();
            }
        }
        // Semi-Automatic
        else if (Gun.Type == GunType.Semi_Automatic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireCooldown;
                Shoot();
            }
        }
        // Burst
        else if (Gun.Type == GunType.Burst)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireCooldown;
                StartCoroutine(BurstFire());
            }
        }
        // Shotgun
        else if (Gun.Type == GunType.Shotgun)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireCooldown;
                ShootShotgun();
            }
        }

        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize && totalMagazines > 1)
        {
            StartCoroutine(Reload());
        }

        UpdateAmmoUI();
    }

    void Shoot()
    {
        Gun.isAnimating = true;
        currentAmmo--;

        if (muzzleFlash != null) muzzleFlash.Play();
        if (gunAudioSource != null) gunAudioSource.Play();
        if (gunAnimator != null) gunAnimator.SetTrigger("Shoot");

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

            if (hit.collider.CompareTag(ENEMY_TAG))
            {
                RobotHealthMech enemyHealth = hit.collider.GetComponent<RobotHealthMech>();
                if (enemyHealth != null)
                {
                    enemyHealth.RemoveHealth(damage);
                    Debug.Log("Enemy hit! Damage: " + damage);
                }
            }
        }

        Gun.isAnimating = false;
    }

    IEnumerator BurstFire()
    {
        isBursting = true;

        for (int i = 0; i < burstCount; i++)
        {
            if (currentAmmo <= 0)
                break;

            Shoot();

            if (i < burstCount - 1)
                yield return new WaitForSeconds(burstDelay);
        }

        isBursting = false;
        Gun.isAnimating = false;
    }

    void ShootShotgun()
    {
        Gun.isAnimating = true;
        currentAmmo--;

        if (muzzleFlash != null) muzzleFlash.Play();
        if (gunAudioSource != null) gunAudioSource.Play();

        for (int i = 0; i < pelletCount; i++)
        {
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);

            Vector3 spreadDirection =
                Quaternion.Euler(randomX, randomY, 0) *
                playerCamera.transform.forward;

            Ray ray = new Ray(playerCamera.transform.position, spreadDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

                if (hit.collider.CompareTag(ENEMY_TAG))
                {
                    RobotHealthMech enemyHealth = hit.collider.GetComponent<RobotHealthMech>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.RemoveHealth(damage);
                    }
                }
            }
        }

        Gun.isAnimating = false;
    }

    IEnumerator Reload()
    {
        Gun.isAnimating = true;
        isReloading = true;

        if (gunAnimator != null)
            gunAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        if (totalMagazines > 1)
        {
            totalMagazines--;
            currentAmmo = magazineSize;
        }

        isReloading = false;
        Gun.isAnimating = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText == null)
            return;

        ammoText.text = string.Format(formatString, currentAmmo, totalMagazines - 1);
        ammoText.color = currentAmmo <= lowAmmoThreshold ? lowAmmoColor : normalColor;
    }

    public void ResetGunData()
    {
        totalMagazines = Gun.DefaultTotalMag;
        currentAmmo = Gun.DefaultMag;
        Gun.WasInInventory = false;
    }
}
