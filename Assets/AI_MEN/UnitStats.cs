using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnitStats", menuName = "Stats/UnitStats")]

[System.Serializable]
public class Stats : ScriptableObject
{
    [Tooltip("Strength increases weapon damage"), Range (1.0f, 10.0f)]
    public int STR;
    [Tooltip("Dexterity increases attack speed"), Range(1.0f, 10.0f)]
    public int DEX;
    [Tooltip("Constitution increases hitpoints"), Range(1.0f, 10.0f)]
    public int CON;
    [Tooltip("Agility increases movement speed"), Range(1.0f, 10.0f)]
    public int AGL;
}
