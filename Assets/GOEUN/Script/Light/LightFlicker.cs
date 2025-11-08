using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light targetLight;
    public float minIntensity = 1f;
    public float maxIntensity = 3f;
    public float flickerSpeed = 0.05f;

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time / flickerSpeed, 0.0f);
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
