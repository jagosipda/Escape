using UnityEngine;
using UnityEngine.Events;

public class InteractRelay : MonoBehaviour, IInteractable
{
    // 이 오브젝트가 클릭(상호작용) 되었을 때 호출할 이벤트
    public UnityEvent onInteract;

    public void Interact()
    {
        onInteract?.Invoke();
    }
}
