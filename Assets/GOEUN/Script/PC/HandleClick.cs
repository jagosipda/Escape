using UnityEngine;

public class HandleClick : MonoBehaviour
{
    public LockerDoorController door;

    void OnMouseDown()
    {
        if (door != null)
            door.Toggle();
    }
}
