using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Button startButton;
    public Button confirmMapButton;
    public Button restartButton;
    public GameManager gameManager;
    public Transform mapPanel;
    public Transform unitPanel;
    public Transform endScreen;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI statsText1;
    public TextMeshProUGUI result;

    private void Awake()
    {
        startButton?.onClick.AddListener(StartSimulation);
        confirmMapButton?.onClick.AddListener(ShowLeftPanel);
        restartButton?.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));

        StartSimulationPossiblity(false);

        InitUnitToggles();
        InitMapToggles();
    }

    private void InitUnitToggles()
    {
        //Find all toogleUnits in children
        ToggleUnit[] toggleUnits = unitPanel.GetComponentsInChildren<ToggleUnit>();
        foreach (ToggleUnit toggleUnit in toggleUnits)
        {
            toggleUnit.Initialize(gameManager);
        }
    }

    private void InitMapToggles()
    {
        //Find all toogleUnits in children
        MapToggle[] mapToggles = mapPanel.GetComponentsInChildren<MapToggle>();
        foreach (MapToggle mapToggle in mapToggles)
        {
            mapToggle.Initialize(gameManager);
        }
    }

    private void StartSimulation()
    {
        gameManager.StartSimulation(3);
        unitPanel.gameObject.SetActive(false);
    }

    public void ShowLeftPanel()
    {
        mapPanel.gameObject.SetActive(false);
        unitPanel.gameObject.SetActive(true);
        gameManager.OnGameStartInitialized();
    }

    public void ShowEndScreen(bool win)
    {
        endScreen?.gameObject.SetActive(true);
        string wynik = win ? "Wygrana" : "Przegrana";
        result.text = wynik;

        statsText.text = "Œrednia wyszukania œcie¿ki: " + Statistics.srednia();

        statsText1.text = "Suma wyszukañ œcie¿ek: " + Statistics.sumaT();
    }

    public void StartSimulationPossiblity(bool isInteractable)
    {
        startButton.interactable = isInteractable;
    }
}
