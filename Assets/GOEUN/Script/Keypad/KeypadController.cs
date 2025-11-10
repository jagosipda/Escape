using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeypadController : MonoBehaviour
{
    [Header("Password Settings")]
    [Tooltip("정답 숫자들 (순서 상관 없음)")]
    public List<int> correctDigits = new List<int> { 1, 4, 6, 7 };

    [Header("Audio")]
    public AudioClip pressSound;      // 버튼 소리
    public AudioClip unlockSound;     // 철컥 소리
    public AudioClip failSound;       // 실패 사운드
    public AudioSource mainAudio;     // 일반 효과음용 (버튼 + 철컥)
    public AudioSource failAudio;     // 실패 전용 (볼륨 낮게 설정)

    [Header("Event")]
    public UnityEvent onUnlocked;

    private List<int> currentInput = new List<int>();

    void Awake()
    {
        // 기본 AudioSource 없으면 자동 추가
        if (!mainAudio)
            mainAudio = gameObject.AddComponent<AudioSource>();
        if (!failAudio)
            failAudio = gameObject.AddComponent<AudioSource>();
    }

    public void PressNumber(int number)
    {
        if (pressSound)
            mainAudio.PlayOneShot(pressSound);

        currentInput.Add(number);
        Debug.Log($"[Keypad] 입력: {string.Join(",", currentInput)}");

        if (currentInput.Count >= 4)
            CheckCode();
    }

    private void CheckCode()
    {
        bool correct = IsSetEqual(currentInput, correctDigits);

        if (correct)
        {
            Debug.Log("[Keypad] 정답! 잠금 해제!");
            if (unlockSound)
                mainAudio.PlayOneShot(unlockSound);
            onUnlocked?.Invoke();
        }
        else
        {
            Debug.Log("[Keypad] 오답! 리셋됩니다.");
            if (failSound)
                failAudio.PlayOneShot(failSound); // 실패 사운드 전용 소스 사용
        }

        currentInput.Clear();
    }

    private bool IsSetEqual(List<int> a, List<int> b)
    {
        if (a.Count != b.Count)
            return false;

        var tempA = new List<int>(a);
        var tempB = new List<int>(b);
        tempA.Sort();
        tempB.Sort();

        for (int i = 0; i < tempA.Count; i++)
        {
            if (tempA[i] != tempB[i])
                return false;
        }
        return true;
    }

    public void ResetInput()
    {
        currentInput.Clear();
        Debug.Log("[Keypad] 입력 리셋됨");
    }
}
