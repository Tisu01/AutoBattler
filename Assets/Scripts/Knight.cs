using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit
{
    


    public override bool CanTakeDamage(Unit enemy)
    {
        return unitNode.neighbors.Contains(closestEnemy.unitNode);
    }
}
