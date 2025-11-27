using UnityEngine;

public class PlayerClickInteract : MonoBehaviour
{
    [Header("기본 설정")]
    public Camera cam;                 // 플레이어가 쓰는 메인 카메라
    public float interactDistance = 5f;
    public LayerMask interactMask = ~0; // 일단 Everything

    [Header("노트 UI")]
    public NoteUI noteUI;              // 이미 쓰고 있는 NoteUI

    void Start()
    {
        // 인스펙터에 안 넣어줬으면 자동으로 카메라 찾기
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
                Debug.LogError("PlayerClickInteract: 카메라를 찾을 수 없음!");
        }
    }

    void Update()
    {
        // 왼쪽 클릭: 노트 + 장기 상호작용
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        // 오른쪽 클릭: 장기만 집기
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClickOrganOnly();
        }
    }

    void HandleLeftClick()
    {
        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        );

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
        {
            // 1) 노트 먼저 체크
            Note note = hit.collider.GetComponent<Note>();
            if (note != null)
            {
                if (noteUI != null)
                {
                    noteUI.Show(note.noteText);
                }
                return;
            }

            // 2) 장기(OrganPickup) 체크
            OrganPickup organ = hit.collider.GetComponent<OrganPickup>();
            if (organ != null)
            {
                if (OrganInventory.Instance != null)
                {
                    OrganInventory.Instance.PickupOrgan(organ);
                    Debug.Log("장기 획득 (왼쪽클릭): " + organ.organName);
                }

                organ.gameObject.SetActive(false);
                return;
            }
        }
    }

    void HandleRightClickOrganOnly()
    {
        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        );

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
        {
            // 오른쪽 클릭은 장기만 체크
            OrganPickup organ = hit.collider.GetComponent<OrganPickup>();
            if (organ != null)
            {
                if (OrganInventory.Instance != null)
                {
                    OrganInventory.Instance.PickupOrgan(organ);
                    Debug.Log("장기 획득 (오른쪽클릭): " + organ.organName);
                }

                organ.gameObject.SetActive(false);
            }
        }
    }
}
