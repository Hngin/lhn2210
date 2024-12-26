using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverImage; // Hình ảnh sẽ hiển thị khi di chuột vào button

    private void Start()
    {
        if (hoverImage != null)
        {
            hoverImage.SetActive(false); // Ẩn hình ảnh khi bắt đầu
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
        {
            hoverImage.SetActive(true); // Hiển thị hình ảnh khi di chuột vào
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverImage != null)
        {
            hoverImage.SetActive(false); // Ẩn hình ảnh khi rời chuột khỏi button
        }
    }
}
