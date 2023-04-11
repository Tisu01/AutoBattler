using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform pivot;

    public void UpdateHealthBar(float healthscale)
    {
        pivot.localScale = new Vector3(healthscale, 1, 1);
    }
    
}
