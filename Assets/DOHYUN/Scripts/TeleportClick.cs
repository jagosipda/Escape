using UnityEngine;

public class ClickTeleportManager : MonoBehaviour
{
    public Transform player;          // 플레이어(또는 플레이어 캡슐)의 Transform
    public float maxDistance = 5f;    // 클릭 가능한 최대 거리
    public LayerMask interactMask;    // 문/상호작용 오브젝트가 있는 레이어

    void Update()
    {
        // 🔒 필수 레퍼런스 확인 (없으면 아무것도 안 하고 리턴)
        if (player == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;   // VR에서 MainCamera 꺼져 있으면 여기서 바로 나감

        // 왼쪽 마우스 클릭
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(
                new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
            );
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, interactMask))
            {
                TeleportDoor door = hit.collider.GetComponent<TeleportDoor>();

                if (door != null && door.targetPoint != null)
                {
                    CharacterController cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    player.SetPositionAndRotation(
                        door.targetPoint.position,
                        door.targetPoint.rotation
                    );

                    if (cc != null) cc.enabled = true;

                    if (LabUIController.Instance != null)
                    {
                        LabUIController.Instance.ShowLabUI(door.showLabUIAfterTeleport);
                    }
                }
            }
        }
    }
}
