using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EndingQuitOnEsc : MonoBehaviour
{
    void Start()
    {
        // 혹시 이전 씬에서 일시정지 되어 넘어왔을 수도 있으니까 안전용
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            // 에디터에서 실행 중일 때는 Play 모드만 종료
            EditorApplication.isPlaying = false;
#else
            // 빌드된 실제 게임에서는 프로그램 완전히 종료
            Application.Quit();
#endif
        }
    }
}
