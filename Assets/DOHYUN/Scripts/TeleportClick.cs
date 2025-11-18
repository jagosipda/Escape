using UnityEngine;

public class ClickTeleportManager : MonoBehaviour
{
    public Transform player;          // 플레이어(또는 플레이어 캡슐)의 Transform
    public float maxDistance = 5f;    // 클릭 가능한 최대 거리
    public LayerMask interactMask;    // 문/상호작용 오브젝트가 있는 레이어

    void Update()
    {
        // 왼쪽 마우스 클릭
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(
                new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
            );
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, interactMask))
            {
                // 클릭한 오브젝트에서 TeleportDoor 찾기
                TeleportDoor door = hit.collider.GetComponent<TeleportDoor>();

                if (door != null && door.targetPoint != null)
                {
                    // CharacterController가 달려 있으면 잠깐 껐다가 켜줘야 튕기지 않아
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
