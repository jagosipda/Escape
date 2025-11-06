using UnityEngine;

public class Key_HandleClick : MonoBehaviour
{
    public LockerDoorController door;

    void OnMouseDown()
    {
        if (door == null) return;

        // 열쇠가 없으면 열리지 않음
        if (KeyManager.Instance != null && !KeyManager.Instance.HasKey())
        {
            Debug.Log("문이 잠겨 있습니다. 열쇠가 필요합니다!");
            return;
        }

        // 열쇠가 있으면 정상 작동
        door.Toggle();
    }

}
