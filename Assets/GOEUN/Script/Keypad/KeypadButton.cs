using UnityEngine;

public class KeypadButton : MonoBehaviour, IInteractable
{
    [Tooltip("이 버튼의 숫자 (예: 1~9)")]
    public int buttonNumber = 1;

    private KeypadController controller;

    void Start()
    {
        controller = GetComponentInParent<KeypadController>();
        if (controller == null)
            Debug.LogWarning($"{name}: KeypadController가 부모에 없습니다!");
    }

    // 중앙점 클릭 또는 VR 트리거로 호출됨
    public void Interact()
    {
        controller?.PressNumber(buttonNumber);
    }
}
