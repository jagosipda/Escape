using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class PianoKey : MonoBehaviour
{
    [Header("Note Info")]
    public string noteName = "C4";  // 이 건반의 음 이름

    [Header("Sound")]
    public AudioClip noteClip;          // 이 건반의 소리 (c4, d4, ...)
    public float playLength = 1.5f;     // 몇 초 동안만 재생할지 (길면 늘리기)

    [Header("Press Visual")]
    public float pressDepth = 0.01f;    // 얼마나 아래로 눌릴지
    public float pressDownTime = 0.03f; // 내려가는 데 걸리는 시간
    public float pressUpTime = 0.07f;   // 올라오는 데 걸리는 시간

    private AudioSource audioSource;
    private Vector3 defaultLocalPos;
    private bool isAnimating = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        defaultLocalPos = transform.localPosition;
    }

    private void OnMouseDown()
    {
        Debug.Log($"건반 클릭됨: {noteName}");
        PlayNote();
    }

    public void PlayNote()
    {
        if (audioSource != null && noteClip != null)
        {
            audioSource.clip = noteClip;
            StartCoroutine(PlayShortClip());
        }

        if (!isAnimating)
            StartCoroutine(PressAnimation());

        // ScoreManager에 눌린 음 전달
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnKeyPressed(noteName);
        }
    }

    private System.Collections.IEnumerator PlayShortClip()
    {
        audioSource.time = 0f;
        audioSource.Play();
        yield return new WaitForSeconds(playLength);
        audioSource.Stop();
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        isAnimating = true;
        Vector3 downPos = defaultLocalPos + Vector3.down * pressDepth;

        float t = 0f;
        while (t < pressDownTime)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(defaultLocalPos, downPos, t / pressDownTime);
            yield return null;
        }

        t = 0f;
        while (t < pressUpTime)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(downPos, defaultLocalPos, t / pressUpTime);
            yield return null;
        }

        transform.localPosition = defaultLocalPos;
        isAnimating = false;
    }
}
