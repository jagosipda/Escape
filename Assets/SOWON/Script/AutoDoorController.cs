using UnityEngine;

/// <summary>
/// 플레이어가 트리거 영역에 진입하면 문을 열고, 이탈하면 문을 닫는 자동 문 제어 스크립트.
/// LockerDoorController.cs에 연결하여 사용합니다.
/// </summary>
public class AutoDoorController : MonoBehaviour
{
    [Header("Door Link")]
    [Tooltip("실제 문 회전을 담당하는 LockerDoorController를 연결하세요.")]
    public LockerDoorController doorController;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그 확인
        if (other.CompareTag("Player"))
        {
            if (doorController != null)
            {
                // 문이 현재 닫혀있다면 엽니다.
                if (!doorController.isOpen)
                {
                    doorController.Toggle();
                    Debug.Log("플레이어 접근 감지: 문을 엽니다.");
                }
            }
            else
            {
                Debug.LogError("DoorController가 연결되지 않았습니다! Inspector를 확인해주세요.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어 태그 확인
        if (other.CompareTag("Player"))
        {
            if (doorController != null)
            {
                // 문이 현재 열려있다면 닫습니다.
                if (doorController.isOpen)
                {
                    doorController.Toggle();
                    Debug.Log("플레이어 이탈 감지: 문을 닫습니다.");
                }
            }
        }
    }
}