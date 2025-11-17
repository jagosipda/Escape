using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Unity.XR.Oculus;

public class VR_ControlsHUD : MonoBehaviour
{
    [Header("이미지 참조")]
    public Image arrowUp;
    public Image arrowDown;
    public Image arrowLeft;
    public Image arrowRight;
    public Image clickIcon;

    [Header("색상")]
    public Color normalColor = Color.white;
    public Color activeColor = Color.green;

    [Header("PC 클릭 키(선택)")]
    public KeyCode extraClickKey = KeyCode.None; // 필요하면 여기에 E 같은 키 넣어도 됨

    void Update()
    {
        // =========================
        // 1) PC 키보드 방향 입력
        // =========================
        bool forward_pc = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool back_pc    = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool left_pc    = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool right_pc   = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        // =========================
        // 2) PC 클릭 입력
        // =========================
        bool click_pc = Input.GetMouseButton(0);
        if (extraClickKey != KeyCode.None)
        {
            click_pc |= Input.GetKey(extraClickKey);
        }

        // =========================
        // 3) VR 스틱 / 트리거 입력
        // =========================
        bool forward_vr = false;
        bool back_vr    = false;
        bool left_vr    = false;
        bool right_vr   = false;
        bool click_vr   = false;

        if (XRSettings.isDeviceActive)
        {
            // 왼손 스틱 기준 (PrimaryThumbstick)
            Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            forward_vr = stick.y >  0.3f;
            back_vr    = stick.y < -0.3f;
            right_vr   = stick.x >  0.3f;
            left_vr    = stick.x < -0.3f;

            // 양손 트리거 둘 중 하나라도 누르면 클릭
            click_vr |= OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
            click_vr |= OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
        }

        // =========================
        // 4) PC + VR OR 결합 → HUD 표시
        // =========================
        SetActive(arrowUp,    forward_pc || forward_vr);
        SetActive(arrowDown,  back_pc    || back_vr);
        SetActive(arrowLeft,  left_pc    || left_vr);
        SetActive(arrowRight, right_pc   || right_vr);
        SetActive(clickIcon,  click_pc   || click_vr);
    }

    void SetActive(Image img, bool active)
    {
        if (img == null) return;
        img.color = active ? activeColor : normalColor;
    }
}
