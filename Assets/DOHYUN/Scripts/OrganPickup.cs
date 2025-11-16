using UnityEngine;

public class OrganPickup : MonoBehaviour
{
    [Tooltip("Brain / Eye / HEART 중 하나를 정확히 입력")]
    public string organName;          // 장기 이름 (퍼즐용 키값)

    [Tooltip("인벤토리에 표시할 이름 (예: 뇌, 눈, 심장)")]
    public string displayName;        // UI에 보여줄 이름
}
