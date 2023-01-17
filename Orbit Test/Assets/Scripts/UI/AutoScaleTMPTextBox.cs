using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
public class AutoScaleTMPTextBox : MonoBehaviour
{
    public float LineSizeInPx = 16;
    public void Start()
    {
        // Damn this was made by Github Autopilot by it just looking at the name of the file

        var text = GetComponent<TextMeshProUGUI>();
        var rect = GetComponent<RectTransform>();
        var lines = text.text.Split('\n');
        var lineCount = lines.Length;
        var lineHeight = text.fontSize;
        var lineWidth = text.preferredWidth;
        var newHeight = lineCount * lineHeight;
        var newWidth = lineWidth;
        var newSize = new Vector2(newWidth, newHeight);
        rect.sizeDelta = newSize;
    }
}
