using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seed", menuName = "ScriptableObjects/RNG", order = 2)]

public class RNGSeed : ScriptableObject
{
    System.Random RNG;
    [Header("Non Functional")]
    public int rngUsed = 0;

    public void setSeed(int seed)
    {
        RNG = new System.Random(seed);
        rngUsed = 0;
    }
    public int Next(int minValue, int maxValue)
    {
        rngUsed++;
        return RNG.Next(minValue, maxValue);
    }
    public int Next()
    {
        rngUsed++;
        return RNG.Next();
    }
}
