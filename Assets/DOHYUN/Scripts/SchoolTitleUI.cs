using UnityEngine;
using UnityEngine.XR;

public class SchoolTitleUI : MonoBehaviour
{
    [Header("Pause")]
    public PauseManager pauseManager;

    [Header("PC에서만 쓰는 카메라")]
    public Camera openingCamera;   // 시작 화면용
    public Camera mainCamera;      // 플레이용 (PC에서만 사용)

    [Header("UI")]
    public GameObject titleUI;     // TitlePanel (Start / Quit)
    public GameObject hudCanvas;   // HUD_Canvas (방향키, 클릭)
    public GameObject reticle;     // 중앙 점(UI)

    [Header("Player")]
    public VR_PlayerMovement playerMovement;

    bool vrActive; // HMD 연결 여부

    void Awake()
    {
        vrActive = XRSettings.isDeviceActive;
    }

    void Start()
    {
        // --- 카메라 설정 ---
        if (!vrActive)
        {
            // 📺 PC 모드: OpeningCamera로 타이틀 비추기
            if (openingCamera) openingCamera.enabled = true;
            if (mainCamera)    mainCamera.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        else
        {
            // 🥽 VR 모드: 카메라는 전혀 건드리지 않음
            // (도현이/팀원이 미리 MainCamera 끄고 OVRCameraRig 켜놓은 상태 그대로 사용)
            // 필요하면 OpeningCamera만 꺼도 됨
            if (openingCamera) openingCamera.enabled = false;
            // mainCamera.enabled는 아예 건드리지 않는 게 안전 (비활성화된 상태여도 OK)
        }

        // --- UI / 플레이어 상태 ---
        if (titleUI)   titleUI.SetActive(true);    // 타이틀 UI 보이기
        if (hudCanvas) hudCanvas.SetActive(false); // HUD는 나중에
        if (reticle)   reticle.SetActive(false);   // 오프닝에서는 reticle 안 보이게
        if (playerMovement) playerMovement.enabled = false; // 아직 이동 금지

        if (pauseManager) pauseManager.enabled = true;
    }

    // Start 버튼
    public void OnClickStart()
    {
        if (!vrActive)
        {
            // 📺 PC 모드: 카메라 전환 + 마우스 잠그기
            if (openingCamera) openingCamera.enabled = false;
            if (mainCamera)    mainCamera.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
        else
        {
            // 🥽 VR 모드: 카메라 건드리지 않음 (OVRCameraRig 그대로 사용)
            // 여기서는 UI/플레이어만 켜주면 됨
        }

        // 공통: 게임 시작 상태로 전환
        if (titleUI)   titleUI.SetActive(false);
        if (hudCanvas) hudCanvas.SetActive(true);
        if (reticle)   reticle.SetActive(true);
        if (playerMovement) playerMovement.enabled = true;

        if (pauseManager) pauseManager.EnablePause();
    }

    // Quit 버튼
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
