using UnityEngine;

/// <summary>
/// 시침/분침 공용:
/// - 중앙점(Reticle)을 맞춘 뒤 마우스 클릭 또는 VR 트리거를 누르고 있는 동안 회전.
/// - Collider는 시침/분침 Mesh에, 스크립트도 같은 Mesh에 붙인다.
/// - 서로 겹치지 않게 콜라이더를 살짝 띄우거나 Layer 분리.
/// - 플레이어 스크립트는 수정하지 않아도 작동.
/// </summary>
public class ClockHandController : MonoBehaviour, IInteractable
{
    [Header("Pivot Reference")]
    [Tooltip("이 바늘이 회전할 중심(Pivot)을 지정하세요.")]
    public Transform pivot; // 회전 중심 (부모 등)

    [Header("Rotation Settings")]
    [Tooltip("회전 축 (보통 Z축)")]
    public Vector3 rotationAxis = Vector3.forward;

    [Tooltip("마우스 감도 (2~4 추천)")]
    public float mouseSensitivity = 3f;

    private bool dragging = false;
    private float lastMouseX;

    // 동시에 여러 바늘이 움직이지 않도록 제어용 static 변수
    private static ClockHandController currentActive = null;

    void Start()
    {
        if (!pivot && transform.parent != null)
            pivot = transform.parent;
    }

    // VR_PlayerMovement.TryInteract()가 호출하는 함수
    public void Interact()
    {
        // 이미 다른 바늘을 회전 중이면 무시
        if (currentActive != null && currentActive != this)
            return;

        dragging = true;
        currentActive = this;
        lastMouseX = Input.mousePosition.x;
        Debug.Log($"{name} : 회전 시작");
    }

    void Update()
    {
        if (!dragging || pivot == null)
            return;

        // 마우스 / 트리거 버튼에서 손 떼면 중단
        bool stopInput =
            Input.GetMouseButtonUp(0) ||
            Input.GetButtonUp("Fire1") ||
            Input.GetButtonUp("Fire2");

        if (stopInput)
        {
            dragging = false;
            if (currentActive == this)
                currentActive = null;
            Debug.Log($"{name} : 회전 종료");
            return;
        }

        // 회전량 계산
        float mouseDelta = Input.mousePosition.x - lastMouseX;
        lastMouseX = Input.mousePosition.x;

        float deltaRotation = mouseDelta * mouseSensitivity;
        if (deltaRotation < 0)
            deltaRotation = -deltaRotation;

        // Pivot 기준 시계 방향 회전
        pivot.Rotate(rotationAxis, -deltaRotation, Space.Self);
    }

    void OnDisable()
    {
        if (currentActive == this)
            currentActive = null;
        dragging = false;
    }
}
