using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeypadController : MonoBehaviour
{
    [Header("Password Settings")]
    [Tooltip("정답 숫자 조합 (순서는 상관 없음)")]
    public List<int> correctDigits = new List<int> { 1, 4, 6, 7 };

    [Header("Audio")]
    public AudioClip pressSound;      // 버튼 소리
    public AudioClip unlockSound;     // 철컥 소리
    public AudioClip failSound;       // 실패 소리
    public AudioSource mainAudio;     // 메인 오디오 소스 (버튼 + 철컥)
    public AudioSource failAudio;     // 실패 오디오 소스 (실패 소리)

    [Header("Event")]
    public UnityEvent onUnlocked;

    private List<int> currentInput = new List<int>();

    void Awake()
    {
        // 기본 AudioSource 컴포넌트 할당
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
            Debug.Log("[Keypad] 성공! 문이 열렸습니다!");
            if (unlockSound)
                mainAudio.PlayOneShot(unlockSound);
            onUnlocked?.Invoke();
        }
        else
        {
            Debug.Log("[Keypad] 실패! 다시 시도해주세요.");
            if (failSound)
                failAudio.PlayOneShot(failSound); // 실패 소리 재생
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
        Debug.Log("[Keypad] 입력 초기화");
    }
}
