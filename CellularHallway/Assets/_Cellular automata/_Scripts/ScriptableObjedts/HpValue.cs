using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HpValue", menuName = "ScriptableObjects/HpValue", order = 4)]
public class HpValue : ScriptableObject
{
    public uint maxHP;
    public uint currentHP;
    public float HpPercentage;

    public void ResetHP()
    {
        currentHP = maxHP;
        CalculateHpPercentage();
    }

    public void CalculateHpPercentage()
    {
        HpPercentage = (float)currentHP / (float)maxHP;
    }
}
