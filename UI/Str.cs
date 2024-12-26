using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Str : MonoBehaviour
{
    public TMP_Text storyText; // Tham chiếu đến đối tượng TextMeshPro trong Canvas
    private string story = "  Tại một vương quốc thanh bình, nơi tiếng cười vang vọng khắp nơi, bóng tối bất ngờ ập đến nuốt chửng khu rừng lân cận và thả tự do vô số quái vật gớm ghiếc. Trong hỗn loạn, một sinh vật hắc ám đã đột nhập cung điện và bắt cóc công chúa Miokn – người được cả vương quốc yêu thương. Bạn, hiệp sĩ Hnlee, không thể đứng nhìn. Với thanh kiếm trên tay, bạn quyết tâm bước vào hành trình nguy hiểm, giải cứu công chúa và đánh bại lời nguyền để mang ánh sáng trở lại trước khi vương quốc chìm vào diệt vong."; // Thay thế bằng cốt truyện của bạn
    private int index = 0;
    private Coroutine displayCoroutine;

    void OnEnable()
    {
        // Đảm bảo rằng khi scene được kích hoạt lại, trạng thái được reset và chữ sẽ chạy lại
        ResetStory();
        displayCoroutine = StartCoroutine(DisplayStory());
    }

    IEnumerator DisplayStory()
    {
        while (index < story.Length)
        {
            storyText.text = story.Substring(0, index);
            index++;
            yield return new WaitForSeconds(0.1f); // Chỉnh thời gian để điều chỉnh tốc độ chạy của chữ
        }
    }

    public void LoadGame()
    {
        // Dừng coroutine khi chuyển scene
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
        SceneManager.LoadScene("S1");
    }

    private void ResetStory()
    {
        // Reset trạng thái
        index = 0;
        storyText.text = "";
        // Đảm bảo rằng coroutine được khởi động lại từ đầu
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
        displayCoroutine = StartCoroutine(DisplayStory());
    }
}
