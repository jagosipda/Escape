using UnityEngine;

/// <summary>
/// 열쇠 오브젝트 클릭 시 획득 처리 (중앙점 + 트리거 지원)
/// Collider는 열쇠 Mesh에, 스크립트도 같은 오브젝트에 붙인다.
/// </summary>
public class KeyPickup : MonoBehaviour, IInteractable
{
    [Header("SFX & Effect")]
    public AudioSource sfx;
    public AudioClip pickupSfx;
    public GameObject onPickupEffect;

    private bool pickedUp = false;

    // 플레이어의 중앙점 클릭(또는 VR 트리거)에 의해 호출됨
    public void Interact()
    {
        if (pickedUp) return;
        pickedUp = true;

        Debug.Log($"{gameObject.name} 획득!");

        // 플레이어에게 열쇠 전달
        if (KeyManager.Instance != null)
        {
            KeyManager.Instance.ObtainKey();
        }

        // 효과음 재생
        if (sfx && pickupSfx)
            sfx.PlayOneShot(pickupSfx);

        // 획득 이펙트 생성
        if (onPickupEffect)
            Instantiate(onPickupEffect, transform.position, Quaternion.identity);

        // 열쇠 비활성화
        gameObject.SetActive(false);
    }
}
