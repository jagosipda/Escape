using UnityEngine;

public class VRClickInteract : MonoBehaviour
{
    public Transform handRayOrigin;   // 컨트롤러에서 Ray 쏠 위치
    public float interactDistance = 5f;
    public LayerMask interactMask;

    void Update()
    {
        // Oculus / Pico 계열 트리거 입력
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Ray ray = new Ray(handRayOrigin.position, handRayOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
            {
                // Note
                Note note = hit.collider.GetComponent<Note>();
                if (note != null)
                {
                    FindObjectOfType<NoteUI>().Show(note.noteText);
                    return;
                }

                // Organ
                OrganPickup organ = hit.collider.GetComponent<OrganPickup>();
                if (organ != null)
                {
                    OrganInventory.Instance.PickupOrgan(organ);
                    organ.gameObject.SetActive(false);
                    return;
                }
            }
        }
    }
}
