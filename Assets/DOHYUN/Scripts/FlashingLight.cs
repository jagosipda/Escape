using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    public Light targetLight;          // 깜빡일 라이트(비워두면 자동으로 자기 Light 씀)
    public float minIntensity = 0f;   // 최소 밝기
    public float maxIntensity = 3f;   // 최대 밝기

    public float minInterval = 0.05f; // 가장 빠른 깜빡임 간격
    public float maxInterval = 0.2f;  // 가장 느린 깜빡임 간격

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        while (true)
        {
            // 랜덤 밝기
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);

            // 랜덤 대기 시간
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
        }
    }
}
