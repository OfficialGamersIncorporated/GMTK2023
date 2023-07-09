using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUnitHolder : MonoBehaviour {

    public static MainUnitHolder Singleton;
    public Transform Goons;
    public Transform Invaders;

    private void Awake() {
        Singleton = this;
    }

}
