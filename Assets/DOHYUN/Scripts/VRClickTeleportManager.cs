using UnityEngine;
using UnityEngine.XR;      // XRSettings
using Unity.XR.Oculus;     // OVRInput 쓰면 있으면 좋고, 없어도 컴파일은 됨

public class VRClickTeleportManager : MonoBehaviour
{
    [Header("필수 레퍼런스")]
    public Transform player;      // 플레이어(캡슐) Transform
    public Transform rayOrigin;   // 레이 쏠 기준 (예: RightHandAnchor)

    [Header("텔레포트 설정")]
    public float maxDistance = 5f;        // 최대 거리
    public LayerMask interactMask;        // TeleportDoor 가 있는 레이어
    public OVRInput.Button teleportButton = OVRInput.Button.PrimaryIndexTrigger;
    // ↑ 어떤 버튼으로 텔레포트할지 (오른손 트리거)

    void Update()
    {
        // HMD 안 켜져 있으면 동작 안 함 (PC 플레이 때는 무시)
        if (!XRSettings.isDeviceActive) return;

        // 필수 레퍼런스 없으면 그냥 리턴
        if (player == null || rayOrigin == null) return;

        // 컨트롤러 버튼 입력
        if (OVRInput.GetDown(teleportButton))
        {
            // 컨트롤러에서 앞으로 레이 쏘기
            Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, interactMask))
            {
                TeleportDoor door = hit.collider.GetComponent<TeleportDoor>();

                if (door != null && door.targetPoint != null)
                {
                    // CharacterController 튕김 방지
                    CharacterController cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    player.SetPositionAndRotation(
                        door.targetPoint.position,
                        door.targetPoint.rotation
                    );

                    if (cc != null) cc.enabled = true;

                    // 실험실 UI 같은 거 띄우는 부분 (PC용이랑 동일)
                    if (LabUIController.Instance != null)
                    {
                        LabUIController.Instance.ShowLabUI(door.showLabUIAfterTeleport);
                    }
                }
            }
        }
    }
}
