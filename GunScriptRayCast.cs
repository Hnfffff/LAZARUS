using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class GunScriptRayCast : MonoBehaviour
{
    Animator gunAnimator;

    [Header("Gun Category")]
    [SerializeField] string gunCategory;

    [Header("Bullet Properties")]
    [SerializeField] int bulletAmount;
    [SerializeField] float shotDistance;
    [SerializeField] GameObject shotImpactDecal;
    [SerializeField] float bulletSpreadBounds;
    [SerializeField] float bulletDamage;
    [SerializeField] int bulletsPerShot;
    Vector3 bulletSpread;

    Ray bullet;
    RaycastHit bulletHitPos;

    [Header("Ammo/Magazine")]
    [SerializeField] int magazineAmmunition;
    [SerializeField] int magazineSize;
    [SerializeField] int reserveAmmo;
    [SerializeField] int maxReserveAmmo;

    [Header("Shooting Information")]
    [SerializeField] float reloadTime;
    [SerializeField] float shotCooldown;

    [SerializeField] private bool isReloading;
    [SerializeField] private bool onCooldown;

    [SerializeField] private Transform gunPos;
    [SerializeField] private Transform camTransform;

    [SerializeField] AudioSource gunSoundEmiiter;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;

    [SerializeField] GameObject weaponObject;

    TMP_Text reloadUI;
    GameController gameController;

    [SerializeField] Transform throwPos;

    //PlayerLook PlayerLook;
    [SerializeField] GameObject playerOrientation;

    // Start is called before the first frame update
    void Awake()
    {
        gunAnimator = GetComponent<Animator>();

        reloadUI = GameObject.Find("Ammo Text").GetComponent<TMP_Text>();

        gunSoundEmiiter = GetComponent<AudioSource>();

        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (magazineAmmunition > 0 && !onCooldown && !gameController.isPaused)
            {
                magazineAmmunition -= bulletsPerShot;
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log("Trying To Reload");
            if (magazineAmmunition != magazineSize && !isReloading && !gameController.isPaused)
            {
                StartCoroutine(ReloadTimer());
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(!gameController.isPaused)
            {
                reloadUI.text = "";
                GameObject weaponInstance = Instantiate(weaponObject, throwPos.position, new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
                weaponInstance.GetComponent<Rigidbody>().velocity = playerOrientation.transform.forward * 5 + playerOrientation.transform.up * 2;
                weaponInstance.GetComponent<GunPrefab>().magazineAmmunition = magazineAmmunition;
                weaponInstance.transform.SetParent(null);
                gunSoundEmiiter.Stop();

                gunAnimator.Play("Base Layer.Idle");
                isReloading = false;
                onCooldown = false;

                gameController.SetHeldItem("none");
                gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        OnPickUp();
    }

    private void Shoot()
    {
        gunSoundEmiiter.clip = shootSound;
        gunSoundEmiiter.Play();
        bulletSpread = camTransform.transform.forward;
        //Debug.Log($"Magazine Ammunition = {magazineAmmunition}");

        foreach(int i in Enumerable.Range(0,bulletAmount))
        {
            bulletSpread = bulletSpread + camTransform.transform.TransformDirection(new Vector3(Random.Range(-bulletSpreadBounds, bulletSpreadBounds), Random.Range(-bulletSpreadBounds, bulletSpreadBounds)));

            if (Physics.Raycast(gunPos.position, bulletSpread, out bulletHitPos, shotDistance))
            {
                if(bulletHitPos.transform.tag == "shootable")
                {
                    Destroy(Instantiate(shotImpactDecal, bulletHitPos.point + bulletHitPos.normal * 0.0001f, Quaternion.FromToRotation(Vector3.up, bulletHitPos.normal)), 10000f);
                    //Debug.Log("Hit Wall");
                }
            }
        }
        StartCoroutine(ShotCooldown(shotCooldown));

        reloadUI.text = $"{magazineAmmunition} | {InvokeGetAmmo(gunCategory)}";
    }

    private void Reload()
    {
        if(InvokeGetAmmo(gunCategory) >= magazineSize)
        {
            InvokeChangeAmmo(gunCategory, -magazineSize);
            magazineAmmunition = magazineSize;
        }
        else
        {
            magazineAmmunition = reserveAmmo;
            InvokeSetAmmo(gunCategory, 0);
        }
    }

    IEnumerator ReloadTimer()
    {
        
        if (InvokeGetAmmo(gunCategory) !=0)
        {
            gunSoundEmiiter.clip = reloadSound;
            gunSoundEmiiter.Play();

            reloadUI.text = $"RELOADING | {InvokeGetAmmo(gunCategory)}";
            gunAnimator.Play("Base Layer.Reload");

            isReloading = true;

            yield return new WaitForSeconds(reloadTime);
            Reload();

            isReloading = false;
            reloadUI.text = $"{magazineAmmunition} | {InvokeGetAmmo(gunCategory)}";
        }
    }

    IEnumerator ShotCooldown(float ShotCooldown)
    {
        gunAnimator.Play("Base Layer.Shoot");
        onCooldown = true;
        yield return new WaitForSeconds(ShotCooldown);
        onCooldown = false;

        if (magazineAmmunition == 0 && !isReloading && !onCooldown)
        {
            StartCoroutine(ReloadTimer());
        }
    }

    int InvokeGetAmmo(string GunCategory)
    {
        MethodInfo method = typeof(GameController).GetMethod($"Get{GunCategory}Ammo");
        //Debug.Log($"Get{GunCategory}Ammo");
        return (int)method.Invoke(gameController, null);
    }

    int InvokeChangeAmmo(string GunCategory, int Parametres)
    {
        MethodInfo method = typeof(GameController).GetMethod($"Change{GunCategory}Ammo");
        object[] methodParams = { Parametres };
        //Debug.Log($"Change{GunCategory}Ammo");
        return (int)method.Invoke(gameController, methodParams);
    }

    int InvokeSetAmmo(string GunCategory, int Parametres)
    {
        MethodInfo method = typeof(GameController).GetMethod($"Set{GunCategory}Ammo");
        object[] methodParams = { Parametres };
        //Debug.Log($"Set{GunCategory}Ammo");
        return (int)method.Invoke(gameController, methodParams);
    }

    public void OnPickUp()
    {
        gunSoundEmiiter.Stop();

        switch (gunCategory)
        {
            case "Shotgun":
                gunCategory = "Shotgun";
                bulletAmount = 12;
                shotDistance = 10;

                bulletSpreadBounds = 0.025f;
                bulletDamage = 1;
                bulletsPerShot = 1;

                magazineAmmunition = gameController.magazineAmmo;
                magazineSize = 2;
                reserveAmmo = gameController.GetShotgunAmmo();
                maxReserveAmmo = 32;

                reloadTime = 1.5f;
                shotCooldown = 0.5f;
                break;

            case "Rifle":
                gunCategory = "Rifle";
                bulletAmount = 1;
                shotDistance = 40;
                bulletSpreadBounds = 0;
                bulletDamage = 1;
                bulletsPerShot = 1;

                magazineAmmunition = gameController.magazineAmmo;
                magazineSize = 1;
                reserveAmmo = gameController.GetRifleAmmo();
                maxReserveAmmo = 32;

                reloadTime = 1;
                shotCooldown = 0.3f;
                break;

            default:
                break;
        }

        gunAnimator.Play("Base Layer.Pickup");
        reloadUI.text = $"{magazineAmmunition} | {InvokeGetAmmo(gunCategory)}";
    }
}
