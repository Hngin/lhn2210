using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // Giá trị của mỗi coin

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Kiểm tra nếu object chạm là Player
        {
            CoinManager.instance.AddCoins(coinValue); // Gọi hàm thêm coin
            Destroy(gameObject); // Hủy object coin
        }
    }
}
