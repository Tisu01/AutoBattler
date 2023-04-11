using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Inspector fields
    [Header("References")]
    public GridManager gridManager;
    public GameObject knightPrefab;
    public GameObject archerPrefab;
    public Transform unitParent;
    public UIManager uiManager;
    public PathFindingMode pathFindingMode;

    //private fields
    private TileColider focusedTileCollider;
    private UnitType leftPanelUnit;

    public Dictionary<Team, List<Unit>> teamUnits;

    public void OnGameStartInitialized()
    {
        //Create Graph
        if (gridManager != null)
            gridManager.GenerateGraph();

        //Add two lists of Units to Dictionary
        teamUnits = new Dictionary<Team, List<Unit>>();
        teamUnits.Add(Team.Team1, new List<Unit>());
        teamUnits.Add(Team.Team2, new List<Unit>());
    }

    public void StartSimulation(int spawnUnitCount)
    {
        //Spawn enemies
        for (int i = 0; i < spawnUnitCount; i++)
            SpawnUnitAtRandomPosition(UnitType.Knight, Team.Team2);


        //Send info to ALL of units that the simulation is active
        for (int i = 0; i < teamUnits[Team.Team1].Count; i++)
            teamUnits[Team.Team1][i].StartSimulation();

        for (int i = 0; i < teamUnits[Team.Team2].Count; i++)
            teamUnits[Team.Team2][i].StartSimulation();

        gridManager.DestroyTileColliders();
    }


    void Update()
    {

        if (Input.GetMouseButtonDown(0) && focusedTileCollider != null)
        {
            Node nodeToSpawn = focusedTileCollider.node;
            SpawnUnitAtSpecificNode(leftPanelUnit, nodeToSpawn, Team.Team1);
        }
        if (Input.GetMouseButtonDown(1) && focusedTileCollider != null)
        {
            Node nodeToRemove = focusedTileCollider.node;
            RemoveUnitFromSpecificNode(nodeToRemove);
        }
    }

    public void TileColliderFocused(TileColider tileCollider)
    {
        focusedTileCollider = tileCollider;
    }

    public void TileColliderUnfocesued(TileColider tileCollider)
    {
        if (focusedTileCollider == tileCollider)
            focusedTileCollider = null;
    }

    public void UpdateLeftPanelUnit(UnitType unit)
    {
        leftPanelUnit = unit;
    }

    public void SpawnUnitAtRandomPosition(UnitType unit, Team unitTeam)
    {
        int randomIndex = Random.Range(0, gridManager.graph.unitSlots[unitTeam].Count);
        Node nodeToSpawnUnit = gridManager.graph.unitSlots[unitTeam][randomIndex];

        SpawnUnitAtSpecificNode(unit, nodeToSpawnUnit, unitTeam);
    }

    public void SpawnUnitAtSpecificNode(UnitType unit, Node nodeToSpawn, Team unitTeam)
    {
        Debug.Log("SpawnUnitAtSpecificNode() called with nodeToSpawn: " + nodeToSpawn + " and unitTeam: " + unitTeam);
        if (nodeToSpawn.Ocupatebyunit != null)
        {
            return;
        }

        if (gridManager.graph.unitSlots[unitTeam].Contains(nodeToSpawn) == false)
        {
            return;
        }

        if (nodeToSpawn.connectedTileData.teamSlot == TeamSlot.both)
        {
            return;
        }


        Unit spawnedUnit = SpawnUnit(unit, nodeToSpawn, unitTeam);

        UpdateUnitSlots(nodeToSpawn, unitTeam);

        UpdateUnitList(nodeToSpawn, unitTeam, spawnedUnit);

        Debug.Log("Team1 unit count: " + teamUnits[Team.Team1].Count);
        Debug.Log("Team2 unit count: " + teamUnits[Team.Team2].Count);
    }

    private Unit SpawnUnit(UnitType unit, Node nodeToSpawn, Team unitTeam)
    {
        GameObject unitPrefab = GetUnitPrefab(unit);

        GameObject spawnedKnight = Instantiate(unitPrefab, nodeToSpawn.worldPosition, Quaternion.identity);
        spawnedKnight.transform.parent = unitParent;
        Unit unitComponent = spawnedKnight.GetComponent<Unit>();
        unitComponent.Initialize(unitTeam, nodeToSpawn, this);


        nodeToSpawn.Ocupatebyunit = unitComponent;
        Debug.Log("Unit spawned at node " + nodeToSpawn + " with team " + unitTeam + " and position " + nodeToSpawn.worldPosition);

        if (unitTeam == Team.Team1)
            uiManager.StartSimulationPossiblity(true);

        return unitComponent;
    }

    private void UpdateUnitSlots(Node nodeToSpawn, Team unitTeam)
    {
        if (gridManager.graph.unitSlots[unitTeam].Contains(nodeToSpawn) == true)
            gridManager.graph.unitSlots[unitTeam].Remove(nodeToSpawn);
        Debug.Log("Node " + nodeToSpawn + " removed from unit slot for team " + unitTeam);

        Team opponentTeam = Unit.GetOpponentTeam(unitTeam);

        if (gridManager.graph.unitSlots[opponentTeam].Contains(nodeToSpawn) == true)
            gridManager.graph.unitSlots[opponentTeam].Remove(nodeToSpawn);
    }

    private void UpdateUnitList(Node nodeToSpawn, Team unitTeam, Unit unit)
    {
        if (teamUnits.ContainsKey(unitTeam) == false)
            return;

        if (teamUnits[unit.myTeam].Contains(unit) == true)
            return;

        teamUnits[unit.myTeam].Add(unit);
    }

    public void RemoveUnitFromSpecificNode(Node nodeToRemove)
    {
        if (nodeToRemove.Ocupatebyunit == null)
            return;

        Unit unitFromNode = nodeToRemove.Ocupatebyunit;

        Team unitTeam = unitFromNode.myTeam;

        nodeToRemove.Ocupatebyunit = null;
        Debug.Log("Unit removed from node " + nodeToRemove);

        gridManager.graph.unitSlots[unitTeam].Add(nodeToRemove);
        Debug.Log("Node " + nodeToRemove + " added to unit slot for team " + unitTeam);

        teamUnits[unitTeam].Remove(unitFromNode);
        Destroy(unitFromNode.gameObject);

        if (teamUnits[Team.Team1].Count == 0)
            uiManager.StartSimulationPossiblity(false);
    }

    public void KillUnit(Unit unit)
    {
        unit.unitNode.Ocupatebyunit = null;
        teamUnits[unit.myTeam].Remove(unit);
        Destroy(unit.gameObject);

        if (teamUnits[Team.Team1].Count == 0)
            StopSimulation(false);

        if (teamUnits[Team.Team2].Count == 0)
            StopSimulation(true);
    }

    public void StopSimulation(bool win)
    {
        uiManager.ShowEndScreen(win);
    }

    private GameObject GetUnitPrefab(UnitType unitType)
    {
        if (unitType == UnitType.Knight)
            return knightPrefab;
        else
            return archerPrefab;
    }
}

public enum UnitType
{
    Knight,
    Archer
}

public enum PathFindingMode
{
    Simple,
    Dijkstrya
}