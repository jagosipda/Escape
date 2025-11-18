using UnityEngine;
using UnityEngine.XR;

public class SchoolTitleUI : MonoBehaviour
{

    public PauseManager pauseManager;
    
    [Header("Camera")]
    public Camera openingCamera;   // 오프닝용 카메라
    public Camera mainCamera;      // 플레이용 카메라

    [Header("UI")]
    public GameObject titleUI;     // START / QUIT UI (TitlePanel 들어있는 Canvas)
    public GameObject hudCanvas;   // 방향키 & 클릭 HUD_Canvas

    [Header("Player")]
    public VR_PlayerMovement playerMovement; // Player 오브젝트에 붙어있는 스크립트

    [Header("Reticle")]
    public GameObject reticle;

    void Start()
    {
        // ---- 오프닝 상태로 시작 ----
        if (openingCamera) openingCamera.enabled = true;
        if (mainCamera)    mainCamera.enabled = false;

        if (titleUI)   titleUI.SetActive(true);    // 타이틀 보이기
        if (hudCanvas) hudCanvas.SetActive(false); // HUD 숨기기

        if (playerMovement) playerMovement.enabled = false; // 플레이어 움직임 잠깐 꺼두기

        // 커서 보이게 & 잠금 해제 (PC용)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (reticle) reticle.SetActive(false);   // ★ 타이틀 상태에서 Reticle 숨기기
    }

    // START 버튼 OnClick 에 연결할 함수
    public void OnClickStart()
    {
        // 카메라 전환
        if (openingCamera) openingCamera.enabled = false;
        if (mainCamera)    mainCamera.enabled = true;

        // UI 전환
        if (titleUI)   titleUI.SetActive(false);  // 타이틀 숨기기
        if (hudCanvas) hudCanvas.SetActive(true); // HUD 켜기

        // 플레이어 움직임 켜기
        if (playerMovement) playerMovement.enabled = true;

        // PC 환경이면 커서 다시 잠궈서 FPS 느낌으로
        if (!XRSettings.isDeviceActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        if (pauseManager) pauseManager.EnablePause();   // ★ 게임 시작 후부터 ESC 사용 허용

        if (reticle) reticle.SetActive(true);   // 본게임에서 reticle 다시 켬
    }

    // QUIT 버튼 OnClick 에 연결할 함수
    public void OnClickQuit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
