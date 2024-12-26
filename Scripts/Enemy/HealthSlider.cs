using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    public Slider healthSlider;
    public EnemyHealth enemyHealth;
    public Transform enemyTransform; 
    public Vector3 offset = new Vector3(0, 1, 0); 
    private Camera mainCamera;
    private Coroutine hideSliderCoroutine;

    void Start()
    {
        if (enemyHealth != null)
        {
            enemyHealth.onHealthChanged.AddListener(UpdateSlider);
        }

        healthSlider.gameObject.SetActive(false);

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (healthSlider.gameObject.activeSelf && enemyTransform != null)
        {
            Vector3 worldPosition = enemyTransform.position + offset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            healthSlider.transform.position = screenPosition;
        }
    }

    public void UpdateSlider(float healthPercentage)
    {
        healthSlider.gameObject.SetActive(true);

        healthSlider.value = healthPercentage;

        if (healthPercentage < 0.3f)
        {
            healthSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else
        {
            healthSlider.fillRect.GetComponent<Image>().color = Color.green;
        }

        if (hideSliderCoroutine != null)
        {
            StopCoroutine(hideSliderCoroutine);
        }
        hideSliderCoroutine = StartCoroutine(HideSliderAfterDelay(2f));
    }

    private IEnumerator HideSliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        healthSlider.gameObject.SetActive(false);
    }
}
