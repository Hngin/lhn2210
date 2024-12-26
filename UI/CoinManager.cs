using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance; // Singleton để dễ truy cập

    private int totalCoins = 0; // Tổng số coin thu được
    public TextMeshProUGUI coinText; // TMP UI Text để hiển thị số coin

    private void Awake()
    {
        // Đảm bảo instance được khởi tạo
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCoins(); // Tải giá trị coin từ PlayerPrefs
        UpdateCoinText(); // Cập nhật text khi bắt đầu
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveCoins();
        UpdateCoinText();
    }


    private void UpdateCoinText()
    {
        coinText.text = "Vàng: " + totalCoins; // Cập nhật TMP UI Text
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins); // Lưu số coin vào PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadCoins()
    {
        if (PlayerPrefs.HasKey("TotalCoins"))
        {
            totalCoins = PlayerPrefs.GetInt("TotalCoins"); // Tải số coin từ PlayerPrefs
        }
        else
        {
            totalCoins = 0; // Nếu không có dữ liệu, đặt giá trị mặc định
        }
    }
    public int GetTotalCoins()
    {
        return totalCoins;
    }

    public void ResetCoins()
    {
        totalCoins = 0; // Đặt lại số coin về 0
        SaveCoins(); // Lưu giá trị mới
        UpdateCoinText(); // Cập nhật text
    }
}
