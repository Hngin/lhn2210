using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerAimWeapon : MonoBehaviour
{
    [SerializeField] private Transform bow;
    [SerializeField] private float bowDistance = 0.8f;
    private bool aimFacingRight = true;
    public bool canFire = true;
    private float timer;
    public float timeBetweenFiring;
    public GameObject bullet;
    private Camera mainCam;

    public int maxShots = 7;
    private int currentShots;
    public TMP_Text shotsText;

    public int parallelShotLevel = 0;
    public float parallelShotSpacing = 0.5f;

    public AudioSource audioSource;
    public AudioClip attackBow;

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        currentShots = maxShots;
        UpdateShotsUI();
        audioSource = GetComponent<AudioSource>();
        LoadPlayerAimWeaponData();
    }

    public void SavePlayerAimWeaponData()
    {
        PlayerPrefs.SetInt("PlayerAimWeapon_MaxShots", maxShots);
        PlayerPrefs.SetInt("PlayerAimWeapon_ParallelShotLevel", parallelShotLevel);
        PlayerPrefs.Save();
    }

    public void LoadPlayerAimWeaponData()
    {
        maxShots = PlayerPrefs.GetInt("PlayerAimWeapon_MaxShots", 7);
        parallelShotLevel = PlayerPrefs.GetInt("PlayerAimWeapon_ParallelShotLevel", 0);
        UpdateShotsUI();
    }

    public static void ResetPlayerAimWeaponData()
    {
        PlayerPrefs.DeleteKey("PlayerAimWeapon_MaxShots");
        PlayerPrefs.DeleteKey("PlayerAimWeapon_ParallelShotLevel");
    }

    private void Update()
    {
        if (canFire)
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - transform.position;
            float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            bow.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
            bow.position = transform.position + Quaternion.Euler(0, 0, rotZ) * new Vector3(bowDistance, 0, 0);

            if (Input.GetMouseButtonDown(0) && currentShots > 0 && canFire)
            {
                Shoot();
            }

            BowFlipController(mousePos);

            if (!Input.GetMouseButton(0))
            {
                timer += Time.deltaTime;
                if (timer >= 1f && currentShots < maxShots)
                {
                    StartCoroutine(Reload());
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void BowFlipController(Vector3 mousePos)
    {
        if (mousePos.x < bow.position.x && aimFacingRight)
        {
            BowFlip();
        }
        else if (mousePos.x > bow.position.x && !aimFacingRight)
        {
            BowFlip();
        }
    }

    private void BowFlip()
    {
        aimFacingRight = !aimFacingRight;
        bow.localScale = new Vector3(bow.localScale.x * -1, bow.localScale.y, bow.localScale.z);
    }

    private void Shoot()
    {
        PlaySound(attackBow);

        for (int i = 0; i <= parallelShotLevel; i++)
        {
            float offset = (i - parallelShotLevel / 2f) * parallelShotSpacing;
            Vector3 bulletPosition = bow.position + bow.up * offset;
            Instantiate(bullet, bulletPosition, bow.rotation);
        }

        currentShots--;
        UpdateShotsUI();
        StartCoroutine(FireCooldown());
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(timeBetweenFiring);
        canFire = true;
    }

    private IEnumerator Reload()
    {
        while (currentShots < maxShots)
        {
            yield return new WaitForSeconds(0.5f);
            if (currentShots < maxShots)
            {
                currentShots++;
                UpdateShotsUI();
            }
        }
    }

    private void UpdateShotsUI()
    {
        shotsText.text = currentShots.ToString();
    }
}
