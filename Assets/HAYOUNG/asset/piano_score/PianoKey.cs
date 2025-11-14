using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class PianoKey : MonoBehaviour, IInteractable
{
    [Header("Note Info")]
    public string noteName = "C4";

    [Header("Sound")]
    public AudioClip noteClip;
    public float playLength = 1.5f;

    [Header("Press Visual")]
    public float pressDepth = 0.01f;
    public float pressDownTime = 0.03f;
    public float pressUpTime = 0.07f;

    [Header("Input Cooldown")]
    public float inputCooldown = 0.1f;   // 같은 건반 입력 최소 간격(초)
    private float lastInputTime = -999f;

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
        Debug.Log($"건반 클릭됨 (OnMouseDown): {noteName}");
        PlayNote();
    }

    public void Interact()
    {
        Debug.Log($"건반 인터랙트됨 (IInteractable): {noteName}");
        PlayNote();
    }

    public void PlayNote()
    {
        // ★ 같은 프레임/아주 짧은 시간에 중복 호출되면 무시
        if (Time.time - lastInputTime < inputCooldown)
            return;
        lastInputTime = Time.time;

        if (audioSource != null && noteClip != null)
        {
            audioSource.clip = noteClip;
            StartCoroutine(PlayShortClip());
        }

        if (!isAnimating)
            StartCoroutine(PressAnimation());

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
