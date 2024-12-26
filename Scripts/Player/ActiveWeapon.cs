using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveWeapon : MonoBehaviour
{
    public GameObject Sword; // Tham chiếu đến đối tượng Sword
    public GameObject Aim;   // Tham chiếu đến đối tượng Bow
    public GameObject swordUIImage; // Tham chiếu đến ảnh UI của Sword
    public GameObject bowUIImage;   // Tham chiếu đến ảnh UI của Bow

    private MonoBehaviour currentWeaponScript; // Script của vũ khí hiện tại
    private SpriteRenderer swordRenderer;
    private Transform[] aimChildren;

    // Audio variables
    public AudioSource audioSource; // Tham chiếu đến AudioSource
    public AudioClip sword;
    public AudioClip bow;

    void Start()
    {
        // Lấy các SpriteRenderer
        swordRenderer = Sword.GetComponent<SpriteRenderer>();
        aimChildren = Aim.GetComponentsInChildren<Transform>(true);

        // Khởi tạo vũ khí mặc định là Sword
        currentWeaponScript = Sword.GetComponent<MonoBehaviour>();
        currentWeaponScript.enabled = true;
        Aim.GetComponent<MonoBehaviour>().enabled = false; // Tắt script của Bow khi bắt đầu
        SetAimChildrenActive(false); // Tắt các đối tượng con của Aim

        // Hiển thị ảnh UI của Sword, tắt ảnh UI của Bow
        swordUIImage.SetActive(true);
        bowUIImage.SetActive(false);
        audioSource = GetComponent<AudioSource>(); // Lấy AudioSource từ Player object
    }

    void Update()
    {
        // Kiểm tra xem phím Q có được nhấn không
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }
    }

    private void SwitchWeapon()
    {
        if (currentWeaponScript == Sword.GetComponent<MonoBehaviour>())
        {
            if (!audioSource.isPlaying)
            {
                PlaySound(bow);
            }

            // Chuyển sang Bow
            currentWeaponScript.enabled = false; // Vô hiệu hóa script Sword
            currentWeaponScript = Aim.GetComponent<MonoBehaviour>(); // Chuyển sang script Bow
            currentWeaponScript.enabled = true;  // Kích hoạt script Bow

            // Đặt lại trạng thái Bow
            PlayerAimWeapon playerAimWeapon = Aim.GetComponent<PlayerAimWeapon>();
            if (playerAimWeapon != null)
            {
                playerAimWeapon.canFire = true; // Đặt lại trạng thái có thể bắn
            }

            // Hiển thị ảnh UI của Bow, tắt ảnh UI của Sword
            swordUIImage.SetActive(false);
            bowUIImage.SetActive(true);

            // Ẩn hiển thị của Sword, hiển thị của Bow
            swordRenderer.enabled = false;
            SetAimChildrenActive(true); // Bật các đối tượng con của Aim
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                PlaySound(sword);
            }

            // Chuyển sang Sword
            currentWeaponScript.enabled = false; // Vô hiệu hóa script Bow
            currentWeaponScript = Sword.GetComponent<MonoBehaviour>(); // Chuyển sang script Sword
            currentWeaponScript.enabled = true;  // Kích hoạt script Sword

            // Hiển thị ảnh UI của Sword, tắt ảnh UI của Bow
            swordUIImage.SetActive(true);
            bowUIImage.SetActive(false);

            // Ẩn hiển thị của Bow, hiển thị của Sword
            SetAimChildrenActive(false); // Tắt các đối tượng con của Aim
            swordRenderer.enabled = true;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void SetAimChildrenActive(bool isActive)
    {
        foreach (Transform child in aimChildren)
        {
            if (child != Aim.transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }
}
