using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus; // OVRInput 사용 (Meta Quest용)

public class test : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float gravity = -9.81f;

    [Header("PC Turn Settings")]
    public float pcTurnSpeed = 45f;   // ← PC용 F/G 회전 속도 필드 추가

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

    private CharacterController controller;
    private float cameraPitch = 0f;
    private Vector3 velocity;
    private bool vrActive = false;
    private float reticleBaseScale = 1f;
    private Vector3 currentMove = Vector3.zero;

    [Header("VR Rotation Settings")]
    public float vrTurnSpeed = 45f;
    public bool smoothTurn = true;
    public float snapAngle = 45f;
    private float snapCooldown = 0.4f;
    private float lastSnapTime = 0f;

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
        {
            HandleMouseLook();
            HandlePCRotation();  // ← PC에서 F/G 회전 적용
        }
        else
        {
            HandleVRRotation();
        }

        UpdateReticle();
        HandleInteractionInput();
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

    // ---------- PC F/G 회전 ----------
    void HandlePCRotation()
    {
        if (Input.GetKey(KeyCode.F))
        {
            transform.Rotate(0f, -pcTurnSpeed * Time.deltaTime, 0f);
        }

        if (Input.GetKey(KeyCode.G))
        {
            transform.Rotate(0f, pcTurnSpeed * Time.deltaTime, 0f);
        }
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

    // ---------- 클릭/트리거 통합 상호작용 ----------
    void HandleInteractionInput()
    {
        bool triggerPressed =
            Input.GetKeyDown(pcInteractKey) ||
            Input.GetKeyDown(vrInteractButton) ||
            (XRSettings.isDeviceActive &&
             (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
              OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)));

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
                Debug.Log("[Interact] " + hit.collider.name);
            }
        }
    }

    // ---------- 레티클 ----------
    void UpdateReticle()
    {
        if (!reticleUI) return;

        bool onTarget = false;
        Ray ray = vrActive && vrCamera != null
            ? new Ray(vrCamera.position, vrCamera.forward)
            : new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
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
            if (!footstepSource.isPlaying)
            {
                footstepSource.clip = footstepClips[0];
                footstepSource.loop = true;
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }
}
