using UnityEngine;
using TMPro;

public class UIPromt : MonoBehaviour
{
    public TextMeshProUGUI promptText;

    void Awake()
    {
        if (promptText != null)
        {
            promptText.enabled = false;
        }
    }

    public void Show(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
            promptText.enabled = true;
        }
    }

    public void Hide()
    {
        if (promptText != null)
        {
            promptText.enabled = false;
        }
    }
}
