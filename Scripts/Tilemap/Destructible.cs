using System.Collections;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destructionEffect; // Hiệu ứng tan biến
    public GameObject[] lootItems; // Danh sách vật phẩm rớt ra
    public int minLoot = 1; // Số lượng vật phẩm tối thiểu
    public int maxLoot = 3; // Số lượng vật phẩm tối đa
    public string[] damagingTags; // Các tag gây sát thương như Sword, Bullet

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in damagingTags)
        {
            if (collision.CompareTag(tag))
            {
                StartCoroutine(DestroyObject());
                return; // Chỉ destroy object có gắn script Destructible
            }
        }
    }

    private IEnumerator DestroyObject()
    {
        // Hiển thị hiệu ứng tan biến
        Instantiate(destructionEffect, transform.position, Quaternion.identity);

        // Đợi một chút trước khi destroy object
        yield return new WaitForSeconds(0f);

        // Destroy object
        Destroy(gameObject);

        // Rớt vật phẩm ngẫu nhiên
        int lootCount = Random.Range(minLoot, maxLoot);
        for (int i = 0; i < lootCount; i++)
        {
            int randomIndex = Random.Range(0, lootItems.Length);
            Instantiate(lootItems[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
