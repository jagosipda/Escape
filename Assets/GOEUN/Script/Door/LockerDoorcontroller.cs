using System.Collections;
using UnityEngine;

public class LockerDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;               // 회전할 Transform (보통 자기 자신)

    [Header("Open Angles (°)")]
    public float openAngleX = 0f;        // X축 회전 각도
    public float openAngleY = 0f;        // Y축 회전 각도
    public float openAngleZ = 0f;        // Z축 회전 각도

    public float duration = 0.8f;        // 여닫는 속도
    public bool isOpen = false;          // 현재 상태 저장

    [Header("Sound (Optional)")]
    public AudioSource audioSource;
    public AudioClip openSfx;
    public AudioClip closeSfx;

    private Quaternion closedRot;
    private Quaternion openRot;
    private Coroutine animRoutine;

    void Start()
    {
        if (door == null)
            door = transform;

        // 닫힌 상태 기준 회전 저장
        closedRot = door.localRotation;

        // 세 축 회전 반영 (기본 0,0,0 → 지정 값만큼 회전)
        openRot = closedRot * Quaternion.Euler(openAngleX, openAngleY, openAngleZ);
    }

    public void Toggle()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        // 열린 상태면 닫고, 닫힌 상태면 열기
        animRoutine = StartCoroutine(RotateDoor(isOpen ? closedRot : openRot));
        isOpen = !isOpen;

        // 사운드 재생
        if (audioSource)
        {
            AudioClip clip = isOpen ? openSfx : closeSfx;
            if (clip) audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator RotateDoor(Quaternion targetRot)
    {
        Quaternion startRot = door.localRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, duration);
            door.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        door.localRotation = targetRot;
        animRoutine = null;
    }
}
