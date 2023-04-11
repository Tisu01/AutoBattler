using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImage : MonoBehaviour
{
    public Image image;
    public Color toggleOnColor;
    public Color toggleOffColor;

    Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();

        if (toggle == null)
            return;

        toggle.onValueChanged.AddListener(UpdateImageColor);

        UpdateImageColor(toggle.isOn);
    }

    void UpdateImageColor(bool isOn)
    {
        if (image == null)
            return;

        image.color = isOn ? toggleOnColor : toggleOffColor;
    }
}
