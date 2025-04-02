using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverColor : MonoBehaviour
{
    public Button button;
    public Color wantedColor;
    private Color originalColor;
    private ColorBlock cb;

    void Start()
    {
        cb = button.colors;
        originalColor = cb.selectedColor;
    }

    public void changeWhenHover()
    {
        cb.selectedColor = wantedColor;
        button.colors = cb;
    }

    public void changeWhenLeaves()
    {
        cb.selectedColor = originalColor;
        button.colors = cb;
    }
}
