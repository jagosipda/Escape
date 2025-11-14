using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class OrganInventory : MonoBehaviour
{
    public static OrganInventory Instance;

    [Header("UI")]
    public TextMeshProUGUI inventoryText;   // '획득한 장기: ...'
    public TextMeshProUGUI codeText;        // 정답 숫자 표시할 텍스트

    [Header("퍼즐 설정")]
    public int codeDigit = 9;               // 정답 숫자 (지금은 9 고정)

    // 플레이어가 획득한 장기 이름들 (brain, Eye, HEART)
    private HashSet<string> organs = new HashSet<string>();

    // 퍼즐 완료 여부 (한 번만 9를 찍기 위해)
    private bool puzzleSolved = false;

    // 필수 장기 목록
    private readonly string[] requiredOrgans = { "Brain", "Eye", "HEART" };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PickupOrgan(OrganPickup organ)
    {
        organs.Add(organ.organName);
        RefreshUI();
        CheckPuzzle();
    }

    public bool HasOrgan(string organName)
    {
        return organs.Contains(organName);
    }

    // 지금은 안 써도 상관 없지만, 남겨둬도 에러는 안 생김
    public bool UseOrgan(string organName)
    {
        if (organs.Remove(organName))
        {
            RefreshUI();
            return true;
        }
        return false;
    }

    private void RefreshUI()
    {
        if (inventoryText == null) return;

        if (organs.Count == 0)
        {
            inventoryText.text = "획득한 장기: 없음";
            return;
        }

        string list = string.Join(", ", organs.Select(o => o));
        inventoryText.text = "획득한 장기: " + list;
    }

    private void CheckPuzzle()
    {
        if (puzzleSolved) return;

        // brain, Eye, HEART 세 개를 모두 가지고 있는지 확인
        foreach (var name in requiredOrgans)
        {
            if (!organs.Contains(name))
                return; // 아직 하나라도 없음 → 그대로
        }

        // 여기까지 오면 세 개 모두 획득한 상태!
        puzzleSolved = true;

        if (codeText != null)
        {
            codeText.text = codeDigit.ToString();   // 예: "9"
        }

        Debug.Log("장기 퍼즐 완료! 코드: " + codeDigit);
    }
}
