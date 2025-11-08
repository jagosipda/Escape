using UnityEngine;

public class VR_HandleClick : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public LockerDoorController door;

    public void Interact()
    {
        if (door == null)
        {
            Debug.LogWarning($"{name} : 연결된 문(door)이 없습니다!");
            return;
        }

        Debug.Log($"{name} 클릭됨 → 문 여닫기 실행");
        door.Toggle();
    }
}
