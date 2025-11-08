using UnityEngine;

public class FlashlightPickup : MonoBehaviour, IInteractable
{
    [Header("Light Settings")]
    [Tooltip("플래시라이트가 켜질 때 생성되는 Spot Light 프리팹")]
    public GameObject flashlightLightPrefab;

    private bool pickedUp = false;

    public void Interact()
    {
        if (pickedUp) return;
        pickedUp = true;

        // 손전등 오브젝트 숨기기
        gameObject.SetActive(false);

        // 플레이어 카메라 찾기
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("Main Camera를 찾을 수 없습니다!");
            return;
        }

        // Spot Light 생성 (카메라 자식으로 붙이기)
        GameObject lightObj = Instantiate(flashlightLightPrefab, mainCam.transform);
        lightObj.transform.localPosition = new Vector3(0f, 0f, 0.3f); // 카메라 앞 약간
        lightObj.transform.localRotation = Quaternion.identity;

        Debug.Log("손전등을 집었습니다. 플래시라이트 켜짐!");
    }
}
