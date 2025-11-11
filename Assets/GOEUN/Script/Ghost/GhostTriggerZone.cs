using System.Collections;
using UnityEngine;

public class GhostTriggerZone : MonoBehaviour
{
    [Header("References")]
    public Light[] lightsToControl;     // 복도의 형광등 여러 개
    public GameObject ghost;            // 귀신 오브젝트 (기본 비활성화)
    public AudioSource audioSource;     // 사용할 오디오 소스 (직접 연결)
    public AudioClip flickerSound;      // 깜빡일 때 소리 (선택)
    public AudioClip scareSound;        // 귀신 등장 소리 (선택)

    [Header("Timing Settings")]
    public float ghostVisibleTime = 1.5f;  // 귀신이 보이는 시간(초)
    public float lightIntensity = 5f;      // 형광등 켜졌을 때 밝기
    public int flickerCount = 3;           // 깜빡이는 횟수
    public float flickerDelay = 0.2f;      // 깜빡임 간격 (초)

    private bool hasTriggered = false;

    void Start()
    {
        // audioSource가 비어있으면 자동 추가 (예외 대비용)
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }

        // 시작 시 불 꺼짐 + 귀신 꺼짐
        if (ghost != null)
            ghost.SetActive(false);

        foreach (Light light in lightsToControl)
        {
            if (light != null)
                light.intensity = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(GhostSequence());
        }
    }

    IEnumerator GhostSequence()
    {
        // 귀신 등장 (처음 불과 함께)
        if (ghost != null)
            ghost.SetActive(true);

        if (scareSound != null)
            audioSource.PlayOneShot(scareSound);

        // 불이 켜졌다 꺼졌다 반복
        for (int i = 0; i < flickerCount; i++)
        {
            // 켜기
            foreach (Light light in lightsToControl)
            {
                if (light != null)
                    light.intensity = lightIntensity;
            }

            if (flickerSound != null)
                audioSource.PlayOneShot(flickerSound);

            yield return new WaitForSeconds(flickerDelay);

            // 끄기
            foreach (Light light in lightsToControl)
            {
                if (light != null)
                    light.intensity = 0f;
            }

            yield return new WaitForSeconds(flickerDelay);
        }

        // 마지막에 불 켜진 상태에서 잠깐 귀신 유지
        foreach (Light light in lightsToControl)
        {
            if (light != null)
                light.intensity = lightIntensity;
        }

        yield return new WaitForSeconds(ghostVisibleTime);

        // 귀신 사라지고 불 꺼짐
        if (ghost != null)
            ghost.SetActive(false);

        foreach (Light light in lightsToControl)
        {
            if (light != null)
                light.intensity = 0f;
        }
    }
}
