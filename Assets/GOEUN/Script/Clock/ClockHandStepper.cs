using UnityEngine;

/// <summary>
/// 시침/분침 공용: 클릭한 후 마우스를 드래그하면
/// 부모 Pivot(회전 중심)을 기준으로 시계방향으로 부드럽게 회전.
/// Collider는 바늘 Mesh에, 스크립트도 같은 Mesh에 붙인다.
/// </summary>
public class ClockHandController : MonoBehaviour
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

    void Start()
    {
        // Pivot이 비어 있으면 부모를 자동 설정
        if (!pivot && transform.parent != null)
            pivot = transform.parent;
    }

    void OnMouseDown()
    {
        dragging = true;
        lastMouseX = Input.mousePosition.x;
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void Update()
    {
        if (!dragging || pivot == null)
            return;

        float mouseDelta = Input.mousePosition.x - lastMouseX;
        lastMouseX = Input.mousePosition.x;

        // 감도 적용
        float deltaRotation = mouseDelta * mouseSensitivity;

        // 항상 시계 방향으로만 회전
        if (deltaRotation < 0)
            deltaRotation = -deltaRotation;

        // 회전 적용 (Pivot 기준)
        pivot.Rotate(rotationAxis, -deltaRotation, Space.Self);
    }
}
