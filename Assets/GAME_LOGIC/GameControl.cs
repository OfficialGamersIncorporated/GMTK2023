using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl: MonoBehaviour
{
    private int roundNumber = 0;
    public List<WaveDefinition> waves;

    public bool waveRunning;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * call this function to spawn a new wave
     **/
    public void StartNewRound() 
    {
        //spawnEnemies();   
        waveRunning = true;
    }
}
