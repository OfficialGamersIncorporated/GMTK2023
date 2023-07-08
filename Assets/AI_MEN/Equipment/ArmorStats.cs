using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ArmorStats", menuName = "Stats/ArmorStats")]

[System.Serializable]
public class ArmorStats : ScriptableObject
{
    [Tooltip("Strength increases weapon damage"), Range(0.0f, 10.0f)]
    public int bonusSTR;
    [Tooltip("Dexterity increases attack speed"), Range(0.0f, 10.0f)]
    public int bonusDEX;
    [Tooltip("Constitution increases hitpoints"), Range(0.0f, 10.0f)]
    public int bonusCON;
    [Tooltip("Agility increases movement speed"), Range(0.0f, 10.0f)]
    public int bonusAGL;
}