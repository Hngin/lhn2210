using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class BarrierControl : MonoBehaviour
{
    public Tilemap barrierTilemap; // Tham chiếu tới Tilemap hàng rào
    public Transform enemyParent; // Đối tượng chứa tất cả các enemy
    public TextMeshProUGUI messageText; // Text UI để hiển thị thông báo
    public float messageDuration = 2f; // Thời gian hiển thị thông báo

    private bool messageDisplayed = false;

    void Start()
    {
        // Ban đầu, hàng rào sẽ hoạt động và ẩn text UI
        barrierTilemap.gameObject.SetActive(true);
        messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckEnemies();
    }

    void CheckEnemies()
    {
        // Kiểm tra nếu không còn enemy nào và thông báo chưa được hiển thị
        if (enemyParent.childCount == 0 && !messageDisplayed)
        {
            // Tắt hàng rào và hiển thị thông báo
            barrierTilemap.gameObject.SetActive(false);
            StartCoroutine(ShowMessage());
            messageDisplayed = true;
        }
    }

    System.Collections.IEnumerator ShowMessage()
    {
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
    }
}
