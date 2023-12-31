using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponStats", menuName = "Stats/WeaponStats")]

[System.Serializable]
public class WeaponStats : ScriptableObject
{
    [Tooltip("Base damage dealt by successful attacks"), Range(0.5f, 10.0f)]
    public float weaponDamage;
    [Tooltip("Number of attacks per second"), Range(0.5f, 5.0f)]
    public float weaponAttackSpeed;
    [Tooltip("Distance from target to be able to attack"), Range(0.5f, 10.0f)]
    public float weaponRange;
}