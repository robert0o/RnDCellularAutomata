using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Seed", menuName = "ScriptableObjects/RNG", order = 2)]

public class RNGSeed : ScriptableObject
{
    System.Random RNG;
    public bool useOwnSeed;
    public string seed;

    public void setSeed(int seed)
    {
        RNG = new System.Random(seed);
    }
    public int Next(int minValue, int maxValue)
    {
        return RNG.Next(minValue, maxValue);
    }
    public int Next()
    {
        return RNG.Next();
    }
}
