using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;   // ¾îµð¼­µç Á¢±Ù °¡´É (½Ì±ÛÅæ)
    private bool hasKey = false;

    void Awake()
    {
        Instance = this;
    }

    public void ObtainKey()
    {
        hasKey = true;
        Debug.Log("¿­¼è¸¦ È¹µæÇß½À´Ï´Ù!");
    }

    public bool HasKey()
    {
        return hasKey;
    }
}
