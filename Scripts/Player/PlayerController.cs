using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Để sử dụng SceneManager

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;
    public int maxStamina = 10;
    public float staminaRegenInterval = 0.5f;
    public float currentStamina;

    private float moveX;
    private float moveY;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private PlayerBar playerBar; // Thay đổi từ `healthBar` sang `playerBar`

    private bool isFacingRight = true;
    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;
    public TrailRenderer myTrailRenderer;

    // Audio variables
    public AudioSource audioSource; // Tham chiếu đến AudioSource
    public AudioClip moveSound;
    public AudioClip dash;

    // Đảm bảo nhân vật không bị hủy khi chuyển scene
    private static PlayerController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Lắng nghe sự kiện sceneLoaded
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
        playerBar = FindObjectOfType<PlayerBar>(); // Tìm `PlayerBar` trong scene
        audioSource = GetComponent<AudioSource>(); // Lấy AudioSource từ Player object

        currentStamina = maxStamina;

        // Cập nhật giá trị thanh stamina lần đầu
        if (playerBar != null)
        {
            playerBar.UpdateStaminaBar(currentStamina / maxStamina);
        }

        StartCoroutine(RegenerateStamina());
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        myAnimator.SetFloat("moveX", moveX);
        myAnimator.SetFloat("moveY", moveY);
        Flip();

        if (movement.magnitude > 0 && !audioSource.isPlaying)
        {
            PlaySound(moveSound); // Phát âm thanh khi di chuyển
        }

        // Kiểm tra nhấn chuột phải để dash
        if (Input.GetMouseButtonDown(1) && currentStamina >= 3)
        {
            Dash();
            PlaySound(dash); // Phát âm thanh khi dash
        }

        if (isDashing)
        {
            if (Time.time >= dashTime + dashDuration)
            {
                isDashing = false;
                myTrailRenderer.emitting = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
            myTrailRenderer.emitting = true;
        }
        else
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void Dash()
    {
        isDashing = true;
        dashTime = Time.time;
        lastDashTime = Time.time;
        currentStamina -= 3;

        // Cập nhật giá trị thanh stamina
        if (playerBar != null)
        {
            playerBar.UpdateStaminaBar(currentStamina / maxStamina);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void Flip()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (isFacingRight && mousePos.x < transform.position.x || !isFacingRight && mousePos.x > transform.position.x)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private IEnumerator RegenerateStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(staminaRegenInterval);

            if (currentStamina < maxStamina)
            {
                currentStamina += 1;

                // Cập nhật giá trị thanh stamina
                if (playerBar != null)
                {
                    playerBar.UpdateStaminaBar(currentStamina / maxStamina);
                }
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Gọi coroutine để trì hoãn tìm kiếm đối tượng "Point"
        StartCoroutine(SetPositionToPoint());
    }

    private IEnumerator SetPositionToPoint()
    {
        yield return new WaitForEndOfFrame(); // Đợi đến cuối frame để đảm bảo mọi object đã được load

        GameObject point = GameObject.FindGameObjectWithTag("Point");
        if (point != null)
        {
            transform.position = point.transform.position; // Di chuyển Player đến vị trí của Point
            Debug.Log($"Player moved to Point at {point.transform.position}");
        }
        else
        {
            Debug.LogWarning("No object with tag 'Point' found in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh lỗi khi object bị hủy
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
