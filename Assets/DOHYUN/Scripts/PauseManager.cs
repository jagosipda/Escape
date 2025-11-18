using UnityEngine;
using UnityEngine.XR;

public class PauseManager : MonoBehaviour
{
    [Header("참조")]
    public GameObject pauseUI;          // PauseCanvas
    public GameObject hudCanvas;        // HUD_Canvas
    public VR_PlayerMovement playerMovement;  // Player에 붙은 스크립트

    private bool isPaused = false;
    private bool canPause = false;

    // TitleManager에서 게임 시작할 때 불러 줄 함수
    public void EnablePause()
    {
        canPause = true;
    }

    void Update()
    {
        if (!canPause) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else          PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if (pauseUI)   pauseUI.SetActive(true);
        if (hudCanvas) hudCanvas.SetActive(false);
        if (playerMovement) playerMovement.enabled = false;

        Time.timeScale = 0f;  // 게임 정지

        // 마우스 보이게 (PC용)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pauseUI)   pauseUI.SetActive(false);
        if (hudCanvas) hudCanvas.SetActive(true);
        if (playerMovement) playerMovement.enabled = true;

        Time.timeScale = 1f;  // 다시 재생

        // HMD 없을 때만 마우스 잠금
        if (!XRSettings.isDeviceActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // 혹시 일시정지가 걸려 있으면 풀어주기
        
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 종료

    #else
        Application.Quit();  // 빌드에서 실제 종료
    #endif
    }

}
