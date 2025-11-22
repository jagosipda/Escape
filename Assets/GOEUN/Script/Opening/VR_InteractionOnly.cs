using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;

public class VR_InteractionOnly : MonoBehaviour
{
    [Header("View / Camera")]
    public float mouseSensitivity = 3f;
    public Camera mainCamera;
    public Transform vrCamera;

    private float cameraPitch = 0f;
    private bool vrActive = false;

    [Header("Interact Settings")]
    public float interactDistance = 3f;
    public LayerMask interactMask = ~0;
    public KeyCode pcInteractKey = KeyCode.Mouse0;
    public string vrInteractButton = "joystick button 15"; // PC Link 시

    [Header("Reticle (Optional)")]
    public RectTransform reticleUI;
    public float reticleHitScale = 1.4f;
    public float reticleLerp = 15f;
    private float reticleBaseScale = 1f;

    void Awake()
    {
        vrActive = XRSettings.isDeviceActive;

        if (!mainCamera)
            mainCamera = GetComponentInChildren<Camera>();

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
        if (!vrActive)
            HandleMouseLook();

        // VR은 머리 방향이 자동으로 카메라 방향이므로 별도의 회전 없음

        UpdateReticle();
        HandleInteractionInput();
    }

    // ------------------ PC 마우스 회전 ------------------
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX); // 좌우 회전

        cameraPitch -= mouseY;                 // 상하 회전
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        if (mainCamera)
            mainCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
    }

    // ------------------ 클릭 / 트리거 입력 ------------------
    void HandleInteractionInput()
    {
        bool triggerPressed =
            Input.GetKeyDown(pcInteractKey) ||                       // PC 마우스 클릭
            Input.GetKeyDown(vrInteractButton) ||                    // PC 링크 모드
            (XRSettings.isDeviceActive &&
            (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || // VR 오른손
             OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)  // VR 왼손
            ));

        if (triggerPressed)
            TryInteract();
    }

    // ------------------ 상호작용 처리 ------------------
    void TryInteract()
    {
        Ray ray = vrActive && vrCamera != null
            ? new Ray(vrCamera.position, vrCamera.forward)
            : new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            var target = hit.collider.GetComponent<IInteractable>();
            if (target != null)
            {
                target.Interact();
                Debug.Log("Interact: " + hit.collider.name);
            }
        }
    }

    // ------------------ 레티클 피드백 ------------------
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

        reticleUI.localScale = Vector3.Lerp(
            reticleUI.localScale,
            new Vector3(targetScale, targetScale, 1f),
            Time.deltaTime * reticleLerp
        );
    }
}
