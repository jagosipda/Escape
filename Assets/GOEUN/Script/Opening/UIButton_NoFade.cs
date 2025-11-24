using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButton_NoFade : MonoBehaviour, IInteractable
{
    public enum ButtonType { Play, Exit }
    public ButtonType type;

    public string nextSceneName = "School";

    public void Interact()
    {
        if (type == ButtonType.Play)
        {
            Debug.Log("Play 버튼 클릭됨 → 씬 이동");
            SceneManager.LoadScene(nextSceneName);
        }
        else if (type == ButtonType.Exit)
        {
            Debug.Log("Exit 버튼 클릭됨 → 게임 종료");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
