using System.Collections;
using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    public float duration = 2f; // 페이드 시간 (초)

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f; // 처음에는 완전 암전
    }

    private IEnumerator Start()
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            canvasGroup.alpha = 1f - normalized;   // 1 → 0 으로 감소
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // 다 밝아지면 패널 끄기(선택)
    }
}
