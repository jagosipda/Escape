using UnityEngine;
using UnityEngine.XR;

public class VR_PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float gravity = -9.81f;

    [Header("Interact")]
    public float interactDistance = 3f;
    public LayerMask interactMask = ~0;     // 필요시 Interactable 레이어로 교체
    public KeyCode pcInteractKey = KeyCode.Mouse0;
    public string vrInteractButton = "joystick button 15"; // 예: Quest 트리거(입력 매핑에 따라 수정)

    [Header("Cameras")]
    public Camera mainCamera;               // PC 카메라(필수)
    public Transform vrCamera;              // HMD 카메라(XR Origin의 카메라)

    [Header("Reticle (Optional)")]
    public RectTransform reticleUI;         // 화면 중앙 점 UI (선택)
    public float reticleHitScale = 1.4f;    // 맞췄을 때 커지는 비율
    public float reticleLerp = 15f;         // 스무딩

    private CharacterController controller;
    private float cameraPitch = 0f;
    private Vector3 velocity;
    private bool vrActive = false;
    private float reticleBaseScale = 1f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!mainCamera) mainCamera = GetComponentInChildren<Camera>();

        // HMD 착용 여부
        vrActive = XRSettings.isDeviceActive;

        if (!vrActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (reticleUI)
            reticleBaseScale = reticleUI.localScale.x;
    }

    void Update()
    {
        HandleMove();

        if (!vrActive)
            HandleMouseLook();

        UpdateReticle();

        // PC 클릭 or VR 트리거
        if (Input.GetKeyDown(pcInteractKey) || Input.GetKeyDown(vrInteractButton))
        {
            TryInteract();
        }

        ApplyGravity();
    }

    // ---------- 이동 ----------
    void HandleMove()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    // ---------- 시야 회전(PC) ----------
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        if (mainCamera)
            mainCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
    }

    // ---------- 상호작용 ----------
    void TryInteract()
    {
        Ray ray;
        if (vrActive && vrCamera != null)
        {
            ray = new Ray(vrCamera.position, vrCamera.forward);     // HMD 시선
        }
        else
        {
            ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward); // 화면 중앙
        }

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, QueryTriggerInteraction.Ignore))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                Debug.Log($"[Interact] {hit.collider.name}");
            }
        }
    }

    // ---------- 레티클 피드백(선택) ----------
    void UpdateReticle()
    {
        if (!reticleUI) return;

        bool onTarget = false;

        Ray ray = vrActive && vrCamera != null
            ? new Ray(vrCamera.position, vrCamera.forward)
            : new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, QueryTriggerInteraction.Ignore))
        {
            onTarget = hit.collider.GetComponent<IInteractable>() != null;
        }

        float targetScale = onTarget ? reticleBaseScale * reticleHitScale : reticleBaseScale;
        Vector3 s = reticleUI.localScale;
        float lerp = Mathf.Clamp01(Time.deltaTime * reticleLerp);
        reticleUI.localScale = new Vector3(
            Mathf.Lerp(s.x, targetScale, lerp),
            Mathf.Lerp(s.y, targetScale, lerp),
            1f
        );
    }

    // ---------- 중력 ----------
    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
