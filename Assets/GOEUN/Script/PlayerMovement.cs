using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 70f;     // A/D 회전 속도
    public float pitchSpeed = 50f;      // R/F 위아래 회전 속도
    public float gravity = -9.81f;

    [Header("Interaction Settings")]
    public float interactDistance = 3f; // 클릭 거리
    public LayerMask interactMask;      // 클릭 가능한 레이어

    private CharacterController controller;
    private Transform cam;
    private Vector3 velocity;
    private bool vrActive = false;      // HMD 감지 여부
    private float cameraPitch = 0f;     // 상하 회전 값 저장

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>().transform;

        // HMD 연결 여부 체크
        vrActive = XRSettings.isDeviceActive;

        // PC 테스트 시 커서 보이게
        if (!vrActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (!vrActive)
        {
            HandleKeyboardMove();
            HandleKeyboardRotate();
            HandlePitchRotate();   // 위아래 회전 추가
        }

        ApplyGravity();

        // 마우스 클릭 시 인터랙션
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    // ---------- 이동 ----------
    void HandleKeyboardMove()
    {
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    // ---------- 좌우 회전 (A, D 키) ----------
    void HandleKeyboardRotate()
    {
        float h = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * h * rotateSpeed * Time.deltaTime);
    }

    // ---------- 상하 회전 (R, F 키) ----------
    void HandlePitchRotate()
    {
        float r = 0f;

        if (Input.GetKey(KeyCode.R))
            r = -1f;
        else if (Input.GetKey(KeyCode.F))
            r = 1f;

        cameraPitch += r * pitchSpeed * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f); // 너무 위/아래로 안돌아가게 제한
        cam.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    // ---------- 중력 ----------
    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ---------- 상호작용 ----------
    void TryInteract()
    {
        Ray ray;

        if (vrActive)
        {
            // VR일 때는 HMD 시선 기준
            ray = new Ray(cam.position, cam.forward);
        }
        else
        {
            // PC일 때는 화면 중앙 기준 클릭
            ray = cam.GetComponent<Camera>().ScreenPointToRay(
                new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        }

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            var clickable = hit.collider.GetComponent<HandleClick>();
            if (clickable != null)
            {
                clickable.door.Toggle();
            }
        }
    }
}
