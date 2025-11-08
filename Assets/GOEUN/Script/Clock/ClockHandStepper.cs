using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// 시침/분침 공용:
/// - 마우스 클릭 또는 VR 트리거를 누르고 있는 동안 자동 회전.
/// - 손 떼면 즉시 정지.
/// - 회전 중에만 효과음 반복 재생.
/// </summary>
public class ClockHandController : MonoBehaviour, IInteractable
{
    [Header("Pivot Reference")]
    [Tooltip("이 바늘이 회전할 중심(Pivot)을 지정하세요.")]
    public Transform pivot;

    [Header("Rotation Settings")]
    [Tooltip("회전 축 (보통 Z축)")]
    public Vector3 rotationAxis = Vector3.forward;

    [Tooltip("초당 회전 속도 (도/초)")]
    public float rotationSpeed = 60f;  // 60° per second = 한 바퀴 6초

    [Header("Sound Settings")]
    [Tooltip("회전 중 재생할 효과음 (짧은 루프형 사운드)")]
    public AudioSource audioSource;
    public AudioClip rotationSfx;

    [Header("VR Input Settings")]
    [Tooltip("VR 컨트롤러 종류 (보통 오른손)")]
    public XRNode controllerNode = XRNode.RightHand;

    private bool rotating = false;
    private static ClockHandController currentActive = null;
    private bool usingVR = false;

    void Start()
    {
        if (!pivot && transform.parent != null)
            pivot = transform.parent;

        // 오디오 설정 초기화
        if (audioSource)
        {
            audioSource.clip = rotationSfx;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f;
            audioSource.Stop();
        }
    }

    public void Interact()
    {
        // 다른 바늘이 돌고 있으면 무시
        if (currentActive != null && currentActive != this)
            return;

        rotating = true;
        currentActive = this;

        // VR 입력 장치 감지
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        usingVR = device.isValid;

        // 사운드 재생
        if (audioSource && rotationSfx && !audioSource.isPlaying)
            audioSource.Play();

        Debug.Log($"{name} : 자동 회전 시작 ({(usingVR ? "VR" : "Mouse")})");
    }

    void Update()
    {
        if (!rotating || pivot == null)
            return;

        bool stopInput = false;

        if (usingVR)
        {
            // 트리거에서 손 떼면 멈춤
            InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && !triggerPressed)
                stopInput = true;
        }
        else
        {
            // 마우스 클릭 해제 시 멈춤
            stopInput =
                Input.GetMouseButtonUp(0) ||
                Input.GetButtonUp("Fire1") ||
                Input.GetButtonUp("Fire2");
        }

        // 회전 중이면 계속 시계방향 회전
        if (!stopInput)
        {
            pivot.Rotate(rotationAxis, -rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            rotating = false;
            if (currentActive == this)
                currentActive = null;

            if (audioSource && audioSource.isPlaying)
                audioSource.Stop();

            Debug.Log($"{name} : 회전 종료");
        }
    }

    void OnDisable()
    {
        if (currentActive == this)
            currentActive = null;
        rotating = false;

        if (audioSource && audioSource.isPlaying)
            audioSource.Stop();
    }
}
