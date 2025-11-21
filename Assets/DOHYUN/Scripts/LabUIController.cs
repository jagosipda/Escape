using UnityEngine;

public class LabUIController : MonoBehaviour
{
    public static LabUIController Instance;

    [Header("Lab에서만 보일 UI 루트")]
    public GameObject labUIRoot;  // LabUI 오브젝트를 넣어줄 것

    private void Awake()
    {
        if (labUIRoot != null)
        labUIRoot.SetActive(false);   // 시작할 땐 끈 상태

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public void ShowLabUI(bool show)
    {
        if (labUIRoot != null)
            labUIRoot.SetActive(show);
    }
}
