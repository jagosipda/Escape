using UnityEngine;

public class WhisperTrigger : MonoBehaviour
{
    public AudioSource whisperAudio;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!whisperAudio.isPlaying)
                whisperAudio.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            whisperAudio.Stop();
        }
    }
}
