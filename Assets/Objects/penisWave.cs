using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New penis", menuName = "penis calibar")]
public class penisWave : ScriptableObject {

    public float dicksize;
    public string dickname;
    public List<typeOfGuy> guysToSpawn;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}

[System.Serializable]
public class typeOfGuy {
    public GameObject guyPrefab;
    public int guyCount;
}