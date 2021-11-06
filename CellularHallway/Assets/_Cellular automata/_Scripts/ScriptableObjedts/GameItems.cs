using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameItems", menuName = "ScriptableObjects/GameItems", order = 3)]
public class GameItems : ScriptableObject
{
    public GameObject[] items;
}
