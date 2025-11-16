using UnityEngine;

public class SchoolTitleUI : MonoBehaviour
{
    // 오프닝 화면용 카메라 (학교 특정 부분 비추는 카메라)
    public Camera openingCamera;

    // 기존에 쓰던 메인 카메라 (VR_PlayerMovement에서 쓰는 그 카메라)
    public Camera mainCamera;

    // 플레이어 오브젝트 (스크린샷에 있는 Player)
    public GameObject player;

    // 실제로 움직임/조작 담당하는 스크립트
    public VR_PlayerMovement playerMovement;

    // 시작/종료 버튼 있는 Canvas
    public GameObject titleUI;

    void Start()
    {
        // 게임 시작하면: 오프닝 상태로 만들기
        // 오프닝 마우스 커서 보이게 하기
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 플레이어 조작 막기
        if (playerMovement != null)
            playerMovement.enabled = false;

        // 플레이어 자체를 안 보이게 하고 싶으면 켜기
        // if (player != null)
        //     player.SetActive(false);

        // 카메라 전환: 오프닝 카메라만 켜고, 메인 카메라는 끄기
        if (openingCamera != null)
            openingCamera.enabled = true;

        if (mainCamera != null)
            mainCamera.enabled = false;
    }

    public void OnClickStart()
    {
        // 여기서부터 진짜 플레이 시작

        // 게임 시작할 땐 다시 마우스 숨기고, 잠그기
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 카메라 전환: 메인 카메라 켜고, 오프닝 카메라는 끄기
        if (openingCamera != null)
            openingCamera.enabled = false;

        if (mainCamera != null)
            mainCamera.enabled = true;

        // 플레이어 보이게 & 조작 켜기
        if (player != null)
            player.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        // 타이틀 UI 숨기기
        if (titleUI != null)
            titleUI.SetActive(false);
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
