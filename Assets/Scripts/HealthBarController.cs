using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] 
    private Color fullHPFillColor;
    
    [SerializeField] 
    private Color noHPFillColor;

    [SerializeField] 
    private Transform fillBarTransform = null;

    [SerializeField] 
    private SpriteRenderer fillBarSpriteRenderer = null;
    
    public void SetHealthBarActive(bool argIsActive)
    {
        gameObject.SetActive(argIsActive);
    }

    public void SetHealthBarFill(float argHealthPercent)
    {
        fillBarTransform.localScale = new Vector3(argHealthPercent, 1, 1);
        fillBarTransform.localPosition = new Vector3((argHealthPercent / 2) - .5f, 0, 0);
        SetFillColor(argHealthPercent);
    }

    private void SetFillColor(float argHealthPercent)
    {
        float easedLerpValue = GetEasedLerp(argHealthPercent);
        
        Color newColor = fillBarSpriteRenderer.color;
        
        newColor.r = Mathf.Lerp(noHPFillColor.r, fullHPFillColor.r, easedLerpValue);
        newColor.b = Mathf.Lerp(noHPFillColor.b, fullHPFillColor.b, easedLerpValue);
        newColor.g = Mathf.Lerp(noHPFillColor.g, fullHPFillColor.g, easedLerpValue);
        
        newColor.a = Mathf.Lerp(noHPFillColor.a, fullHPFillColor.a, easedLerpValue);

        fillBarSpriteRenderer.color = newColor;
    }

    private float GetEasedLerp(float argHealthPercent)
    {
        //https://easings.net/#easeInOutCubic
        
        if (argHealthPercent < .5f)
        {
            return 4 * Mathf.Pow(argHealthPercent, 3);
        }
        else
        {
            return 1 - Mathf.Pow(-2 * argHealthPercent + 2, 3) / 2;
        }
    }
}
