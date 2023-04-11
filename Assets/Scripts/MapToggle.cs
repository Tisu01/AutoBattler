using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapToggle : MonoBehaviour
{
    public GameObject map;

    private GameManager gameManager;

    public void UpdateMapVisibility(bool isOn)
    {
        map?.SetActive(isOn);

        if (isOn)
            gameManager.gridManager.grid = map.GetComponentInChildren<Tilemap>();
    }

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;

        GetComponent<Toggle>()?.onValueChanged.AddListener(UpdateMapVisibility);
    }
}
