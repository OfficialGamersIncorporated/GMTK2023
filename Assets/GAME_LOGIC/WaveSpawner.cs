using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UIElements;

public class WaveSpawner : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public int currentWave;
    public int currentWaveValue;
    public int baseWaveValue;
    private List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform spawnLocation;
    public int waveDuration;
    [SerializeField] float waveTimer;
    [SerializeField] float spawnInterval;
    [SerializeField] float spawnTimer;
    [SerializeField] float gateTimer = 5f;
    [SerializeField] float waveCooldown = 00f;
    [SerializeField] float waveCooldownTimer;
    private Vector3 nextSpawn;

    // these are private varibles to help build units out in a triange
    private const int TRIANGLE_X = 0;
    private const int TRIANGLE_Y = 0;
    private int triangleHeight = 1;
    private int triangleWidth = 1;
    private float triangleCoordWidth = 1;
    private float triangleCoordHeight = 1;
    // item1 = width, item2 = height
    private List<int> currentTriangleCoord;


    private bool gateOpen = false;


    [SerializeField] GameObject gate;

    //[SerializeField] DifficultyManager difficulty;


    void Start()
    {
        //GenerateWave();
        nextSpawn = spawnLocation.transform.position;
        currentTriangleCoord = new List<int>() { 1,1};
    }

    void Update()
    {
    }

    public void GenerateWave()
    {
        baseWaveValue = baseWaveValue + Mathf.CeilToInt(currentWave * 1.2f); // TEMP DIFFICULTY CURVE
                                                                             //Instantiate(enemies[0].enemyPrefab, spawnLocation.position, Quaternion.identity);
        currentWaveValue = baseWaveValue;                                                                    

       
        GenerateEnemies();
        
        currentWave++;
    }

    private Vector3 getPosition()
    {
        // make a copy of the spawn position
        // nextSpawn is where we will be spawning, and update spawnHereNow for where you want to go next spawn
        Vector3 spawnHereNow = nextSpawn;

        // time to make new triangle row
        if (currentTriangleCoord[TRIANGLE_X] == currentTriangleCoord[TRIANGLE_Y])
        {
            // go down by pre-determined height, and go left by the pre-determined width times the width of the previous row
            nextSpawn.y -= triangleCoordHeight;
            nextSpawn.x -= triangleCoordWidth * (currentTriangleCoord[0]);
            currentTriangleCoord[TRIANGLE_X] = 1;// reset x to 1
            currentTriangleCoord[TRIANGLE_Y] = currentTriangleCoord[TRIANGLE_Y] + 2; // y is now
            return spawnHereNow;
        }
        // move right and move x,y index for triangle
        nextSpawn.x += triangleCoordWidth;
        currentTriangleCoord[TRIANGLE_X]++;
        return spawnHereNow;
    }

    private void smartInstantiate(List<GameObject> enemies)
    {
        foreach(var enemy in enemies)
        {
            Instantiate(enemy, getPosition(), Quaternion.identity);
        }
        nextSpawn = spawnLocation.transform.position;
        currentTriangleCoord = new List<int> { 1, 1 };
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (currentWaveValue > 0)
        {
            int randEnemyIndex = UnityEngine.Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyIndex].cost;

            if (currentWaveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyIndex].enemyPrefab);
                currentWaveValue -= randEnemyCost;
            }
            else
            {
                break;
            }
        }

        smartInstantiate(generatedEnemies);
    }
}


[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}