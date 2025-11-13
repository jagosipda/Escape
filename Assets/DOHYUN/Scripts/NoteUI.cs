using UnityEngine;
using TMPro;   // TextMeshPro 쓰면 필요!

public class NoteUI : MonoBehaviour
{
    public GameObject panel;             // NotePanel
    public TextMeshProUGUI noteTextUI;   // NoteText

    public void Show(string text)
    {
        panel.SetActive(true);
        noteTextUI.text = text;

        // 마우스 커서 보이게 + 멈추고 싶으면:
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;   // 게임 일시정지 (원하면 빼도 됨)
    }

    public void Hide()
    {
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        // ESC 또는 마우스 오른쪽 등으로 메모 닫기
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }
}
