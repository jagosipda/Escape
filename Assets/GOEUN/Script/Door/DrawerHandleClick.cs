using UnityEngine;

public class DrawerHandleClick : MonoBehaviour, IInteractable
{
    [Header("Drawer Reference")]
    public DrawerController drawer;

    public void Interact()
    {
        if (drawer == null)
        {
            Debug.LogWarning($"{name}: DrawerController가 연결되지 않았습니다!");
            return;
        }

        drawer.ToggleDrawer();
        Debug.Log("서랍을 여닫습니다.");
    }
}
