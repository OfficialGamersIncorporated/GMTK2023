using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnitStats", menuName = "Stats/UnitStats")]

[System.Serializable]
public class UnitStats : ScriptableObject
{
    [Tooltip("Strength increases weapon damage"), Range (1.0f, 10.0f)]
    public int STR;
    [Tooltip("Dexterity increases attack speed"), Range(1.0f, 10.0f)]
    public int DEX;
    [Tooltip("Constitution increases hitpoints"), Range(1.0f, 10.0f)]
    public int CON;
    [Tooltip("Agility increases movement speed"), Range(1.0f, 10.0f)]
    public int AGL;
    [Tooltip("Base hitpoints"), Range(1.0f, 25.0f)]
    public float baseHP;
    [Tooltip("Base movespeed"), Range(0.25f, 10.0f)]
    public float baseMovementSpeed;
}
