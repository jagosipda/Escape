using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections;

public class OrganInventory : MonoBehaviour
{
    public static OrganInventory Instance;

    [Header("UI")]
    public TextMeshProUGUI inventoryText;   // '획득한 장기: ...'
    public TextMeshProUGUI codeText;        // 정답 숫자 표시할 텍스트

    [Header("퍼즐 설정")]
    public int codeDigit = 7;               // 정답 숫자 (지금은 7 고정)

    [Header("사운드 설정")]
    public AudioClip allOrganSound;         // 세 장기 모았을 때 재생할 mp3
    public int repeatCount = 5;             // 몇 번 반복할지
    public float repeatInterval = 0.2f;     // 각 재생 사이 딜레이(초)
    public float soundVolume = 1f;          // 볼륨 (0~1)

    // 내부 상태 ----------------------
    private HashSet<string> organs = new HashSet<string>();
    private bool puzzleSolved = false;
    private readonly string[] requiredOrgans = { "Brain", "Eye", "HEART" };
    private Coroutine playSoundRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PickupOrgan(OrganPickup organ)
    {
        organs.Add(organ.organName);
        RefreshUI();
        CheckPuzzle();
        Debug.Log("장기 획득: " + organ.organName);
    }

    public bool HasOrgan(string organName)
    {
        return organs.Contains(organName);
    }

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

        // brain, Eye, HEART 세 개 모두 있는지 확인
        foreach (var name in requiredOrgans)
        {
            if (!organs.Contains(name))
                return;
        }

        // 여기까지 오면 퍼즐 완료
        puzzleSolved = true;

        if (codeText != null)
        {
            codeText.text = codeDigit.ToString();
        }

        Debug.Log("장기 퍼즐 완료! 코드: " + codeDigit);

        // 사운드 재생
        if (playSoundRoutine != null)
            StopCoroutine(playSoundRoutine);
        playSoundRoutine = StartCoroutine(PlayCodeSoundRoutine());
    }

    private IEnumerator PlayCodeSoundRoutine()
    {
        if (allOrganSound == null)
        {
            Debug.LogWarning("OrganInventory: allOrganSound에 클립이 비어 있음!");
            yield break;
        }

        // 카메라 위치에서 소리가 나게 (PC/VR 둘 다 Camera.main 기준)
        Vector3 pos = Vector3.zero;
        if (Camera.main != null)
            pos = Camera.main.transform.position;

        for (int i = 0; i < repeatCount; i++)
        {
            AudioSource.PlayClipAtPoint(allOrganSound, pos, soundVolume);
            // 클립 길이 + 약간의 간격만큼 기다렸다가 다시 재생
            yield return new WaitForSeconds(allOrganSound.length + repeatInterval);
        }
    }
}
