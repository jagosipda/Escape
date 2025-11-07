using UnityEngine;

/// <summary>
/// 열쇠 보유 여부 확인 후 문 여는 상호작용.
/// 중앙점(Reticle) 클릭 또는 VR 트리거 입력 시 작동.
/// </summary>
public class Key_HandleClick : MonoBehaviour, IInteractable
{
    [Header("Door Reference")]
    public LockerDoorController door;

    public void Interact()
    {
        if (door == null)
        {
            Debug.LogWarning($"{name}: 연결된 문(door)이 없습니다!");
            return;
        }

        // 열쇠 보유 여부 확인
        if (KeyManager.Instance != null && !KeyManager.Instance.HasKey())
        {
            Debug.Log("문이 잠겨 있습니다. 열쇠가 필요합니다!");
            return;
        }

        // 열쇠가 있으면 문 여닫기 실행
        door.Toggle();
        Debug.Log("열쇠를 사용하여 문을 엽니다!");
    }
}
