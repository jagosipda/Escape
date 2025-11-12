using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("정답 악보 (노트 이름 순서)")]
    public string[] targetSequence = { "G4", "G4", "A4", "A4", "G4", "G4", "E4",
                                       "G4", "G4", "E4", "E4", "D4" };

    [Header("정답 후 자동으로 눌릴 건반들")]
    public string[] autoPlayKeys = { "G4", "G4", "A4", "A4", "G4", "G4", "E4",
                                       "G4", "E4", "D4", "E4", "C4" }; // 눌릴 건반 이름들

    [Header("각 건반의 눌림 지연시간 (초 단위, 위와 순서 일치)")]
    public float[] autoPlayDelays = { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1.0f,
                                        0.5f, 0.5f, 0.5f, 0.5f, 2.0f }; // 각각 다른 시간 지정

    [Header("자동 연출 시작 전 전체 대기 시간 (초 단위)")]
    public float delayBeforeAutoPlay = 2.0f;

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnKeyPressed(string noteName)
    {
        Debug.Log($"입력됨: {noteName}, 기대값: {targetSequence[currentIndex]}");

        if (noteName == targetSequence[currentIndex])
        {
            currentIndex++;

            if (currentIndex >= targetSequence.Length)
            {
                Debug.Log("악보 완성! 자동 눌림 준비 중...");
                currentIndex = 0;

                // 자동눌림 전체 대기 후 실행
                StartCoroutine(StartAutoPlayAfterDelay());
            }
        }
        else
        {
            Debug.Log("틀림! 처음부터 다시.");
            currentIndex = 0;

            if (noteName == targetSequence[0])
            {
                currentIndex = 1;
            }
        }
    }

    private IEnumerator StartAutoPlayAfterDelay()
    {
        // 정답 직후 잠시 기다리기 (연출용)
        Debug.Log($"자동 눌림까지 {delayBeforeAutoPlay}초 대기...");
        yield return new WaitForSeconds(delayBeforeAutoPlay);

        // 자동눌림 시작
        StartCoroutine(AutoPressSelectedKeys());
    }

    private IEnumerator AutoPressSelectedKeys()
    {
        for (int i = 0; i < autoPlayKeys.Length; i++)
        {
            yield return new WaitForSeconds(autoPlayDelays[i]);

            string keyName = autoPlayKeys[i];
            PianoKey[] allKeys = FindObjectsOfType<PianoKey>();

            foreach (PianoKey key in allKeys)
            {
                if (key.noteName == keyName)
                {
                    key.PlayNote();
                    Debug.Log($"{keyName} 눌림 (지연 {autoPlayDelays[i]}초)");
                    break;
                }
            }
        }

        Debug.Log("자동 눌림 연출 완료!");
    }
}