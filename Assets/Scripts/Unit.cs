using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //References to other classes
    public SpriteRenderer spriteRenderer;
    public Node unitNode;
    [HideInInspector] public GameManager gameManager;
    
    //References in this class
    public List<Node> deadEnds = new List<Node>();
    public Team myTeam;
    public List<Node> path = new List<Node>();
    public Unit closestEnemy;
    public Unit previousClosestEnemy;
    

    // stats for units
    public float rateOfJump;
    public float rateOfAttack;
    public float health;
    public float maxhealth;
    public float dmg;
    public HealthBar healthBar;

    // non-public. Common to all units
    float waitTime = 0.1f;
    float timeToFirstAction = 1;

    bool actionLoopIsActive = false;

    
    #region Initialize,Start Simulation
    public void Initialize(Team team, Node unitNode, GameManager gameManager)
    {

        maxhealth = health;
        myTeam = team;
        this.unitNode = unitNode;
        this.gameManager = gameManager;
        if (myTeam == Team.Team2)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void StartSimulation()
    {
        //Stop loop if is active
        if (actionLoopIsActive == true)
            CancelInvoke("ActionLoop");

        //Start new action loop
        Invoke("ActionLoop", timeToFirstAction);
        actionLoopIsActive = true;
    }
    #endregion

    #region attack/move/closest enemy/ ActionLoop
    private void ActionLoop()
    {
        previousClosestEnemy = closestEnemy;
        closestEnemy = ClosestEnemy();

        //if there is no enemy - wait
        if (closestEnemy == null)
        {
            if (myTeam == Team.Team2)
                Debug.Log("Closest enemy is null");

            Invoke("ActionLoop", waitTime);
            path.Clear();
            return;
        }


        //if enemy is next to this unit - attack it
        if (CanTakeDamage(closestEnemy))
        {
            if (myTeam == Team.Team2)
                Debug.Log("Attack Archer");
            AttackEnemy();
            Invoke("ActionLoop", rateOfAttack);
            path.Clear();
            return;
        }

        //follow the path if possible
        if (previousClosestEnemy == closestEnemy &&
            path.Count > 2 && path[1].Ocupatebyunit == false)
        {
            Debug.Log("Move to Enemy");
            MoveToEnemy(path);
            Invoke("ActionLoop", rateOfJump);
        }
        //else find a new path
        else
        {
            path.Clear();
            path = FindPathWithStats();

            //if path cannot be found - wait
            if (path.Count == 0)
            {
                Invoke("ActionLoop", waitTime);
            }
            //else move to enemy
            else
            {
                Debug.Log("Move to Enemy");
                MoveToEnemy(path);
                Invoke("ActionLoop", rateOfJump);
            }
        }
    }

    public virtual bool CanTakeDamage(Unit enemy)
    {
        return true;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        healthBar.UpdateHealthBar(health / maxhealth);
        if (health <= 0)
        {
            health = 0;

            gameManager.KillUnit(this);
        }
    }

    public virtual void AttackEnemy()
    {
        Debug.Log("Attack");

        closestEnemy?.TakeDamage(dmg);
    }

    public void MoveToEnemy(List<Node> path)
    {
        if (path.Count > 1)
        {
            //move unit
            transform.DOKill();
            transform.DOMove(path[1].worldPosition, rateOfJump).SetEase(Ease.Linear);
            //transform.position = path[1].worldPosition;

            //set parameters of nodes and path
            path[0].Ocupatebyunit = null;
            path[1].Ocupatebyunit = this;
            unitNode = path[1];

            //remove first path's node
            path.RemoveAt(0);
        }
        else
        {
            Debug.Log("<color=yellow>Cannot move to enemy because enemy is too close</color>");
        }
    }

    public Unit ClosestEnemy()
    {
        //Get all opponents
        List<Unit> opponentTeam = gameManager.teamUnits[GetOpponentTeam(myTeam)];

        //If there is no opponent - return null
        if (opponentTeam.Count == 0)
            return null;



        //by default closestUnit is null, and distance is the maximum value float can take
        Unit closestOpponent = null;
        float distanceToClosestOpponent = float.MaxValue;

        //compare all opponents and choose the closest one
        foreach (Unit opponent in opponentTeam)
        {
            float distanceToNewOpponent = Vector3.Distance(unitNode.worldPosition, opponent.unitNode.worldPosition);

            if (distanceToClosestOpponent > distanceToNewOpponent)
            {
                closestOpponent = opponent;
                distanceToClosestOpponent = distanceToNewOpponent;
            }
        }

        //Finally, return the closest opponent
        return closestOpponent;
    }
    #endregion
    #region finding path

    public List<Node> FindPathWithStats()
    {
        DateTime before = DateTime.Now;
        List<Node> foundPath = new List<Node>();
        if (gameManager.pathFindingMode == PathFindingMode.Simple)
        {
            foundPath = FindPath();
        }
        else
        {
             foundPath = AlgorytmDijkstry.FindPath(gameManager.gridManager.graph, unitNode, closestEnemy.unitNode);
        }
        DateTime after = DateTime.Now;

        TimeSpan duration = after.Subtract(before);

        Statistics.findPathTicks.Add(duration.Ticks);
        Statistics.PrintAvg();
        Statistics.PrintAvg1();

        return foundPath;
    }

    public List<Node> FindPath()
    {
        List<Node> tmpPath = new List<Node>();
        deadEnds.Clear();
        //First node in List is the node the unit is standing on
        tmpPath.Add(unitNode);


        while (tmpPath[tmpPath.Count - 1] != closestEnemy.unitNode)
        {
            Node closestNode = ClosestNodeToEnemy(tmpPath[tmpPath.Count - 1], tmpPath, false);

            //if this is a dead end - return an empty list
            if (closestNode == null)
            {
                if (tmpPath[tmpPath.Count - 1] == unitNode)
                    return new List<Node>();

                deadEnds.Add(tmpPath[tmpPath.Count - 1]);
                tmpPath.RemoveAt(tmpPath.Count - 1);
            }
            //else add node to the list
            else
                tmpPath.Add(closestNode);
        }

        return tmpPath;
    }

    public List<Node> FindPathDijkstra()
    {
        return new List<Node>();
    }

    //if the nearest neighbor cannot be determined (dead end) - returns null
    public Node ClosestNodeToEnemy(Node node, List<Node> currentPath = null, bool ignoreOccupiedNodes = true)
    {
        //by default Node is null, and distance is the maximum value float can take
        Node closestNode = null;
        float distanceToClosestNode = float.MaxValue;

        for (int i = 0; i < node.neighbors.Count; i++)
        {
            //if the node is occupied by the enemy - return it (Because it's our final destination)
            if (node.neighbors[i] == closestEnemy.unitNode)
                return node.neighbors[i];
            //don't consider nodes from deadEnds List
            if (deadEnds.Contains(node.neighbors[i]) == true)
                continue;
            //don't go back in the path
            if (currentPath.Contains(node.neighbors[i]) == true)
                continue;

            //don't consider nodes occupied by other units
            if (ignoreOccupiedNodes == false && node.neighbors[i].Ocupatebyunit == true)
                continue;

            //compare all neighbors and choose the closest one
            float distanceToNewNode = Vector3.Distance(node.neighbors[i].worldPosition, closestEnemy.unitNode.worldPosition);
            if (distanceToNewNode < distanceToClosestNode)
            {
                closestNode = node.neighbors[i];
                distanceToClosestNode = distanceToNewNode;
            }
        }

        //Finally, return the closest node
        return closestNode;
    }

    #endregion

    public static Team GetOpponentTeam(Team myTeam)
    {
        if (myTeam == Team.Team1)
            return Team.Team2;

        return Team.Team1;
    }

    private void OnDrawGizmos()
    {
        if (path.Count == 0)
            return;

        Color pathColor = myTeam == Team.Team1 ? Color.blue : Color.red;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i].worldPosition, path[i + 1].worldPosition, pathColor, 0.5f);
        }
    }
}

public enum Team
{
    Team1,
    Team2
}
