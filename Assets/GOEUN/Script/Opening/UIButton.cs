using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIButton : MonoBehaviour, IInteractable
{
    public enum ButtonType { Play, Exit }
    public ButtonType type;

    public string nextSceneName = "School";

    [Header("Fade Settings")]
    public Image fadePanel;       // Canvas 안의 검은 Image
    public float fadeSpeed = 1.5f;

    // IInteractable 인터페이스
    public void Interact()
    {
        if (type == ButtonType.Play)
        {
            StartCoroutine(FadeOutAndLoad());
        }
        else if (type == ButtonType.Exit)
        {
            StartCoroutine(FadeOutAndQuit());
        }
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
        fadePanel.gameObject.SetActive(true);

        Color c = fadePanel.color;
        c.a = 0;
        fadePanel.color = c;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = c;
            yield return null;
        }
    }
}
