using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;                 // 플레이어 카메라
    public float interactDistance = 5f;
    public KeyCode interactKey = KeyCode.E;

    public NoteUI noteUI;              // 노트 UI

    void Start()
    {
        // 혹시 인스펙터에서 안 넣었으면 자동으로 찾아보기 (보조용)
        if (cam == null)
        {
            cam = GetComponentInChildren<Camera>();
            if (cam == null)
                Debug.LogError("PlayerInteract: 카메라를 찾을 수 없음!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // 화면 정중앙에서 Ray 쏘기
            Ray ray = cam.ScreenPointToRay(
                new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
            );
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // 1) 노트인지 먼저 확인
                Note note = hit.collider.GetComponent<Note>();
                if (note != null)
                {
                    if (noteUI != null)
                    {
                        noteUI.Show(note.noteText);
                    }
                    return; // 노트면 여기서 끝
                }

                // 2) 장기인지 확인
                OrganPickup organ = hit.collider.GetComponent<OrganPickup>();
                if (organ != null)
                {
                    if (OrganInventory.Instance != null)
                    {
                        OrganInventory.Instance.PickupOrgan(organ);
                        Debug.Log("장기 획득: " + organ.organName);
                    }

                    // 월드에서 장기 숨기기
                    organ.gameObject.SetActive(false);
                    return;
                }

                // 3) 그 외 다른 것들… 필요하면 나중에 추가
            }
        }
    }
}
