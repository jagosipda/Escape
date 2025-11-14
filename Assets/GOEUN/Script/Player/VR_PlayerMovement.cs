using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus; // OVRInput 사용 (Meta Quest용)

public class VR_PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float gravity = -9.81f;

    [Header("Interact")]
    public float interactDistance = 3f;
    public LayerMask interactMask = ~0;
    public KeyCode pcInteractKey = KeyCode.Mouse0;
    public string vrInteractButton = "joystick button 15"; // PC 링크 상태일 때 트리거

    [Header("Cameras")]
    public Camera mainCamera;
    public Transform vrCamera;

    [Header("Reticle (Optional)")]
    public RectTransform reticleUI;
    public float reticleHitScale = 1.4f;
    public float reticleLerp = 15f;

    [Header("Footstep Sound")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.5f;
    private float footstepTimer = 0f;

    private CharacterController controller;
    private float cameraPitch = 0f;
    private Vector3 velocity;
    private bool vrActive = false;
    private float reticleBaseScale = 1f;

    [Header("VR Rotation Settings")]
    public float vrTurnSpeed = 45f;
    public bool smoothTurn = true;
    public float snapAngle = 45f;
    private float snapCooldown = 0.4f;
    private float lastSnapTime = 0f;

    // 현재 프레임 이동 벡터 (발소리용)
    private Vector3 currentMove = Vector3.zero;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!mainCamera) mainCamera = GetComponentInChildren<Camera>();
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
        else
            HandleVRRotation();

        UpdateReticle();
        HandleInteractionInput(); // 통합 트리거 입력 처리
        ApplyGravity();
        HandleFootsteps();
    }

    // ---------- 이동 ----------
    void HandleMove()
    {
        if (vrActive)
        {
            Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            Vector3 forward = vrCamera.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = vrCamera.right;
            right.y = 0f;
            right.Normalize();

            currentMove = forward * input.y + right * input.x;
            controller.Move(currentMove * moveSpeed * Time.deltaTime);
        }
        else
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            currentMove = transform.right * h + transform.forward * v;
            controller.Move(currentMove * moveSpeed * Time.deltaTime);
        }
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

    // ---------- VR 몸 회전 ----------
    void HandleVRRotation()
    {
        Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (smoothTurn)
        {
            float yaw = rightStick.x * vrTurnSpeed * Time.deltaTime;
            transform.Rotate(0f, yaw, 0f);
        }
        else
        {
            if (Mathf.Abs(rightStick.x) > 0.7f && Time.time - lastSnapTime > snapCooldown)
            {
                float angle = Mathf.Sign(rightStick.x) * snapAngle;
                transform.Rotate(0f, angle, 0f);
                lastSnapTime = Time.time;
            }
        }
    }

    // ---------- 트리거/클릭 통합 입력 ----------
    void HandleInteractionInput()
    {
        bool triggerPressed =
            Input.GetKeyDown(pcInteractKey) ||                         // 마우스 클릭
            Input.GetKeyDown(vrInteractButton) ||                      // PC Link 상태 (조이스틱 버튼)
            (XRSettings.isDeviceActive &&                              // Meta Quest Standalone 빌드
             (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||  // 오른손 트리거
              OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))  // 왼손 트리거
            );

        if (triggerPressed)
        {
            TryInteract();
        }
    }

    // ---------- 상호작용 ----------
    void TryInteract()
    {
        Ray ray = vrActive && vrCamera != null
            ? new Ray(vrCamera.position, vrCamera.forward)
            : new Ray(mainCamera.transform.position, mainCamera.transform.forward);

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

    // ---------- 레티클 피드백 ----------
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

    // ---------- 발자국 소리 ----------
    void HandleFootsteps()
    {
        if (!footstepSource || footstepClips.Length == 0) return;

        Vector3 horizontalMove = new Vector3(currentMove.x, 0f, currentMove.z);
        float speed = horizontalMove.magnitude;
        bool movingNow = speed > 0.1f && controller.isGrounded;

        if (movingNow)
        {
            // 걷는 중인데 아직 소리 안 나면 루프 시작
            if (!footstepSource.isPlaying)
            {
                footstepSource.clip = footstepClips[0]; // mp3 하나만 사용
                footstepSource.loop = true;
                footstepSource.Play();
            }
        }
        else
        {
            // 멈추면 즉시 중단
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    void PlayFootstep()
    {
        if (!footstepSource || footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.PlayOneShot(clip);
    }

}