using UnityEngine;

public class FootstepTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;   // 발자국 소리 재생할 AudioSource (직접 연결)
    public AudioClip footstepSound;   // 발자국 효과음 (짧은 .wav / .mp3)
    [Range(0f, 1f)]
    public float volume = 0.8f;       // 볼륨 조절

    private bool hasPlayed = false;   // 한 번만 재생할 경우 true로 고정

    void Start()
    {
        // 안전장치: audioSource가 비어 있으면 자동 추가
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;  // 3D 소리로 공간감 있게
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return; // 이미 재생했으면 무시

        if (other.CompareTag("Player"))
        {
            if (footstepSound != null && audioSource != null)
                audioSource.PlayOneShot(footstepSound, volume);

            hasPlayed = true; // 한 번만 재생
        }
    }
}