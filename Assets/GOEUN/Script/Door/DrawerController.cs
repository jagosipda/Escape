using UnityEngine;
using System.Collections;

/// <summary>
/// 서랍 여닫기 제어 (위치 이동 기반)
/// Inspector에서 이동 방향, 거리, 속도, 사운드 설정 가능.
/// </summary>
public class DrawerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("서랍이 앞으로 나올 방향 (예: (0,0,1))")]
    public Vector3 moveDirection = Vector3.forward;

    [Tooltip("서랍이 이동할 거리 (단위: 미터)")]
    public float moveDistance = 0.3f;

    [Tooltip("서랍 이동 속도 (m/s)")]
    public float moveSpeed = 1.5f;

    [Header("Audio Settings")]
    [Tooltip("서랍이 열릴 때 재생할 효과음")]
    public AudioClip openSound;

    [Tooltip("서랍이 닫힐 때 재생할 효과음")]
    public AudioClip closeSound;

    [Tooltip("효과음을 재생할 AudioSource (비워두면 자동 생성)")]
    public AudioSource audioSource;

    private bool isOpen = false;
    private bool isMoving = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + moveDirection.normalized * moveDistance;

        // AudioSource 자동 연결
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void ToggleDrawer()
    {
        if (!isMoving)
            StartCoroutine(MoveDrawer(isOpen ? openPosition : closedPosition, isOpen ? closedPosition : openPosition));
    }

    private IEnumerator MoveDrawer(Vector3 from, Vector3 to)
    {
        isMoving = true;

        // 효과음 재생
        if (audioSource != null)
        {
            if (!isOpen && openSound != null)
                audioSource.PlayOneShot(openSound);
            else if (isOpen && closeSound != null)
                audioSource.PlayOneShot(closeSound);
        }

        float elapsed = 0f;
        float duration = Vector3.Distance(from, to) / moveSpeed;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = to;
        isOpen = !isOpen;
        isMoving = false;
    }
}
