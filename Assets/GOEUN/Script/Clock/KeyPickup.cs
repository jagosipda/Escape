using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public AudioSource sfx;
    public AudioClip pickupSfx;
    public GameObject onPickupEffect;

    private bool pickedUp = false;

    void OnMouseDown()
    {
        if (pickedUp) return;
        pickedUp = true;

        Debug.Log($"{gameObject.name} 획득!");

        // 플레이어에게 열쇠 전달
        if (KeyManager.Instance != null)
        {
            KeyManager.Instance.ObtainKey();
        }

        // 효과음
        if (sfx && pickupSfx)
            sfx.PlayOneShot(pickupSfx);

        // 이펙트
        if (onPickupEffect)
            Instantiate(onPickupEffect, transform.position, Quaternion.identity);

        // 열쇠 사라지기
        gameObject.SetActive(false);
    }
}
