using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public GameObject dialogueBox; // Quản lý hiển thị khung chat
    public Canvas nextCanvas; // Canvas sẽ hiển thị sau khi kết thúc khung chat

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;
    public float typingSpeed = 0.02f; // Điều chỉnh tốc độ gõ chữ

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo DialogueManager không bị phá hủy khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }

        lines = new Queue<DialogueLine>();
        dialogueBox.SetActive(false); // Ẩn khung chat khi bắt đầu
        nextCanvas.gameObject.SetActive(false); // Ẩn canvas khi bắt đầu
    }

    private void Update()
    {
        // Kiểm tra nếu khung chat đang hoạt động và nhấn phím Space để tiếp tục
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogueLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        dialogueBox.SetActive(true); // Hiển thị khung chat

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue(); // Kết thúc hội thoại ngay lập tức
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        characterName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.line));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueArea.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false); // Ẩn khung chat khi kết thúc

        nextCanvas.gameObject.SetActive(true); // Hiển thị canvas đã ẩn
        Time.timeScale = 0f; // Dừng thời gian
    }

    public void ShowDialogueBox()
    {
        dialogueBox.SetActive(true); // Hiển thị khung chat
    }

    public void HideDialogueBox()
    {
        dialogueBox.SetActive(false); // Ẩn khung chat
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Đảm bảo thời gian chạy bình thường

        // Reset toàn bộ dữ liệu trước khi load MainMenu
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.ResetAllData();
        }

        // Chuyển về scene MainMenu
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
