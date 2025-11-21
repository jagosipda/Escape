using UnityEngine;
using UnityEngine.XR;

public class SchoolTitleUI : MonoBehaviour
{
    [Header("Pause")]
    public PauseManager pauseManager;

    [Header("PC에서만 쓰는 카메라")]
    public Camera openingCamera;   // 시작 화면용 카메라
    public Camera mainCamera;      // PC 플레이용 카메라

    [Header("UI")]
    public GameObject titleUI;     // TitlePanel (Start / Quit)
    public GameObject hudCanvas;   // HUD_Canvas
    public GameObject reticle;     // 중앙 점 UI

    [Header("Player")]
    public VR_PlayerMovement playerMovement;

    bool vrActive;

    void Awake()
    {
        vrActive = XRSettings.isDeviceActive;
    }

    void Start()
    {
        // 🥽==== VR 모드일 때: 오프닝 건너뛰고 바로 게임 시작 상태로 세팅 ====🥽
        if (vrActive)
        {
            // 카메라는 팀원이 직접 MainCamera 끄고 OVRCameraRig 켜서 씀
            if (openingCamera) openingCamera.enabled = false;
            // mainCamera는 건들지 않음 (비활성이어도 상관 없음)

            // 타이틀은 VR에선 안 쓰니까 숨기기
            if (titleUI)   titleUI.SetActive(false);

            // 바로 HUD / reticle / 플레이어 켜기
            if (hudCanvas) hudCanvas.SetActive(true);
            if (reticle)   reticle.SetActive(true);
            if (playerMovement) playerMovement.enabled = true;

            if (pauseManager) pauseManager.EnablePause();

            // 여기서 끝! (아래 PC용 로직은 타지 않음)
            return;
        }

        // 💻==== PC 모드 (모니터 플레이) ====💻
        // 오프닝 카메라로 타이틀 비추기
        if (openingCamera) openingCamera.enabled = true;
        if (mainCamera)    mainCamera.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // UI / 플레이어 기본 상태
        if (titleUI)   titleUI.SetActive(true);    // 타이틀 보이기
        if (hudCanvas) hudCanvas.SetActive(false); // HUD는 나중에
        if (reticle)   reticle.SetActive(false);   // 오프닝에서 reticle 숨기기
        if (playerMovement) playerMovement.enabled = false;

        if (pauseManager) pauseManager.enabled = true;
    }

    // ====== PC에서만 실제로 쓰이는 Start 버튼 ======
    public void OnClickStart()
    {
        if (!vrActive)
        {
            // PC 모드에서만 카메라 전환 + 마우스 잠그기
            if (openingCamera) openingCamera.enabled = false;
            if (mainCamera)    mainCamera.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        // 공통: 게임 진행 상태로 전환
        if (titleUI)   titleUI.SetActive(false);
        if (hudCanvas) hudCanvas.SetActive(true);
        if (reticle)   reticle.SetActive(true);
        if (playerMovement) playerMovement.enabled = true;

        if (pauseManager) pauseManager.EnablePause();
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
