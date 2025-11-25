using UnityEngine;
using UnityEngine.Events; // UnityEvent 사용을 위해 추가

// IInteractable 인터페이스를 구현하여 플레이어 스크립트에서 호출 가능하게 함
public class BookPullInteractable : MonoBehaviour, IInteractable
{
    [Header("Book Movement Settings")]
    public float pullDistance = 0.2f;       // 책이 당겨지는 거리 (예: 20cm)
    public float pullSpeed = 5f;            // 책이 목표 위치로 이동하는 속도
    public Vector3 pullDirection = Vector3.forward; // 당겨지는 방향 (로컬 Z축을 사용하면 편리함)

    [Header("State and Events")]
    public bool isPulled = false;           // 현재 책이 당겨진 상태인지 여부
    public UnityEvent OnBookPulledOut;      // 책이 당겨졌을 때 실행될 이벤트 (퍼즐 연동 등)
    public UnityEvent OnBookPushedBack;     // 책이 밀려들어갔을 때 실행될 이벤트

    private Vector3 initialLocalPosition;   // 책의 초기 로컬 위치
    private Vector3 targetLocalPosition;    // 당겨진 후의 최종 로컬 위치

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pullSound;             // 당길 때 재생할 사운드
    public AudioClip pushSound;             // 밀 때 재생할 사운드

    void Awake()
    {
        // 씬 시작 시 초기 위치와 목표 위치를 계산하여 저장합니다.
        initialLocalPosition = transform.localPosition;

        // 목표 위치는 초기 위치에서 설정된 방향과 거리만큼 이동한 지점입니다.
        targetLocalPosition = initialLocalPosition + (pullDirection.normalized * pullDistance);

        // AudioSource 컴포넌트가 없으면 추가합니다.
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 현재 isPulled 상태에 따라 목표 위치를 결정합니다.
        Vector3 currentTarget = isPulled ? targetLocalPosition : initialLocalPosition;

        // 현재 위치를 목표 위치로 부드럽게(Lerp) 이동시킵니다.
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            currentTarget,
            Time.deltaTime * pullSpeed
        );
    }

    // IInteractable 인터페이스의 핵심 함수
    public void Interact()
    {
        // 상태 토글: 당겨짐 <-> 밀림
        isPulled = !isPulled;

        if (isPulled)
        {
            // 당겨지는 경우
            Debug.Log($"{gameObject.name} pulled out.");
            if (pullSound && audioSource) audioSource.PlayOneShot(pullSound);
            OnBookPulledOut.Invoke(); // 이벤트 발생
        }
        else
        {
            // 밀려들어가는 경우
            Debug.Log($"{gameObject.name} pushed back in.");
            if (pushSound && audioSource) audioSource.PlayOneShot(pushSound);
            OnBookPushedBack.Invoke(); // 이벤트 발생
        }
    }
}