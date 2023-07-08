using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Wave", menuName = "Wave/WaveDefinition")]
public class WaveDefinition : ScriptableObject
{
    public List<typeOfEnemy> guysToSpawn;
}

[System.Serializable]
public class typeOfEnemy
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public int pointBuy;
}