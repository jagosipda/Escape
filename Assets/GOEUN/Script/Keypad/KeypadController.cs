using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;   // ⬅ 추가

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

    [Header("Ending Scene")]
    public bool loadEndingScene = true;          // 정답 시 엔딩을 불러올지 여부
    public string endingSceneName = "DemoGrassFlowers"; // 엔딩 씬 이름
    public float endingLoadDelay = 1.5f;         // 철컥 소리 듣고 넘어가게 딜레이

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

            // ✅ 엔딩 씬 로드
            if (loadEndingScene && !string.IsNullOrEmpty(endingSceneName))
            {
                StartCoroutine(LoadEndingAfterDelay());
            }
        }
        else
        {
            Debug.Log("[Keypad] 실패! 다시 시도해주세요.");
            if (failSound)
                failAudio.PlayOneShot(failSound); // 실패 소리 재생
        }

        currentInput.Clear();
    }

    private System.Collections.IEnumerator LoadEndingAfterDelay()
    {
        // unlockSound 길이만큼 기다리고 싶으면:
        float delay = endingLoadDelay;

        if (unlockSound != null)
        {
            // 굳이 인스펙터에서 안 건드리고 싶으면 이 줄만 써도 됨
            delay = unlockSound.length;
        }

        yield return new WaitForSeconds(delay);

        Debug.Log("[Keypad] 엔딩 씬 로드: " + endingSceneName);
        SceneManager.LoadScene(endingSceneName);
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
