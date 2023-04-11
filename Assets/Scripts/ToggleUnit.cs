using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUnit : MonoBehaviour
{
    public UnitType unitType;
    [HideInInspector]
    public GameManager gameManager;

    Toggle toggle;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;

        toggle = GetComponent<Toggle>();

        if (toggle == null)
            return;

        toggle.onValueChanged.AddListener(ReportToggleActive);
    }

    void ReportToggleActive(bool isOn)
    {
        if (isOn == true)
            gameManager.UpdateLeftPanelUnit(unitType);
    }
}
