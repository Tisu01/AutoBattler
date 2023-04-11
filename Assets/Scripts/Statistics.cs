using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statistics
{
    public static List<long> findPathTicks = new List<long>();

    public static void PrintAvg()
    {
        Debug.Log("Sredni czas obliczania trasy jednostek w Tickach: " + srednia());

    }
    public static void PrintAvg1()
    {
        Debug.Log("Suma obliczana tras jednostek w Tickach: " + sumaT());
    }


    public static long sumaT()
    {
        long sumaT = 0;
        for (int i = 0; i < findPathTicks.Count; i++)
        {
            sumaT += findPathTicks[i];
        }
        return sumaT;
    }

    public static long srednia()
    {
        long suma = 0;
        for (int i = 0; i < findPathTicks.Count; i++)
        {
            suma += findPathTicks[i];
        }
        return (suma / findPathTicks.Count);
    }


}
