using UnityEngine;
using UnityEngine.Events;

public class ClockPuzzleManager : MonoBehaviour
{
    [Header("Clock Hands (Pivot 기준)")]
    public Transform hourPivot;
    public Transform minutePivot;

    [Header("Target Time")]
    [Range(0, 11)] public int targetHour = 3;
    [Range(0, 59)] public int targetMinute = 0;

    [Header("Tolerance (deg)")]
    public float minuteToleranceDeg = 2f;
    public float hourToleranceDeg = 2f;

    [Header("Cover Drop (동시에 떨어질 Rigidbody들)")]
    public Rigidbody[] dropObjects; // 여러 오브젝트 한 번에 떨어짐
    public float dropForce = 1f;

    public AudioSource sfx;
    public AudioClip solvedSfx;
    public UnityEvent onSolved;

    [Header("Rotation Offset (모델 보정용)")]
    [Tooltip("시계의 12시가 Unity 기준 Y각도로 몇 도인지 직접 넣으세요.")]
    public float minuteOffset = 0f;
    public float hourOffset = 0f;

    private bool solved = false;

    void Update()
    {
        if (solved || !hourPivot || !minutePivot)
            return;

        // 시계방향 증가 기준 회전값 변환
        float minDeg = Mathf.Repeat(360f - minutePivot.localEulerAngles.y - minuteOffset, 360f);
        float hourDeg = Mathf.Repeat(360f - hourPivot.localEulerAngles.y - hourOffset, 360f);

        // 목표 각도 계산
        float targetMinDeg = targetMinute * 6f;
        float targetHourDeg = (targetHour % 12) * 30f + targetMinute * 0.5f;

        bool minuteOK = Mathf.Abs(Mathf.DeltaAngle(minDeg, targetMinDeg)) <= minuteToleranceDeg;
        bool hourOK = Mathf.Abs(Mathf.DeltaAngle(hourDeg, targetHourDeg)) <= hourToleranceDeg;

        if (minuteOK && hourOK)
            Solve();

        // 디버그용
        Debug.Log($"시침:{hourDeg:F1}° / 분침:{minDeg:F1}° (목표 {targetHourDeg:F1}°, {targetMinDeg:F1}°)");
    }

    void Solve()
    {
        solved = true;
        Debug.Log("Clock Puzzle Solved!");

        // 여러 개 오브젝트 동시에 떨어지게
        if (dropObjects != null && dropObjects.Length > 0)
        {
            foreach (var rb in dropObjects)
            {
                if (rb == null) continue;

                Debug.Log($"Drop Start: {rb.name}");
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogWarning("No dropObjects assigned!");
        }

        if (sfx && solvedSfx)
            sfx.PlayOneShot(solvedSfx);

        onSolved?.Invoke();
    }
}
