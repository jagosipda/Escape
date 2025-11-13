using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;                 // 플레이어 카메라
    public float interactDistance = 5f;
    public KeyCode interactKey = KeyCode.E;

    public NoteUI noteUI;              // NoteUI 스크립트

    void Start()
    {
        // 혹시 인스펙터에서 안 넣었으면 자동으로 찾아보기
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
            Debug.Log("E 눌림!");

            if (cam == null)
            {
                Debug.LogError("cam 이 비어 있음");
                return;
            }

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green, 1f);

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                Note note = hit.collider.GetComponent<Note>();
                if (note != null)
                {
                    Debug.Log("Found Note! 내용: " + note.noteText);
                    if (noteUI != null)
                    {
                        noteUI.Show(note.noteText);
                    }
                    else
                    {
                        Debug.LogError("noteUI 가 비어 있음!");
                    }
                }
                else
                {
                    Debug.Log("Hit 했지만 Note 컴포넌트 없음");
                }
            }
            else
            {
                Debug.Log("Raycast NO HIT");
            }
        }
    }
}
