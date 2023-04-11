using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public float attackRange;
    public GameObject arrowPrefab;

    public override bool CanTakeDamage(Unit enemy)
    {
        return closestEnemy != null && Vector3.Distance(transform.position, closestEnemy.transform.position) <= attackRange;
    }

    public override void AttackEnemy()
    {
        GameObject spawnedArrow = Instantiate(arrowPrefab);
        spawnedArrow.transform.position = transform.position;
        spawnedArrow.transform.LookAt(closestEnemy.transform, spawnedArrow.transform.right);

        spawnedArrow.transform.DOMove(closestEnemy.transform.position, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            base.AttackEnemy();
            Destroy(spawnedArrow);
        });
    }
}
