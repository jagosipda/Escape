using UnityEngine;

public class MusicBoxHandle : MonoBehaviour
{
    public float rotateSpeed = 180f;

    void Update()
    {
        // X축 기준 회전
        transform.Rotate(rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
    }
}
