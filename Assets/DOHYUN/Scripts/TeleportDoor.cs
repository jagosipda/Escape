using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    public Transform targetPoint;

    [Header("이 문을 사용한 뒤 Lab UI를 켤까?")]
    public bool showLabUIAfterTeleport = false;
}
