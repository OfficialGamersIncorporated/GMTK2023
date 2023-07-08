using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public int currentWave;
    public int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform spawnLocation;
    public int waveDuration;
    [SerializeField] float waveTimer;
    [SerializeField] float spawnInterval;
    [SerializeField] float spawnTimer;
    [SerializeField] float gateTimer = 5f;
    [SerializeField] float waveCooldownStat = 00f;
    [SerializeField] float waveCooldown;


    private bool gateOpen = false;


    [SerializeField] GameObject gate;

    //[SerializeField] DifficultyManager difficulty;


    void Start()
    {
        GenerateWave();
    }

    void Update()
    {


        if (spawnTimer <= 0)
        {
            if (enemiesToSpawn.Count > 0)
            {
                Instantiate(enemiesToSpawn[0], spawnLocation.position, Quaternion.identity);
                enemiesToSpawn.RemoveAt(0);
                spawnTimer = spawnInterval;
            }

        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }

        waveTimer -= Time.deltaTime;

        if (waveTimer <= 0 && !gateOpen)
        {
            gate.GetComponent<Collider>().enabled = false;
            gateOpen = true;
        }
        if (gateOpen)
        {
            gateTimer -= Time.deltaTime;
        }
        if (gateTimer <= 0)
        {
            gate.GetComponent<Collider>().enabled = true;
            gateOpen = false;
            gateTimer = 5;
        }

        if (waveTimer <= 0)
        {
            waveCooldown -= Time.deltaTime;
        }
        if (waveCooldown <= 0)
        {
            GenerateWave();
        }
    }

    public void GenerateWave()
    {
        waveValue = Mathf.CeilToInt(currentWave * 1.2f); // TEMP DIFFICULTY CURVE
        GenerateEnemies();


        float enemiesToSpawnFloat = enemiesToSpawn.Count;
        spawnInterval = waveDuration / enemiesToSpawnFloat;
        waveTimer = waveDuration;
        currentWave++;
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }

        waveCooldown = waveCooldownStat;
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }
}


[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}