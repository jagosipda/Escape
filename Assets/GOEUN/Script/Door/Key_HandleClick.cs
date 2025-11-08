using UnityEngine;

/// <summary>
/// 열쇠 보유 여부 확인 후 문 여는 상호작용.
/// 중앙점(Reticle) 클릭 또는 VR 트리거 입력 시 작동.
/// </summary>
public class Key_HandleClick : MonoBehaviour, IInteractable
{
    [Header("Door Reference")]
    public LockerDoorController door;

    [Header("SFX")]
    public AudioSource sfx;              // 오디오 소스 (문 근처에 붙이기)
    public AudioClip lockedSfx;          // 잠김 효과음
    public AudioClip unlockSfx;          // 문 열림 효과음 (선택 사항)

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

            // 잠김 효과음 재생
            if (sfx && lockedSfx)
                sfx.PlayOneShot(lockedSfx);

            return;
        }

        // 열쇠가 있을 때 문 여닫기 실행
        door.Toggle();
        Debug.Log("열쇠를 사용하여 문을 엽니다!");

        // 문 열림 효과음 재생 (선택 사항)
        if (sfx && unlockSfx)
            sfx.PlayOneShot(unlockSfx);
    }
}
