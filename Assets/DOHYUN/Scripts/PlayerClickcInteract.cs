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
        // 마우스 왼쪽 클릭
        if (Input.GetMouseButtonDown(0))
        {
            // 화면 중앙에서 Ray 쏘기 (빨간 점 위치라고 생각하면 됨)
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
                        Debug.Log("장기 획득: " + organ.organName);
                    }

                    // 월드에서 숨기기
                    organ.gameObject.SetActive(false);
                    return;
                }

                // 3) 필요하면 나중에 다른 상호작용도 여기다 추가 가능
            }
        }
    }
}
