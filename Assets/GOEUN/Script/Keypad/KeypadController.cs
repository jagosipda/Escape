using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeypadController : MonoBehaviour
{
    [Header("Password Settings")]
    [Tooltip("���� ���ڵ� (���� ��� ����)")]
    public List<int> correctDigits = new List<int> { 1, 4, 6, 7 };

    [Header("Audio")]
    public AudioClip pressSound;      // ��ư �Ҹ�
    public AudioClip unlockSound;     // ö�� �Ҹ�
    public AudioClip failSound;       // ���� ����
    public AudioSource mainAudio;     // �Ϲ� ȿ������ (��ư + ö��)
    public AudioSource failAudio;     // ���� ���� (���� ���� ����)

    [Header("Event")]
    public UnityEvent onUnlocked;

    private List<int> currentInput = new List<int>();

    void Awake()
    {
        // �⺻ AudioSource ������ �ڵ� �߰�
        if (!mainAudio)
            mainAudio = gameObject.AddComponent<AudioSource>();
        if (!failAudio)
            failAudio = gameObject.AddComponent<AudioSource>();
    }

    public void PressNumber(int number)
    {
        if (pressSound)
            mainAudio.PlayOneShot(pressSound);

        currentInput.Add(number);
        Debug.Log($"[Keypad] �Է�: {string.Join(",", currentInput)}");

        if (currentInput.Count >= 4)
            CheckCode();
    }

    private void CheckCode()
    {
        bool correct = IsSetEqual(currentInput, correctDigits);

        if (correct)
        {
            Debug.Log("[Keypad] ����! ��� ����!");
            if (unlockSound)
                mainAudio.PlayOneShot(unlockSound);
            onUnlocked?.Invoke();
        }
        else
        {
            Debug.Log("[Keypad] ����! ���µ˴ϴ�.");
            if (failSound)
                failAudio.PlayOneShot(failSound); // ���� ���� ���� �ҽ� ���
        }

        currentInput.Clear();
    }

    private bool IsSetEqual(List<int> a, List<int> b)
    {
        if (a.Count != b.Count)
            return false;

        var tempA = new List<int>(a);
        var tempB = new List<int>(b);
        tempA.Sort();
        tempB.Sort();

        for (int i = 0; i < tempA.Count; i++)
        {
            if (tempA[i] != tempB[i])
                return false;
        }
        return true;
    }

    public void ResetInput()
    {
        currentInput.Clear();
        Debug.Log("[Keypad] �Է� ���µ�");
    }
}
