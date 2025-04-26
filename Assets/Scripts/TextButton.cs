using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextButton : Button
{
    public TextMeshProUGUI textObject = null;

    public string currentText => textObject == null ? null : textObject.text;

    protected override void Awake()
    {
        base.Awake();

        textObject = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string argText)
    {
        if (textObject == null)
        {
            return;
        }

        textObject.text = argText;
    }

    public void SetOnClicked(UnityAction argCallback)
    {
        onClick.RemoveAllListeners();
        AddOnClicked(argCallback);
    }
    
    public void AddOnClicked(UnityAction argCallback)
    {
        onClick.AddListener(argCallback);
    }
}
