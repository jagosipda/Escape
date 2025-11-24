using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIButton : MonoBehaviour, IInteractable
{
    public enum ButtonType { Play, Exit }
    public ButtonType type;

    public string nextSceneName = "School";

    [Header("Fade Panels")]
    public Image fadePanel_PC;    // Overlay Canvas
    public Image fadePanel_VR;    // Screen Space Camera Canvas

    [Header("Fade Settings")]
    public float fadeSpeed = 1.5f;

    // 자동으로 PC/VR에 맞는 페이드 패널 선택
    Image GetFadePanel()
    {
        // VR 기기 활성화 여부로 판단
        if (UnityEngine.XR.XRSettings.isDeviceActive && fadePanel_VR != null)
            return fadePanel_VR;

        return fadePanel_PC;
    }

    public void Interact()
    {
        if (type == ButtonType.Play)
            StartCoroutine(FadeOutAndLoad());
        else if (type == ButtonType.Exit)
            StartCoroutine(FadeOutAndQuit());
    }

    IEnumerator FadeOutAndLoad()
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeOutAndQuit()
    {
        yield return StartCoroutine(FadeOut());

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator FadeOut()
    {
        Image panel = GetFadePanel();

        if (panel == null)
        {
            Debug.LogWarning("Fade Panel이 설정되지 않음.");
            yield break;
        }

        panel.gameObject.SetActive(true);

        Color c = panel.color;
        c.a = 0;
        panel.color = c;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime * fadeSpeed;
            panel.color = c;
            yield return null;
        }
    }
}
